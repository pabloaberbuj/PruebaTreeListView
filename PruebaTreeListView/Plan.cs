﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaQ;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.Model.API;
using EvilDICOM.Core.Modules;
using System.Numerics;
using System.Windows;

namespace PruebaTreeListView
{
    public class Plan
    {
        public Ecl.PlanSetup PlanEclipse { get; set; }
        public Ecl.PlanSum PlanEclipseSum { get; set; }
        public AriaQ.PlanSetup PlanAria { get; set; }
        public Tecnica Tecnica { get; set; }
        public double DosisTotal { get; set; }
        public double DosisDia { get; set; }
        public double DosisFraccion { get; set; }
        public bool EsCamillaEspecial { get; set; }
        public bool EsPediatrico { get; set; }
        public bool EsPlanSuma { get; set; }
        public List<Plan> PlanesSumandos { get; set; }
        public bool EsParteDeUnPlanSuma { get; set; }
        public string nombreMasID { get; set; }
        public string nombreMasIDCorregida { get; set; }
        public string nombreMasIDDRR { get; set; }

        public Plan(Ecl.PlanSetup _planEclipse, AriaQ.PlanSetup _planAria, Tecnica _tecnica, double _dosisTotal, double _dosisDia, double _dosisFraccion, bool _esCamillaEspecial, bool _esPediatrico, bool _esParteDeplanSuma)
        {
            PlanEclipse = _planEclipse;
            PlanAria = _planAria;
            Tecnica = _tecnica;
            DosisTotal = _dosisTotal;
            DosisDia = _dosisDia;
            DosisFraccion = _dosisFraccion;
            EsCamillaEspecial = _esCamillaEspecial;
            EsPediatrico = _esPediatrico;
            EsPlanSuma = false;
            EsParteDeUnPlanSuma = _esParteDeplanSuma;
            nombreMasID = NombreMasID();
            nombreMasIDCorregida = NombreMasIDCorregida();
            nombreMasIDDRR = NombreMasIDDRR();


        }
        public Plan(Plan planOriginal, Tecnica _tecnica, double _dosisTotal, double _dosisDia, double _dosisFraccion, bool _esCamillaEspecial, bool _esPediatrico)
        {
            PlanEclipse = planOriginal.PlanEclipse;
            PlanAria = planOriginal.PlanAria;
            Tecnica = _tecnica;
            DosisTotal = _dosisTotal;
            DosisDia = _dosisDia;
            DosisFraccion = _dosisFraccion;
            EsCamillaEspecial = _esCamillaEspecial;
            EsPediatrico = _esPediatrico;
            EsPlanSuma = false;
            EsParteDeUnPlanSuma = planOriginal.EsParteDeUnPlanSuma;
            nombreMasIDCorregida = NombreMasIDCorregida();
            nombreMasIDDRR = NombreMasIDDRR();
        }

        public Plan(Ecl.PlanSetup _planEclipse, Aria aria, Tecnica _tecnica, double _dosisTotal, double _dosisDia, double _dosisFraccion, bool _esCamillaEspecial, bool _esPediatrico, bool _esParteDeplanSuma)
        {
            PlanEclipse = _planEclipse;
            PlanAria = MetodosAuxiliares.AriaPlanDeEclipse(aria, _planEclipse);
            Tecnica = _tecnica;
            DosisTotal = _dosisTotal;
            DosisDia = _dosisDia;
            DosisFraccion = _dosisFraccion;
            EsCamillaEspecial = _esCamillaEspecial;
            EsPediatrico = _esPediatrico;
            EsPlanSuma = false;
            EsParteDeUnPlanSuma = _esParteDeplanSuma;
            nombreMasIDCorregida = NombreMasIDCorregida();
            nombreMasIDDRR = NombreMasIDDRR();
        }

        public Plan(VMS.TPS.Common.Model.API.PlanSum planSumaEclipse, Aria aria)
        {
            EsPlanSuma = true;
            PlanesSumandos = new List<Plan>();
            foreach (VMS.TPS.Common.Model.API.PlanSetup planSetup in planSumaEclipse.PlanSetups)
            {
                PlanesSumandos.Add(new Plan(planSetup, MetodosAuxiliares.AriaPlanDeEclipse(aria, planSetup), Tecnica.Indefinida, double.NaN, double.NaN, double.NaN, false, false,true));
            }
            nombreMasIDCorregida = planSumaEclipse.Course.Patient.LastName.ToUpper() + ", " + planSumaEclipse.Course.Patient.FirstName.ToUpper() + " " + IDSitramed(planSumaEclipse);
            nombreMasIDDRR = planSumaEclipse.Course.Patient.LastName.ToUpper() + ", " + planSumaEclipse.Course.Patient.FirstName.ToUpper() + "-" + planSumaEclipse.Course.Patient.Id;
            PlanEclipseSum = planSumaEclipse;
        }

