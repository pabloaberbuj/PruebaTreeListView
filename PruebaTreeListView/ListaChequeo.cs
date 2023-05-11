using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class ListaChequeo
    {
        public string Nombre { get; set; }
        public List<Chequeo> Chequeos { get; set; }

        /*public ListaChequeo(string _nombreLista)
        {
            Nombre = _nombreLista;
            Chequeos = Chequeo.GetChequeos().Where(c => c.NombreLista == Nombre).ToList();
        }*/



    }
    /*public class ListaTotal
    {
        public List<ListaChequeo> Listas { get; set; }

        public ListaTotal()
        {
            Listas = new List<ListaChequeo>();
            Listas.Add(new ListaChequeo("Lista1"));
            Listas.Add(new ListaChequeo("Lista2"));
            Listas.Add(new ListaChequeo("Lista3"));
        }

    }*/
}

