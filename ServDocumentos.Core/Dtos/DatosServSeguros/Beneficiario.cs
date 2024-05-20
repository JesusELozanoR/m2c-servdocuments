using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosServSeguros
{
    public class Beneficiario
    {
        [JsonProperty("id")]
        public string Id { get; set;}
        [JsonProperty("descripcionParentesco")]
        public string DescripcionParentesco {get; set;}
        [JsonProperty("codigoCredito")]
        public string CodigoCredito {get; set;}
        [JsonProperty("apellidoPaterno")]
        public string ApellidoPaterno {get; set;}
        [JsonProperty("apellidoMaterno")]
        public string ApellidoMaterno {get; set;}
        [JsonProperty("nombres")]
        public string Nombre {get; set;}
        [JsonProperty("fechaNacimiento")]
        public string FechaNacimiento {get; set;}
        [JsonProperty("codigoParentesco")]
        public string CodigoParentesco {get; set;}
        [JsonProperty("porcentaje")]
        public string Porcentaje {get; set;}
        [JsonProperty("codigoSeguro")]
        public string CodigoSeguro {get; set;}
        [JsonProperty("clienteId")]
        public string ClienteId {get; set;}

        public string PaternoMaternoNombre { get; set; }

    }
}
