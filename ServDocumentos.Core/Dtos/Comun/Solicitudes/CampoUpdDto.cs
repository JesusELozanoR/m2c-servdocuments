using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoUpdDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }
        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

        /// <summary>
        /// Descripcion
        /// </summary>
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        // [EnumDataType(typeof(CampoTipo), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Dato Conjunto
        /// </summary>
        [JsonProperty("DatoConjunto")]
        public string DatoConjunto { get; set; }

        /// <summary>
        /// Dato Campo
        /// </summary>
        [JsonProperty("DatoCampo")]
        public string DatoCampo { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Dato Conjunto Grupal
        /// </summary>
        [JsonProperty("DatoConjuntoGrupal")]
        public string DatoConjuntoGrupal { get; set; }

        /// <summary>
        /// Ejemplo del Valor
        /// </summary>
        [JsonProperty("Ejemplo")]
        public string Ejemplo { get; set; }
    }
}
