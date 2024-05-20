using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosServSeguros
{
    public class Seguro
    {
        [JsonProperty("id")]
        public string Id { get; set;}
        [JsonProperty("descripcionEstadoSolicitud")]
        public string DescripcionEstadoSolicitud { get; set; }
        [JsonProperty("descripcionTipoSeguro")]
        public string DescripcionTipoSeguro { get; set; }
        [JsonProperty("codigoCredito")]
        public string CodigoCredito { get; set; }
        [JsonProperty("poliza")]
        public string Poliza {get; set;}
        [JsonProperty("grupoId")]
        public string GrupoId {get; set;}
        [JsonProperty("clienteId")]
        public string ClienteId {get; set;}
        [JsonProperty("codigoSeguro")]
        public string CodigoSeguro {get; set;}
        [JsonProperty("cantidad")]
        public string Cantidad {get; set;}
        [JsonProperty("montoTotal")]
        public string MontoTotal {get; set;}
        [JsonProperty("codigoEstadoSolicitud")]
        public string CodigoEstadoSolicitud {get; set;}
        [JsonProperty("fechaInicio")]
        public string FechaInicio {get; set;}
        [JsonProperty("fechaVencimiento")]
        public string FechaVencimiento {get; set; }
        [JsonProperty("beneficiarios")]
        public List<Beneficiario> Beneficiarios {get; set;}
    }
}
