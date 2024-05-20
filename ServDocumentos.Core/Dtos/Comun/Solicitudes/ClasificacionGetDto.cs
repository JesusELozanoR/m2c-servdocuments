using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ClasificacionGetDto
    {
        /// <summary>
        /// Id de la Clasificacion
        /// </summary>
        [JsonProperty("ClasificacionId")]
        public int ClasificacionId { get; set; }

        /// <summary>
        /// Nombre de la Clasificacion
        /// </summary>
        [JsonProperty("Clasificacion")]
        public string Clasificacion { get; set; }

        /// <summary>
        /// Nombre de la Empresa
        /// </summary>
        [EnumDataType(typeof(EmpresaSel), ErrorMessageResourceName = nameof(MensajesDataAnnotations.Empresa), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Empresa")]
        public EmpresaSel Empresa { get; set; }
    }
}
