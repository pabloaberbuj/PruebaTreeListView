using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class MetodoChequeoExactrac
    {
        public static string RutaExactracEq4 = @"\\ET6XWIN10\fileRef";
        public static bool HayConexionExactracEq4()
        {
            return Directory.Exists(RutaExactracEq4);
        }

        public static bool? EstaExportadoPlanAExactrac(Plan plan)
        {
            string[] carpetas = CarpetaPaciente(plan);
            List<string> rps = new List<string>();
            if (carpetas != null && carpetas.Count() > 0)
            {
                foreach (string carpeta in carpetas)
                {
                    rps.AddRange(Directory.GetFiles(carpeta, "RTPLAN*", SearchOption.AllDirectories).ToList());
                }
                if (rps.Count > 0)
                {
                    foreach (string rp in rps)
                    {
                        Dcm dcm = new Dcm(rp);
                        if (dcm.coincide(plan))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int CTsExportadas(Plan plan)
        {
            string[] carpetas = CarpetaPaciente(plan);
            List<string> cts = new List<string>();
            if (carpetas != null && carpetas.Count()>0)
            {
                foreach (string carpeta in carpetas)
                {
                    cts.AddRange(Directory.GetFiles(carpeta, "CT*", SearchOption.AllDirectories).ToList());
                }
                return cts.Count;
            }
            return 0;

        }
        public static bool? EstaExportadaCT(Plan plan)
        {
            if (CTsExportadas(plan) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool? SeExportaronMenosDe400Cortes(Plan plan)
        {
            if (MetodosChequeoEclipse.CTTieneMasDe400Cortes(plan) != null)
            {
                if (CTsExportadas(plan) < 400)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return null;
            }
        }
        public static bool? SeExportoSetDeEstructuras(Plan plan)
        {
            string[] carpetas = CarpetaPaciente(plan);
            if (carpetas != null && carpetas.Count() > 0)
            {
                foreach (string carpeta in carpetas)
                {
                    if (Directory.GetFiles(carpeta, "RTSTRUCT*", SearchOption.AllDirectories).Length> 0)
                    {
                        return true;
                    }
                }
            }
            return false;

        }


        public static string[] CarpetaPaciente(Plan plan)
        {
            if (HayConexionExactracEq4())
            {
                var subcarpetas = Directory.GetDirectories(RutaExactracEq4);
                if (subcarpetas.Any(s => s.Contains(plan.PlanEclipse.Course.Patient.LastName.Replace(" ", "").ToUpper())))
                {
                    string apellidonombre = (plan.PlanEclipse.Course.Patient.LastName.ToUpper() + plan.PlanEclipse.Course.Patient.FirstName.ToUpper()).Replace(" ", "");
                    return subcarpetas.Where(s => s.Contains(apellidonombre)).ToArray();
                }
            }
            return null;
        }
    }
}
