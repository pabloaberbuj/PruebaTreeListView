using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ecl = VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using AriaQ;
using System.Globalization;
using System.Text.RegularExpressions;
using VMS.TPS.Common.Model.API;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System.IO;
using System.Net.NetworkInformation;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace PruebaTreeListView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //CollectionView view;
        //List<Chequeo> Chequeos;
        List<string> pacientesRes;
        Ecl.Patient pacienteSeleccionado;
        Ecl.Course cursoSeleccionado;
        //Ecl.PlanningItem planEclipseSeleccionado;
        VMS.TPS.Common.Model.API.Application app;
        Aria aria;
        bool SeAnalizoPlanSuma = false;
        List<Plan> ListaPlanes = null;
        public MainWindow()
        {
            try
            {
                //app = VMS.TPS.Common.Model.API.Application.CreateApplication(null, null);
                app = VMS.TPS.Common.Model.API.Application.CreateApplication("paberbuj", "123qwe");
                
            }
            catch (Exception)
            {
                //MessageBox.Show("No se puede conectar con Eclipse");
            }
            
            aria = new Aria();
            pacientesRes = new List<string>();
            if (app!=null)
            {
                var pacientes = app.PatientSummaries.OrderByDescending(p => p.CreationDateTime);

                foreach (Ecl.PatientSummary pacienteSummary in pacientes)
                {
                    pacientesRes.Add(pacienteSummary.Id + " " + pacienteSummary.LastName + ", " + pacienteSummary.FirstName);
                }
                pacientes = null;
            }
            InitializeComponent();
            if (app!=null)
            {
                TBl_usuario.Text = app.CurrentUser.Name;
            }
            cb_Tecnicas.ItemsSource = Enum.GetValues(typeof(Tecnica));
            ConexionInicial();
        }


        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource).Refresh();
        }


        /*private void RB_OK_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = true;
            view.Refresh();
        }

        private void RB_Falla_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = false;
            view.Refresh();
        }*/

        private void cb_pacientes_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView viewPaciente = (CollectionView)CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource);
            viewPaciente.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(cb_pacientes.Text))
                        return false;
                    else if (cb_pacientes.Text.Length < 3)
                        return false;
                    else
                    {
                        if (((string)o).ToLower().Contains(cb_pacientes.Text.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                });
            viewPaciente.Refresh();
            if (cb_pacientes.Text.Length >= 3)
            {
                cb_pacientes.IsDropDownOpen = true;
                TextBox textBox = (TextBox)((ComboBox)sender).Template.FindName("PART_EditableTextBox", (ComboBox)sender);
                textBox.SelectionStart = ((ComboBox)sender).Text.Length;
                textBox.SelectionLength = 0;
            }
        }

        private void cb_pacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_pacientes.SelectedIndex > -1)
            {
                string ID = ((string)cb_pacientes.SelectedItem).Split(' ')[0];
                if (pacienteSeleccionado != null)
                {
                    cb_cursos.ItemsSource = null;
                    app.ClosePatient();
                    pacienteSeleccionado = null;

                }
                pacienteSeleccionado = app.OpenPatientById(ID);
                if (pacienteSeleccionado != null && pacienteSeleccionado.Courses.Count() > 0)
                {

                    cb_cursos.ItemsSource = pacienteSeleccionado.Courses;
                }
            }
        }

        private void cb_cursos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_cursos.SelectedIndex > -1)
            {
                cursoSeleccionado = (Ecl.Course)cb_cursos.SelectedItem;

                if (cursoSeleccionado.PlanSetups.Count() + cursoSeleccionado.PlanSums.Count() > 0)
                {
                    List<Ecl.PlanningItem> planes = cursoSeleccionado.PlanSetups.Where(p=>p.ApprovalStatus==PlanSetupApprovalStatus.PlanningApproved || p.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).ToList<Ecl.PlanningItem>();
                    //List<Ecl.PlanningItem> planes = cursoSeleccionado.PlanSetups.ToList<Ecl.PlanningItem>();
                    planes.AddRange(cursoSeleccionado.PlanSums);
                    cb_planes.ItemsSource = planes;
                }
            }
        }

        private void cb_pacientes_Loaded(object sender, RoutedEventArgs e)
        {
            cb_pacientes.ItemsSource = pacientesRes;
            ListaPlanes = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            app.Dispose();
        }

        private void cb_planes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlanEclipseSeleccionado() != null)
            {
                if (PlanEclipseSeleccionado() is Ecl.PlanSetup)
                {
                    tbl_fisico.Text = ((Ecl.PlanSetup)PlanEclipseSeleccionado()).CreationUserName;
                    if (((Ecl.PlanSetup)PlanEclipseSeleccionado()).ApprovalStatus == PlanSetupApprovalStatus.PlanningApproved || ((Ecl.PlanSetup)PlanEclipseSeleccionado()).ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved)
                    {
                        tbl_medico.Text = ((Ecl.PlanSetup)PlanEclipseSeleccionado()).PlanningApprover;
                    }
                    cb_Tecnicas.SelectedItem = Plan.ObtenerTecnica((Ecl.PlanSetup)PlanEclipseSeleccionado());
                    chb_TecnicaOK.IsChecked = false;
                    gb_Prescripcion.IsEnabled = true;
                    gb_Caracteristicas.IsEnabled = true;
                    tb_dosisTotal.Text = "";
                    tb_dosisFraccion.Text = "";
                    tb_dosisDia.Text = "";
                    if (ListaPlanes == null)
                    {
                        tbl_PlanesSumandos.Text = "";
                    }

                }
                if (PlanEclipseSeleccionado() is Ecl.PlanSum)
                {
                    gb_Prescripcion.IsEnabled = false;
                    gb_Caracteristicas.IsEnabled = false;
                    tbl_PlanesSumandos.Text = PlanesSumando((Ecl.PlanSum)PlanEclipseSeleccionado());
                }
            }
        }
        private string PlanesSumando(Ecl.PlanSum planSuma)
        {
            string suma = "(";
            foreach (Ecl.PlanSetup plan in planSuma.PlanSetups)
            {
                suma += plan.Id;
                if (plan != planSuma.PlanSetups.Last())
                {
                    suma += "+";
                }
            }
            suma += ")";
            return suma;
        }
        private Plan PlanSeleccionado()
        {
            if (PlanEclipseSeleccionado() != null && PlanEclipseSeleccionado() is Ecl.PlanSetup && ListaPlanes!=null)
            {
                Plan planOriginal = ListaPlanes.First(p => p.PlanEclipse.Id == PlanEclipseSeleccionado().Id);
                return new Plan(planOriginal, (Tecnica)cb_Tecnicas.SelectedItem, Convert.ToDouble(tb_dosisTotal.Text), Convert.ToDouble(tb_dosisDia.Text), Convert.ToDouble(tb_dosisFraccion.Text), chb_esCamillaEspecial.IsChecked == true, chb_esPediatrico.IsChecked == true);
            }
            if (PlanEclipseSeleccionado() != null && PlanEclipseSeleccionado() is Ecl.PlanSetup)
            {
                return new Plan((Ecl.PlanSetup)PlanEclipseSeleccionado(), aria, (Tecnica)cb_Tecnicas.SelectedItem, Convert.ToDouble(tb_dosisTotal.Text), Convert.ToDouble(tb_dosisDia.Text), Convert.ToDouble(tb_dosisFraccion.Text), chb_esCamillaEspecial.IsChecked == true,chb_esPediatrico.IsChecked==true,false);
            }
            else if (PlanEclipseSeleccionado() != null && PlanEclipseSeleccionado() is Ecl.PlanSum)
            {
                SeAnalizoPlanSuma = false;
                return new Plan((Ecl.PlanSum)PlanEclipseSeleccionado(), aria);
            }
            else
            {
                return null;
            }
            

        }
        private PlanningItem PlanEclipseSeleccionado()
        {
            if (cb_planes.SelectedIndex != -1)
            {
                //MetodosChequeoEclipse.NormalizacionVariaMucho((Ecl.PlanSetup)cb_planes.SelectedItem);
                return (Ecl.PlanningItem)cb_planes.SelectedItem;
            }
            return null;
        }

        private void tb_numero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CommandBinding_CanExecuteAnalizar(object sender, CanExecuteRoutedEventArgs e)
        {
            if (PlanEclipseSeleccionado() == null)
            {
                e.CanExecute = false;
            }
            else if (PlanEclipseSeleccionado() is VMS.TPS.Common.Model.API.PlanSetup)
            {
                e.CanExecute = tb_dosisTotal.Text != "" && tb_dosisDia.Text != "" && tb_dosisFraccion.Text != "" && cb_planes.SelectedIndex != -1 && chb_TecnicaOK.IsChecked == true;
            }
            else
            {
                e.CanExecute = true;
            }

        }

        private void CommandBinding_ExecutedAnalizar(object sender, ExecutedRoutedEventArgs e)
        {
            //var res = MetodosChequeoEclipse.MatrizDeCalculoIncluyeBody(PlanSeleccionado());
            if (TabControl != null && TabControl.Items != null)
            {
                foreach (TabItem item in TabControl.Items)
                {
                    if ((string)item.Header == PlanEclipseSeleccionado().Id)
                    {
                        item.Content = null;
                        LV_Chequeos lV_ChequeosIt = new LV_Chequeos(PlanSeleccionado());
                        item.Content = lV_ChequeosIt;
                        item.IsSelected = true;
                        return;
                    }
                }
                TabItem item1 = new TabItem();
                item1.Header = PlanEclipseSeleccionado().Id;
                TabControl.Items.Add(item1);
                item1.IsSelected = true;
                /*Chequeos = PlanSeleccionado().Chequear();
                LV_Chequeos lV_Chequeos = new LV_Chequeos(Chequeos);*/
                LV_Chequeos lV_Chequeos = new LV_Chequeos(PlanSeleccionado());
                item1.Content = lV_Chequeos;

            }
            SeAnalizoPlanSuma = PlanSeleccionado().EsPlanSuma;
        }

        private void CommandBinding_CanExecuteSiguientePlan(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (PlanEclipseSeleccionado() is VMS.TPS.Common.Model.API.PlanSum && SeAnalizoPlanSuma) || (ListaPlanes != null && ListaPlanes.Last().PlanEclipse != PlanEclipseSeleccionado());
        }

        private void CommandBinding_ExecutedSiguientePlan(object sender, ExecutedRoutedEventArgs e)
        {
            if (PlanSeleccionado().EsPlanSuma)
            {
                if (ListaPlanes == null) //Va a ser el primer campo
                {
                    ListaPlanes = PlanSeleccionado().PlanesSumandos;
                    cb_planes.SelectedItem = ListaPlanes.First().PlanEclipse;
                }
            }
            else if (ListaPlanes != null)
            {
                int indexActual = ListaPlanes.IndexOf(ListaPlanes.First(p => p.PlanEclipse == PlanEclipseSeleccionado()));
                cb_planes.SelectedItem = ListaPlanes[indexActual + 1].PlanEclipse;
            }
            gb_Seleccion.IsEnabled = false;
        }

        private void CommandBinding_CanExecuteReiniciar(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TabControl != null && TabControl.Items != null && TabControl.Items.Count > 0;
        }

        private void CommandBinding_ExecutedReiniciar(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl.Items.Clear();
            cb_pacientes.SelectedItem = null;
            tb_dosisTotal.Text = "";
            tb_dosisFraccion.Text = "";
            tb_dosisDia.Text = "";
            tbl_PlanesSumandos.Text = "";
            gb_Seleccion.IsEnabled = true;
            cb_Tecnicas.SelectedIndex = -1;
            ListaPlanes = null;
        }
        private void ChequeoInicial(Ellipse elipse, bool Resultado)
        {
            if (Resultado)
            {
                elipse.Fill = new SolidColorBrush(System.Windows.Media.Colors.LightGreen);
            }
            else
            {
                elipse.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void ConexionInicial()
        {
            ChequeoInicial(Ell_ConexionEclipse, app != null);
            try
            {
                var mach = aria.Machines.ToList();
                ChequeoInicial(Ell_ConexionARIA, true);
            }
            catch (Exception)
            {
                ChequeoInicial(Ell_ConexionARIA, false);
            }
            ChequeoInicial(Ell_ConexionVaData, Directory.Exists(@"\\ariamevadb-svr\va_data$"));
            ChequeoInicial(Ell_ConexionCDD, Directory.Exists(@"\\fisica0\centro_de_datos2018\000_Centro de Datos 2021"));
            ChequeoInicial(Ell_ConexionExactrac, MetodoChequeoExactrac.HayConexionExactracEq4());
            ChequeoInicial(Ell_ConexionDrive, MetodosAuxiliares.ChequearRefToIso());
        }


        private void CommandBinding_CanExecuteImprimir(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TabControl.Items.Count==0)
            {
                e.CanExecute = false;
                return;
            }
            foreach (TabItem item in TabControl.Items)
            {
                var chequeos = ((LV_Chequeos)item.Content).obsCol;
                if (chequeos==null || chequeos.Count==0)
                {
                    e.CanExecute = false;
                    return;
                }
                foreach (var Chequeo in chequeos)
                {
                    if (Chequeo.ResultadoTest==false && Chequeo.Observacion=="")
                    {
                        e.CanExecute = false;
                        return;
                    }
                }
            }
            e.CanExecute = true;
        }

        private void CommandBinding_ExecutedImprimir(object sender, ExecutedRoutedEventArgs e)
        {
            List<MigraDoc.DocumentObjectModel.Tables.Table> Tablas = new List<MigraDoc.DocumentObjectModel.Tables.Table>();
            List<string> planesString = new List<string>();

            foreach (TabItem item in TabControl.Items)
            {
                LV_Chequeos lv_chequeo = (LV_Chequeos)item.Content;
                if (lv_chequeo.planseleccionado.PlanEclipse != null)
                {
                    planesString.Add(lv_chequeo.planseleccionado.PlanEclipse.Id);
                }
                else
                {
                    planesString.Add(lv_chequeo.planseleccionado.PlanEclipseSum.Id);
                }

                //Reporte.CrearReportePlan(lv_chequeo.planseleccionado, lv_chequeo.obsCol, app.CurrentUser.Name);
                Tablas.Add(Reporte.TablaChequeos(lv_chequeo.obsCol));
            }
            Reporte.CrearReportePlan(PlanSeleccionado(), Tablas, planesString, app.CurrentUser.Name);
        }
    }
}

