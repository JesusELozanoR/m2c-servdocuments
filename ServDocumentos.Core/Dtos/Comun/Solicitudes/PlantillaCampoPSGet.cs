using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaCampoPSGet
    {
        /// <summary>
        /// Id del Proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Id del SubProceso
        /// </summary>
        [JsonProperty("SubprocesoId")]
        public int SubprocesoId { get; set; }

        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

        /// <summary>
        /// Descripcion del Campo
        /// </summary>
        [JsonProperty("CampoDescripcion")]
        public string CampoDescripcion { get; set; }

    }
}
