using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosServSeguros
{
    public class CPADto
    {
        [JsonProperty("Seguros")]
        public List<CPASeguro> Seguros { get; set; }

        [JsonProperty("Beneficiarios")]
        public List<CPABeneficiario> Beneficiarios { get; set; }
    }
}
