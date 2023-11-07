using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PruebaTreeListView
{
    public delegate bool? MyStaticMethodInvoker(Plan plan);

    public class Chequeo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string nombre;
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre!=value)
                {
                    nombre = value;
                    OnPropertyChanged("Nombre");
                }
            }
        }

        public Categoria Categoria { get; set; }
        public NivelDeAccion NivelDeAccion { get; set; }
        public MyStaticMethodInvoker TargetMethod;

        


        public bool EsAutomatico { get; set; }
        public bool? resultadoTest;
        public bool? ResultadoTest
        {
            get { return resultadoTest; }
            set
            {
                if (resultadoTest != value)
                {
                    resultadoTest = value;
                    OnPropertyChanged("ResultadoTest");
                }
            }
        }

        public bool AplicaAStatic3DC { get; set; }
        //public bool AplicaAMama3DC { get; set; }
        public bool AplicaAArcos3DC { get; set; }
        public bool AplicaAElectrones { get; set; }
        public bool AplicaAIMRT { get; set; }
        public bool AplicaAVMAT { get; set; }
        public bool AplicaAIGRT { get; set; }
        public bool AplicaARC_HazSRS { get; set; }
        public bool AplicaARC_VMAT { get; set; }
        public bool AplicaASBRT_HazSRS { get; set; }
        public bool AplicaASBRT_VMAT { get; set; }
        public bool AplicaATBI { get; set; }
        public bool? ExclusivoPlanSuma { get; set; }
        public bool? ExclusivoCamEspecial { get; set; }
        public bool? ExclusivoEquiposAria { get; set; }
        public bool? ExclusivoEquiposDicomRT { get; set; }
        public bool? ExclusivoPediatrico { get; set; }
        public string Descripcion { get; set; }
        public string MensajeSiFalso { get; set; }
        public string Observacion { get; set; }



        public Chequeo(string _Nombre, Categoria _Categoria, NivelDeAccion _NivelDeAccion, MyStaticMethodInvoker _TargetMethod, bool _EsAutomatico,
            bool _AplicaAStatic3DC, bool _AplicaAArcos3DC, bool _AplicaAElectrones, bool _AplicaAIMRT, bool _AplicaAVMAT, bool _AplicaAIGRT,
            bool _AplicaARC_HazSRS, bool _AplicaARC_VMAT, bool _AplicaASBRTRC_HazSRS, bool _AplicaASBRT_VMAT, bool _AplicaATBI,
            bool? _ExclusivoPlanSuma, bool? _ExclusivoCamEspecial, bool? _ExclusivoEquiposAria, bool? _ExclusivoEquiposDicomRT, bool? _ExclusivoPediatrico,
            string _MensajeSiFalso="")
        {
            Nombre = _Nombre;
            Categoria = _Categoria;
            NivelDeAccion = _NivelDeAccion;
            TargetMethod = _TargetMethod;
            EsAutomatico = _EsAutomatico;
            ResultadoTest = null;
            AplicaAStatic3DC = _AplicaAStatic3DC;
            //AplicaAMama3DC = _AplicaAMama3DC;
            AplicaAArcos3DC = _AplicaAArcos3DC;
            AplicaAElectrones = _AplicaAElectrones;
            AplicaAIMRT = _AplicaAIMRT;
            AplicaAVMAT = _AplicaAVMAT;
            AplicaAIGRT = _AplicaAIGRT;
            AplicaARC_HazSRS = _AplicaARC_HazSRS;
            AplicaARC_VMAT = _AplicaARC_VMAT;
            AplicaASBRT_HazSRS = _AplicaASBRTRC_HazSRS;
            AplicaASBRT_VMAT = _AplicaASBRT_VMAT;
            AplicaATBI = _AplicaATBI;
            ExclusivoPlanSuma = _ExclusivoPlanSuma;
            ExclusivoCamEspecial = _ExclusivoCamEspecial;
            ExclusivoEquiposAria = _ExclusivoEquiposAria;
            ExclusivoEquiposDicomRT = _ExclusivoEquiposDicomRT;
            ExclusivoPediatrico = _ExclusivoPediatrico;
            //Descripcion = _Descripcion;
            MensajeSiFalso = _MensajeSiFalso;
            Observacion = "";
        }

        public override string ToString()
        {
            return this.Nombre;
        }
   
        public void AplicarMetodo(Plan plan)
        {
            if (this.EsAutomatico)
            {
                ResultadoTest = TargetMethod.Invoke(plan);
            }

        }

        public bool AplicaTecnica(Tecnica tecnica)
        {
            switch (tecnica)
            {
                case Tecnica.Static3DC:
                    return AplicaAStatic3DC;
                case Tecnica.Arcos3DC:
                    return AplicaAArcos3DC;
                case Tecnica.Electrones:
                    return AplicaAElectrones;
                case Tecnica.IMRT:
                    return AplicaAIMRT;
                case Tecnica.VMAT:
                    return AplicaAVMAT;
                case Tecnica.TBI:
                    return AplicaATBI;
                case Tecnica.IGRT:
                    return AplicaAIGRT;
                case Tecnica.RC_HazSRS:
                    return AplicaARC_HazSRS;
                case Tecnica.RC_VMAT:
                    return AplicaARC_VMAT;
                case Tecnica.SBRT_HazSRS:
                    return AplicaASBRT_HazSRS;
                case Tecnica.SBRT_VMAT:
                    return AplicaASBRT_VMAT;
                default:
                    return false;
            }
        }

        public static List<Chequeo> ListaChequeos()
        {
            List<Chequeo> lista = new List<Chequeo>();
            lista.Add(new Chequeo("Treatment Approval hecho", Categoria.APF, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TreatmentApprovalHecho), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Tiempo VMAT", Categoria.APF, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.TiempoVMAT), true, false, false, false, false, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Hay otro curso activo (Excepto QA y fisica)", Categoria.APF, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.HayOtroCursoActivo), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("CT tiene más de 400 cortes", Categoria.APF, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.CTTieneMasDe400Cortes), true, false, false, false, false, false, true, true, true, true, true, false, false, null, null, null, true));
            lista.Add(new Chequeo("Tiene hecho CI fotones estático", Categoria.CalculoIndep, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.HizoCalculoIndependienteFotones), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Dio bien CI fotones estático", Categoria.CalculoIndep, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.CalculoIndependienteFotonesEnTolerancia), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tiene hecho CI fotones arco o electrones", Categoria.CalculoIndep, NivelDeAccion.Advertencia, null, false, false, true, true, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Dio bien CI fotones arco o electrones", Categoria.CalculoIndep, NivelDeAccion.Advertencia, null, false, false, true, true, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Existe carpeta del paciente en DICOM RT del equipo correspondiente", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.ExisteCarpetaEnEquipo), true, true, true, true, true, false, false, false, false, false, false, true, false, null, false, true, null));
            lista.Add(new Chequeo("No existe carpeta del paciente en DICOM RT de otros equipos", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.NoExisteCarpetaEnOtroEquipo), true, true, true, true, true, false, false, false, false, false, false, true, false, null, false, true, null));
            lista.Add(new Chequeo("Existe la carpeta correspondiente a todos los planes del plan Suma", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.ExistenCarpetasDeTodosLosSubPlanes), true, false, false, false, false, false, false, false, false, false, false, false, true, null, false, true, null));
            lista.Add(new Chequeo("Hay 1 archivo dcm en la carpeta correspondiente", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.HayUnicoDicomEnCarpetaPlan), true, true, true, true, true, false, false, false, false, false, false, false, false, null, false, true, null));
            lista.Add(new Chequeo("Plan correcto en Dicom RT", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.ArchivoCoincideConPlan), true, true, true, true, true, false, false, false, false, false, false, true, false, null, false, true, null));
            lista.Add(new Chequeo("Tiempos incluidos en DICOM RT", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.DicomTieneTiempos), true, true, true, true, true, false, false, false, false, false, false, false, false, null, false, true, null));
            lista.Add(new Chequeo("DICOM RT Compatible con consola", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.DicomEsCompatibleConConsolaVieja), true, true, true, true, true, false, false, false, false, false, false, false, false, null, false, true, null));
            lista.Add(new Chequeo("No realizó aplicaciones (DicomRT)", Categoria.Dicom, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoDICOM.NoRealizoAplicacionesDicomRT), true, true, true, true, true, false, false, false, false, false, false, false, false, null, false, true, null));
            lista.Add(new Chequeo("Informe hecho", Categoria.Informe, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.TieneInforme), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("El informe está cargado en Sitramed", Categoria.Informe, NivelDeAccion.Advertencia, null, false, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Técnica correcta en informe", Categoria.Informe, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.TecnicaEnInformeCorrecta), true, true, true, true, true, true, true, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tiene Camilla y debe", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneCamilla), true, true, true, false, true, true, true, true, true, true, true, false, false, false, null, null, null));
            lista.Add(new Chequeo("Tiene Camilla y no debe", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneCamillaYNoDebe), true, false, false, false, false, false, false, false, false, false, false, true, false, true, null, null, null));
            lista.Add(new Chequeo("Camilla correcta", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.CamillaCorrecta), true, true, true, false, true, true, true, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("User Origin difiere de origen DICOM", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.UserOriginEnOrigenDICOM), true, true, true, true, true, true, true, false, false, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Iso cerca del origen", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.IsoCercaDeUserOrigin), true, true, true, false, true, true, true, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Se puede hacer Merge", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.SePuedenFusionarCampos), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Plan suma con isos similares", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.IsosSimilaresEnPlanSuma), true, false, false, false, false, false, false, false, false, false, false, false, true, null, null, null, null));
            lista.Add(new Chequeo("Plan con más de 1 iso", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.MasDeUnIsoEnUnPlan), true, true, true, true, true, true, true, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("TAC antigua", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TomografiaVieja), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Nombre de curso correcta", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.EstructuraNombreCursoCorrecta), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Orientación arcos", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.OrientacionArcos), true, false, true, false, false, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("UM máx por arco", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.UMporGradoAlta), true, false, true, false, false, false, false, false, false, false, false, true, false, null, null, null, null));
            lista.Add(new Chequeo("UM min por arco", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.UMporGradoBaja), true, false, true, false, false, false, false, false, false, false, false, true, false, null, null, null, null));
            lista.Add(new Chequeo("DoseRate indicado", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.DoseRate), true, true, true, true, true, true, true, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("UM por subcampo", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.UMporSubField), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("UM mínima para cuña", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.UMminimaParaCuna), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tamaño x en VMAT", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TamanoXenVMAT), true, false, false, false, false, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Iso en Arcos", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.IsoEnArcos), true, false, false, false, false, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tamaño campo mínimo", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TamanoCampoMinimo), true, true, true, false, true, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("En Plan Suma planes en dif equipos", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.PlanesEnMismoEquipo), true, false, false, false, false, false, false, false, false, false, false, false, true, null, null, null, null));
            lista.Add(new Chequeo("Campo con cuña física", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.CampoConCunaFisica), true, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Dynamic MLC", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.DynamicMLC), true, false, true, false, false, false, false, true, false, true, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Iso fuera del campo X e Y", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.IsoFueraDeCampo), true, true, true, true, true, true, true, true, true, true, true, false, false, false, null, null, null));
            lista.Add(new Chequeo("Iso fuera del campo Y", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.IsoFueraDeCampoSoloY), true, false, false, false, false, false, false, false, false, false, false, false, false, true, null, null, null));
            lista.Add(new Chequeo("User Origin en fiduciales", Categoria.Planificacion, NivelDeAccion.Advertencia, null, false, true, true, true, true, true, true, false, false, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Plan calculado con grilla de 1mm", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.CalculadoCada1mm), true, false, false, false, false, false, false, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Target es tipo PTV", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TargetEsTipoPTV), true, true, true, true, true, true, true, true, true, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tiene punto de referencia primario", Categoria.Prescripcion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TienePrimaryReferencePoint), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Reference Point tiene localización", Categoria.Prescripcion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.ReferencePointTieneLocalizacion), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Fracciones Eclipse coincide con fracciones scheduladas", Categoria.Prescripcion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.FxSceduleCoincideConFxEclipse), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Dosis plan coincide con prescripción sitra", Categoria.Prescripcion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.CoincidenciaDosisConPrescripcion), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Restriccion reference point coincide con prescripción sitra", Categoria.Prescripcion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.CoincidenciaDosisConReferencePoint), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Se hizo plan QA", Categoria.QA, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.TienePlanQA), true, false, false, false, true, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Se midió plan QA Portal", Categoria.QA, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.SeMidioPlanQA), true, false, false, false, true, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Se midió plan QA Otro (MC, SRSMC, etc)", Categoria.QA, NivelDeAccion.Advertencia, null, false, false, false, false, true, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("QA OK", Categoria.QA, NivelDeAccion.Advertencia, null, false, false, false, false, true, true, true, false, true, false, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tiene campos de setup", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneCamposSetUp), true, true, true, true, true, true, true, false, false, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Tiene un campo de Setup Ant y uno Lat", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneCampoAntYLatSetUp), true, true, true, true, true, true, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Campos de Setup LD o LI en función de Xiso", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.CampoSetupLateralCorrecto), true, true, true, true, true, true, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("DRR en campos de set up creada", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.DRRCreadasEnCamposSetup), true, true, true, true, true, true, true, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Imagenes agendadas según equipo", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.ImagenesAgendadasSegunEquipo), true, true, true, true, true, true, true, false, false, true, true, false, false, null, null, false, null));
            lista.Add(new Chequeo("Delta Couch es el mismo para todos los campos", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.DeltaCouchIgualParTodosLosCampos), true, true, true, true, true, true, true, false, false, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Delta Couch coincide con Iso", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.DeltaCouchCoincideConIso), true, true, true, true, true, true, true, false, false, true, true, false, false, null, null, null, null));
            lista.Add(new Chequeo("Revisar que coordenadas de Imager sean correctas", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.PosicionImagerCorrecta), true, true, true, true, true, true, true, false, false, false, false, false, false, null, true, false, null));
            lista.Add(new Chequeo("Tabla de tolerancia incluida", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneTablaDeTolerancia), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Tabla de tolerancia correcta", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TablaToleranciaCorrecta), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("No realizó aplicaciones (en ARIA)", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoAria.NoRealizoAplicacionesARIA), true, true, true, true, true, true, true, true, true, true, true, true, false, null, true, false, null));
            lista.Add(new Chequeo("DRRs en Centro de datos", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.TieneImagenesDRRenCDD), true, true, true, true, true, true, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Cantidad de DRRs Correcta", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.NumeroDeImagenesCorrectas), true, true, true, true, true, true, false, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Corrimientos correctos en GSheet", Categoria.SetUp, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoArchivos.CorrimientosEnRef2IsoOK), true, true, true, true, true, false, false, false, false, false, false, false, false, null, false, true, null));
            lista.Add(new Chequeo("DRR con filtro correcto", Categoria.SetUp, NivelDeAccion.Advertencia, null, false, true, true, true, true, true, true, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Chequeos Pb en bunker Electrones/TBI", Categoria.SetUp, NivelDeAccion.Advertencia, null, false, false, false, true, false, false, false, false, false, false, false, true, false, null, null, null, null));
            lista.Add(new Chequeo("Checklist TBI", Categoria.SetUp, NivelDeAccion.Advertencia, null, false, false, false, false, false, false, false, false, false, false, false, true, false, null, null, null, null));
            lista.Add(new Chequeo("DRR incluye reparos óseos para matcheo", Categoria.SetUp, NivelDeAccion.Advertencia, null, false, true, true, true, true, true, true, false, false, false, false, false, false, null, null, null, null));
            lista.Add(new Chequeo("Está planificado para el equipo 4", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.PlanEnEquipo4), true, false, false, false, false, false, false, true, true, true, true, false, false, null, null, null, true));
            lista.Add(new Chequeo("Tiene Tecnica TOTAL", Categoria.Planificacion, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodosChequeoEclipse.TieneTecnicaTOTAL), true, true, true, true, true, true, true, true, true, true, true, true, false, null, null, null, null));
            lista.Add(new Chequeo("Se exportó el plan a Exactrac", Categoria.Exactrac, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoExactrac.EstaExportadoPlanAExactrac), true, false, false, false, false, false, true, true, true, true, true, false, false, null, null, null, true));
            lista.Add(new Chequeo("Se exportó CT a Exactrac", Categoria.Exactrac, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoExactrac.EstaExportadaCT), true, false, false, false, false, false, true, true, true, true, true, false, false, null, null, null, true));
            lista.Add(new Chequeo("La CT exportada tiene más de 400 cortes", Categoria.Exactrac, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoExactrac.SeExportaronMenosDe400Cortes), true, false, false, false, false, false, true, true, true, true, true, false, false, null, null, null, true));
            lista.Add(new Chequeo("Se exportó set de estructuras a Exactrac", Categoria.Exactrac, NivelDeAccion.Advertencia, new MyStaticMethodInvoker(MetodoChequeoExactrac.SeExportoSetDeEstructuras), true, false, false, false, false, false, true, true, true, true, true, false, false, null, null, null, true));
            return lista;
        }

        public static List<Chequeo> SeleccionarChequeos(Plan plan)
        {
            List<Chequeo> ListaCompletaChequeos = ListaChequeos();
            if (plan.EsPlanSuma)
            {
                return ListaCompletaChequeos.Where(c => c.ExclusivoPlanSuma!=false).ToList();
            }
            List<Chequeo> ListaPlan = ListaCompletaChequeos.Where(c => c.AplicaTecnica(plan.Tecnica)).ToList();
            if (plan.EsCamillaEspecial)
            {
                ListaPlan.AddRange(ListaCompletaChequeos.Where(c => c.ExclusivoCamEspecial==true));
                foreach (Chequeo chequeo in ListaCompletaChequeos.Where(c=>c.ExclusivoCamEspecial==false))
                {
                    if (ListaPlan.Contains(chequeo))
                    {
                        ListaPlan.Remove(chequeo);
                    }
                }
            }
            if (MetodoChequeoDICOM.EquipoEsDicomRT(plan))
            {
                ListaPlan.AddRange(ListaCompletaChequeos.Where(c => c.ExclusivoEquiposDicomRT==true));
                
                foreach (Chequeo chequeo in ListaCompletaChequeos.Where(c => c.ExclusivoEquiposDicomRT==false))
                {
                    if (ListaPlan.Contains(chequeo))
                    {
                        ListaPlan.Remove(chequeo);
                    }
                }
            }
            else
            {
                ListaPlan.AddRange(ListaCompletaChequeos.Where(c => c.ExclusivoEquiposAria==true));
                foreach (Chequeo chequeo in ListaCompletaChequeos.Where(c => c.ExclusivoEquiposAria==false))
                {
                    if (ListaPlan.Contains(chequeo))
                    {
                        ListaPlan.Remove(chequeo);
                    }
                }
            }
            if (plan.EsPediatrico)
            {
                ListaPlan.AddRange(ListaCompletaChequeos.Where(c => c.ExclusivoPediatrico == true));
                foreach (Chequeo chequeo in ListaCompletaChequeos.Where(c => c.ExclusivoPediatrico == false))
                {
                    if (ListaPlan.Contains(chequeo))
                    {
                        ListaPlan.Remove(chequeo);
                    }
                }
            }
            return ListaPlan.Distinct().ToList();
        }
    }

    public enum NivelDeAccion
    {
        Advertencia,
        Justificar,
        NoTratar,
    };

    public enum Categoria
    {
        Calculo,
        CalculoIndep,
        Campos,
        CT,
        Curso,
        Dicom,
        Isocentro,
        PlanSuma,
        Prescripcion,
        Puntos,
        QA,
        SetUp,
        Informe,
        Planificacion,
        APF,
        Exactrac
    };
}
