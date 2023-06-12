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
using Ecl=VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using AriaQ;
using System.Globalization;
using System.Text.RegularExpressions;
using VMS.TPS.Common.Model.API;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace PruebaTreeListView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CollectionView view;
        List<Chequeo> Chequeos;
        List<string> pacientesRes;
        Ecl.Patient pacienteSeleccionado;
        Ecl.Course cursoSeleccionado;
        //Ecl.PlanningItem planEclipseSeleccionado;
        VMS.TPS.Common.Model.API.Application app;
        Aria aria = new Aria();
        public MainWindow()
        {
             app = VMS.TPS.Common.Model.API.Application.CreateApplication("paberbuj", "123qwe");
            
            
            var pacientes = app.PatientSummaries.OrderByDescending(p=>p.CreationDateTime);
            
            pacientesRes = new List<string>();
            foreach (Ecl.PatientSummary pacienteSummary in pacientes)
            {
                pacientesRes.Add(pacienteSummary.Id + " " + pacienteSummary.LastName + ", " + pacienteSummary.FirstName);
            }
            
            pacientes = null;
            InitializeComponent();
            cb_Tecnicas.ItemsSource = Enum.GetValues(typeof(Tecnica));
            //view.GroupDescriptions.Add(groupDescription2);
        }


        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource).Refresh();
        }


        private void RB_OK_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = true;
            view.Refresh();
        }

        private void RB_Falla_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = false;
            view.Refresh();
        }

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
            if (cb_pacientes.Text.Length>=3)
            {
                cb_pacientes.IsDropDownOpen = true;
                TextBox textBox = (TextBox)((ComboBox)sender).Template.FindName("PART_EditableTextBox", (ComboBox)sender);
                textBox.SelectionStart = ((ComboBox)sender).Text.Length;
                textBox.SelectionLength = 0;
            }
        }

        private void cb_pacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_pacientes.SelectedIndex>-1)
            {
                string ID = ((string)cb_pacientes.SelectedItem).Split(' ')[0];
                if (pacienteSeleccionado!=null)
                {
                    cb_cursos.ItemsSource = null;
                    app.ClosePatient();
                    pacienteSeleccionado = null;
                    
                }
                pacienteSeleccionado = app.OpenPatientById(ID);
                if (pacienteSeleccionado != null && pacienteSeleccionado.Courses.Count()>0)
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
                
                if (cursoSeleccionado.PlanSetups.Count()+ cursoSeleccionado.PlanSums.Count() > 0)
                {
                    List<Ecl.PlanningItem> planes = cursoSeleccionado.PlanSetups.ToList<Ecl.PlanningItem>();
                    planes.AddRange(cursoSeleccionado.PlanSums);
                    cb_planes.ItemsSource = planes;
                }
            }
        }

        private void cb_pacientes_Loaded(object sender, RoutedEventArgs e)
        {
            cb_pacientes.ItemsSource = pacientesRes;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            app.Dispose();
        }

        private void cb_planes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlanEclipseSeleccionado()!=null)
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

                }
                if (PlanEclipseSeleccionado() is Ecl.PlanSum)
                {
                    gb_Prescripcion.IsEnabled = false;
                    gb_Caracteristicas.IsEnabled = false;
                }
            }
        }
        private Plan PlanSeleccionado()
        {
            if ((PlanEclipseSeleccionado()!=null && PlanEclipseSeleccionado() is Ecl.PlanSetup))
            {
                return new Plan((Ecl.PlanSetup)PlanEclipseSeleccionado(), aria, (Tecnica)cb_Tecnicas.SelectedItem, Convert.ToDouble(tb_dosisTotal.Text), Convert.ToDouble(tb_dosisDia.Text), Convert.ToDouble(tb_dosisFraccion.Text), chb_esCamillaEspecial.IsChecked == true);
            }
            else
            {
                return new Plan((Ecl.PlanSum)PlanEclipseSeleccionado(), aria);
            }
            
        }
        private PlanningItem PlanEclipseSeleccionado()
        {
            if (cb_planes.SelectedIndex!=-1)
            {
                return (Ecl.PlanningItem)cb_planes.SelectedItem;
            }
            return null;
        }

        private void tb_numero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tb_dosisTotal.Text != "" && tb_dosisDia.Text!= "" && tb_dosisFraccion.Text!="" && cb_planes.SelectedIndex!=-1 && chb_TecnicaOK.IsChecked==true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!PlanSeleccionado().EsPlanSuma)
            {
                TabItem item1 = new TabItem();
                item1.Header = PlanSeleccionado().PlanEclipse.Id;
                LV_Chequeos lV_Chequeos = new LV_Chequeos(Chequeos);
                item1.Content = lV_Chequeos;
                Chequeos = PlanSeleccionado().Chequear();
            }

        }
    }

    /*public class SoloNumerosRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double valor = 0;
            try
            {
                if (((string)value).Length > 0)
                    valor = Double.Parse((String)value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, $"Sólo números");
            }
            return ValidationResult.ValidResult;
        }
    }*/

}

