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

namespace PruebaTreeListView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CollectionView view;
        List<Chequeo> Chequeos;
        public MainWindow()
        {
            InitializeComponent();
            cb_pacientes.Items.Clear();
            cb_pacientes.ItemsSource = Paciente.nuevosPacientes();
            view = (CollectionView)CollectionViewSource.GetDefaultView(LVChequeos.Items);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Lista");
            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Resultado");
            view.GroupDescriptions.Add(groupDescription);
            //view.GroupDescriptions.Add(groupDescription2);
        }


        /*private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource).Refresh();
        }*/

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
            ((Chequeo)((RadioButton)sender).DataContext).Resultado = true;
            view.Refresh();
            //((RadioButton)sender).IsChecked = true;
            //((RadioButton)sender).UpdateLayout();*/
            //LVChequeos.Ite
        }

        private void RB_Falla_Checked(object sender, RoutedEventArgs e)
        {
            ((Chequeo)((RadioButton)sender).DataContext).Resultado = false;
            view.Refresh();
            //((RadioButton)sender).IsChecked = true;
        }

        private void cb_pacientes_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView viewPaciente = (CollectionView)CollectionViewSource.GetDefaultView(cb_pacientes.ItemsSource);
            viewPaciente.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(cb_pacientes.Text))
                        return false;
                    else if (cb_pacientes.Text.Length < 2)
                        return false;
                    else
                    {
                        if (((Paciente)o).ToString().ToLower().Contains(cb_pacientes.Text.ToLower()))
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
        }

        private void BT_HabilitarLV_Click(object sender, RoutedEventArgs e)
        {
            Chequeos = Chequeo.GetChequeos();
            LVChequeos.Visibility = Visibility.Visible;
            LVChequeos.ItemsSource = Chequeos;
        }

        private void cb_pacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_pacientes.SelectedIndex > -1)
            {
                cb_cursos.ItemsSource = ((Paciente)cb_pacientes.SelectedItem).Cursos;
            }

        }

        private void cb_cursos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_cursos.SelectedIndex > -1)
            {
                cb_planes.ItemsSource = ((Curso)cb_cursos.SelectedItem).Planes;
            }
        }
    }
}

