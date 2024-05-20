using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.DatosMambu.Solicitudes
{
    /// <summary>
    /// Cirterios de ordenamiento
    /// </summary>
    public class SortingCriteria
    {
        /// <summary>
        /// Nombre del campo para ordenar
        /// </summary>
        [JsonProperty("field")]
        public string Field { get; set; }
        /// <summary>
        /// Orden (ASC, DESC)
        /// </summary>
        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
