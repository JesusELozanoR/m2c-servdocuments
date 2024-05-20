using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosEdoCuenta
{
    public class EstadoCuentaCreditoDto
    {
        public decimal SaldoVencido { get; set; }
        public decimal SaldoInsoluto { get; set; }
        public decimal CuotaActual { get; set; }
        public decimal PagoMinimo { get; set; }
        public decimal PagoLiquidar { get; set; }
        public string Periodo { get; set; }
        public string FechaLimitePago { get; set; }
        public string NumeroCredito { get; set; }
        public string TipoOperacion { get; set; }
        public decimal MontoCredito { get; set; }
        public string EstadoCredito { get; set; }
        public string NumeroCliente { get; set; }
        public string FechaInicio { get; set; }
        public decimal CAT { get; set; }
        public decimal TasaInteres { get; set; }
        public decimal InteresMoratorio { get; set; }
        public string NombreGrupo { get; set; }
        public string NombreCompleto { get; set; }
        public string Direccion { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Telefono { get; set; }
        public string Sucursal { get; set; }
        public string DireccionSucursal { get; set; }
        public string TelefonoSucursal { get; set; }
        public IList<DetalleTransaccionCredito> Transacciones { get; set; }
        public decimal TotPagos { get; set; }
        public decimal TotCargos { get; set; }
        public decimal TotCapital { get; set; }
        public decimal TotComisionMora { get; set; }
        public decimal TotInteresMora { get; set; }
        public decimal TotInteres { get; set; }
        public decimal TotIVA { get; set; }
        public decimal TotSeguros { get; set; }
        public string TotGastoSupervision { get; set; }
    }

    public class DetalleTransaccionCredito
    {
        public string FechaMovimiento { get; set; }
        public string Concepto { get; set; }
        public decimal Pagos { get; set; }
        public decimal Cargos { get; set; }
        public decimal Capital { get; set; }
        public decimal ComisionMora { get; set; }
        public decimal InteresMora { get; set; }
        public decimal Interes { get; set; }
        public decimal IVA { get; set; }
        public decimal Seguro { get; set; }
        public string GastoSupervision { get; set; }
        public decimal Saldo { get; set; }
    }

    public class TotalDetalleTransaccionCredito
    {
        public decimal TotPagos { get; set; }
        public decimal TotCargos { get; set; }
        public decimal TotCapital { get; set; }
        public decimal TotComisionMora { get; set; }
        public decimal TotInteresMora { get; set; }
        public decimal TotInteres { get; set; }
        public decimal TotIVA { get; set; }
        public decimal TotSeguros { get; set; }
        public string TotGastoSupervision { get; set; }
    }
}
