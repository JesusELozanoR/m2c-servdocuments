using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosMambu.Enums
{
    /// <summary>
    /// Tipo de transaccion de credito
    /// </summary>
    public enum TipoTransaccionCredito
    {
        IMPORT,
        [Description("DESEMBOLSO")]
        DISBURSEMENT,
        DISBURSEMENT_ADJUSTMENT,
        WRITE_OFF,
        WRITE_OFF_ADJUSTMENT,
        [Description("PAGO")]
        REPAYMENT,
        PAYMENT_MADE,
        WITHDRAWAL_REDRAW,
        WITHDRAWAL_REDRAW_ADJUSTMENT,
        FEE_APPLIED,
        [Description("CARGO")]
        FEE,
        [Description("CARGO")]
        FEE_CHARGED,
        FEES_DUE_REDUCED,
        FEE_ADJUSTMENT,
        [Description("INTERES POR MORA")]
        PENALTY_APPLIED,
        PENALTY_ADJUSTMENT,
        PENALTIES_DUE_REDUCED,
        REPAYMENT_ADJUSTMENT,
        PAYMENT_MADE_ADJUSTMENT,
        INTEREST_RATE_CHANGED,
        TAX_RATE_CHANGED, 
        PENALTY_RATE_CHANGED, 
        [Description("INTERES")]
        INTEREST_APPLIED, 
        INTEREST_APPLIED_ADJUSTMENT,
        INTEREST_DUE_REDUCED,
        PENALTY_REDUCTION_ADJUSTMENT,
        FEE_REDUCTION_ADJUSTMENT, 
        INTEREST_REDUCTION_ADJUSTMENT, 
        DEFERRED_INTEREST_APPLIED,
        DEFERRED_INTEREST_APPLIED_ADJUSTMENT,
        DEFERRED_INTEREST_PAID,
        DEFERRED_INTEREST_PAID_ADJUSTMENT,
        INTEREST_LOCKED,
        FEE_LOCKED, 
        PENALTY_LOCKED,
        INTEREST_UNLOCKED,
        FEE_UNLOCKED,
        PENALTY_UNLOCKED,
        REDRAW_TRANSFER,
        REDRAW_REPAYMENT,
        REDRAW_TRANSFER_ADJUSTMENT,
        REDRAW_REPAYMENT_ADJUSTMENT, 
        TRANSFER, TRANSFER_ADJUSTMENT,
        BRANCH_CHANGED,
        TERMS_CHANGED,
        CARD_TRANSACTION_REVERSAL, 
        CARD_TRANSACTION_REVERSAL_ADJUSTMENT,
        DUE_DATE_CHANGED, 
        DUE_DATE_CHANGED_ADJUSTMENT,
        ACCOUNT_TERMINATED,
        ACCOUNT_TERMINATED_ADJUSTMENT
    }
}
