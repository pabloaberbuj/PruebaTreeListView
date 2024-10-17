﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

//using EvilDICOM.Network.Messaging;

namespace PruebaTreeListView
{
    public static class IO
    {
        /// <summary>
        /// Escribe un objeto como Json en un archivo
        /// </summary>
        /// <param name="file">la ruta del archivo</param>
        /// <param name="theObj">el objeto para escribir</param>
        /*public static void writeObjectAsJson(string file, object theObj)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            File.WriteAllText(file, JsonConvert.SerializeObject(theObj,settings));
        }
        /// <summary>
        /// Agrega un objeto en la última posición de una lista en Json
        /// 
        /// </summary>
        /// <typeparam name="T">El tipo de la lista => List<T></typeparam>
        /// <param name="file">la ruta del archivo</param>
        /// <param name="theObj">el objeto para agregar</param>
        public static void appendObjectAsJson<T>(string file, object theObj)
        {
            BindingList<T> lista = readJsonList<T>(file);
            lista.Add((T)theObj);
            writeObjectAsJson(file, lista);
        }
        /// <summary>
        /// Lee una lista desde un archiov Json
        /// </summary>
        /// <typeparam name="T">El tipo de la lista => List<T></typeparam>
        /// <param name="file">la ruta del archivo</param>
        /// <returns>Devuelve una lista del tipo indicado List<T></returns>
        public static BindingList<T> readJsonList<T>(string file)
        {
            try
            {
                BindingList<T> lista = JsonConvert.DeserializeObject<BindingList<T>>(File.ReadAllText(file));
                return lista;
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show("No existe el archivo: " + file);
                BindingList<T> lista = new BindingList<T>();
                return lista;
            }
        }

        public static T readJson<T>(string file)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;
                T t = JsonConvert.DeserializeObject<T>(File.ReadAllText(file),settings);
                return t;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Devuelve un string con un nombre único para un archivo
        /// </summary>
        /// <param name="path">La ruta a la carpeta</param>
        /// <param name="baseName">El nombre deseado</param>
        /// <param name="maxAttempts">el número máximo que se le concatenará al baseName</param>
        /// <returns></returns>
        /// */
        public static string crearCarpetaPaciente(string pacienteLastName, string pacienteFirstName, string pacienteId, string cursoId, string planId)
        {
            string nombreMasID = pacienteLastName.ToUpper() + ", " + pacienteFirstName.ToUpper() + "-" + pacienteId;
            string pathDirectorio = Properties.Settings.Default.PathPacientes + @"\" + nombreMasID;
            if (!Directory.Exists(pathDirectorio))
            {
                Directory.CreateDirectory(pathDirectorio);
            }
            string aux = planId.Replace(':', '_').Replace('\\', '_').Replace('/', '_') + " (" + cursoId + ")";
            aux.Replace('\\', '-');
            if (!Directory.Exists(pathDirectorio + @"\" + aux))
            {
                Directory.CreateDirectory(pathDirectorio + @"\" + aux);
            }
            return pathDirectorio + @"\" + aux;
        }

        /*public static string crearCarpetaPacienteImagenes(string pacienteLastName, string pacienteFirstName, string pacienteId, string cursoId, string planId, string Equipo)
        {
            string nombreMasID = pacienteLastName.ToUpper() + ", " + pacienteFirstName.ToUpper() + "-" + pacienteId;
            string pathDirectorio = Properties.Settings.Default.PathImagenesPacientes + @"\" + Equipo + @"\" + nombreMasID;
            if (!Directory.Exists(pathDirectorio))
            {
                Directory.CreateDirectory(pathDirectorio);
            }
            string aux = planId.Replace(':', '_').Replace('\\', '_').Replace('/', '_') + " (" + cursoId + ")";
            aux.Replace('\\', '-');
            if (!Directory.Exists(pathDirectorio + @"\" + aux))
            {
                Directory.CreateDirectory(pathDirectorio + @"\" + aux);
            }
            return pathDirectorio + @"\" + aux;
        }*/

        public static string GetUniqueFilename(string path, string baseName, string extention = "txt", int maxAttempts = 128)
        {
            if (!File.Exists(string.Format("{0}{1}.{2}", path, baseName, extention)))
            {
                return string.Format("{0}{1}.{2}", path, baseName, extention);
            }
            else
            {
                for (int i = 1; i < maxAttempts; i++)
                {
                    if (!File.Exists(string.Format("{0}{1} ({2}).{3}", path, baseName, i, extention)))
                    {
                        return string.Format("{0}{1} ({2}).{3}", path, baseName, i, extention);
                    }
                }
            }
            return string.Format("{0}{1} - {2:yyyy-MM-dd_hh-mm-ss}.{3}", path, baseName, DateTime.Now, extention);
        }
        public static void crearCarpeta(string nombre)
        {
            if (!Directory.Exists(nombre))
            {
                Directory.CreateDirectory(nombre);
            }
        }

        public static string RutaFisica0()
        {
            if (Directory.Exists(@"\\fisica0\centro_de_datos2018\"))
            {
                return "fisica0";
            }
            else
            {
                return "10.100.0.252";
            }
        }

        /*public static void moverArchivo(string pathOrigen, string pathDestino)
        {
            if (!File.Exists(pathOrigen))
            {
                MessageBox.Show("No se encuentra el archivo dcm en origen");
            }
            else if (File.Exists(pathDestino))
            {
                MessageBox.Show("El archivo dcm ya existe en destino");
            }
            else
            {
                File.Move(pathOrigen, pathDestino);
            }
        }*/
    }
}