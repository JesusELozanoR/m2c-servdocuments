using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubprocesoPlantillaGetDto
    {
        /// <summary>
        /// id del SubProceso Plantilla
        /// </summary>
        [JsonProperty("SubpPantId")]
        public int SubpPantId { get; set; }

        /// <summary>
        /// id del SubProceso
        /// </summary>
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// Nombre del Proceso
        /// </summary>
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        ///// <summary>
        ///// Usuario
        ///// </summary>
        //[JsonProperty("Estado")]
        //public bool Estado { get; set; }
        
        /// <summary>
        /// Usuario
        /// </summary>
        [JsonProperty("Reca")]
        public string Reca { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }
    }
}
