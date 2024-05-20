using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class AccountArrearsSettings
    {
        public string encodedKey { get; set; }
        public string toleranceCalculationMethod { get; set; }
        public string dateCalculationMethod { get; set; }
        public string nonWorkingDaysMethod { get; set; }
        public int tolerancePeriod { get; set; }
    }

    public class Balances
    {
        public double redrawBalance { get; set; }
        public double principalDue { get; set; }
        public double principalPaid { get; set; }
        public double principalBalance { get; set; }
        public double interestDue { get; set; }
        public double interestPaid { get; set; }
        public double interestBalance { get; set; }
        public double interestFromArrearsBalance { get; set; }
        public double interestFromArrearsDue { get; set; }
        public double interestFromArrearsPaid { get; set; }
        public double feesDue { get; set; }
        public double feesPaid { get; set; }
        public double feesBalance { get; set; }
        public double penaltyDue { get; set; }
        public double penaltyPaid { get; set; }
        public double penaltyBalance { get; set; }
        public double holdBalance { get; set; }
    }

    public class TransactionDetails
    {
        public string encodedKey { get; set; }

        public string targetDepositAccountKey { get; set; }
        public string transactionChannelKey { get; set; }
        public string transactionChannelId { get; set; }
        public bool internalTransfer { get; set; }
    }
    public class ReferenciasPago
    {
        public string Referencia_Pago_Tipo { get; set; }
        public string Referencia_Pago_Valor { get; set; }
    }

    public class DisbursementDetails
    {
        public string encodedKey { get; set; }
        public DateTime expectedDisbursementDate { get; set; }
        public DateTime disbursementDate { get; set; }
        public TransactionDetails transactionDetails { get; set; }
        public IList<Fees> fees { get; set; }
    }

    public class PrepaymentSettings
    {
    }

    public class PenaltySettings
    {
        public string loanPenaltyCalculationMethod { get; set; }
        public double penaltyRate { get; set; }
    }

    public class ScheduleSettings
    {
        public bool hasCustomSchedule { get; set; }
        public int principalRepaymentInterval { get; set; }
        public int gracePeriod { get; set; }
        public string gracePeriodType { get; set; }
        public int repaymentInstallments { get; set; }
        public int repaymentPeriodCount { get; set; }
        public string scheduleDueDatesMethod { get; set; }
        public int periodicPayment { get; set; }
        public string repaymentPeriodUnit { get; set; }
        public string repaymentScheduleMethod { get; set; }
        public IList<object> paymentPlan { get; set; }
    }

    public class InterestSettings
    {
        public string interestRateSource { get; set; }
        public bool accrueInterestAfterMaturity { get; set; }
        public string interestApplicationMethod { get; set; }
        public string interestBalanceCalculationMethod { get; set; }
        public string interestCalculationMethod { get; set; }
        public string interestChargeFrequency { get; set; }
        public double interestRate { get; set; }
        public string interestType { get; set; }
        public bool accrueLateInterest { get; set; }
        public decimal CuotaMensual { get; set; }
    }

    public class DatosAdicionales
    {
        public string Url_Identificacion_Anverso { get; set; }
        public string Referencia_Retiro { get; set; }
        public string Referencia_Pago { get; set; }
        public string Tabla_Pagos { get; set; }
        public string Monto_Bruto_Datos_Adicionales { get; set; }
        public string Interes_Mensual_Adicionales { get; set; }
        public string Url_Identificacion_Reverso { get; set; }
        public string CAT_Datos_Adicionales { get; set; }
        public string Estatus_Retiro { get; set; }
        public string Tipo_Seguro_Voluntario { get; set; }
        public double Monto_Seguro_Voluntario { get; set; }
        public double Monto_Seguro_Basico { get; set; }
        public double Total_Pagar { get; set; }
        public decimal Tasa_Moratoria_Anual { get; set; }
        public decimal Monto_Cuota { get; set; }
        public string Referencia_Pago_OXXO { get; set; }
        public string Referencia_Pago_Valor { get; set; }
        /// <summary>
        /// Encoded key del grupo
        /// </summary>
        public string Grupo { get; set; }
    }

    public class RelacionesCredito { 
        public string Titular_Credito  { get; set; }
    }

    public class Credito
    {
        public string encodedKey { get; set; }
        public string id { get; set; }
        public string accountHolderType { get; set; }
        public string accountHolderKey { get; set; }
        public string settlementAccountKey { get; set; }

        public DateTime creationDate { get; set; }
        public DateTime approvedDate { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public string activationTransactionKey { get; set; }
        public DateTime lastSetToArrearsDate { get; set; }
        public int daysInArrears { get; set; }
        public int daysLate { get; set; }
        public DateTime lastAccountAppraisalDate { get; set; }
        public string accountState { get; set; }
        public string productTypeKey { get; set; }
        public string loanName { get; set; }
        public double loanAmount { get; set; }
        public string paymentMethod { get; set; }
        public double accruedInterest { get; set; }
        public double interestFromArrearsAccrued { get; set; }
        public DateTime lastInterestAppliedDate { get; set; }
        public int accruedPenalty { get; set; }
        public bool allowOffset { get; set; }
        public int arrearsTolerancePeriod { get; set; }
        public AccountArrearsSettings accountArrearsSettings { get; set; }
        public string latePaymentsRecalculationMethod { get; set; }
        public Balances balances { get; set; }
        public DisbursementDetails disbursementDetails { get; set; }
        public PrepaymentSettings prepaymentSettings { get; set; }
        public PenaltySettings penaltySettings { get; set; }
        public ScheduleSettings scheduleSettings { get; set; }
        public InterestSettings interestSettings { get; set; }
        public IList<object> tranches { get; set; }
        public string futurePaymentsAcceptance { get; set; }
        public DatosAdicionales _Datos_Adicionales { get; set; }
        public DatosAdicionales _Datos_Adicionales_Credito { get; set; }
        public RelacionesCredito _Relaciones_Credito { get; set; }
        public IList<ReferenciasPago> _Referencias_Pago { get; set; }
        public string assignedBranchKey { get; set; }
        public ReferenciasCore referenciasPago { get; set; }
        public IList<IntegranteCredito> _Integrantes_Credito { get; set; }

        public decimal TasaMoratoriaAnual { get; set; }
        public decimal CuotaMensual { get; set; }
        public decimal InteresMoratorioMensual { get; set; }
        public string ReferenciaPagoOPENPAY { get; set; }
        public string ReferenciaPagoBBVA { get; set; }
        public string ReferenciaPagoNumero { get; set; }

    }

    public class ReferenciasPagoCore
    {
        public string referenciaPago { get; set; }
        public string corresponsal { get; set; }
    }
    public class ReferenciasCore
    {
        public IList<ReferenciasPagoCore> ReferenciasPago { get; set; }
    }
    public class IntegranteCredito
    {
        public int _index { get; set; }
        public string Rol_integrante { get; set; }
        public string Cliente_Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Direccion { get; set; }
    }
    public class Fees
    {
        public double amount { get; set; }
    }
}
