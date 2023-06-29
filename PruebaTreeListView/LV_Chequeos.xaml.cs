using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LV_Chequeos.xaml
    /// </summary>
    public partial class LV_Chequeos : UserControl
    {
        CollectionView view;
        Plan planseleccionado;
        List<Chequeo> chequeos;
        public LV_Chequeos(List<Chequeo> _chequeos)
        {
            chequeos = _chequeos;
            InitializeComponent();
            LVChequeos.ItemsSource = chequeos;
            view = (CollectionView)CollectionViewSource.GetDefaultView(LVChequeos.Items);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Categoria");
            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("ResultadoTest");
            view.GroupDescriptions.Add(groupDescription);
        }

        public LV_Chequeos(Plan _planSeleccionado)
        {
            planseleccionado = _planSeleccionado;
            InitializeComponent();
            //LVChequeos.ItemsSource = planseleccionado.Chequear();
            List<Chequeo> chequeos = Chequeo.SeleccionarChequeos(planseleccionado);
            ObservableCollection<Chequeo> obsCol = new ObservableCollection<Chequeo>(chequeos);
            LVChequeos.ItemsSource = obsCol;
            view = (CollectionView)CollectionViewSource.GetDefaultView(LVChequeos.Items);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Categoria");
            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("ResultadoTest");
            view.GroupDescriptions.Add(groupDescription);
            foreach (Chequeo chequeo in chequeos)
            {
                chequeo.AplicarMetodo(planseleccionado);
                //obsCol.Add(chequeo);
            }
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
    }
}
