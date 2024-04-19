using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using EvilDICOM;
using EvilDICOM.Core.Helpers;
using EvilDICOM.Core.IO.Writing;
using EvilDICOM.Core.IO.Data;
using EvilDICOM.RT;
using EvilDICOM.Core.Selection;
using System.Net;


namespace PruebaTreeListView
{
    public class Dcm
    {
        public string path { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string ID { get; set; }
        public string planID { get; set; }
        public List<CampoDCM> camposDCM { get; set; }
        public string XML { get; set; }

        public bool coincide(Plan plan)
        {
            List<Beam> camposECL = plan.PlanEclipse.Beams.Where(b => b.IsSetupField == false).ToList();
            if (Apellido != plan.PlanEclipse.Course.Patient.LastName || Nombre != plan.PlanEclipse.Course.Patient.FirstName || ID != plan.PlanEclipse.Course.Patient.Id || planID != plan.PlanEclipse.Id || camposDCM.Count != camposECL.Count())
            {
                //MessageBox.Show(Apellido + " " + Nombre + " " + ID + " " + camposDCM.Count.ToString());
                //MessageBox.Show(Path.GetFileName(path) + " no coincide 1");
                return false;
            }
            else
            {
                foreach (CampoDCM _CampoDCM in camposDCM)
                {
                    Beam campoECL = camposECL.Where(c => c.Id == _CampoDCM.ID).First();
                    if (camposECL == null || _CampoDCM.UM != Convert.ToInt32(Math.Round(campoECL.Meterset.Value)))
                    {
                        //MessageBox.Show(Path.GetFileName(path) + " no coincide 2");
                        return false;
                    }
                    //chequeo = _CampoDCM.gantry == campoECL.ControlPoints.First().GantryAngle;
                    //chequeo = _CampoDCM.colimador == campoECL.ControlPoints.First().CollimatorAngle;
                    //chequeo = _CampoDCM.camilla == campoECL.ControlPoints.First().PatientSupportAngle;

                }
            }
            return true;


        }

        /*public static List<string> listaDCM()
        {
            return Directory.GetFiles(Properties.Settings.Default.PathDCMRP, "*.dcm").ToList();
        }*/

        public Dcm(string archivo)
        {
            var objeto = EvilDICOM.Core.DICOMObject.Read(archivo);
            try
            {
                //DATOS
                string nombre = objeto.FindFirst("00100010").DData.ToString();
                nombre = nombre.Replace("??", "Ñ");
                string[] aux = nombre.Split('^');
                //Dcm dcm = new Dcm();
                Apellido = aux[0];
                Nombre = aux[1];
                ID = objeto.FindFirst("00100020").DData.ToString();
                planID = objeto.FindFirst("300A0002").DData.ToString();
                path = archivo;

                camposDCM = new List<CampoDCM>();

                var Beams = objeto.FindFirst("300A00B0").DData_;
                var RefBeams = objeto.FindFirst("300C0004").DData_;
                var XMLStream = objeto.FindFirst("32531000").DData_.Cast<byte>().ToArray();
                XML = Encoding.Default.GetString((byte[])XMLStream);

                foreach (var beam in Beams)
                {
                    if ((string)((EvilDICOM.Core.DICOMObject)beam).FindFirst("300A00CE").DData=="SETUP")
                    {
                        break;
                    }
                    var ControlPoints = ((EvilDICOM.Core.DICOMObject)beam).FindFirst("300A0111").DData_;
                    CampoDCM CampoDCM = new CampoDCM();
                    CampoDCM.ID = (string)((EvilDICOM.Core.DICOMObject)beam).FindFirst("300A00C2").DData;
                    foreach (var refBeam in RefBeams)
                    {
                        if (((EvilDICOM.Core.DICOMObject)refBeam).FindFirst("300C0006").DData.Equals(((EvilDICOM.Core.DICOMObject)beam).FindFirst("300A00C0").DData))
                        {
                            //((EvilDICOM.Core.DICOMObject)refBeam).TryGetDataValue<double>(TagHelper.BeamMeterset,0)
                            CampoDCM.UM = Convert.ToInt32(Math.Round((double)((EvilDICOM.Core.DICOMObject)refBeam).FindFirst("300A0086").DData));
                            CampoDCM.TieneTiempo = ((EvilDICOM.Core.DICOMObject)refBeam).FindFirst("32491000") != null;
                            break;
                        }
                    }
                    CampoDCM.ControlPoints = new List<ControlPointDCM>();
                    foreach (var controlPoint in ControlPoints)
                    {
                        ControlPointDCM cp = new ControlPointDCM();
                        cp.Index = (int)((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A0112").DData;
                        if (CampoDCM.ControlPoints.Count == 0)
                        {
                            cp.Gantry = (double)((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A011E").DData;
                            cp.Colimador = (double)((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A0120").DData;
                            cp.Camilla = (double)((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A0122").DData;
                        }
                        else
                        {
                            cp.Gantry = ExtraerSiExiste((EvilDICOM.Core.DICOMObject)controlPoint, "300A011E", CampoDCM.ControlPoints.Last().Gantry, 0); //ESTA EN IEC
                            cp.Colimador = ExtraerSiExiste((EvilDICOM.Core.DICOMObject)controlPoint, "300A0120", CampoDCM.ControlPoints.Last().Colimador, 0); //ESTA EN IEC
                            cp.Camilla = ExtraerSiExiste((EvilDICOM.Core.DICOMObject)controlPoint, "300A0122", CampoDCM.ControlPoints.Last().Camilla, 0); //ESTA EN IEC
                        }
                        try
                        {
                            //IList BeamLimitingSeq = new IList<object>();
                            if (((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A011A") != null)
                            {
                                var BeamLimitingSeq = ((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A011A").DData_;
                                foreach (var BeamLimDev in BeamLimitingSeq)
                                {
                                    if ((string)((EvilDICOM.Core.DICOMObject)BeamLimDev).FindFirst("300A00B8").DData == "MLCX")
                                    {
                                        cp.MLCPosit = new double[120];
                                        cp.MLCPosit = ((List<double>)(((EvilDICOM.Core.DICOMObject)BeamLimDev).FindFirst("300A011C").DData_)).ToArray();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                cp.MLCPosit = CampoDCM.ControlPoints.Last().MLCPosit;
                            }




                        }
                        catch (Exception)
                        {

                        }
                        cp.CummulativeWeight = (double)((EvilDICOM.Core.DICOMObject)controlPoint).FindFirst("300A0134").DData;


                        CampoDCM.ControlPoints.Add(cp);
                    }
                    camposDCM.Add(CampoDCM);
                }
            }
            catch (Exception)
            {
            }

        }

        public Dcm(PlanSetup planEclipse)
        {
            Apellido = planEclipse.Course.Patient.LastName;
            Nombre = planEclipse.Course.Patient.FirstName;
            ID = planEclipse.Course.Patient.Id;
            planID = planEclipse.Id;
            camposDCM = new List<CampoDCM>();
            foreach (Beam beam in planEclipse.Beams.Where(b => !b.IsSetupField))
            {
                CampoDCM campoDCM = new CampoDCM();
                campoDCM.ID = beam.Id;
                campoDCM.UM = Convert.ToInt32(beam.Meterset.Value);
                campoDCM.ControlPoints = new List<ControlPointDCM>();
                foreach (ControlPoint cp in beam.ControlPoints)
                {
                    int i = 0;
                    ControlPointDCM cpDCM = new ControlPointDCM();
                    cpDCM.Camilla = cp.PatientSupportAngle;
                    cpDCM.Gantry = cp.GantryAngle;
                    cpDCM.Colimador = cp.CollimatorAngle;
                    cpDCM.CummulativeWeight = cp.MetersetWeight;
                    cpDCM.Index = i;
                    //cpDCM.MLCPosit = cp.LeafPositions.Cast<float>().ToArray().Select(x=>Convert.ToDouble(x));
                    cpDCM.MLCPosit = cp.LeafPositions.Cast<float>().Select(x => Convert.ToDouble(x)).ToArray();
                    campoDCM.ControlPoints.Add(cpDCM);
                    i++;
                }
                camposDCM.Add(campoDCM);
            }
        }

        public T ExtraerSiExiste<T>(EvilDICOM.Core.DICOMObject dICOMObject, string tag, T defaultValue1, T defaultValue2)
        {
            var estructura = dICOMObject.FindFirst(tag);
            if (estructura != null)
            {
                return (T)(estructura.DData);
            }
            else if (defaultValue1 != null)
            {
                return defaultValue1;
            }
            else
            {
                return defaultValue2;
            }
        }
        /*public static string obtenerDCM(Patient paciente, PlanSetup plan)
        {
            foreach (string dcmPath in listaDCM())
            {
                Dcm dcm = new Dcm();
                dcm.crear(dcmPath);
                if (dcm.coincide(paciente, plan))
                {
                    return dcmPath;
                }
            }
            return "No se encontró coincidencia";
        }*/

        /*public static bool moverDCM(Patient paciente, PlanSetup plan, bool esPlanSuma, bool vieneDeEq1oEq4=false, string equipoOrigen=null, string equipoDestino=null)
        {
            string path = obtenerDCM(paciente, plan);
            if (path != "No se encontró coincidencia")
            {
                string reingresoCurso = plan.Course.Id[1].ToString();
                string reingresoID = paciente.Id.Last().ToString();
                string IdCorregida = paciente.Id;
                if (reingresoCurso!=reingresoID)
                {
                    MessageBox.Show("El dígito de reingreso en el curso es " + reingresoCurso + " y difiere del hallado en la HC del paciente en Eclipse. Se toma el del curso para el nombre de la carpeta en DicomRT");
                    IdCorregida = paciente.Id.Remove(paciente.Id.Length - 1, 1) + reingresoCurso;
                }
                string pathPaciente="";
                if (plan.Beams.First().TreatmentUnit.Id=="2100CMLC")
                {
                    pathPaciente = Properties.Settings.Default.PathDCMEquipo + @"\" + paciente.LastName.ToUpper() + ", " + paciente.FirstName + " " + IdCorregida;
                }
                else if (plan.Beams.First().TreatmentUnit.Id== "Equipo 2 6EX")
                {
                    pathPaciente = Properties.Settings.Default.PathDCMEquipo2 + @"\" + paciente.LastName.ToUpper() + ", " + paciente.FirstName + " " + IdCorregida;
                }
                else if (vieneDeEq1oEq4)
                {
                    if (equipoDestino=="2100CMLC")
                    {
                        pathPaciente = Properties.Settings.Default.PathDCMEquipo + @"\" + paciente.LastName.ToUpper() + ", " + paciente.FirstName + " " + IdCorregida + " (" + equipoOrigen + ")";
                    }
                    else if (equipoDestino == "Equipo 2 6EX")
                    {
                        pathPaciente = Properties.Settings.Default.PathDCMEquipo2 + @"\" + paciente.LastName.ToUpper() + ", " + paciente.FirstName + " " + IdCorregida + " (" + equipoOrigen + ")";
                    }
                }
                IO.crearCarpeta(pathPaciente);
                if (esPlanSuma)
                {
                    try
                    {
                        string pathPlan = pathPaciente + @"\" + plan.Id;
                        IO.crearCarpeta(pathPlan);
                        IO.crearCarpeta(pathPlan + @"\BACKUP");
                        IO.moverArchivo(path, pathPlan + @"\" + plan.Id + ".dcm");
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("No se puede acceder a la carpeta\n" + e.ToString());
                    }


                }
                else
                {
                    try
                    {
                        IO.crearCarpeta(pathPaciente + @"\BACKUP");
                        IO.moverArchivo(path, pathPaciente + @"\" + plan.Id + ".dcm");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("No se puede acceder a la carpeta\n" + e.ToString());
                    }
                }



                return true;
            }
            else
            {
                return false;
            }
        }


        }*/

        public struct CampoDCM
        {
            public string ID { get; set; }
            public int UM { get; set; }
            public List<ControlPointDCM> ControlPoints { get; set; }
            public bool TieneTiempo { get; set; }
        }

        public struct ControlPointDCM
        {
            public int Index { get; set; }
            public double Gantry { get; set; }
            public double Colimador { get; set; }
            public double Camilla { get; set; }
            public double[] MLCPosit { get; set; }
            public double CummulativeWeight { get; set; }
        }



    }
}
