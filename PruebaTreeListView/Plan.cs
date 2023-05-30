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
    public class Plan
    {
        public Ecl.PlanSetup PlanEclipse { get; set; }
        public PlanSetup PlanAria { get; set; }
        public Tecnica Tecnica { get; set; }
        public double DosisTotal { get; set; }
        public double DosisDia { get; set; }
        public double DosisFraccion { get; set; }
        public bool EsCamillaEspecial { get; set; }
        public bool EsPlanSuma { get; set; }
        public Ecl.PlanSum PlanSumaEclipse { get; set; }

        public Plan(Ecl.PlanSetup _planEclipse, PlanSetup _planAria, Tecnica _tecnica, double _dosisTotal, double _dosisDia, double _dosisFraccion, bool _esCamillaEspecial, bool _esPlanSuma, Ecl.PlanSum _planSumaEclipse = null)
        {
            PlanEclipse = _planEclipse;
            PlanAria = _planAria;
            Tecnica = _tecnica;
            DosisTotal = _dosisTotal;
            DosisDia = _dosisDia;
            DosisFraccion = _dosisFraccion;
            EsCamillaEspecial = _esCamillaEspecial;
            EsPlanSuma = _esPlanSuma;
            PlanSumaEclipse = _planSumaEclipse;
        }

        public Plan(Ecl.PlanSetup _planEclipse, Aria aria, Tecnica _tecnica, double _dosisTotal, double _dosisDia, double _dosisFraccion, bool _esCamillaEspecial, bool _esPlanSuma, Ecl.PlanSum _planSumaEclipse = null)
        {
            PlanEclipse = _planEclipse;
            PlanAria = MetodosAuxiliares.AriaPlanDeEclipse(aria,_planEclipse);
            Tecnica = _tecnica;
            DosisTotal = _dosisTotal;
            DosisDia = _dosisDia;
            DosisFraccion = _dosisFraccion;
            EsCamillaEspecial = _esCamillaEspecial;
            EsPlanSuma = _esPlanSuma;
            PlanSumaEclipse = _planSumaEclipse;
        }

        public string NombreMasID()
        {
            return PlanEclipse.Course.Patient.LastName.ToUpper() + ", " + PlanEclipse.Course.Patient.FirstName.ToUpper() + " " + PlanEclipse.Course.Patient.Id;
        }
        public string NombreMasIDDRR()
        {
            return PlanEclipse.Course.Patient.LastName.ToUpper() + ", " + PlanEclipse.Course.Patient.FirstName.ToUpper() + "-" + PlanEclipse.Course.Patient.Id;
        }

        public void ObtenerTecnica()
        {
            if (PlanEclipse.Beams.Count(f => !f.IsSetupField)>0)
            {
                Ecl.Beam primerCampo = PlanEclipse.Beams.Where(f => !f.IsSetupField).First();
                if (primerCampo.EnergyModeDisplayName.Contains("E"))
                {
                    Tecnica = Tecnica.Electrones;
                    return;
                }
                if (MetodosAuxiliares.esRadioCirugia(PlanEclipse))
                {
                    if (primerCampo.EnergyModeDisplayName == "6X-SRS")
                    {
                        Tecnica = Tecnica.RC_HazSRS;
                        return;
                    }
                    else
                    {
                        Tecnica = Tecnica.RC_VMAT;
                        return;
                    }
                }
                else if (MetodosAuxiliares.esSBRT(PlanEclipse))
                {
                    if (primerCampo.EnergyModeDisplayName == "6X-SRS")
                    {
                        Tecnica = Tecnica.SBRT_HazSRS;
                        return;
                    }
                    else
                    {
                        Tecnica = Tecnica.SBRT_VMAT;
                        return;
                    }
                }
                else if (primerCampo.MLCPlanType==MLCPlanType.DoseDynamic && primerCampo.ControlPoints.Count>10)
                {
                    Tecnica = Tecnica.IMRT;
                    return;
                }
                else if (primerCampo.MLCPlanType==MLCPlanType.VMAT)
                {
                    Tecnica = Tecnica.VMAT;
                    return;
                }
                else if (primerCampo.Technique.Id == "ARC" && primerCampo.MLCPlanType != MLCPlanType.VMAT)
                {
                    if (PlanEclipse.Id.ToUpper().Contains("TBI") && primerCampo.ControlPoints.First().JawPositions==new VRect<double>(20,20,20,20))
                    {
                        Tecnica = Tecnica.TBI;
                        return;
                    }
                    else
                    {
                        Tecnica = Tecnica.Arcos3DC;
                        return;
                    }
                }
                else
                {
                    Tecnica = Tecnica.Static3DC;
                    return;
                }
            }
            else
            {
                Tecnica = Tecnica.Indefinida;
                return;
            }
            
        }

        public List<Chequeo> Chequear()
        {
            List<Chequeo> chequeos = Chequeo.SeleccionarChequeos(this);
            foreach (Chequeo chequeo in chequeos)
            {
                chequeo.AplicarMetodo(this);
            }
            return chequeos;
        }
    }


    public enum Tecnica
    {
        Static3DC,
        Mama3DC, //quitar porque todos llevan las mismas imágenes: Entra en static3D
        Arcos3DC,//
        Electrones,//
        IMRT,//
        VMAT,//
        RC_HazSRS,//
        RC_VMAT,//
        SBRT_HazSRS,//
        SBRT_VMAT,//
        TBI,//
        IGRT, //ver CHHIP y otras WABRT
        Indefinida,
    }
}
