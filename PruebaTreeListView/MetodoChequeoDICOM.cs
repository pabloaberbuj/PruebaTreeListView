using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public static class MetodoChequeoDICOM
    {
        public static List<EquipoDicomRT> equiposDicomRT()
        {
            List<EquipoDicomRT> equiposDicomRT = new List<EquipoDicomRT>();
            equiposDicomRT.Add(new EquipoDicomRT("Equipo 2", @"\\10.0.0.57\equipo2\DICOM RT", "Equipo 2 6EX"));
            //equiposDicomRT.Add(new EquipoDicomRT("Equipo 3", @"\\10.0.0.57\equipo3\DICOM RT", "2100CMLC"));
            return equiposDicomRT;
        }
        public static bool EquipoEsDicomRT(Plan plan)
        {
            List<EquipoDicomRT> _equiposDicomRT = equiposDicomRT();
            return _equiposDicomRT.Any(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id);
        }


        public static string CarpetaPacienteInicios(Plan plan, string EquipoId)
        {
            if (equiposDicomRT().Any(e => e.ID == EquipoId))
            {
                if (plan.Tecnica == Tecnica.TBI)
                {
                    return equiposDicomRT().First(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id).Path + @"\1 - Inicios\1 - TBI\" + plan.NombreMasID();
                }
                else
                {
                    var IdEclipse = plan.PlanEclipse.Beams.First().TreatmentUnit.Id;
                    var equipo = equiposDicomRT().First(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id);
                    return equiposDicomRT().First(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id).Path + @"\1 - Inicios\" + plan.NombreMasID();
                }
            }
            else
            {
                return null;
            }

        }
        public static string CarpetaPacienteEnTratamiento(Plan plan, string EquipoId)
        {
            if (equiposDicomRT().Any(e => e.ID == EquipoId))
            {
                if (plan.Tecnica == Tecnica.TBI)
                {
                    return equiposDicomRT().First(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id).Path + @"\2 - En tratamiento\1 - TBI\" + plan.NombreMasID();
                }
                else
                {
                    return equiposDicomRT().First(e => e.ID == plan.PlanEclipse.Beams.First().TreatmentUnit.Id).Path + @"\2 - En tratamiento\" + plan.NombreMasID();
                }
            }
            else
            {
                return null;
            }
        }

        public static string CarpetaPaciente(Plan plan)
        {
            
            string carpetaEnTto = CarpetaPacienteEnTratamiento(plan, plan.PlanEclipse.Beams.First().TreatmentUnit.Id);
            if (Directory.Exists(carpetaEnTto))
            {
                return carpetaEnTto;
            }
            else
            {
                string carpetaInicios = CarpetaPacienteInicios(plan, plan.PlanEclipse.Beams.First().TreatmentUnit.Id);
                if (Directory.Exists(carpetaInicios))
                {
                    return carpetaInicios;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string CarpetaPlan(Plan plan)
        {
            if (plan.EsPlanSuma)
            {
                string carpetaPaciente = CarpetaPaciente(plan);

                if (Directory.GetDirectories(carpetaPaciente).Any(d => new DirectoryInfo(d).Name == plan.PlanEclipse.Id))
                {
                    return Directory.GetDirectories(carpetaPaciente).First(d => new DirectoryInfo(d).Name == plan.PlanEclipse.Id);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return CarpetaPaciente(plan);
            }
        }

        public static Dcm ObtenerDCM(Plan plan)
        {
            string archivoDCM = Directory.GetFiles(CarpetaPaciente(plan)).First(f => Path.GetExtension(f) == ".dcm");
            return new Dcm(archivoDCM);
        }

        public static bool? ExisteCarpetaEnEquipo(Plan plan)
        {
            string EquipoId = plan.PlanEclipse.Beams.First().TreatmentUnit.Id;
            return Directory.Exists(CarpetaPacienteInicios(plan, EquipoId)) || Directory.Exists(CarpetaPacienteEnTratamiento(plan, EquipoId));
        }

        public static bool? NoExisteCarpetaEnOtroEquipo(Plan plan)
        {
            foreach (EquipoDicomRT equipo in equiposDicomRT().Where(e => e.ID != plan.PlanEclipse.Beams.First().TreatmentUnit.Id))
            {
                if (Directory.Exists(CarpetaPacienteInicios(plan, equipo.ID)) || Directory.Exists(CarpetaPacienteEnTratamiento(plan, equipo.ID)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool? ExistenCarpetasDeTodosLosSubPlanes(Plan plan)
        {
            if (!equiposDicomRT().Any(e => e.ID == plan.PlanesSumandos.First().PlanEclipse.Beams.First().TreatmentUnit.Id))
            {
                return null;
            }
            foreach (var subPlan in plan.PlanesSumandos)
            {
                if (!Directory.GetDirectories(CarpetaPaciente(subPlan)).Any(d => new DirectoryInfo(d).Name == subPlan.PlanEclipse.Id))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool? HayUnicoDicomEnCarpetaPlan(Plan plan)
        {
            var archivos = Directory.GetFiles(CarpetaPaciente(plan));
            var extensiones = Path.GetExtension(archivos.First());
            return Directory.GetFiles(CarpetaPaciente(plan)).Where(f => Path.GetExtension(f) == ".dcm").Count() == 1;
        }

        public static bool? ArchivoCoincideConPlan(Plan plan)
        {
            if (ExisteCarpetaEnEquipo(plan) == false || HayUnicoDicomEnCarpetaPlan(plan) == false)
            {
                return null;
            }
            else
            {
                return ObtenerDCM(plan).coincide(plan);
            }
        }

        public static bool? DicomTieneTiempos(Plan plan)
        {
            if (ArchivoCoincideConPlan(plan) == false)
            {
                return null;
            }
            else
            {
                Dcm dcm = ObtenerDCM(plan);
                foreach (Dcm.CampoDCM campoDCM in dcm.camposDCM)
                {
                    if (!campoDCM.TieneTiempo)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool? DicomEsCompatibleConConsolaVieja(Plan plan)
        {
            if (ArchivoCoincideConPlan(plan) == false)
            {
                return null;
            }
            Dcm dcm = ObtenerDCM(plan);
            if (dcm.XML.Contains("FieldType"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public struct EquipoDicomRT
        {
            public string Nombre;
            public string Path;
            public string ID;

            public EquipoDicomRT(string _Nombre, string _Path, string _ID)
            {
                Nombre = _Nombre;
                Path = _Path;
                ID = _ID;
            }
        }


    }
}
