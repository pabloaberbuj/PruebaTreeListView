using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using System.Windows;
using MigraDoc.Rendering;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Collections.ObjectModel;

namespace PruebaTreeListView
{
    public static class Reporte
    {
        /*public static void CrearReportePlan(Plan plan, ObservableCollection<Chequeo> chequeos, string usuario)
        {
            var paciente = plan.PlanEclipse.Course.Patient;
            string nombreMasID = paciente.LastName.ToUpper() + ", " + paciente.FirstName.ToUpper() + "-" + paciente.Id;
            string pathDirectorio = IO.crearCarpetaPaciente(paciente.LastName, paciente.FirstName, paciente.Id, plan.PlanEclipse.Course.Id, plan.PlanEclipse.Id);
            string path = IO.GetUniqueFilename("", nombreMasID + "_Chequeo", "pdf");
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            Document documento = Documento(plan,chequeos,usuario);
            if (documento != null)
            {
                pdfRenderer.Document = documento;
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save(pathDirectorio + @"\" + path);
                MessageBox.Show("Se ha generado el reporte del paciente correctamente");
            }
        }*/

        public static void CrearReportePlan(Plan plan, List<MigraDoc.DocumentObjectModel.Tables.Table> tablasChequeo, List<string> planesString, string usuario)
        {
            var paciente = plan.PlanEclipse.Course.Patient;
            string nombreMasID = paciente.LastName.ToUpper() + ", " + paciente.FirstName.ToUpper() + "-" + paciente.Id;
            string pathDirectorio = IO.crearCarpetaPaciente(paciente.LastName, paciente.FirstName, paciente.Id, plan.PlanEclipse.Course.Id, planesString.First());
            string path = IO.GetUniqueFilename("", nombreMasID + "_Chequeo", "pdf");
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            Document documento = Documento(plan,tablasChequeo, planesString, usuario);
            if (documento != null)
            {
                pdfRenderer.Document = documento;
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save(pathDirectorio + @"\" + path);
                MessageBox.Show("Se ha generado el reporte del paciente correctamente");
            }
        }

        public static Document Documento(Plan plan, List<MigraDoc.DocumentObjectModel.Tables.Table> tablasChequeo, List<String> planesString, string usuario)
        {
            Document informe = new Document();
            Estilos.definirEstilos(informe);
            Section seccion = new Section();
            Estilos.formatearSeccion(seccion);
            seccion.AddParagraph("Chequeo en plan de tratamiento", "Titulo");
            cargarEncabezado(seccion, plan, usuario);
            for (int i=0;i<tablasChequeo.Count;i++)
            {
                seccion.AddParagraph("");
                seccion.Add(TItuloPlan(planesString[i]));
                seccion.Add(tablasChequeo[i]);
            }
            informe.Add(seccion);
            return informe;
        }

        public static Paragraph TItuloPlan(string nombrePlan)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddText(nombrePlan);
            paragraph.Style = "Subtitulo";
            return paragraph;
        }

        private static void cargarEncabezado(Section seccion, Plan plan, string usuario)
        {
            //seccion.AddParagraph("Analisis de restricciones en plan de tratamiento", "Titulo");
            string paciente = plan.PlanEclipse.Course.Patient.LastName + ", " + plan.PlanEclipse.Course.Patient.FirstName;
            MigraDoc.DocumentObjectModel.Tables.Table tabla = new MigraDoc.DocumentObjectModel.Tables.Table();
            tabla.Format.Alignment = ParagraphAlignment.Center;
            tabla.Borders = null;
            tabla.AddColumn(250);
            tabla.AddColumn(250);
            tabla.Borders.Width = 0.5;
            for (int i = 0; i < 3; i++)
            {
                tabla.AddRow();
            }
            tabla.Rows[0].Cells[0].Add(Estilos.etiquetaYValor("Paciente", paciente));
            tabla.Rows[1].Cells[0].Add(Estilos.etiquetaYValor("HC", plan.PlanEclipse.Course.Patient.Id));
            //tabla.Rows[2].Cells[0].Add(Estilos.etiquetaYValor("Plan", plan.PlanEclipse.Id));
            tabla.Rows[2].Cells[0].Add(Estilos.etiquetaYValor("Usuario", usuario));
            tabla.Rows[0].Cells[1].MergeDown = 2;
            tabla.Rows[0].Cells[1].Format.Alignment = ParagraphAlignment.Right;
            Paragraph parrafoImage = new Paragraph();
            parrafoImage.Format.Alignment = ParagraphAlignment.Right;
            var imagen = parrafoImage.AddImage(@"\\ariamevadb-svr\va_data$\PlanExplorer\LogoMeva.png");
            imagen.ScaleHeight = 0.5;
            imagen.ScaleHeight = 0.5;
            tabla.Rows[0].Cells[1].Add(parrafoImage);
            seccion.Add(tabla);
            seccion.AddParagraph();
            seccion.AddParagraph();
        }

        public static MigraDoc.DocumentObjectModel.Tables.Table TablaChequeos(ObservableCollection<Chequeo> chequeos)
        {
            MigraDoc.DocumentObjectModel.Tables.Table tabla = new MigraDoc.DocumentObjectModel.Tables.Table();
            for (int i = 0; i < 3; i++)
            {
                tabla.AddColumn();
            }
            MigraDoc.DocumentObjectModel.Tables.Row header = tabla.AddRow();
            header.HeadingFormat = true;
            header.Format.Font.Bold = true;
            header.Cells[0].AddParagraph("Chequeo");
            //header.Cells[1].AddParagraph("Resultado");
            header.Cells[2].AddParagraph("Observación");
            foreach (Chequeo chequeo in chequeos)
            {
                MigraDoc.DocumentObjectModel.Tables.Row fila = tabla.AddRow();
                fila.Cells[0].AddParagraph(chequeo.Nombre);
                
                if (chequeo.ResultadoTest == true)
                {
                    fila.Cells[1].Shading.Color = Colors.LightGreen;
                }
                else if (chequeo.ResultadoTest == false)
                {
                    fila.Cells[1].Shading.Color = Colors.LightCoral;
                }
                if (chequeo.Observacion!=null)
                {
                    fila.Cells[2].AddParagraph(chequeo.Observacion);
                }
            }
            Estilos.formatearTabla(tabla);
            tabla.Columns[0].Width = 200;
            tabla.Columns[1].Width = 20;
            tabla.Columns[2].Width = 300;
            tabla.Columns[2].Format.Alignment = ParagraphAlignment.Left;
            //tabla.Columns[1].Format.Alignment = ParagraphAlignment.Center;
            tabla.Rows.LeftIndent = "0.5cm";
            return tabla;
        }
    }
}
