using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


namespace PruebaTreeListView
{
    public static class MetodosChequeoEclipse //un resultado true significa que el chequeo está ok y un false que está mal, más alla de como se llama el método
    {
        public static bool? TieneCamilla(Plan plan)
        {

            return plan.PlanEclipse.StructureSet.Structures.Any(s => s.DicomType == "SUPPORT");
        }

        public static bool? TieneCamillaYNoDebe(Plan plan)
        {
            return !plan.PlanEclipse.StructureSet.Structures.Any(s => s.DicomType == "SUPPORT");
        }

        public static bool? CamillaCorrecta(Plan plan)
        {
            if (TieneCamilla(plan) == false)
            {
                return null;
            }
            foreach (Structure estructura in plan.PlanEclipse.StructureSet.Structures)
            {
                if (estructura.DicomType == "SUPPORT" && MetodosAuxiliares.CoincidenciaCamillas(estructura.Name, plan.PlanEclipse))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool? UserOriginEnOrigenDICOM(Plan plan)
        {

            return !plan.PlanEclipse.StructureSet.Image.UserOrigin.Equals(new VVector(0, 0, 0));
        }

        public static bool? IsoCercaDeUserOrigin(Plan plan)
        {
            if (UserOriginEnOrigenDICOM(plan) == false)
            {
                return null;
            }
            double distanciaChica = 20;
            return MetodosAuxiliares.distanciaMaxima(plan.PlanEclipse.StructureSet.Image.UserOrigin, plan.PlanEclipse.Beams.First().IsocenterPosition) < 0.01 ||
                MetodosAuxiliares.distanciaMaxima(plan.PlanEclipse.StructureSet.Image.UserOrigin, plan.PlanEclipse.Beams.First().IsocenterPosition) > distanciaChica;
        }

        public static bool? TienePrimaryReferencePoint(Plan plan)
        {

            foreach (FieldReferencePoint punto in plan.PlanEclipse.Beams.First().FieldReferencePoints)
            {
                if (punto.IsPrimaryReferencePoint)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool? ReferencePointTieneLocalizacion(Plan plan)
        {
            if (TienePrimaryReferencePoint(plan) == false)
            {
                return null;
            }
            foreach (FieldReferencePoint punto in plan.PlanEclipse.Beams.First().FieldReferencePoints)
            {
                if (punto.IsPrimaryReferencePoint)
                {
                    if (!double.IsNaN(punto.RefPointLocation.x) && !double.IsNaN(punto.RefPointLocation.y) && !double.IsNaN(punto.RefPointLocation.z))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool? SePuedenFusionarCampos(Plan plan)
        {

            if (plan.PlanEclipse.Beams.Any(b => b.ControlPoints.Count > 2))
            {
                return true;
            }
            else
            {
                foreach (Beam campo1 in plan.PlanEclipse.Beams)
                {
                    if (campo1.MLCPlanType != MLCPlanType.VMAT && campo1.MLCPlanType != MLCPlanType.ArcDynamic && !campo1.IsSetupField && campo1.Wedges.Count() == 0 && campo1.Applicator == null)
                    {
                        foreach (Beam campo2 in plan.PlanEclipse.Beams)
                        {
                            if (campo2.MLCPlanType != MLCPlanType.VMAT && campo2.MLCPlanType != MLCPlanType.ArcDynamic && !campo2.IsSetupField && campo2.Wedges.Count() == 0 && campo2.Applicator == null)
                            {
                                if (campo1.Id != campo2.Id && MetodosAuxiliares.camposFusionables(campo1, campo2))
                                {
                                    return false;
                                }
                            }
                        }
                    }

                }
            }
            return true;
        }


        public static bool? MasDeUnIsoEnUnPlan(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (MetodosAuxiliares.distanciaMaxima(plan.PlanEclipse.Beams.First().IsocenterPosition, campo.IsocenterPosition) > 0.01)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? TomografiaVieja(Plan plan)
        {

            if (plan.PlanEclipse.Series.Study.CreationDateTime != null)
            {
                DateTime fechaTomo = (DateTime)plan.PlanEclipse.StructureSet.Image.CreationDateTime;
                return (DateTime.Today - fechaTomo).Days < 30;
            }
            return false; //raro pero por las dudas que salte error
        }


        public static bool? EstructuraNombreCursoCorrecta(Plan plan)
        {

            Regex estructura = new Regex(@"^C[0-9]_[a-zA-Z]{3}[0-9]{2}$");
            return estructura.IsMatch(plan.PlanEclipse.Course.Id);
        }

        public static bool? OrientacionArcos(Plan plan)
        {
            List<double> longitudes = new List<double>();
            if (plan.PlanEclipse.Beams.Any(c => c.Technique.Id == "ARC"))
            {
                longitudes = plan.PlanEclipse.Beams.Where(c => c.Technique.Id == "ARC").Select(c => MetodosAuxiliares.IECaVarian(c.ControlPoints.Last().GantryAngle) - MetodosAuxiliares.IECaVarian(c.ControlPoints.First().GantryAngle)).ToList();
                return Math.Abs(longitudes.Sum()) <= longitudes.Select(l => Math.Abs(l)).Max();
            }
            return null; //No tiene arcos
        }

        public static bool? TreatmentApprovalHecho(Plan plan)
        {

            return plan.PlanEclipse.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved;
        }

        public static bool? TieneCamposSetUp(Plan plan)
        {

            return plan.PlanEclipse.Beams.Any(b => b.IsSetupField);
        }

        public static bool? TieneCampoAntYLatSetUp(Plan plan)
        {
            if (TieneCamposSetUp(plan) == false)
            {
                return null;
            }
            return plan.PlanEclipse.Beams.Any(b => b.IsSetupField && b.ControlPoints.First().GantryAngle == 0) && plan.PlanEclipse.Beams.Any(b => b.IsSetupField && b.ControlPoints.First().GantryAngle == 270 || b.ControlPoints.First().GantryAngle == 90);
        }

        public static bool? CampoSetupLateralCorrecto(Plan plan)
        {

            if (TieneCamposSetUp(plan) == false || TieneCampoAntYLatSetUp(plan) == false)
            {
                return null;
            }
            if ((plan.PlanEclipse.Beams.First().IsocenterPosition.x - plan.PlanEclipse.StructureSet.Image.UserOrigin.x) < -30) //3cm a la derecha
            {
                return plan.PlanEclipse.Beams.Any(b => b.IsSetupField && b.ControlPoints.First().GantryAngle == 270);
            }
            else if ((plan.PlanEclipse.Beams.First().IsocenterPosition.x - plan.PlanEclipse.StructureSet.Image.UserOrigin.x) > 30) //3cm a la izquierda
            {
                return plan.PlanEclipse.Beams.Any(b => b.IsSetupField && b.ControlPoints.First().GantryAngle == 90);
            }
            else //entre 3 y -3
            {
                return true;
            }
        }



        public static bool? DRRCreadasEnCamposSetup(Plan plan)
        {
            if (TieneCamposSetUp(plan) == false)
            {
                return null;
            }
            var camposSetup = plan.PlanEclipse.Beams.Where(b => b.IsSetupField).ToList();
            foreach (Beam campo in camposSetup)
            {
                if (campo.ReferenceImage == null || !campo.ReferenceImage.Id.Contains("DRR"))
                {
                    return false;
                }
            }
            return true;
        }


        public static bool? UMporGradoAlta(Beam campo)
        {
            if (campo.Technique.Id == "ARC" && campo.MLCPlanType != MLCPlanType.VMAT) //solo arcos conformados, no vmat
            {
                return campo.Meterset.Value / campo.ArcLength < 20;
            }
            return true;
        }

        public static bool? UMporGradoAlta(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (UMporGradoAlta(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? UMporGradoBaja(Beam campo)
        {
            if (campo.Technique.Id == "ARC" && campo.MLCPlanType != MLCPlanType.VMAT) //solo arcos conformados, no vmat
            {
                return ((campo.ArcLength * campo.DoseRate) / (campo.Meterset.Value * 60)) < 4.2;
            }
            return true;
        }

        public static bool? UMporGradoBaja(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (UMporGradoBaja(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? DoseRate(Beam campo)
        {
            if (campo.EnergyModeDisplayName == "6X-SRS")
            {
                return campo.DoseRate == 1000;
            }
            else if (campo.MLCPlanType.Equals(MLCPlanType.VMAT))
            {
                return campo.DoseRate == 600;
            }
            else if ((campo.TreatmentUnit.Id == "Equipo1" || campo.TreatmentUnit.Id == "D-2300CD") && campo.MLCPlanType.Equals(MLCPlanType.DoseDynamic))
            {
                return campo.DoseRate == 600;
            }
            else
            {
                return campo.DoseRate == 400;
            }
        }

        public static bool? DoseRate(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams.Where(c => !c.IsSetupField))
            {
                if (DoseRate(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? UMporSubField(Beam campo)
        {
            if (campo.ControlPoints.Count < 10)
            {
                for (int i = 1; i < campo.ControlPoints.Count; i++)
                {
                    if (Math.Round(campo.ControlPoints[i].MetersetWeight - campo.ControlPoints[i - 1].MetersetWeight, 2) != 0 &&
                        Math.Round(campo.Meterset.Value * (campo.ControlPoints[i].MetersetWeight - campo.ControlPoints[i - 1].MetersetWeight), 0) < 10)
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public static bool? UMporSubField(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (UMporSubField(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? UMminimaParaCuna(Beam campo)
        {
            if (campo.Wedges.Count() > 0 && Math.Round(campo.Meterset.Value, 0) < 20)
            {
                return false;
            }
            return true;
        }

        public static bool? UMminimaParaCuna(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (UMminimaParaCuna(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? TamanoXenVMAT(Beam campo)
        {
            if (campo.MLCPlanType == MLCPlanType.VMAT && (-campo.ControlPoints.First().JawPositions.X1 + campo.ControlPoints.First().JawPositions.X2) > 160)
            {
                return false;
            }
            return true;
        }

        public static bool? TamanoXenVMAT(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (TamanoXenVMAT(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? IsoEnArcos(Beam campo, PlanningItem planCorrespondiente)
        {
            PlanSetup plan = planCorrespondiente as PlanSetup;
            bool origen = plan.StructureSet.Image.UserOrigin.Equals(new VVector(0, 0, 0));
            if (!origen && campo.MLCPlanType == MLCPlanType.VMAT)
            {
                if ((campo.IsocenterPosition.x - plan.StructureSet.Image.UserOrigin.x) >= 30.5 && campo.ControlPoints.Select(c => c.GantryAngle).Max() > 210)
                {
                    return false;
                }
                else if ((campo.IsocenterPosition.x - plan.StructureSet.Image.UserOrigin.x) <= -30.5 && campo.ControlPoints.Select(c => c.GantryAngle).Min() < 110)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? IsoEnArcos(Plan plan)
        {
            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (IsoEnArcos(campo, plan.PlanEclipse) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? TamanoCampoMinimo(Beam campo)
        {
            if (campo.EnergyModeDisplayName != "6X-SRS")
            {
                if ((-campo.ControlPoints.First().JawPositions.X1 + campo.ControlPoints.First().JawPositions.X2) < 30 || (-campo.ControlPoints.First().JawPositions.Y1 + campo.ControlPoints.First().JawPositions.Y2) < 30)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? TamanoCampoMinimo(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (TamanoCampoMinimo(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? CampoConCunaFisica(Beam campo)
        {
            if (campo.Wedges.Count() > 0)
            {
                return campo.Wedges.Any(c => c.Id.Contains("EDW"));
            }
            return true;
        }

        public static bool? CampoConCunaFisica(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (CampoConCunaFisica(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }


        //nuevos

        public static bool? IsoFueraDeCampo(Beam campo)
        {
            var mordazas = campo.ControlPoints.First().JawPositions;
            double MordazaMasCerrada = new List<double> { -mordazas.X1, mordazas.X2, -mordazas.Y1, mordazas.Y2 }.Min();
            return MordazaMasCerrada > -10;
        }

        public static bool? IsoFueraDeCampo(Plan plan)
        {
            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (IsoFueraDeCampo(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? IsoFueraDeCampoSoloY(Beam campo)
        {
            var mordazas = campo.ControlPoints.First().JawPositions;
            double MordazaMasCerrada = new List<double> { -mordazas.Y1, mordazas.Y2 }.Min();
            return MordazaMasCerrada > -10;
        }

        public static bool? IsoFueraDeCampoSoloY(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (IsoFueraDeCampoSoloY(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }



        public static bool? DynamicMLC(Beam campo)
        {
            if (campo.ControlPoints.Count > 2)
            {
                float[] valores = new float[campo.ControlPoints.Count()];
                for (int i = 0; i < 60; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int cp = 0; cp < campo.ControlPoints.Count(); cp++)
                        {
                            valores[cp] = Math.Abs(campo.ControlPoints[cp].LeafPositions[j, i]);
                        }
                        var max = valores.Max();
                        var min = valores.Min();
                        if (max - min >= 1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        public static bool? DynamicMLC(Plan plan)
        {

            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (DynamicMLC(campo) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? CoincidenciaDosisConPrescripcion(Plan plan)
        {
            return plan.DosisTotal == Math.Round(plan.PlanEclipse.TotalPrescribedDose.Dose, 0) && plan.DosisFraccion == Math.Round(plan.PlanEclipse.UniqueFractionation.PrescribedDosePerFraction.Dose, 2); //Falta DosisDia
        }



        public static bool? IsosSimilaresEnPlanSuma(Plan plan)
        {
            if (plan.EsPlanSuma)
            {
                List<PlanSetup> planesEclipse = plan.planesEclipse();
                foreach (PlanSetup etapaA in planesEclipse)
                {
                    foreach (PlanSetup etapaB in planesEclipse)
                    {
                        if (MetodosAuxiliares.isosDiferentes(etapaA, etapaB))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return null;


        }

        public static bool? PlanesEnMismoEquipo(Plan plan)
        {
            if (plan.EsPlanSuma)
            {
                List<PlanSetup> planesEclipse = plan.planesEclipse();
                string IdEquipo = planesEclipse.First().Beams.First().TreatmentUnit.Id;
                foreach (PlanSetup planS in planesEclipse.Skip(1))
                {
                    foreach (Beam campo in planS.Beams)
                    {
                        if (!campo.TreatmentUnit.Id.Equals(IdEquipo))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return null;
        }
        public static bool? TieneTablaDeTolerancia(Plan plan)
        {
            return !plan.PlanEclipse.Beams.Any(b => string.IsNullOrEmpty(b.ToleranceTableLabel));
        }

        public static bool? TablaToleranciaCorrecta(Plan plan)
        {
            if (TieneTablaDeTolerancia(plan) == false)
            {
                return null;
            }
            string tablaCorrecta = MetodosAuxiliares.TablaDeTolerancia(plan);
            foreach (Beam campo in plan.PlanEclipse.Beams)
            {
                if (campo.ToleranceTableLabel != tablaCorrecta)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? TargetEsTipoPTV(Plan plan)
        {
            Structure target = plan.PlanEclipse.StructureSet.Structures.First(s => s.Id == plan.PlanEclipse.TargetVolumeID);
            return target.DicomType == "PTV";// || target.Id.ToLower().Contains("ptv");
        }
        public static bool? CTTieneMasDe400Cortes(Plan plan)
        {
            return plan.PlanEclipse.StructureSet.Image.Series.Images.Count() - 1 < 400;
        }
        public static bool? PlanEnEquipo4(Plan plan)
        {
            return plan.PlanEclipse.Beams.First().TreatmentUnit.Id == "D-2300CD";
        }

    }
}
