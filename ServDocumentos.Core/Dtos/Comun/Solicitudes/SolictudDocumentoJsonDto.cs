using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolictudDocumentoJsonDto : SolictudDocumentoDto
    {
        /// <summary>
        /// Json Cliente engine
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("jsonSolicitud")]
        public string JsonSolicitud { get; set; }
    }
}
