using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class MensajeErrorFuncionalDto
    {
        /// <summary>
        /// Nombre del Servicio donde se origina el Conflicto o la Excepción
        /// </summary>
        [Required]
        [JsonProperty("origen")]
        public string Origen { get; set; }
        /// <summary>
        /// Mensaje indicativo del Conflicto o la Excepción
        /// </summary>
        [Required]        
        [JsonProperty("mensajes")]
        public IEnumerable<string> Mensajes { get; set; }

    }
}
