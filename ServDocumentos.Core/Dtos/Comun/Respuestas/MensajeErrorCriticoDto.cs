
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class MensajeErrorCriticoDto : MensajeErrorFuncionalDto
    {
        /// <summary>
        /// Código que genera el servicio para indenficar el Error Critico 
        /// </summary>
        [Required]
        [JsonProperty("codigoRastreo")]
        public string CodigoRastreo { get; set; }
    }
}
