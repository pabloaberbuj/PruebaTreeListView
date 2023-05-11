using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class Plan
    {
        public string Nombre { get; set; }
        public Tecnica Tecnica { get; set; }

        public Plan(string _nombre, Tecnica _tecnica)
        {
            Nombre = _nombre;
            Tecnica = _tecnica;
        }
        public override string ToString()
        {
            return Nombre;
        }
    }
    

    public enum Tecnica
    {
        Static3D,
        IMRT,
        VMAT,
    };
}
