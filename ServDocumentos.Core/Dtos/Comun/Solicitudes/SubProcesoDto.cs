using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubProcesoDto
    {        

        /// <summary>
        /// Nombre del SubProceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoNombre")]
        public string SubProcesoNombre { get; set; }

        /// <summary>
        /// Descripcion del SubProceso
        /// </summary>
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Proceso Id
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        // [EnumDataType(typeof(TipoProcesos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }
    }
}
