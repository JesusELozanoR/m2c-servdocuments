using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Respuesta de transaccion de credito de mamabu
    /// </summary>
    public class TransaccionCreditoDto
    {
        /// <summary>
        /// Fecha de la transaccion
        /// </summary>
        public DateTime ValueDate { get; set; }
        /// <summary>
        /// Tipo de transaccion
        /// </summary>
        public TipoTransaccionCredito? Type { get; set; }
        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Taxes
        /// </summary>
        public Taxes Taxes { get; set; }
        /// <summary>
        /// Montos afectados
        /// </summary>
        public AffectedAmounts AffectedAmounts { get; set; }
        /// <summary>
        /// Balances
        /// </summary>
        public AccountBalances AccountBalances { get; set; }
        /// <summary>
        /// Ajuste de transaccion 
        /// </summary>
        public string AdjustmentTransactionKey { get; set; }
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

    /// <summary>
    /// Taxes
    /// </summary>
    public class Taxes
    {
        /// <summary>
        /// Tax on fees amount
        /// </summary>
        public decimal TaxOnFeesAmount { get; set; }
        /// <summary>
        /// Tax on penalty amount
        /// </summary>
        public decimal TaxOnPenaltyAmount { get; set; }
        /// <summary>
        /// Tax on interest amount
        /// </summary>
        public decimal TaxOnInterestAmount { get; set; }
    }
    /// <summary>
    /// Montos afectados
    /// </summary>
    public class AffectedAmounts
    {
        /// <summary>
        /// Monto principal
        /// </summary>
        public decimal PrincipalAmount { get; set; }
        /// <summary>
        /// Monto interes
        /// </summary>
        public decimal InterestAmount { get; set; }
        /// <summary>
        /// Monto cuota
        /// </summary>
        public decimal FeesAmount { get; set; }
        /// <summary>
        /// Monto multa
        /// </summary>
        public decimal PenaltyAmount { get; set; }
    }
}
