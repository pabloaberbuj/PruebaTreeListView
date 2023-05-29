using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTreeListView
{
    public class PacienteCorrimientos
    {
        public string HC { get; set; }
        public string Nombre_Plan { get; set; }
        public string Iso { get; set; }
        public string DespLat { get; set; }
        public string SentLat { get; set; }
        public string DespVert { get; set; }
        public string SentVert { get; set; }
        public string DespLong { get; set; }
        public string SentLong { get; set; }
        public string Fecha { get; set; }

        public PacienteCorrimientos(IList<Object> Linea)
        {
            HC = Linea[0].ToString();
            Nombre_Plan = Linea[1].ToString();
            DespLat = Linea[3].ToString();
            SentLat = Linea[4].ToString();
            DespVert = Linea[5].ToString();
            SentVert = Linea[6].ToString();
            DespLong = Linea[7].ToString();
            SentLong = Linea[8].ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(PacienteCorrimientos))
            {
                PacienteCorrimientos objeto = (PacienteCorrimientos)obj;
                return HC == objeto.HC && Nombre_Plan == objeto.Nombre_Plan && DespLat == objeto.DespLat && SentLat == objeto.SentLat
                    && DespVert == objeto.DespVert && SentVert == objeto.SentVert && SentLong == objeto.SentLong;
            }
            else
            {
                return false;
            }
            
        }
    }
}
