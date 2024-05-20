using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ObtenerDatosDto : SolicitudBaseDto
    {
        /// <summary>
        /// Tipo de proceso a seguir, los valores permitodos son 1 y dos
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        // [EnumDataType(typeof(TipoProcesos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("proceso")]
        public string Proceso { get; set; }         // Nombre del proceso
        public string Subproceso { get; set; }

        

    }
}
