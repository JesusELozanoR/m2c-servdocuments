using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubProcesoClasDto
    {
        /// <summary>
        /// Nombre de la Empresa
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [EnumDataType(typeof(EmpresaSel), ErrorMessageResourceName = nameof(MensajesDataAnnotations.Empresa), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Empresa")]
        public EmpresaSel Empresa { get; set; }
        
        /// <summary>
        /// Proceso Id
        /// </summary>
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }
        
        /// <summary>
        /// SubProceso
        /// </summary>
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// Nombre de la Clasificacion
        /// </summary>
        [JsonProperty("ClasificacionId")]
        public int ClasificacionId { get; set; }
    }
}
