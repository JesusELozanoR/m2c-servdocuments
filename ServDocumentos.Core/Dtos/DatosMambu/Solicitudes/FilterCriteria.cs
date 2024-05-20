using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu.Solicitudes
{
    /// <summary>
    /// Criterio para filtrar
    /// </summary>
    public class FilterCriteria
    {
        /// <summary>
        /// Nombre del campo para filtrar
        /// </summary>
        [JsonProperty("field")]
        public string Field { get; set; }
        /// <summary>
        /// Operador par realizar la comparacion
        /// </summary>
        [JsonProperty("operator")]
        public string Operator { get; set; }
        /// <summary>
        /// Valor a comparar
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// Segundo valor a comprar (Usado con el operador BETWEEN)
        /// </summary>
        [JsonProperty("secondValue")]
        public string SecondValue { get; set; }
        /// <summary>
        /// Valores a comprar (usado con el operado IN)
        /// </summary>
        [JsonProperty("values")]
        public IEnumerable<string> Values { get; set; }
    }
}
