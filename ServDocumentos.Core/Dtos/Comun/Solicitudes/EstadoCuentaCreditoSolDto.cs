using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class EstadoCuentaCreditoSolDto
    {
        /// <summary>
        /// Nombre del proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("procesoNombre")]
        public string ProcesoNombre { get; set; }
        /// <summary>
        /// Nombre del Subproceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("subProcesoNombre")]
        public string SubProcesoNombre { get; set; }
        /// <summary>
        /// Número de Cliente
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCliente")]
        public string NumeroCliente { get; set; }
        /// <summary>
        /// Número de Crédito
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCredito")]
        public string NumeroCredito { get; set; }
        /// <summary>
        /// Empresa
        /// </summary>
        [JsonProperty("empresa")]
        public string Empresa { get; set; }
        /// <summary>
        /// Lo regresa en zip
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("comprimido")]
        public bool Comprimido { get; set; }
        /// <summary>
        /// Si no se pide en base 64 se regresa en bytes
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty("Base64")]
        public bool Base64 { get; set; }
    }
}