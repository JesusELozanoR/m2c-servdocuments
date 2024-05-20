namespace ServDocumentos.Core.Dtos.DatosEdoCuenta
{
    public class SaldoLiquidacion
    {
        public string CreditoId { get; set; }
        public double SaldoCargos { get; set; }
        public double SaldoInteres { get; set; }
        public double SaldoMulta { get; set; }
        public double SaldoPrincipal { get; set; }
        public double SaldoTotal { get; set; }
    }
}
