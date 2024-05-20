using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class AhorroDto
    {

        public string EncodedKey { get; set; }
        public string CreationDate { get; set; }
        public string LastModifiedDate { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccountHolderType { get; set; }
        public string AccountHolderKey { get; set; }
        public string AccountState { get; set; }
        public string ProductTypeKey { get; set; }
        public string AccountType { get; set; }
        public string ApprovedDate { get; set; }
        public string ActivationDate { get; set; }
        public string LastInterestCalculationDate { get; set; }
        public string LastAccountAppraisalDate { get; set; }
        public string CurrencyCode { get; set; }
        public InternalControls InternalControls { get; set; }
        public OverdraftSettings OverdraftSettings { get; set; }
        public InterestSettingsAhorro InterestSettings { get; set; }
        public OverdraftInterestSettings OverdraftInterestSettings { get; set; }
        public BalancesAhorro Balances { get; set; }
        public AccruedAmounts AccruedAmounts { get; set; }
        public Retenciones Retenciones { get; set; }
        public List<_Relaciones_Ahorro> _Relaciones_Ahorro { get; set; } = new List<_Relaciones_Ahorro>();
        public _Datos_Adicionales _Datos_Adicionales { get; set; }
        public IList<_Referencias_Deposito> _Referencias_Deposito { get; set; }
        public _Datos_Adicionales_Cuenta _Datos_Adicionales_Cuenta { get; set; }
        public string maturityDate { get; set; }
        public string Amount { get; set; }

        [JsonProperty("closedDate")]
        public string ClosedDate { get; set; }

        
    }

    public partial class InternalControls
    {
        public string RecommendedDepositAmount { get; set; }
    }

    public partial class OverdraftSettings
    {
        public string AllowOverdraft { get; set; }
        public string OverdraftLimit { get; set; }

    }

    public partial class InterestSettingsAhorro
    {
        public InterestRateSettings InterestRateSettings { get; set; }
        public InterestPaymentSettings InterestPaymentSettings { get; set; }
    }

    public partial class InterestRateSettings
    {
        public string EncodedKey { get; set; }
        public string InterestRate { get; set; }
        public string InterestChargeFrequency { get; set; }
        public string InterestChargeFrequencyCount { get; set; }
        public InterestRateTiers[] InterestRateTiers { get; set; }
        public string InterestRateTerms { get; set; }
    }

    public partial class InterestPaymentSettings
    {
        public string InterestPaymentPoint { get; set; }
        public List<InterestPaymentDates> InterestPaymentDates { get; set; } = new List<InterestPaymentDates>();
    }

    public partial class OverdraftInterestSettings
    {
    }

    public partial class InterestPaymentDates
    {
        public int? month { get; set; }
        public int? day { get; set; }
    }

    public partial class BalancesAhorro
    {
        public string TotalBalance { get; set; }
        public string OverdraftAmount { get; set; }
        public string TechnicalOverdraftAmount { get; set; }
        public string LockedBalance { get; set; }
        public string AvailableBalance { get; set; }
        public string HoldBalance { get; set; }
        public string OverdraftInterestDue { get; set; }
        public string TechnicalOverdraftInterestDue { get; set; }
        public string FeesDue { get; set; }
        public string BlockedBalance { get; set; }
        public string ForwardAvailableBalance { get; set; }
    }

    public partial class AccruedAmounts
    {
        public string InterestAccrued { get; set; }
        public string OverdraftInterestAccrued { get; set; }
        public string TechnicalOverdraftInterestAccrued { get; set; }
    }

    public partial class Retenciones
    {
        public string SaldoAhorroISR { get; set; }
        public string DiasISR { get; set; }
        public string MontoISRDia { get; set; }
        public string FechaCalculo { get; set; }

    }

    public partial class _Relaciones_Ahorro
    {
        public string _Tipo_Relacion { get; set; }
        public string _index { get; set; }
        public string _Encoded_Key_Relacionado { get; set; }
    }

    public partial class _Datos_Adicionales
    {
        public decimal? Gat_Real { get; set; }
        public int? No_Constancia { get; set; }
        public decimal? Gat_Nominal { get; set; }
        public int? _Plazo { get; set; }
        public int? Dias_Termino_Inversion { get; set; }
        public string Cuenta_Deposito { get; set; }
        public string _TipoPersona { get; set; }
        
    }

    public partial class _Referencias_Deposito
    {
        public string Referencia_Deposito_Tipo { get; set; }
        public string Referencia_Deposito_Valor { get; set; }
    }

    public partial class _Datos_Adicionales_Cuenta
    {
        public decimal? Gat_Real { get; set; }
        public int? No_Constancia { get; set; }
        public decimal? Gat_Nominal { get; set; }
        public int? _Plazo { get; set; }
        public int? Dias_Termino_Inversion { get; set; }
        public string Cuenta_Deposito { get; set; }
        public string _Cuenta_Clabe { get; set; }
        public string _Tipo_Plazo { get; set; }
        public string _Tipo_Cuenta { get; set; }
        public string _Instrucciones_Tipo_Plazo { get; set; }
        public string _Instrucciones_Vencimiento { get; set; }
        public decimal? TasaIntAnualFijaReal { get; set; }
        public decimal? MontoReal { get; set; }

    }

    public class InterestRateTiers
    {
        public string encodedKey { get; set; }
        public decimal endingBalance { get; set; }
        public double interestRate { get; set; }
    }
}
