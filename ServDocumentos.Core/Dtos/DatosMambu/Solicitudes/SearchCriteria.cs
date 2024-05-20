using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu.Solicitudes
{
    /// <summary>
    /// Criterio de busqueda
    /// </summary>
    public class SearchCriteria
    {
        /// <summary>
        /// Lista de condiciones para realizar filtro
        /// </summary>
        [JsonProperty("filterCriteria")]
        public List<FilterCriteria> FilterCriteria { get; private set; } = new List<FilterCriteria>();
        /// <summary>
        /// Criterios de ordenamiento
        /// </summary>
        [JsonProperty("sortingCriteria")]
        public SortingCriteria SortingCriteria { get; set; }
        /// <summary>
        /// Parametros de la obtencion de los datos
        /// </summary>
        [JsonIgnore]
        public Dictionary<string,string> Parameters { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// Agregar un filtro a los criterios de busqueda
        /// </summary>
        /// <param name="field">Nombre del campo</param>
        /// <param name="operador">Operador para comprar</param>
        /// <param name="value">Valor del campo</param>
        public void AgregarFiltro(string field, string operador, string value)
        {
            FilterCriteria.Add(new FilterCriteria
            {
                Field = field,
                Operator = operador,
                Value = value
            });
        }
        /// <summary>
        /// Agrega un filtro IN
        /// </summary>
        /// <param name="field">Nombre del campo</param>
        /// <param name="values">Valores (se usa con el operador IN)</param>
        public void AgregarFiltroIn(string field, IEnumerable<string> values)
        {
            FilterCriteria.Add(new FilterCriteria
            {
                Field = field,
                Operator = "IN",
                Values = values
            });
        }
        /// <summary>
        /// Agrega un filtro between
        /// </summary>
        /// <param name="field">Nombre del campo</param>
        /// <param name="value">Valor de inicio</param>
        /// <param name="secondValue">Valor final</param>
        public void AgregarFiltroBetween(string field, string value, string secondValue)
        {
            FilterCriteria.Add(new FilterCriteria
            {
                Field=field,
                Operator = "BETWEEN",
                Value = value,
                SecondValue = secondValue
            });
        }
        /// <summary>
        /// Agrega ordenamiento
        /// </summary>
        /// <param name="field">Nombre del campo</param>
        /// <param name="order">Tipo de ordenamiento (ASC, DESC)</param>
        public void AgregarOrdenamiento(string field, string order)
        {
            SortingCriteria = new SortingCriteria()
            {
                Field = field,
                Order = order
            };
        }
        /// <summary>
        /// Agregar limite
        /// </summary>
        /// <param name="limite">Cantidad de registro a obtener</param>
        public void AgregarLimite(int limite)
        {
            if (Parameters.ContainsKey("limit"))
                Parameters["limit"] = limite.ToString();
            else
                Parameters.Add("limit", limite.ToString());
        }
        /// <summary>
        /// Agregar offset
        /// </summary>
        /// <param name="offset">Cantidad de registros a omitir</param>
        public void AgregarOffset(int offset) {

            if (Parameters.ContainsKey("offset"))
                Parameters["offset"] = offset.ToString();
            else
                Parameters.Add("offset", offset.ToString());
        }
        public void AgregarFiltroEsVacio(string field)
        {
            FilterCriteria.Add(new FilterCriteria
            {
                Field = field,
                Operator = "EMPTY"
            });
        }
    }
}
