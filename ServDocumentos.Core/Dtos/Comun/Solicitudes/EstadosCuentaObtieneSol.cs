using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class EstadosCuentaObtieneSol
    {
        /// <summary>
        /// Empresa
        /// </summary>
        [JsonProperty("empresa")]
        public string Empresa { get; set; }

        /// <summary>
        /// Periodo Año y Mes
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("fecha")]
        public DateTime fecha { get; set; }

        /// <summary>
        /// Cantidad a Procesar
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("cantidad")]
        public int cantidad { get; set; }
    }
}
