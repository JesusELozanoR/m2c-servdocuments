using System.ComponentModel;

namespace ServDocumentos.Core.Dtos.DatosMambu.Enums
{
    /// <summary>
    /// Tipo de transaccion de ahorro
    /// </summary>
    public enum TipoTransaccionAhorro
    {
        [Description("Importación")]
        IMPORT,
        [Description("Perdida")]
        WRITE_OFF,
        WRITE_OFF_ADJUSTMENT,
        [Description("Depósito")]
        DEPOSIT,
        ADJUSTMENT,
        [Description("Retiro")]
        WITHDRAWAL,
        WITHDRAWAL_ADJUSTMENT,
        [Description("Reversa de transacción de tarjeta")]
        CARD_TRANSACTION_REVERSAL,
        CARD_TRANSACTION_REVERSAL_ADJUSTMENT,
        [Description("Transferencia")]
        TRANSFER,
        TRANSFER_ADJUSTMENT,
        [Description("Tarifa aplicada")]
        FEE_APPLIED,
        FEE_ADJUSTED,
        [Description("Tarifas debidas reducidas")]
        FEES_DUE_REDUCED,
        [Description("Interés Aplicado")]
        INTEREST_APPLIED,
        INTEREST_APPLIED_ADJUSTMENT,
        NET_DIFF_INTEREST,
        FEE_REDUCTION_ADJUSTMENT,
        [Description("Retención de impuestos")]
        WITHHOLDING_TAX,
        WITHHOLDING_TAX_ADJUSTMENT,
        [Description("Interés Valoración cambiada")]
        INTEREST_RATE_CHANGED,
        [Description("Cambios en la tasa de interés sobregiro")]
        OVERDRAFT_INTEREST_RATE_CHANGED,
        [Description("Cambio de limite sobre giro")]
        OVERDRAFT_LIMIT_CHANGED,
        [Description("Cambio sucursal")]
        BRANCH_CHANGED,
        [Description("Préstamo financiado")]
        LOAN_FUNDED,
        LOAN_FUNDED_ADJUSTMENT,
        [Description("Préstamo reembolsado")]
        LOAN_REPAID,
        LOAN_REPAID_ADJUSTMENT,
        [Description("Fracción de préstamo comprado")]
        LOAN_FRACTION_BOUGHT,
        LOAN_FRACTION_BOUGHT_ADJUSTMENT,
        [Description("Fracción de préstamo vendido")]
        LOAN_FRACTION_SOLD,
        LOAN_FRACTION_SOLD_ADJUSTMENT,
        [Description("Monto ajustado")]
        SEIZED_AMOUNT
    }
}
