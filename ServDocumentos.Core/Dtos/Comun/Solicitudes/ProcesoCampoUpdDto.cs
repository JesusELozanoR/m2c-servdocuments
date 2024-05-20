using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ProcesoCampoUpdDto
    {
        /// <summary>
        /// Id del Proceso Campo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoCampoId")]
        public int ProcesoCampoId { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Id del Campo
        /// </summary>
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

    }
}
