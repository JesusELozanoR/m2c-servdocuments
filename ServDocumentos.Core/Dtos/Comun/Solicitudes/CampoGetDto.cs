using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoGetDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }
        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        // [EnumDataType(typeof(CamposTipos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Dato Campo
        /// </summary>
        [JsonProperty("DatoCampo")]
        public string DatoCampo { get; set; }
    }
}
