using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosServSeguros
{
    public class CPABeneficiario
    {
        [JsonProperty("ApellidoPaterno")]
        public string ApellidoPaterno { get; set; }

        [JsonProperty("ApellidoMaterno")]
        public string ApellidoMaterno { get; set; }

        [JsonProperty("Nombres")]
        public string Nombres { get; set; }

        public string NombreCompletoReves { get; set; }

        [JsonProperty("FechaNacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonProperty("Parentesco")]
        public string Parentesco { get; set; }

        [JsonProperty("Porcentaje")]
        public string Porcentaje { get; set; }

    }
}
