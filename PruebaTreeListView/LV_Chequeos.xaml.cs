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
    /// Interaction logic for LV_Chequeos.xaml
    /// </summary>
    public partial class LV_Chequeos : ListView
    {
        CollectionView view;
        List<Chequeo> chequeos;
        public LV_Chequeos(List<Chequeo> _chequeos)
        {
            chequeos = _chequeos;
            InitializeComponent();
            LVChequeos.ItemsSource = chequeos;
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
