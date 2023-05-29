using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PruebaTreeListView
{
    public class Paciente
    {
        public string ID { get; set; }
        public string Nombre { get; set; }
        public List<Curso> Cursos { get; set; }

        public Paciente(PatientSummary patientSummary)
        {
            ID = patientSummary.Id;
            Nombre = patientSummary.LastName + ", " + patientSummary.FirstName;
        }
        public Paciente(string _id, string _nombre)
        {
            ID = _id;
            Nombre = _nombre;
            /*Cursos = new List<Curso>()
            {
                new Curso(Nombre + " Curso 1"),
                new Curso(Nombre + " Curso 2"),
            };*/
        }

        public override string ToString()
        {
            return ID + " - " + Nombre;
        }
        public static List<Paciente> nuevosPacientes()
        {
            return new List<Paciente>
            {
                new Paciente("123", "Juan"),
                new Paciente("456", "Pedro"),
                new Paciente("789", "Luisa"),
                new Paciente("901", "Marta Luisa"),
                new Paciente("902", "Monica")
            };
        }
        public static List<Paciente> MuchosPacientes()
        {
            List<Paciente> Muchos = nuevosPacientes();
            for (int i=0;i<2000;i++)
            {
                List<Paciente> agregar = nuevosPacientes();
                foreach (Paciente pac in agregar)
                {
                    pac.ID += "-" + i.ToString();
                }
                Muchos.AddRange(agregar);
            }
            return Muchos;
        }
    }

}