        public List<VMS.TPS.Common.Model.API.PlanSetup> planesEclipse()
        {
            List<VMS.TPS.Common.Model.API.PlanSetup> planesEclipse = new List<VMS.TPS.Common.Model.API.PlanSetup>();
            foreach (Plan plan in this.PlanesSumandos)
            {
                planesEclipse.Add(plan.PlanEclipse);
            }
            return planesEclipse;
        }
        public string NombreMasID()
        {
            return PlanEclipse.Course.Patient.LastName.ToUpper() + ", " + PlanEclipse.Course.Patient.FirstName.ToUpper() + " " + PlanEclipse.Course.Patient.Id;
        }
        public string NombreMasIDCorregida()
        {
            string aux = PlanEclipse.Course.Patient.LastName.ToUpper() + ", " + PlanEclipse.Course.Patient.FirstName.ToUpper() + " " + IDSitramed();
            if (EsParteDeUnPlanSuma)
            {
                aux += @"\" + this.PlanEclipse.Id;
            }
            return aux;
        }

        public string IDSitramed()
        {
            string reingresoCurso = PlanEclipse.Course.Id[1].ToString();
            string reingresoID = PlanEclipse.Course.Patient.Id.Last().ToString();
            string IdCorregida = PlanEclipse.Course.Patient.Id;
            if (reingresoCurso != reingresoID)
            {

                IdCorregida = PlanEclipse.Course.Patient.Id.Remove(PlanEclipse.Course.Patient.Id.Length - 1, 1) + reingresoCurso;
            }
            return IdCorregida;
        }

        public string IDSitramed(Ecl.PlanSum planEclipseSuma)
        {
            string reingresoCurso = planEclipseSuma.Course.Id[1].ToString();
            string reingresoID = planEclipseSuma.Course.Patient.Id.Last().ToString();
            string IdCorregida = planEclipseSuma.Course.Patient.Id;
            if (reingresoCurso != reingresoID)
            {
                IdCorregida = planEclipseSuma.Course.Patient.Id.Remove(planEclipseSuma.Course.Patient.Id.Length - 1, 1) + reingresoCurso;
            }
            return IdCorregida;
        }

        public string NombreMasIDDRR()
        {
            return PlanEclipse.Course.Patient.LastName.ToUpper() + ", " + PlanEclipse.Course.Patient.FirstName.ToUpper() + "-" + PlanEclipse.Course.Patient.Id;
        }

        public string Equipo()
        {
            return PlanEclipse.Beams.First().TreatmentUnit.Id;
        }


        public static Tecnica ObtenerTecnica(Ecl.PlanSetup PlanEclipse)
        {
            if (PlanEclipse.Beams.Count(f => !f.IsSetupField) > 0)
            {
                Ecl.Beam primerCampo = PlanEclipse.Beams.Where(f => !f.IsSetupField).First();
                if (primerCampo.EnergyModeDisplayName.Contains("E"))
                {
                    return Tecnica.Electrones;
                }
                if (MetodosAuxiliares.esRadioCirugia(PlanEclipse))
                {
                    if (primerCampo.EnergyModeDisplayName == "6X-SRS")
                    {
                        return Tecnica.RC_HazSRS;
                    }
                    else
                    {
                        return Tecnica.RC_VMAT;
                    }
                }
                else if (MetodosAuxiliares.esSBRT(PlanEclipse))
                {
                    if (primerCampo.EnergyModeDisplayName == "6X-SRS")
                    {
                        return Tecnica.SBRT_HazSRS;
                    }
                    else
                    {
                        return Tecnica.SBRT_VMAT;
                    }
                }
                else if (primerCampo.MLCPlanType == MLCPlanType.DoseDynamic && primerCampo.ControlPoints.Count > 10)
                {
                    return Tecnica.IMRT;
                }
                else if (primerCampo.MLCPlanType == MLCPlanType.VMAT)
                {
                    return Tecnica.VMAT;
                }
                else if (primerCampo.Technique.Id == "ARC" && primerCampo.MLCPlanType != MLCPlanType.VMAT)
                {
                    if (PlanEclipse.Id.ToUpper().Contains("TBI") && primerCampo.ControlPoints.First().JawPositions == new VRect<double>(-200, -200, 200, 200))
                    {
                        return Tecnica.TBI;
                    }
                    else
                    {
                        return Tecnica.Arcos3DC;
                    }
                }
                else
                {
                    return Tecnica.Static3DC;
                }
            }
            else
            {
                return Tecnica.Indefinida;
            }

        }

        public bool EsTtodeMama()
        {
            return PlanEclipse.StructureSet.Structures.Any(s => s.Id.ToLower().Contains("ptv_wb_eval") || s.Id.ToLower().Contains("ptv_cw_eval"));
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
        //Mama3DC, //quitar porque todos llevan las mismas imágenes: Entra en static3D
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
