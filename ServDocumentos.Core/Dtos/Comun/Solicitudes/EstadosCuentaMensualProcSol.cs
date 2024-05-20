using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class EstadosCuentaMensualProcSol
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
        public DateTime Fecha { get; set; }


        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("elementos")]
        public int Elementos { get; set; }

    }
}
