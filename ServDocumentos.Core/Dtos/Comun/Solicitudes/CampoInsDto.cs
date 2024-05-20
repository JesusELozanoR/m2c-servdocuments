using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoInsDto
    {
        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

        /// <summary>
        /// Descripcion
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Tipo de campo
        /// </summary>
        /// <example>Se compara contra Catalogo</example>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        // [EnumDataType(typeof(CampoTipos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Tipo")]
        [StringLength(20, ErrorMessage = "El tipo no puede tener mas de 20 caracteres.")]
        public string Tipo { get; set; }

        /// <summary>
        /// Dato Conjunto
        /// </summary>
        [JsonProperty("DatoConjunto")]
        public string DatoConjunto { get; set; }

        /// <summary>
        /// Dato Campo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
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

        /// <summary>
        /// ProcesoId
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// SubProcesoId
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

    }
}
