using Newtonsoft.Json;
using System;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    [Serializable]
    public class ArchivoPlantillaDto
    {
        /// <summary>
        /// Archivo en formato Base64
        /// </summary>        
        [JsonProperty("base64")]
        public string Base64 { get; set; }
        /// <summary>
        /// Nombre de la Plantilla
        /// </summary>
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
    }
}
