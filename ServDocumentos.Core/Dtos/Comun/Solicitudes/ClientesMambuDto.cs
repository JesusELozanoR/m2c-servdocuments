using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ClientesMambuDto
    {
        [JsonProperty("filterCriteria")]
        public List<filterCriteria> FilterCriteria = new List<filterCriteria>();
    }

    public partial class filterCriteria 
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("values")]
        public List<string> Value { get; set; } = new List<string>();
    }
}
