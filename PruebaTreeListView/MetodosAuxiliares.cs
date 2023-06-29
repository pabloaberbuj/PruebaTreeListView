using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using AriaQ;
using System.Net.NetworkInformation;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;


namespace PruebaTreeListView
{
    public static class MetodosAuxiliares
    {
        public static bool CoincidenciaCamillas(string camilla, VMS.TPS.Common.Model.API.PlanSetup plan)
        {
            string equipo = plan.Beams.First().TreatmentUnit.Id;
            if (camilla.Contains("Unipanel") && equipo == "PBA_6EX_730")
            {
                return true;
            }
            else if (camilla.Contains("Unipanel") && equipo == "6EX Viamonte")
            {
                return true;
            }
            else if (camilla.Contains("IGRT") && equipo == "Equipo1")
            {
                return true;
            }
            else if (camilla.Contains("IGRT") && equipo == "2100CMLC")
            {
                return true;
            }
            else if (camilla.Contains("IGRT") && equipo == "Equipo 2 6EX")
            {
                return true;
            }
            else if (camilla.Contains("BrainLAB") && equipo == "D-2300CD")
            {
                if (esRadioCirugia(plan))
                {
                    if (camilla.Contains("H&N Extension"))
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
                    if (camilla.Contains("H&N Extension"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else if (camilla.Contains("BL_ICT") && equipo == "D-2300CD")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool camposFusionables(Ecl.Beam campo1, Ecl.Beam campo2)
        {
            if (campo1.ControlPoints.First().GantryAngle != campo2.ControlPoints.First().GantryAngle)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().CollimatorAngle != campo2.ControlPoints.First().CollimatorAngle)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().PatientSupportAngle != campo2.ControlPoints.First().PatientSupportAngle)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().JawPositions.X1 != campo2.ControlPoints.First().JawPositions.X1)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().JawPositions.X2 != campo2.ControlPoints.First().JawPositions.X2)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().JawPositions.Y1 != campo2.ControlPoints.First().JawPositions.Y1)
            {
                return false;
            }
            else if (campo1.ControlPoints.First().JawPositions.Y2 != campo2.ControlPoints.First().JawPositions.Y2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static double distanciaMaxima(VVector punto1, VVector punto2)
        {
            double difX = Math.Abs(punto1.x - punto2.x);
            double difY = Math.Abs(punto1.y - punto2.y);
            double difZ = Math.Abs(punto1.z - punto2.z);
            return Math.Max(difX, Math.Max(difY, difZ));
        }

        public static bool isosDiferentes(VMS.TPS.Common.Model.API.PlanSetup plan1, VMS.TPS.Common.Model.API.PlanSetup plan2)
        {
            if (distanciaMaxima(plan1.Beams.First().IsocenterPosition, plan2.Beams.First().IsocenterPosition) > 0.01 &&
                distanciaMaxima(plan1.Beams.First().IsocenterPosition, plan2.Beams.First().IsocenterPosition) < 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool esRadioCirugia(VMS.TPS.Common.Model.API.PlanSetup plan)
        {
            if (plan.Beams.First().TreatmentUnit.Id == "D-2300CD" && plan.StructureSet!=null && plan.StructureSet.Image.UserOrigin.Equals(new VVector(0, 0, 0))) //es en el equipo4
            {
                if (plan.Beams.First().EnergyModeDisplayName == "6X-SRS") //haz SRS
                {
                    return true;
                }
                else if (plan.Beams.First().MLCPlanType == MLCPlanType.VMAT && plan.UniqueFractionation.PrescribedDosePerFraction.Dose > 390) //es VMAt y el origen es dicom y la dosis prescripta es >390cGy
                {
                    if (plan.StructureSet.Structures.Any(s => s.Id.Contains("Brain")) || plan.StructureSet.Structures.Any(s => s.Id.Contains("Cerebro"))) //hay alguna estructura que contiene Brain o cerebro
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool esSBRT(VMS.TPS.Common.Model.API.PlanSetup plan)
        {
            if (plan.Beams.First().TreatmentUnit.Id == "D-2300CD") //es en el equipo4
            {
                if (plan.Beams.First().EnergyModeDisplayName == "6X-SRS") //haz SRS
                {
                    return true;
                }
                else if (plan.Beams.First().MLCPlanType == MLCPlanType.VMAT && plan.UniqueFractionation.PrescribedDosePerFraction.Dose > 590) //es VMAt y la dosis prescripta es >590cGy
                {
                    return true;
                }
            }
            return false;
        }

        public static double IECaVarian(double valorIEC)
        {
            if (valorIEC <= 180)
            {
                return 180 - valorIEC;
            }
            else
            {
                return 540 - valorIEC;
            }
        }

        /*public static AriaQ.PlanSetup AriaPlanDeEclipse(Aria aria, VMS.TPS.Common.Model.API.PlanSetup planEclipse)
        {
            return aria.PlanSetups.FirstOrDefault(p => p.PlanSetupId == planEclipse.Id && p.Course.CourseId == planEclipse.Course.Id && p.Course.Patient.PatientId == planEclipse.Course.Patient.Id);
        }*/
        public static AriaQ.PlanSetup AriaPlanDeEclipse(Aria aria, VMS.TPS.Common.Model.API.PlanSetup planEclipse)
        {
            var paciente = aria.Patients.FirstOrDefault(p => p.PatientId == planEclipse.Course.Patient.Id);
            var curso = paciente.Courses.FirstOrDefault(c => c.CourseId == planEclipse.Course.Id);
            return curso.PlanSetups.FirstOrDefault(p => p.PlanSetupId == planEclipse.Id);
        }


        public static string CarpetaDRRs(Plan plan)
        {
            return MetodoChequeoArchivos.pathDRRs + @"\" + NombreEquipoDRRs(plan.PlanEclipse) + @"\" + plan.NombreMasIDDRR() + @"\" + plan.PlanEclipse.Id + " (" + plan.PlanEclipse.Course.Id + @")\";
        }

        public static string TablaDeTolerancia(Plan plan)
        {
            if (plan.EsCamillaEspecial)
            {
                return "T_CamEspecial";
            }
            if (plan.Tecnica == Tecnica.IGRT || plan.Tecnica == Tecnica.RC_HazSRS || plan.Tecnica == Tecnica.RC_VMAT || plan.Tecnica == Tecnica.SBRT_HazSRS || plan.Tecnica == Tecnica.SBRT_VMAT)
            {
                return "T_IGRT";
            }
            else if (plan.Tecnica == Tecnica.Electrones)
            {
                return "T_Elec";
            }
            else if (plan.Tecnica == Tecnica.TBI)
            {
                return "T_TBI";
            }
            else
            {
                return "T1";
            }
        }

        public static string GetUniqueFilename(string path, string baseName, string extention = "txt", int maxAttempts = 128)
        {
            if (!File.Exists(string.Format("{0}{1}.{2}", path, baseName, extention)))
            {
                return string.Format("{0}{1}.{2}", path, baseName, extention);
            }
            else
            {
                for (int i = 1; i < maxAttempts; i++)
                {
                    if (!File.Exists(string.Format("{0}{1} ({2}).{3}", path, baseName, i, extention)))
                    {
                        return string.Format("{0}{1} ({2}).{3}", path, baseName, i, extention);
                    }
                }
            }
            return string.Format("{0}{1} - {2:yyyy-MM-dd_hh-mm-ss}.{3}", path, baseName, DateTime.Now, extention);
        }

        public static string ArchivoCI(Plan plan)
        {
            string pathDirectorio = MetodoChequeoArchivos.pathPacientes + @"\" + plan.NombreMasIDDRR();
            Ecl.PlanSetup planCI = PlanCI(plan);
            if (planCI != null)
            {
                string aux = planCI.Id.Replace(':', '_').Replace('\\', '_').Replace('/', '_') + " (" + plan.PlanAria.Course.CourseId + ")";
                return pathDirectorio + @"\" + aux + @"\" + plan.NombreMasIDDRR() + "_CI.txt";
            }
            else
            {
                return null;
            }
            //string[] carpetasPosibles = Directory.GetDirectories(pathDirectorio);

        }

        public static string ArchivoInforme(Plan plan)
        {
            string pathDirectorio = MetodoChequeoArchivos.pathPacientes + @"\" + plan.NombreMasIDDRR();
            if (plan.EsPlanSuma) //ver si hay que cambiarlo
            {
                return pathDirectorio + @"\" + plan.PlanEclipse.Id + " (" + plan.PlanEclipse.Course.Id + @")\" + plan.NombreMasIDDRR() + "_Informe.pdf";
            }
            return pathDirectorio + @"\" + plan.PlanEclipse.Id + " (" + plan.PlanEclipse.Course.Id + @")\" + plan.NombreMasIDDRR() + "_Informe.pdf";
        }

        public static string ObtenerTextoInforme(Plan plan)
        {
            PdfDocument informe = PdfDocument.Open(ArchivoInforme(plan));
            var primerPagina = informe.GetPages().First();
            var lista = primerPagina.GetWords().Select(w => w.Text).ToArray();
            return String.Join(" ", lista);
        }


        public static Ecl.PlanSetup PlanCI(Plan plan)
        {
            foreach (Ecl.PlanSetup ps in plan.PlanEclipse.Course.PlanSetups)
            {
                if (ps.Id != plan.PlanEclipse.Id && ps.Id.ToUpper().Contains("CI"))
                {
                    if (PlanesEquivalentes(ps, plan.PlanEclipse))
                    {
                        return ps;
                    }
                }
            }
            return null;
        }

        public static bool CamposIguales(Ecl.Beam campo1, Ecl.Beam campo2)
        {
            return (campo1.Id == campo2.Id && campo1.Meterset.Value == campo2.Meterset.Value && campo1.ControlPoints.First().JawPositions.Equals(campo2.ControlPoints.First().JawPositions)
                && campo1.ControlPoints.First().GantryAngle == campo2.ControlPoints.First().GantryAngle && campo1.WeightFactor == campo2.WeightFactor);
        }

        public static bool PlanesEquivalentes(Ecl.PlanSetup plan1, Ecl.PlanSetup plan2)
        {
            if (plan1.Beams.Where(b => !b.IsSetupField).Count() != plan2.Beams.Where(b => !b.IsSetupField).Count())
            {
                return false;
            }
            else
            {
                foreach (Ecl.Beam campo1 in plan1.Beams)
                {
                    if (!plan2.Beams.Any(b => CamposIguales(b, campo1)))
                    {
                        return false;
                    }
                }
            }
            return true;

        }
        private static string NombreEquipoDRRs(Ecl.PlanSetup plan)
        {
            if (plan.Beams.First().TreatmentUnit.Id == "Equipo1")
            {
                return "Equipo1";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "Equipo 2 6EX")
            {
                return "Equipo2";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "2100CMLC")
            {
                return "Equipo3";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "D-2300CD")
            {
                return "Equipo4";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "CL21EX")
            {
                return "EquipoMedrano";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "PBA_6EX_730")
            {
                return "EquipoCetro";
            }
            else
            {
                return null;
            }
        }

        public static string NombreEquiposRef2Iso(Ecl.PlanSetup plan)
        {
            if (plan.Beams.First().TreatmentUnit.Id == "Equipo 2 6EX")
            {
                return "Equipo 2";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "2100CMLC")
            {
                return "Equipo 3";
            }
            else if (plan.Beams.First().TreatmentUnit.Id == "CL21EX")
            {
                return "Medrano";
            }
            else
            {
                return null;
            }
        }

        #region googleDrive
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "UploadPatMove";


        public static IList<IList<Object>> TraerTodosLosCorrimientos(string Equipo)
        {
            Ping p1 = new Ping();
            PingReply PR = p1.Send("drive.google.com");
            // check when the ping is not success
            if (!PR.Status.ToString().Equals("Success"))
            {
                //MessageBox.Show("No se puede conectar con google drive\nReintentar en un rato");
                return null;
            }
            UserCredential credential;
            // Load client secrets.
            using (var stream =
                   new FileStream(@"\\ariamevadb-svr\va_data$\Calculo Independiente\credentials.json", FileMode.Open, FileAccess.Read))
            {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                string credPath = @"\\ariamevadb-svr\va_data$\Calculo Independiente\token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                                    Scopes,
                                    "user",
                                    CancellationToken.None,
                                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,

            });

            // Define request parameters.
            String spreadsheetId = "1HvxYpnQAe3eklrKRYf79mRkSb5R7ThePgOR7kglN-bE";
            String range = "Pacientes " + Equipo + "!A:J";
            //String range2 = "Hoja 1!F3:H3";
            var valueRange = new ValueRange();
            //List<object> lista = new List<object> {"1-123-1","Ca, Jose","2","Right","3","In","4","Up",DateTime.Today.ToShortDateString()};
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();
            return response.Values;
        }

        /*public static List<Object> Corrimientos(Plan plan)
        {

        }*/
        public static VVector corregirPorPatientOrientation(Ecl.PlanSetup planEclipse)
        {
            VVector isoToRef = restaVectores(planEclipse.Beams.First().IsocenterPosition, planEclipse.StructureSet.Image.UserOrigin);
            VVector nuevoIso = isoToRef;
            if (planEclipse.TreatmentOrientation == PatientOrientation.FeetFirstSupine)
            {
                nuevoIso.x = -1 * isoToRef.x;
                nuevoIso.z = -1 * isoToRef.z;
            }
            else if (planEclipse.TreatmentOrientation == PatientOrientation.FeetFirstProne)
            {
                nuevoIso.y = -1 * isoToRef.y;
                nuevoIso.z = -1 * isoToRef.z;
            }
            else if (planEclipse.TreatmentOrientation == PatientOrientation.HeadFirstProne)
            {
                nuevoIso.x = -1 * isoToRef.x;
                nuevoIso.y = -1 * isoToRef.y;
            }
            return nuevoIso;
        }
        public static VVector restaVectores(VVector v1, VVector v2)
        {
            VVector resta = new VVector();
            resta.x = v1.x - v2.x;
            resta.y = v1.y - v2.y;
            resta.z = v1.z - v2.z;
            return resta;
        }
        #endregion

        public static bool CoincideTextoInformeConTecnica(string informe, Plan plan)
        {
            if (informe.Contains("(SRS)") && (plan.Tecnica == Tecnica.RC_HazSRS || plan.Tecnica == Tecnica.RC_VMAT))
            {
                return true;
            }
            else if (informe.Contains("(SBRT)") && (plan.Tecnica==Tecnica.SBRT_HazSRS || plan.Tecnica==Tecnica.SBRT_VMAT))
            {
                return true;
            }
            else if (informe.Contains("(IGRT)") && plan.Tecnica==Tecnica.IGRT)
            {
                return true;
            }
            else if (informe.Contains("VMAT") && plan.Tecnica==Tecnica.VMAT)
            {
                return true;
            }
            else if (informe.Contains("(IMRT)") && plan.Tecnica==Tecnica.IMRT)
            {
                return true;
            }
            else if (informe.Contains("(3DC)") && (plan.Tecnica==Tecnica.Arcos3DC || plan.Tecnica == Tecnica.Electrones) || plan.Tecnica == Tecnica.Mama3DC || plan.Tecnica == Tecnica.Static3DC) //TBI no tiene informe
            {
                return true;
            }
            return false;
        }
    }

}

