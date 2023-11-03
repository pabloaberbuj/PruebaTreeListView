using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PruebaTreeListView
{
    public static class Commands
    {
        public static readonly RoutedUICommand Analizar = new RoutedUICommand
            (
                "Analizar",
                "Analizar",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand SiguientePlan = new RoutedUICommand
            (
                "Siguiente plan",
                "Siguiente plan",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand Reiniciar = new RoutedUICommand
            (
                "Reiniciar",
                "Reiniciar",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand Imprimir = new RoutedUICommand
            (
                "Imprimir",
                "Imprimir",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Control)
                }
            );

    }
}
