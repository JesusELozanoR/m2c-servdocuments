using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class RespTransacionesDepDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("valueDate")]
        public string ValueDate
        {
            get; set;
        }
    }
}
