using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaInsDto
    {
        /// <summary>
        /// Archivo de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Archivo64")]
        public string Archivo64 { get; set; }

        /// <summary>
        /// Extension del Archivo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Extension")]
        public ArchivosExtension Extension { get; set; }
        /// <summary>
        /// Descripción de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Version de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Version")]
        public string Version { get; set; }

        /// <summary>
        /// Usuario Creacion
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Descripcion del Documento
        /// </summary>
        [JsonProperty("DescripcionDocumentos")] 
        public string DescripcionDocumentos { get; set; }


        /// <summary>
        /// SubProcesoId
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// RECA asignado al Documento
        /// </summary>
        [JsonProperty("RECA")]
        public string RECA { get; set; }

        /// <summary>
        /// Tipo de Uso I=Individual,G= Grupal
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

    }
}
