using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServDocumentos.Core.Helpers;
using ServDocumentos.Core.Mensajes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolicitudBaseDto
    {

        /// <summary>
        /// Clave del usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("usuario")]
        public string Usuario { get; set; }
        /// <summary>
        /// Número de credito
        /// </summary>        
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCredito")]
        public string NumeroCredito { get; set; }
        /// <summary>
        /// Número del cliente
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCliente")]
        public string NumeroCliente { get; set; }

        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public string Empresa { get; set; }
        
        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public List<int> NumerosClientes { get; set; }

        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public List<int> NumeroDividendos { get; set; }
    }
}
