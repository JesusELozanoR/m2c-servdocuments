using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Transaccion de ahorro del api de mambu
    /// </summary>
    public class TransaccionAhorroDto
    {
        /// <summary>
        /// Id de la transaccion
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Encoded Key de la transaccion
        /// </summary>
        public string EncodedKey { get; set; }
        /// <summary>
        /// Value date
        /// </summary>
        public DateTime ValueDate { get; set; }
        /// <summary>
        /// Tipo de transaccion
        /// </summary>
        public TipoTransaccionAhorro? Type { get; set; }
        /// <summary>
        /// Cantidad de la transaccion
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Moneda
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Comentario de la transaccion
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Balances
        /// </summary>
        public AccountBalances AccountBalances { get; set; }
        /// <summary>
        /// Detalles de la transaccion
        /// </summary>
        public TransactionDetails TransactionDetails { get; set; }
        /// <summary>
        /// Descripcion del tipo de transaccion
        /// </summary>
        [JsonIgnore]
        public string TypeDescription
        {
            get
            {
                return Type
                    ?.GetType()
                    .GetMember(Type.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description ?? Type?.ToString();
            }
        }
    }
}
