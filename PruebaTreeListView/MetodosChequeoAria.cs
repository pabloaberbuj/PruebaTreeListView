using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaQ;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PruebaTreeListView
{
    public static class MetodosChequeoAria
    {
        public static bool? CoincidenciaDosisConReferencePoint(Plan plan)
        {
            if (MetodosChequeoEclipse.CoincidenciaDosisConPrescripcion(plan)==false)
            {
                return null;
            }
            var RefPointPrimario = plan.PlanAria.RTPlans.First().RadiationRefPoints.FirstOrDefault(r => r.RefPoint.RefPointId == plan.PlanEclipse.PrimaryReferencePoint.Id).RefPoint;
            if (RefPointPrimario.TotalDoseLimit!=null && RefPointPrimario.SessionDoseLimit!=null && RefPointPrimario.DailyDoseLimit!=null)
            {
                int factor = 1;
                if (plan.Tecnica==Tecnica.TBI)
                {
                    factor = 2;
                }
                return Math.Round((double)RefPointPrimario.TotalDoseLimit * 100, 1) == plan.DosisTotal*factor && Math.Round((double)RefPointPrimario.SessionDoseLimit * 100, 1) == plan.DosisFraccion* factor && Math.Round((double)RefPointPrimario.DailyDoseLimit * 100, 1) == plan.DosisDia* factor;
            }
            return false;
            
        }

        public static bool? CalculadoCada1mm(Plan plan)
        {
            return plan.PlanAria.DoseMatrices.First().ResX == 1;
        }

        public static bool? TiempoVMAT(Plan plan)
        {
            return !plan.PlanAria.Radiations.Any(r => r.ExternalFieldCommon.TreatmentTime < 2);
        }

        public static bool? HayOtroCursoActivo(Plan plan)
        {
            return !plan.PlanAria.Course.Patient.Courses.Any(c => c.CourseId != plan.PlanAria.Course.CourseId && !c.CourseId.ToLower().Contains("qa") && !c.CourseId.ToLower().Contains("fisica") && c.ClinicalStatus == "ACTIVE");
        }

        public static bool? TienePlanQA(Plan plan)
        {
            return plan.PlanAria.RTPlans.First().PlanRelationships1.Count > 0;
        }

        public static bool? SeMidioPlanQA(Plan plan)
        {
            if (TienePlanQA(plan)==false)
            {
                return null;
            }
            long RTPPlanVerSer = plan.PlanAria.RTPlans.First().PlanRelationships1.FirstOrDefault().RTPlanSer;

            RTPlan RTPlanVer = null;
            foreach (Course curso in plan.PlanAria.Course.Patient.Courses)
            {
                if (curso.PlanSetups.Any(p=>p.RTPlans.FirstOrDefault().RTPlanSer == RTPPlanVerSer))
                {
                    RTPlanVer = curso.PlanSetups.First(p => p.RTPlans.FirstOrDefault().RTPlanSer == RTPPlanVerSer).RTPlans.First();
                }
            }

            //var rad = RTPlanVer.PlanSetup.Radiations.FirstOrDefault();
            //var sl = rad.SliceRTs;
            if (RTPlanVer!=null)// && RTPlanVer.SessionRTPlans.Count > 0)
            {
                return RTPlanVer.PlanSetup.Radiations.FirstOrDefault().SliceRTs.Any(s => s.AcqNote != null);
            }

            // var RTPlanVer = plan.PlanAria.Course.Patient.Courses.Where(c => c.PlanSetups.Where(p => p.RTPlans.FirstOrDefault().RTPlanSer == RTPPlanVerSer).FirstOrDefault()).FirstOrDefault();

            return false;
        }

        public static bool? DeltaCouchIgualParTodosLosCampos(Plan plan)
        {
            string EquipoId = plan.Equipo();
            if (EquipoId == "CL21EX")
            {
                return null;
            }
            foreach (Radiation campoAria in plan.PlanAria.Radiations)
            {
                double? ariaxFirst = plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchLatDelta;
                double? ariayFirst = plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchVrtDelta;
                double? ariazFirst = plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchLngDelta;

                double? ariaxCampo = campoAria.ExternalFieldCommon.CouchLatDelta;
                double? ariayCampo = campoAria.ExternalFieldCommon.CouchVrtDelta;
                double? ariazCampo = campoAria.ExternalFieldCommon.CouchLngDelta;

                if (ariaxFirst != ariaxCampo || ariayFirst != ariayCampo || ariazFirst != ariazCampo)
                {
                    return false;
                }
            }
            return true;
        }


        public static bool? DeltaCouchCoincideConIso(Plan plan)
        {
            string EquipoId = plan.Equipo();
            if (EquipoId == "CL21EX")
            {
                return null;
            }
            foreach (Ecl.Beam campoEclipse in plan.PlanEclipse.Beams)
            {
                Radiation campoAria = plan.PlanAria.Radiations.Where(r => r.RadiationId == campoEclipse.Id).FirstOrDefault();
                VVector ecl = new VVector();

                ecl.x = Math.Round(campoEclipse.IsocenterPosition.x - plan.PlanEclipse.StructureSet.Image.UserOrigin.x, 0);
                ecl.y = Math.Round(campoEclipse.IsocenterPosition.y - plan.PlanEclipse.StructureSet.Image.UserOrigin.y, 0);
                ecl.z = Math.Round(campoEclipse.IsocenterPosition.z - plan.PlanEclipse.StructureSet.Image.UserOrigin.z, 0);

                VVector eclCorregido = MetodosAuxiliares.corregirPorPatientOrientation(ecl, plan.PlanEclipse.TreatmentOrientation);

                double? ariax = -plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchLatDelta * 10;
                double? ariay = plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchVrtDelta * 10;
                double? ariaz = -plan.PlanAria.Radiations.First().ExternalFieldCommon.CouchLngDelta * 10;

                if (eclCorregido.x != ariax || eclCorregido.y != ariay || eclCorregido.z != ariaz)
                {
                    return false;
                }

            }
            return true;
        }

        public static bool? FxSceduleCoincideConFxEclipse(Plan plan)
        {
            return plan.PlanAria.RTPlans.First().SessionRTPlans.Count == plan.PlanEclipse.UniqueFractionation.NumberOfFractions;
        }

        public static bool? NoRealizoAplicacionesARIA(Plan plan)
        {
            return !plan.PlanAria.RTPlans.First().SessionRTPlans.Any(s => s.Status == "COMPLETE");
        }

        public static bool? ImagenesAgendadasSegunEquipoYTecnica(Plan plan)
        {
            List<int> fraccionesConPlaca = new List<int>();
            string sptId = "";
            if (plan.Tecnica == Tecnica.IMRT || plan.Tecnica == Tecnica.VMAT )
            {
                int i = 1;
                while (i < plan.PlanAria.RTPlans.First().NoFractions)
                {
                    fraccionesConPlaca.Add(i);
                    i += 1;
                }
            }
            else if (plan.Tecnica == Tecnica.Arcos3DC || plan.Tecnica == Tecnica.Electrones || plan.Tecnica == Tecnica.Static3DC)
            {
                fraccionesConPlaca.Add(1);
            }
            if (plan.PlanAria.Radiations.First().RadiationDevice.Machine.MachineId == "Equipo1")
            {
                sptId = "SingleExp";
            }
            else if (plan.PlanAria.Radiations.First().RadiationDevice.Machine.MachineId == "D-2300CD")
            {
                sptId = "kV";
            }
            foreach (int f in fraccionesConPlaca)
            {
                var Sesion = plan.PlanAria.RTPlans.First().SessionRTPlans.First(s => s.Session.SessionNum == f);
                if (Sesion.Session.SessionProcedures.Count != 2 || Sesion.Session.SessionProcedures.Any(p => !p.SessionProcedureTemplateId.Contains(sptId)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? ImagenesAgendadasSegunEquipo(Plan plan)
        {
            //List<int> fraccionesConPlaca = new List<int>();
            string sptId = "";
            int cantidad = 2;
            /*if (plan.Tecnica == Tecnica.IMRT || plan.Tecnica == Tecnica.VMAT || plan.Tecnica == Tecnica.Mama3DC)
            {
                int i = 1;
                while (i < plan.PlanAria.RTPlans.First().NoFractions)
                {
                    fraccionesConPlaca.Add(i);
                    i += 5;
                }
            }
            else if (plan.Tecnica == Tecnica.Arcos3DC || plan.Tecnica == Tecnica.Electrones || plan.Tecnica == Tecnica.Static3DC)
            {
                fraccionesConPlaca.Add(1);
            }*/
            if (plan.Tecnica==Tecnica.SBRT_HazSRS ||plan.Tecnica==Tecnica.SBRT_VMAT || plan.Tecnica==Tecnica.IGRT)
            {
                sptId = "CBCT";
                cantidad = 1;
            }
            else if (plan.PlanAria.Radiations.First().RadiationDevice.Machine.MachineId == "Equipo1")
            {
                sptId = "SingleExp";
                
            }
            else if (plan.PlanAria.Radiations.First().RadiationDevice.Machine.MachineId == "D-2300CD")
            {
                sptId = "kV";
                
            }
            var Sessions = plan.PlanAria.RTPlans.First().SessionRTPlans;
            foreach (var session in Sessions)
            {
                var ss = session.Session;
                var sss = ss.SessionProcedures;
                if (session.Session.SessionProcedures.Count < cantidad || session.Session.SessionProcedures.Any(p => !p.SessionProcedureTemplateId.Contains(sptId)))
                //if (session.Session.SessionProcedures.Any(p => !p.SessionProcedureTemplateId.Contains(sptId)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? PosicionImagerCorrecta(Plan plan)
        {
            if (plan.PlanAria.Radiations.First().RadiationDevice.Machine.MachineId == "Equipo1")
            {
                foreach (var radiation in plan.PlanAria.Radiations.Where(r => r.ExternalFieldCommon.SetupFieldFlag == 1))
                {
                    if (radiation.ExternalFieldCommon.IDUPosLng != 0 || radiation.ExternalFieldCommon.IDUPosLat != 0 || radiation.ExternalFieldCommon.IDUPosVrt != -60 || radiation.ExternalFieldCommon.IDURtn != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return null;
        }

    }
}
