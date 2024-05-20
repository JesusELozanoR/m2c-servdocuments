using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ProcesoCampoGetDto
    {
        /// <summary>
        /// Id del Proceso Campo
        /// </summary>
        [JsonProperty("ProcesoCampoId")]
        public int ProcesoCampoId { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Id del  Campo
        /// </summary>
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }

        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

    }
}
