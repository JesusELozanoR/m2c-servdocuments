using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubProcesoPlantillaUpdDto
    {
        /// <summary>
        /// id del SubProceso Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoPlantillaId")]
        public int SubProcesoPlantillaId { get; set; }

        /// <summary>
        /// id del SubProceso
        /// </summary>
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// id de la Plantilla
        /// </summary>
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// RECA
        /// </summary>
        [JsonProperty("RECA")]
        public string RECA { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }


    }
}
