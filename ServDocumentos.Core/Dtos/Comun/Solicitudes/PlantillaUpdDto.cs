using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaUpdDto
    {
        /// <summary>
        /// Archivo de la Plantilla
        /// </summary>
        [JsonProperty("Archivo64")]
        public string Archivo64 { get; set; }

        /// <summary>
        /// Id de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }
        /// <summary>
        /// Nombre de la Plantilla
        /// </summary>
        [JsonProperty("PlantillaNombre")]
        public string PlantillaNombre { get; set; }
        /// <summary>
        /// Descripcion de la Plantilla
        /// </summary>
        [JsonProperty("Descripcion")] 
        public string Descripcion { get; set; }
        /// <summary>
        /// Version de la Plantilla
        /// </summary>
        [JsonProperty("Version")]
        public string Version { get; set; }
        
        /// <summary>
        /// Descripcion Documentos de la Plantilla
        /// </summary>
        [JsonProperty("DescripcionDocumentos")]
        public string DescripcionDocumentos { get; set; }

        /// <summary>
        /// Id de Alfresco
        /// </summary>
        [JsonProperty("AlfrescoId")]
        public string AlfrescoId { get; set; }
        /// <summary>
        /// Url de Alfresco
        /// </summary>
        [JsonProperty("AlfrescoUrl")]
        public string AlfrescoUrl { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("usuario")]
        public string usuario { get; set; }
        
    }
}
