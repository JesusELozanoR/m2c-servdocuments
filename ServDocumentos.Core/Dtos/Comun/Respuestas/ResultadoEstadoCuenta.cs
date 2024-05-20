using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoEstadoCuenta
    {
        //public string Estado { get; set; }
        /// <summary>
        /// Nombre(s) de los archivo(s) pdf
        /// </summary>
        [JsonProperty("mensaje")]
        public string Mensaje { get; set; }
        /// <summary>
        /// Documento en Base64 
        /// </summary>
        [JsonProperty("dato")]
        public string Dato { get; set; }
    }
}
