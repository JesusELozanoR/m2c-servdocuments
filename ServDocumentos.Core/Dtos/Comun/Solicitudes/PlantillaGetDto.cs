using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaGetDto
    {
        /// <summary>
        /// Id de la Plantilla
        /// </summary>
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
        /// Id Alfresco
        /// </summary>
        [JsonProperty("AlfrescoId")]
        public string AlfrescoId { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        [JsonProperty("Estado")]
        public bool Estado { get; set; }

    }
}
