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
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

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
        Patient pacienteSeleccionado;
        Course cursoSeleccionado;
        PlanningItem planSeleccionado;
        VMS.TPS.Common.Model.API.Application app;
        public MainWindow()
        {
             app = VMS.TPS.Common.Model.API.Application.CreateApplication("paberbuj", "123qwe");
            
            
            var pacientes = app.PatientSummaries.OrderByDescending(p=>p.CreationDateTime);
            pacientesRes = new List<string>();
            foreach (PatientSummary pacienteSummary in pacientes)
            {
                pacientesRes.Add(pacienteSummary.Id + " " + pacienteSummary.LastName + ", " + pacienteSummary.FirstName);
            }
            
            pacientes = null;
            //app.Dispose();
            //System.Threading.Thread.Sleep(5000);
            InitializeComponent();
            
            //view.GroupDescriptions.Add(groupDescription2);
        }


        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource).Refresh();
        }

        /*public bool Expanded(Lista lista)
        {
            if (lista == Lista.Tipo1)
            {
                return true;
            }
            else if (lista == Lista.Tipo2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }*/

        private void RB_OK_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = true;
            view.Refresh();
            //((RadioButton)sender).IsChecked = true;
            //((RadioButton)sender).UpdateLayout();*/
            //LVChequeos.Ite
        }

        private void RB_Falla_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).ResultadoTest = false;
            view.Refresh();
            //((RadioButton)sender).IsChecked = true;
        }

        private void cb_pacientes_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView viewPaciente = (CollectionView)CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource);
            viewPaciente.Filter = ((o) =>
                {
                    //string idmasnombre = ((PatientSummary)o).Id + " " + ((PatientSummary)o).LastName + ", " + ((PatientSummary)o).FirstName;
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

        private void BT_HabilitarLV_Click(object sender, RoutedEventArgs e)
        {
            Row_SeleccionPaciente.IsEnabled = false;
            Chequeos = Chequeo.ListaChequeos();
            LVChequeos.Visibility = Visibility.Visible;
            LVChequeos.ItemsSource = Chequeos;
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
                cursoSeleccionado = (Course)cb_cursos.SelectedItem;
                
                if (cursoSeleccionado.PlanSetups.Count()+ cursoSeleccionado.PlanSums.Count() > 0)
                {
                    List<PlanningItem> planes = cursoSeleccionado.PlanSetups.ToList<PlanningItem>();
                    planes.AddRange(cursoSeleccionado.PlanSums);
                    cb_planes.ItemsSource = planes;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void cb_pacientes_Loaded(object sender, RoutedEventArgs e)
        {
            cb_pacientes.ItemsSource = pacientesRes;
            view = (CollectionView)CollectionViewSource.GetDefaultView(LVChequeos.Items);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Lista");
            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Resultado");
            view.GroupDescriptions.Add(groupDescription);
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            app.Dispose();
        }

        private void cb_planes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            planSeleccionado = (PlanningItem)cb_planes.SelectedItem;
            if (planSeleccionado is PlanSetup)
            {
                tbl_fisico.Text = ((PlanSetup)planSeleccionado).CreationUserName;
                if (((PlanSetup)planSeleccionado).ApprovalStatus==PlanSetupApprovalStatus.PlanningApproved || ((PlanSetup)planSeleccionado).ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved)
                {
                    tbl_medico.Text = ((PlanSetup)planSeleccionado).PlanningApprover;
                }
            }
        }
    }
}

