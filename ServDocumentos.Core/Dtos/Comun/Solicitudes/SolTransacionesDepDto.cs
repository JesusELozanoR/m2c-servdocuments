using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolTransacionesDepDto
    {
        [JsonProperty("filterCriteria")]
        public List<searchCriteria> FilterCriteria = new List<searchCriteria>();

        [JsonProperty("sortingCriteria")]
        public sortingCriteria SortingCriteria { get; set; }
    }

    public class searchCriteria
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class sortingCriteria
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
