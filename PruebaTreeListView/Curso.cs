using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class Curso
    {
        public string Nombre { get; set; }
        public List<Plan> Planes { get; set; }

        /*public Curso(string _nombre)
        {
            Nombre = _nombre;
            Planes = new List<Plan>()
            {
                new Plan(_nombre + "Plan 1",Tecnica.Static3D),
                new Plan(_nombre + "Plan 2",Tecnica.IMRT),
                new Plan(_nombre + "Plan 3",Tecnica.VMAT),
            };
        }*/
        public override string ToString()
        {
            return Nombre;
        }
    }
}

