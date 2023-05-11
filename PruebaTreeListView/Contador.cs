using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PruebaTreeListView
{
    public class Contador : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ResultadoOK = 0;
            IEnumerable<object> groupItems = value as IEnumerable<object>;

            foreach (Chequeo chequeo in groupItems)
            {
                if (chequeo.Resultado==true)
                    ResultadoOK++;
            }

            return ResultadoOK;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

