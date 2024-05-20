using Newtonsoft.Json;
using ServDocumentos.Core.Entidades.Comun;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoUrlDocumeto
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
        public List<DatosUrl> Dato { get; set; }
        /// <summary>
        /// lista de strings con el valor en formato Bae64 delos archivos obtenidos
        /// (solo si se en la solicitud no se se pidió comprimido)
        /// </summary>
        [JsonProperty("listaDatos")]
        public List<DatosUrl> ListaDatos { get; set; }

        public ResultadoUrlDocumeto()
        {
            //Estado = "";
            Mensaje = "";
            Dato = new List<DatosUrl>();
            ListaDatos = new List<DatosUrl>();
        }
    }
}
