using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace ServDocumentos.Core.Dtos.DatosMambu
{
   

    public partial class ProductosCreditoMambu
    {
        [JsonProperty("accountLinkSettings")]
        public AccountLinkSettings AccountLinkSettings { get; set; }

        [JsonProperty("accountingSettings")]
        public AccountingSettings AccountingSettings { get; set; }

        [JsonProperty("allowCustomRepaymentAllocation")]
        public bool AllowCustomRepaymentAllocation { get; set; }

        [JsonProperty("arrearsSettings")]
        public ArrearsSettings ArrearsSettings { get; set; }

        [JsonProperty("availabilitySettings")]
        public AvailabilitySettings AvailabilitySettings { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("creditArrangementSettings")]
        public CreditArrangementSettings CreditArrangementSettings { get; set; }

        [JsonProperty("currency")]
        public Currency Currency { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("feesSettings")]
        public FeesSettings FeesSettings { get; set; }

        [JsonProperty("fundingSettings")]
        public FundingSettings FundingSettings { get; set; }

        [JsonProperty("gracePeriodSettings")]
        public GracePeriodSettings GracePeriodSettings { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("interestSettings")]
        public InterestSettings2 InterestSettings { get; set; }

        [JsonProperty("internalControls")]
        public InternalControls InternalControls { get; set; }

        [JsonProperty("lastModifiedDate")]
        public string LastModifiedDate { get; set; }

        [JsonProperty("loanAmountSettings")]
        public LoanAmountSettings LoanAmountSettings { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("newAccountSettings")]
        public NewAccountSettings NewAccountSettings { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("offsetSettings")]
        public OffsetSettings OffsetSettings { get; set; }

        [JsonProperty("paymentSettings")]
        public PaymentSettings PaymentSettings { get; set; }

        [JsonProperty("penaltySettings")]
        public PenaltySettings2 PenaltySettings { get; set; }

        [JsonProperty("redrawSettings")]
        public RedrawSettings RedrawSettings { get; set; }

        [JsonProperty("scheduleSettings")]
        public ScheduleSettings2 ScheduleSettings { get; set; }

        [JsonProperty("securitySettings")]
        public SecuritySettings SecuritySettings { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("taxSettings")]
        public TemperaturesTaxSettings TaxSettings { get; set; }

        [JsonProperty("templates")]
        public List<Template> Templates { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class AccountLinkSettings
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("linkableDepositProductKey")]
        public string LinkableDepositProductKey { get; set; }

        [JsonProperty("linkedAccountOptions")]
        public List<string> LinkedAccountOptions { get; set; }

        [JsonProperty("settlementMethod")]
        public string SettlementMethod { get; set; }
    }

    public partial class AccountingSettings
    {
        [JsonProperty("accountingMethod")]
        public string AccountingMethod { get; set; }

        [JsonProperty("accountingRules")]
        public List<AccountingRule> AccountingRules { get; set; }

        [JsonProperty("interestAccrualCalculation")]
        public string InterestAccrualCalculation { get; set; }

        [JsonProperty("interestAccruedAccountingMethod")]
        public string InterestAccruedAccountingMethod { get; set; }
    }

    public partial class AccountingRule
    {
        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("financialResource")]
        public string FinancialResource { get; set; }

        [JsonProperty("glAccountKey")]
        public string GlAccountKey { get; set; }

        [JsonProperty("transactionChannelKey")]
        public string TransactionChannelKey { get; set; }
    }

    public partial class ArrearsSettings
    {
        [JsonProperty("dateCalculationMethod")]
        public string DateCalculationMethod { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("monthlyToleranceDay")]
        public long MonthlyToleranceDay { get; set; }

        [JsonProperty("nonWorkingDaysMethod")]
        public string NonWorkingDaysMethod { get; set; }

        [JsonProperty("toleranceCalculationMethod")]
        public string ToleranceCalculationMethod { get; set; }

        [JsonProperty("toleranceFloorAmount")]
        public long ToleranceFloorAmount { get; set; }

        [JsonProperty("tolerancePercentageOfOutstandingPrincipal")]
        public TolerancePercentageOfOutstandingPrincipal TolerancePercentageOfOutstandingPrincipal { get; set; }

        [JsonProperty("tolerancePeriod")]
        public TolerancePercentageOfOutstandingPrincipal TolerancePeriod { get; set; }
    }

    public partial class TolerancePercentageOfOutstandingPrincipal
    {
        [JsonProperty("defaultValue")]
        public long DefaultValue { get; set; }

        [JsonProperty("maxValue")]
        public long MaxValue { get; set; }

        [JsonProperty("minValue")]
        public long MinValue { get; set; }

        [JsonProperty("encodedKey", NullValueHandling = NullValueHandling.Ignore)]
        public string EncodedKey { get; set; }
    }

    public partial class AvailabilitySettings
    {
        [JsonProperty("availableFor")]
        public List<string> AvailableFor { get; set; }

        [JsonProperty("branchSettings")]
        public BranchSettings BranchSettings { get; set; }
    }

    public partial class BranchSettings
    {
        [JsonProperty("availableProductBranches")]
        public List<string> AvailableProductBranches { get; set; }

        [JsonProperty("forAllBranches")]
        public bool ForAllBranches { get; set; }
    }

    public partial class CreditArrangementSettings
    {
        [JsonProperty("creditArrangementRequirement")]
        public string CreditArrangementRequirement { get; set; }
    }

    public partial class Currency
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public partial class FeesSettings
    {
        [JsonProperty("allowArbitraryFees")]
        public bool AllowArbitraryFees { get; set; }

        [JsonProperty("fees")]
        public List<Fee> Fees { get; set; }
    }

    public partial class Fee
    {
        [JsonProperty("accountingRules")]
        public List<AccountingRule> AccountingRules { get; set; }

        [JsonProperty("amortizationSettings")]
        public AmortizationSettings AmortizationSettings { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("amountCalculationMethod")]
        public string AmountCalculationMethod { get; set; }

        [JsonProperty("applyDateMethod")]
        public string ApplyDateMethod { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("feeApplication")]
        public string FeeApplication { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lastModifiedDate")]
        public string LastModifiedDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("percentageAmount")]
        public double PercentageAmount { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("taxSettings")]
        public FeeTaxSettings TaxSettings { get; set; }

        [JsonProperty("trigger")]
        public string Trigger { get; set; }
    }

    public partial class AmortizationSettings
    {
        [JsonProperty("amortizationProfile")]
        public string AmortizationProfile { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("feeAmortizationUponRescheduleRefinanceOption")]
        public string FeeAmortizationUponRescheduleRefinanceOption { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("intervalCount")]
        public long IntervalCount { get; set; }

        [JsonProperty("intervalType")]
        public string IntervalType { get; set; }

        [JsonProperty("periodCount")]
        public long PeriodCount { get; set; }

        [JsonProperty("periodUnit")]
        public string PeriodUnit { get; set; }
    }

    public partial class FeeTaxSettings
    {
        [JsonProperty("taxableCalculationMethod")]
        public string TaxableCalculationMethod { get; set; }
    }

    public partial class FundingSettings
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("funderInterestCommission")]
        public TolerancePercentageOfOutstandingPrincipal FunderInterestCommission { get; set; }

        [JsonProperty("funderInterestCommissionAllocationType")]
        public string FunderInterestCommissionAllocationType { get; set; }

        [JsonProperty("lockFundsAtApproval")]
        public bool LockFundsAtApproval { get; set; }

        [JsonProperty("organizationInterestCommission")]
        public TolerancePercentageOfOutstandingPrincipal OrganizationInterestCommission { get; set; }

        [JsonProperty("requiredFunds")]
        public long RequiredFunds { get; set; }
    }

    public partial class GracePeriodSettings
    {
        [JsonProperty("gracePeriod")]
        public TolerancePercentageOfOutstandingPrincipal GracePeriod { get; set; }

        [JsonProperty("gracePeriodType")]
        public string GracePeriodType { get; set; }
    }

    public partial class InterestSettings2
    {
        [JsonProperty("accrueLateInterest")]
        public bool AccrueLateInterest { get; set; }

        [JsonProperty("compoundingFrequency")]
        public string CompoundingFrequency { get; set; }

        [JsonProperty("daysInYear")]
        public string DaysInYear { get; set; }

        [JsonProperty("indexRateSettings")]
        public IndexRateSettings IndexRateSettings { get; set; }

        [JsonProperty("interestApplicationMethod")]
        public string InterestApplicationMethod { get; set; }

        [JsonProperty("interestBalanceCalculationMethod")]
        public string InterestBalanceCalculationMethod { get; set; }

        [JsonProperty("interestCalculationMethod")]
        public string InterestCalculationMethod { get; set; }

        [JsonProperty("interestRateSettings")]
        public List<InterestRateSetting> InterestRateSettings { get; set; }

        [JsonProperty("interestType")]
        public string InterestType { get; set; }

        [JsonProperty("scheduleInterestDaysCountMethod")]
        public string ScheduleInterestDaysCountMethod { get; set; }
    }

    public partial class IndexRateSettings
    {
        [JsonProperty("accrueInterestAfterMaturity")]
        public bool AccrueInterestAfterMaturity { get; set; }

        [JsonProperty("allowNegativeInterestRate")]
        public bool AllowNegativeInterestRate { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("indexSourceKey")]
        public string IndexSourceKey { get; set; }

        [JsonProperty("interestChargeFrequency")]
        public string InterestChargeFrequency { get; set; }

        [JsonProperty("interestChargeFrequencyCount")]
        public long InterestChargeFrequencyCount { get; set; }

        [JsonProperty("interestRate")]
        public TolerancePercentageOfOutstandingPrincipal InterestRate { get; set; }

        [JsonProperty("interestRateCeilingValue")]
        public long InterestRateCeilingValue { get; set; }

        [JsonProperty("interestRateFloorValue")]
        public long InterestRateFloorValue { get; set; }

        [JsonProperty("interestRateReviewCount")]
        public long InterestRateReviewCount { get; set; }

        [JsonProperty("interestRateReviewUnit")]
        public string InterestRateReviewUnit { get; set; }

        [JsonProperty("interestRateSource")]
        public string InterestRateSource { get; set; }

        [JsonProperty("interestRateTerms")]
        public string InterestRateTerms { get; set; }

        [JsonProperty("interestRateTiers")]
        public List<InterestRateTier> InterestRateTiers { get; set; }
    }

    public partial class InterestRateTier
    {
        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("endingBalance")]
        public long EndingBalance { get; set; }

        [JsonProperty("interestRate")]
        public long InterestRate { get; set; }
    }

    public partial class InterestRateSetting
    {
        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("indexSourceKey")]
        public string IndexSourceKey { get; set; }

        [JsonProperty("interestRate")]
        public TolerancePercentageOfOutstandingPrincipal InterestRate { get; set; }

        [JsonProperty("interestRateCeilingValue")]
        public long InterestRateCeilingValue { get; set; }

        [JsonProperty("interestRateFloorValue")]
        public long InterestRateFloorValue { get; set; }

        [JsonProperty("interestRateReviewCount")]
        public long InterestRateReviewCount { get; set; }

        [JsonProperty("interestRateReviewUnit")]
        public string InterestRateReviewUnit { get; set; }

        [JsonProperty("interestRateSource")]
        public string InterestRateSource { get; set; }
    }

    public partial class InternalControls
    {
        [JsonProperty("dormancyPeriodDays")]
        public long DormancyPeriodDays { get; set; }

        [JsonProperty("fourEyesPrinciple")]
        public FourEyesPrinciple FourEyesPrinciple { get; set; }

        [JsonProperty("lockSettings")]
        public LockSettings LockSettings { get; set; }
    }

    public partial class FourEyesPrinciple
    {
        [JsonProperty("activeForLoanApproval")]
        public bool ActiveForLoanApproval { get; set; }
    }

    public partial class LockSettings
    {
        [JsonProperty("cappingConstraintType")]
        public string CappingConstraintType { get; set; }

        [JsonProperty("cappingMethod")]
        public string CappingMethod { get; set; }

        [JsonProperty("cappingPercentage")]
        public long CappingPercentage { get; set; }

        [JsonProperty("lockPeriodDays")]
        public long LockPeriodDays { get; set; }
    }

    public partial class LoanAmountSettings
    {
        [JsonProperty("loanAmount")]
        public TolerancePercentageOfOutstandingPrincipal LoanAmount { get; set; }

        [JsonProperty("trancheSettings")]
        public TrancheSettings TrancheSettings { get; set; }
    }

    public partial class TrancheSettings
    {
        [JsonProperty("maxNumberOfTranches")]
        public long MaxNumberOfTranches { get; set; }
    }

    public partial class NewAccountSettings
    {
        [JsonProperty("accountInitialState")]
        public string AccountInitialState { get; set; }

        [JsonProperty("idGeneratorType")]
        public string IdGeneratorType { get; set; }

        [JsonProperty("idPattern")]
        public string IdPattern { get; set; }
    }

    public partial class OffsetSettings
    {
        [JsonProperty("allowOffset")]
        public bool AllowOffset { get; set; }
    }

    public partial class PaymentSettings
    {
        [JsonProperty("amortizationMethod")]
        public string AmortizationMethod { get; set; }

        [JsonProperty("latePaymentsRecalculationMethod")]
        public string LatePaymentsRecalculationMethod { get; set; }

        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonProperty("prepaymentSettings")]
        public PrepaymentSettings2 PrepaymentSettings { get; set; }

        [JsonProperty("principalPaymentSettings")]
        public PrincipalPaymentSettings PrincipalPaymentSettings { get; set; }

        [JsonProperty("repaymentAllocationOrder")]
        public List<string> RepaymentAllocationOrder { get; set; }
    }

    public partial class PrepaymentSettings2
    {
        [JsonProperty("applyInterestOnPrepaymentMethod")]
        public string ApplyInterestOnPrepaymentMethod { get; set; }

        [JsonProperty("elementsRecalculationMethod")]
        public string ElementsRecalculationMethod { get; set; }

        [JsonProperty("futurePaymentsAcceptance")]
        public string FuturePaymentsAcceptance { get; set; }

        [JsonProperty("prepaymentAcceptance")]
        public string PrepaymentAcceptance { get; set; }

        [JsonProperty("prepaymentRecalculationMethod")]
        public string PrepaymentRecalculationMethod { get; set; }

        [JsonProperty("principalPaidInstallmentStatus")]
        public string PrincipalPaidInstallmentStatus { get; set; }
    }

    public partial class PrincipalPaymentSettings
    {
        [JsonProperty("amount")]
        public TolerancePercentageOfOutstandingPrincipal Amount { get; set; }

        [JsonProperty("defaultPrincipalRepaymentInterval")]
        public long DefaultPrincipalRepaymentInterval { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("includeFeesInFloorAmount")]
        public bool IncludeFeesInFloorAmount { get; set; }

        [JsonProperty("includeInterestInFloorAmount")]
        public bool IncludeInterestInFloorAmount { get; set; }

        [JsonProperty("percentage")]
        public TolerancePercentageOfOutstandingPrincipal Percentage { get; set; }

        [JsonProperty("principalCeilingValue")]
        public long PrincipalCeilingValue { get; set; }

        [JsonProperty("principalFloorValue")]
        public long PrincipalFloorValue { get; set; }

        [JsonProperty("principalPaymentMethod")]
        public string PrincipalPaymentMethod { get; set; }

        [JsonProperty("totalDueAmountFloor")]
        public long TotalDueAmountFloor { get; set; }

        [JsonProperty("totalDuePayment")]
        public string TotalDuePayment { get; set; }
    }

    public partial class PenaltySettings2
    {
        [JsonProperty("loanPenaltyCalculationMethod")]
        public string LoanPenaltyCalculationMethod { get; set; }

        [JsonProperty("loanPenaltyGracePeriod")]
        public long LoanPenaltyGracePeriod { get; set; }

        [JsonProperty("penaltyRate")]
        public TolerancePercentageOfOutstandingPrincipal PenaltyRate { get; set; }
    }

    public partial class RedrawSettings
    {
        [JsonProperty("allowRedraw")]
        public bool AllowRedraw { get; set; }
    }

    public partial class ScheduleSettings2
    {
        [JsonProperty("billingCycles")]
        public BillingCycles BillingCycles { get; set; }

        [JsonProperty("defaultRepaymentPeriodCount")]
        public long DefaultRepaymentPeriodCount { get; set; }

        [JsonProperty("firstRepaymentDueDateOffset")]
        public TolerancePercentageOfOutstandingPrincipal FirstRepaymentDueDateOffset { get; set; }

        [JsonProperty("fixedDaysOfMonth")]
        public List<long> FixedDaysOfMonth { get; set; }

        [JsonProperty("numInstallments")]
        public TolerancePercentageOfOutstandingPrincipal NumInstallments { get; set; }

        [JsonProperty("previewSchedule")]
        public PreviewSchedule PreviewSchedule { get; set; }

        [JsonProperty("repaymentPeriodUnit")]
        public string RepaymentPeriodUnit { get; set; }

        [JsonProperty("repaymentReschedulingMethod")]
        public string RepaymentReschedulingMethod { get; set; }

        [JsonProperty("repaymentScheduleEditOptions")]
        public List<string> RepaymentScheduleEditOptions { get; set; }

        [JsonProperty("repaymentScheduleMethod")]
        public string RepaymentScheduleMethod { get; set; }

        [JsonProperty("roundingSettings")]
        public RoundingSettings RoundingSettings { get; set; }

        [JsonProperty("scheduleDueDatesMethod")]
        public string ScheduleDueDatesMethod { get; set; }

        [JsonProperty("shortMonthHandlingMethod")]
        public string ShortMonthHandlingMethod { get; set; }
    }

    public partial class BillingCycles
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("startDays")]
        public List<long> StartDays { get; set; }
    }

    public partial class PreviewSchedule
    {
        [JsonProperty("numberOfPreviewedInstalments")]
        public long NumberOfPreviewedInstalments { get; set; }

        [JsonProperty("previewScheduleEnabled")]
        public bool PreviewScheduleEnabled { get; set; }
    }

    public partial class RoundingSettings
    {
        [JsonProperty("repaymentCurrencyRounding")]
        public string RepaymentCurrencyRounding { get; set; }

        [JsonProperty("repaymentElementsRoundingMethod")]
        public string RepaymentElementsRoundingMethod { get; set; }

        [JsonProperty("roundingRepaymentScheduleMethod")]
        public string RoundingRepaymentScheduleMethod { get; set; }
    }

    public partial class SecuritySettings
    {
        [JsonProperty("isCollateralEnabled")]
        public bool IsCollateralEnabled { get; set; }

        [JsonProperty("isGuarantorsEnabled")]
        public bool IsGuarantorsEnabled { get; set; }

        [JsonProperty("requiredGuaranties")]
        public long RequiredGuaranties { get; set; }
    }

    public partial class TemperaturesTaxSettings
    {
        [JsonProperty("taxCalculationMethod")]
        public string TaxCalculationMethod { get; set; }

        [JsonProperty("taxSourceKey")]
        public string TaxSourceKey { get; set; }

        [JsonProperty("taxesOnFeesEnabled")]
        public bool TaxesOnFeesEnabled { get; set; }

        [JsonProperty("taxesOnInterestEnabled")]
        public bool TaxesOnInterestEnabled { get; set; }

        [JsonProperty("taxesOnPenaltyEnabled")]
        public bool TaxesOnPenaltyEnabled { get; set; }
    }

    public partial class Template
    {
        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("encodedKey")]
        public string EncodedKey { get; set; }

        [JsonProperty("lastModifiedDate")]
        public string LastModifiedDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}

