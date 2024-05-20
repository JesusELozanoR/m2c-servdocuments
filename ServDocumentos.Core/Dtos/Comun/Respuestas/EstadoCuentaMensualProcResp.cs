using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class EstadoCuentaMensualProcResp
    {
        /// <summary>
        /// Número de Cliente
        /// </summary>
        [JsonProperty("numeroCliente")]
        public string NumeroCliente { get; set; }

        /// <summary>
        /// Número de Crédito
        /// </summary>
        [JsonProperty("numeroCredito")]
        public string NumeroCuenta { get; set; }
    }
}
