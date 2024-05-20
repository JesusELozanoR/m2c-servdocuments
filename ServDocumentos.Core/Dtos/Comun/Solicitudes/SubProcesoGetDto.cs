using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubProcesoGetDto
    {
        /// <summary>
        /// Id del Proceso
        /// </summary>
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// Nombre del SubProceso
        /// </summary>
        [JsonProperty("SubProcesoNombre")]
        public string SubProcesoNombre { get; set; }

        /// <summary>
        /// Proceso Id
        /// </summary>
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Nombre del Proceso
        /// </summary>
        [JsonProperty("ProcesoNombre")]
        public string ProcesoNombre { get; set; }

        /// <summary>
        /// Nombre del Proceso
        /// </summary>
        [JsonProperty("ProcesoDescripcion")]
        public string ProcesoDescripcion { get; set; }
    }
}
