using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoDocumentoDto
    {
        //public string Estado { get; set; }
        /// <summary>
        /// Nombre(s) de los archivo(s) pdf, separados por "|"
        /// </summary>
        [JsonProperty("mensaje")]
        public string Mensaje { get; set; }
        /// <summary>
        /// Documento en Base64 (comprimido solo si se en la solicitud se pidió asi)
        /// </summary>
        [JsonProperty("dato")]
        public string Dato { get; set; }
        /// <summary>
        /// lista de strings con el valor en formato Bae64 delos archivos obtenidos
        /// (solo si se en la solicitud no se se pidió comprimido)
        /// </summary>
        [JsonProperty("listaDatos")]
        public List<string> ListaDatos { get; set; }
        /// <summary>
        /// Lista de Documentos en bytes (comprimido solo si se en la solicitud se pidió asi)
        /// </summary>
        [JsonProperty("archivo")]
        public List<byte[]> ListaArchivos { get; set; }

        /// <summary>
        /// Lista de Archivos en caso de Solicitar guardadi en Alfresco
        /// </summary>
        public List<ArchivosAlfresco> ListaArchivosAlfresco { get; set; }
        /// <summary>
        /// Lista de archivos almacenados
        /// </summary>
        public List<ArchivoGuardado> ArchivosGuardados { get; set; }

        public ResultadoDocumentoDto()
        {
            Mensaje = "";
            Dato = "";
            ListaDatos = new List<string>();
            ListaArchivos = new List<byte[]>();
            ListaArchivosAlfresco = new List<ArchivosAlfresco>();
            ArchivosGuardados = new List<ArchivoGuardado>();
        }
    }
    /// <summary>
    /// Clase con las propiedades de un archivo guardado
    /// </summary>
    public class ArchivoGuardado
    {
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Url de archivo
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Id de documento
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Mensaje
        /// </summary>
        public string Mensaje { get; set; }
    }

    public class ArchivosAlfresco { 
        public string Archivo { get; set; }
        public string AlfrescoId { get; set; }
        public string AlfrescoUrl { get; set; }
        public string Mensaje { get; set; }
    }
}
