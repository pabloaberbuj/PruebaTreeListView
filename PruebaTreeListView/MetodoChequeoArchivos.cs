using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Runtime.Remoting.Messaging;

namespace PruebaTreeListView
{
    public static class MetodoChequeoArchivos
    {
        public static string pathPacientes = @"\\ariamevadb-svr\va_data$\Pacientes";
        public static string pathDRRs = @"\\fisica0\centro de datos2018\000_Centro de Datos 2021\Pacientes DRRs";
        public static bool? HizoCalculoIndependienteFotones(Plan plan)
        {
            string archivoCI = MetodosAuxiliares.ArchivoCI(plan);
            if (archivoCI != null)
            {
                return File.Exists(archivoCI);
            }
            else
            {
                return false;
            }

        }

        public static bool? CalculoIndependienteFotonesEnTolerancia(Plan plan)
        {
            double tol_porciento = 4; // 4 % de tolerancia en dosis por campo
            double tol_dosis = 2;     //2 cGy de tolerancia en dosis por campo

            if (HizoCalculoIndependienteFotones(plan) == false)
            {
                return null;
            }
            else
            {
                string[] input = File.ReadAllLines(MetodosAuxiliares.ArchivoCI(plan));
                foreach (Ecl.Beam campo in plan.PlanEclipse.Beams.Where(b => !b.IsSetupField))
                {
                    try
                    {
                        double sesgoRel = Convert.ToDouble(input.FirstOrDefault(l => l.Split(';')[0] == campo.Id).Split(';')[1]);
                        double sesgoAbs = Convert.ToDouble(input.FirstOrDefault(l => l.Split(';')[0] == campo.Id).Split(';')[2]);
                        if (Math.Abs(sesgoRel) > tol_porciento && Math.Abs(sesgoAbs) <= tol_dosis)
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }
                return true;
            }

        }

        public static bool? TieneInforme(Plan plan)
        {
            return File.Exists(MetodosAuxiliares.ArchivoInforme(plan));
        }

        public static bool? TecnicaEnInformeCorrecta(Plan plan)
        {
            if (TieneInforme(plan)==false)
            {
                return null;
            }
            else
            {
                string textoInforme = MetodosAuxiliares.ObtenerTextoInforme(plan);
                return MetodosAuxiliares.CoincideTextoInformeConTecnica(textoInforme, plan);
                
            }

        }

        public static bool? TieneImagenesDRRenCDD(Plan plan)
        {
            //var carpeta = MetodosAuxiliares.CarpetaDRRs(plan);
            //var archivos = Directory.GetFiles(MetodosAuxiliares.CarpetaDRRs(plan));
            
            if (Directory.Exists(MetodosAuxiliares.CarpetaDRRs(plan)) && Directory.GetFiles(MetodosAuxiliares.CarpetaDRRs(plan)).Any(f => Path.GetExtension(f)==".png"))
            {
                var imagenes = Directory.GetFiles(MetodosAuxiliares.CarpetaDRRs(plan)).Where(f => Path.GetExtension(f) == ".png");
                if (imagenes != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static bool? NumeroDeImagenesCorrectas(Plan plan)
        {
            if(TieneImagenesDRRenCDD(plan) == true)
            {
                var imagenes = Directory.GetFiles(MetodosAuxiliares.CarpetaDRRs(plan)).Where(f => Path.GetExtension(f) == ".png");
                if (plan.Tecnica==Tecnica.Static3DC)
                {
                    return imagenes.Count() >= plan.PlanEclipse.Beams.Count();
                }
                else
                {
                    return imagenes.Count() >= 2;
                }
            }
            else
            {
                return null;
            }
            
            
        }


        public static bool? CorrimientosEnRef2IsoOK(Plan plan)
        {
            VVector iso = MetodosAuxiliares.corregirPorPatientOrientation(plan.PlanEclipse);
            double desplazX = Math.Abs(Math.Round(iso.x / 10, 1));
            double desplazY = Math.Abs(Math.Round(iso.y / 10, 1));
            double desplazZ = Math.Abs(Math.Round(iso.z / 10, 1));
            string sentidoX = iso.x >= 0 ? "Left" : "Right";
            string sentidoY = iso.y >= 0 ? "Up" : "Down";
            string sentidoZ = iso.z >= 0 ? "Out" : "In";
            string Equipo = "";
            Ecl.Patient paciente = plan.PlanEclipse.Course.Patient;
            if (plan.PlanEclipse.Beams.First().TreatmentUnit.Id == "2100CMLC")
            {
                Equipo = "Equipo 3";
            }
            else if (plan.PlanEclipse.Beams.First().TreatmentUnit.Id == "Equipo 2 6EX")
            {
                Equipo = "Equipo 2";
            }
            else if (plan.PlanEclipse.Beams.First().TreatmentUnit.Id == "CL21EX")
            {
                Equipo = "Medrano";
            }
            else
            {
                Equipo = null;
            }
            if (Equipo == null)
            {
                return null;
            }
            List<object> listaPlan = new List<object> { paciente.Id, paciente.LastName.ToUpper() + ", " + paciente.FirstName + "_" + plan.PlanEclipse.Id, "Iso 1", desplazX.ToString(), sentidoX, desplazY.ToString(), sentidoY, desplazZ.ToString(), sentidoZ, DateTime.Today.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), "Eclipse" };
            PacienteCorrimientos pacientePlan = new PacienteCorrimientos(listaPlan);
            List<PacienteCorrimientos> pacientesCargados = new List<PacienteCorrimientos>();
            IList<IList<Object>> TraerLista = MetodosAuxiliares.TraerTodosLosCorrimientos(Equipo);
            foreach (var objeto in TraerLista)
            {
                pacientesCargados.Add(new PacienteCorrimientos(objeto));
            }
            //int ubicacion = pacientesCargados.IndexOf(pacientePlan);
            return pacientesCargados.Contains(pacientePlan);
            
        }

    }

}

