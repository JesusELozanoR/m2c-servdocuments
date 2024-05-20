using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosServSeguros
{
    public class CPASeguro
    {
        public CPASeguro()
        {
            FechaInicio = String.Empty;
            CodigoCredito = String.Empty;
            Poliza = String.Empty;
            ClienteId = String.Empty;
            CodigoSeguro = String.Empty;
            CodigoSeguroDescripcion = String.Empty;
            Cobertura = string.Empty;
            MontoTotal = String.Empty;
        }

        [JsonProperty("FechaInicio")]
        public string FechaInicio { get; set; }

        [JsonProperty("CodigoCredito")]
        public string CodigoCredito { get; set; }

        [JsonProperty("Poliza")]
        public string Poliza { get; set; }

        [JsonProperty("ClienteId")]
        public string ClienteId { get; set; }

        [JsonProperty("CodigoSeguro")]
        public string CodigoSeguro { get; set; }

        [JsonProperty("CodigoSeguroDescripcion")]
        public string CodigoSeguroDescripcion { get; set; }

        [JsonProperty("Cobertura")]
        public string Cobertura { get; set; }

        [JsonProperty("MontoTotal")]
        public string MontoTotal { get; set; }

    }
}
