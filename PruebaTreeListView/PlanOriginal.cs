using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class PlanOriginal
    {
        public string Nombre { get; set; }
        public Tecnica Tecnica { get; set; }

        public PlanOriginal(string _nombre, Tecnica _tecnica)
        {
            Nombre = _nombre;
            Tecnica = _tecnica;
        }
        public override string ToString()
        {
            return Nombre;
        }
    }
    

    public enum TecnicaOriginal
    {
        Static3D,
        IMRT,
        VMAT,
    };
}
