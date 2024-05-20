using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Mensajes;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolictudVistaPreviaDto
    {
        /// <summary>
        /// Id del Proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

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
        /// Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("PlantillaBase64")]
        public string PlantillaBase64 { get; set; }


        /// <summary>
        /// Listado de Campos
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ListaCampos")]
        public IEnumerable<CampoPreviewDto> ListaCampos { get; set; }

    }
}
