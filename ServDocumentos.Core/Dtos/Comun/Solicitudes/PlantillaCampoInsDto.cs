using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaCampoInsDto
    {
        /// <summary>
        /// Id de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        /// <summary>
        /// Lista Id's Campos
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ListaCampoIds")]
        public List<int> ListaCampoIds { get; set; }

        /// <summary>
        /// usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

    }
}
