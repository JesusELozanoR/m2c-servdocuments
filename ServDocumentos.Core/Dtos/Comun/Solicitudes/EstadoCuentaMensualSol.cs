using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class EstadoCuentaMensualSol
    {
        /// <summary>
        /// Nombre del proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("proceso")]
        public string Proceso { get; set; }

        /// <summary>
        /// Nombre del Subproceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("subProceso")]
        public string SubProceso { get; set; }

        /// <summary>
        /// Número de Cliente
        /// </summary>
        [JsonProperty("numeroCliente")]
        public string NumeroCliente { get; set; }

        /// <summary>
        /// Número de Crédito
        /// </summary>
        [JsonProperty("numeroCuenta")]
        public string NumeroCuenta { get; set; }

        /// <summary>
        /// Mes
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }

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
        /// <summary>
        /// Si no se especifica es porque se trata de un crédito individual
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("Grupal")]
        public bool Grupal { get; set; }
    }
}
