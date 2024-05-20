using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaRegDto
    {
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
        /// Descripcion del  Documento
        /// </summary>
        [JsonProperty("DescripcionDocumentos")]
        public string DescripcionDocumentos { get; set; }
    }
}
