using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.Content.Objects;

namespace PruebaTreeListView
{
    public static class MetodoChequeoDICOM
    {
        public static List<EquipoDicomRT> equiposDicomRT()
        {
            List<EquipoDicomRT> equiposDicomRT = new List<EquipoDicomRT>();
            string fisica0 = IO.RutaFisica0();
            equiposDicomRT.Add(new EquipoDicomRT("Equipo 2", @"\\" + fisica0 + @"\equipo2\DICOM RT", "Equipo 2 6EX"));
            equiposDicomRT.Add(new EquipoDicomRT("Medrano", @"\\" + fisica0 + @"\compartido\DEMEDRANO\Pacientes_Eclipse_Medrano", "CL21EX"));
            //equiposDicomRT.Add(new EquipoDicomRT("Equipo 3", @"\\" + fisica0 + @"\equipo3\DICOM RT", "Equipo3"));
            return equiposDicomRT;
        }
        public static bool EquipoEsDicomRT(Plan plan)
        {
            List<EquipoDicomRT> _equiposDicomRT = equiposDicomRT();
            return _equiposDicomRT.Any(e => e.ID == plan.Equipo());
        }


        public static string CarpetaPacienteInicios(Plan plan, string EquipoId)
        {
            if (equiposDicomRT().Any(e => e.ID == EquipoId))
            {
                if (plan.Tecnica == Tecnica.TBI)
                {
                    return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\1 - Inicios\1 - TBI\" + plan.nombreMasIDCorregida;
                }
                else
                {
                    var IdEclipse = plan.Equipo();
                    var equipo = equiposDicomRT().First(e => e.ID == EquipoId);

                    if (EquipoId == "CL21EX")
                    {
                        return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\" + plan.nombreMasIDDRR + @"\" + plan.PlanEclipse.Id + " (" + plan.PlanEclipse.Course.Id + @")\";
                    }
                    return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\1 - Inicios\" + plan.nombreMasIDCorregida;
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
                    return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\2 - En tratamiento\1 - TBI\" + plan.nombreMasIDCorregida;
                }
                else
                {
                    if (EquipoId == "CL21EX")
                    {
                        return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\" + plan.nombreMasIDDRR + @"\" + plan.PlanEclipse.Id + " (" + plan.PlanEclipse.Course.Id + @")\";
                    }
                    return equiposDicomRT().First(e => e.ID == EquipoId).Path + @"\2 - En tratamiento\" + plan.nombreMasIDCorregida;
                }
            }
            else
            {
                return null;
            }
        }

        public static string CarpetaPaciente(Plan plan)
        {
            string carpetaEnTto = CarpetaPacienteEnTratamiento(plan, plan.Equipo());
            if (Directory.Exists(carpetaEnTto))
            {
                return carpetaEnTto;
            }
            else
            {
                string carpetaInicios = CarpetaPacienteInicios(plan, plan.Equipo());
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
            if (ExisteCarpetaEnEquipo(plan) == true)
            {
                string archivoDCM = Directory.GetFiles(CarpetaPaciente(plan)).First(f => Path.GetExtension(f) == ".dcm" && !Path.GetFileName(f).ToLower().Contains("record"));
                return new Dcm(archivoDCM);
            }
            return null;

        }

        public static bool? ExisteCarpetaEnEquipo(Plan plan)
        {
            string EquipoId = plan.Equipo();
            if (EquipoId == "CL21EX")
            {
                return null;
            }
            return Directory.Exists(CarpetaPacienteInicios(plan, EquipoId)) || Directory.Exists(CarpetaPacienteEnTratamiento(plan, EquipoId));
        }

        public static bool? NoRealizoAplicacionesDicomRT(Plan plan)
        {
            string EquipoId = plan.Equipo();
            if (EquipoId == "CL21EX")
            {
                return null;
            }
            if (ExisteCarpetaEnEquipo(plan) == false)
            {
                return null;
            }
            else if (Directory.Exists(CarpetaPacienteEnTratamiento(plan, EquipoId)))
            {
                var carpetabackup = Directory.GetDirectories(CarpetaPacienteEnTratamiento(plan, EquipoId)).First(d => d.ToLower().Contains("bac"));
                if (Directory.GetFiles(carpetabackup).Count() > 0)
                {
                    return Directory.GetFiles(carpetabackup).Where(f => f.ToLower().Contains("record")).Count() == 0;
                }
            }
            return true;

        }

        public static bool? NoExisteCarpetaEnOtroEquipo(Plan plan)
        {
            foreach (EquipoDicomRT equipo in equiposDicomRT().Where(e => e.ID != plan.Equipo()))
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
            if (plan.PlanesSumandos==null || plan.PlanesSumandos.Count==0)
            {
                return null;
            }
            if (!equiposDicomRT().Any(e => e.ID == plan.PlanesSumandos.First().Equipo()))
            {
                return null;
            }
            foreach (var subPlan in plan.PlanesSumandos)
            {
                //if (!Directory.GetDirectories(CarpetaPaciente(subPlan)).Any(d => new DirectoryInfo(d).Name == subPlan.PlanEclipse.Id))
                if (!Directory.Exists(CarpetaPaciente(subPlan)))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool? HayUnicoDicomEnCarpetaPlan(Plan plan)
        {
            if (ExisteCarpetaEnEquipo(plan) == true)
            {
                var archivos = Directory.GetFiles(CarpetaPaciente(plan));
                var extensiones = Path.GetExtension(archivos.First());
                return Directory.GetFiles(CarpetaPaciente(plan)).Where(f => Path.GetExtension(f) == ".dcm" && !Path.GetFileNameWithoutExtension(f).Contains("RI") && !Path.GetFileNameWithoutExtension(f).Contains("CT") && !Path.GetFileNameWithoutExtension(f).Contains("RS")).Count()  == 1;
            }
            else
            {
                return null;
            }

        }

        public static bool? ArchivoCoincideConPlan(Plan plan)
        {
            if (ExisteCarpetaEnEquipo(plan) != true || HayUnicoDicomEnCarpetaPlan(plan) != true)
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
            if (ExisteCarpetaEnEquipo(plan) != true || HayUnicoDicomEnCarpetaPlan(plan) != true || ArchivoCoincideConPlan(plan) != true)
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
            if (plan.Equipo()=="Equipo3" || plan.Equipo()=="CL21EX")
            {
                return null;
            }
            if (ExisteCarpetaEnEquipo(plan) != true || HayUnicoDicomEnCarpetaPlan(plan) != true || ArchivoCoincideConPlan(plan) != true)
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
