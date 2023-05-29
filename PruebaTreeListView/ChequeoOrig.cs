using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class Chequeo: INotifyPropertyChanged
    {
        public string Nombre { get; set; }
        public Lista Lista {get; set;}
        public bool EsManual { get; set; }
        public bool? resultado;
        public bool? Resultado
        {
            get { return this.resultado; }
            set
            {
                if (this.resultado!=value)
                {
                    this.resultado = value;
                    this.NotifyPropertyChanged("Resultado");
                }
            }
        }

        public string mensajeSiFalso;
        public string MensajeSiFalso
        {
            get { return this.mensajeSiFalso; }
            set
            {
                if (this.mensajeSiFalso!=value)
                {
                    this.mensajeSiFalso = value;
                    this.NotifyPropertyChanged("MensajeSiFalso");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



        public Chequeo(string _nombre, Lista _lista, bool _esManual, bool? _resultado, string _mensajeSiFalso)
        {
            Nombre = _nombre;
            Lista = _lista;
            EsManual = _esManual;
            Resultado = _resultado;
            MensajeSiFalso = _mensajeSiFalso;
        }
        public static List<Chequeo> GetChequeos()
        {
            List<Chequeo> lista = new List<Chequeo>();
            lista.Add(new Chequeo("n1", Lista.Tipo1, false, true,"Dio mal"));
            lista.Add(new Chequeo("n2", Lista.Tipo1, false, false, "Dio feo"));
            lista.Add(new Chequeo("n3", Lista.Tipo2, false, true, "Revisar"));
            lista.Add(new Chequeo("n4", Lista.Tipo2, false, false, "No seguir"));
            lista.Add(new Chequeo("n5", Lista.Tipo3, false, true, "Horrible"));
            lista.Add(new Chequeo("n6", Lista.Tipo1, true, null, "No es tan grave"));
            return lista;
        }

    }
    public enum Lista
    {
        Tipo1, Tipo2, Tipo3,
    }



}
