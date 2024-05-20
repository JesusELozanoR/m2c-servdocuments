using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEdoCuenta
{
    public class EstadoCuenta
    {
        public string NumeroCuenta { get; set; }
        public string CuentaCLABE { get; set; }
        public string NumeroCliente { get; set; }
        public string NombreCliente { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string Direccion3 { get; set; }
        public string Direccion4 { get; set; }
        public string Telefono { get; set; }
        public string Moneda { get; set; }
        public decimal TasaAnual { get; set; }
        public string Estado { get; set; }
        public string PeriodoDel { get; set; }
        public string PeriodoAl { get; set; }
        public string Ejecutivo { get; set; }
        public string Sucursal{ get; set; }
        public string TelefonoSucursal{ get; set; }
        public string Oficina { get; set; }
        public string TelefonoOficina { get; set; }
        public decimal GATNominal { get; set; }
        public decimal GATReal { get; set; }
        public decimal UdiPesos { get; set; }
        public decimal SaldoMinimo { get; set; }
        public decimal SaldoDiario { get; set; }
        public decimal ImpuestosRetenidos { get; set; }
        public decimal Rendimientos { get; set; }
        public decimal Comisiones { get; set; }
        public string Correo { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal OtrosCargos { get; set; }
        public decimal Depositos { get; set; }
        public decimal DepositosD { get; set; }
        public decimal TRendimientos { get; set; }
        public decimal Retiros { get; set; }
        public decimal RetirosD { get; set; }
        public decimal TComisiones { get; set; }
        public decimal SaldoFinal { get; set; }
        public IList<DetalleTransaccion> Transacciones { get; set; }
    }

    public class EstadoCuentaDescripcion
    {
        public string NumeroCuenta { get; set; }
        public string CuentaCLABE { get; set; }
        public string NumeroCliente { get; set; }
        public string NombreCliente { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string Direccion3 { get; set; }
        public string Direccion4 { get; set; }
        public string Telefono { get; set; }
        public string Moneda { get; set; }
        public decimal TasaAnual { get; set; }
        public string Estado { get; set; }
        public string PeriodoDel { get; set; }
        public string PeriodoAl { get; set; }
        public string Ejecutivo { get; set; }
        public string Oficina { get; set; }
        public string TelefonoOficina { get; set; }
        public decimal GATNominal { get; set; }
        public decimal GATReal { get; set; }
        public decimal UdiPesos { get; set; }
        public decimal SaldoMinimo { get; set; }
        public decimal SaldoDiario { get; set; }
        public decimal ImpuestosRetenidos { get; set; }
        public decimal Rendimientos { get; set; }
        public decimal Comisiones { get; set; }
        public string Correo { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal OtrosCargos { get; set; }
        public decimal Depositos { get; set; }
        public decimal DepositosD { get; set; }
        public decimal TRendimientos { get; set; }
        public decimal Retiros { get; set; }
        public decimal RetirosD { get; set; }
        public decimal TComisiones { get; set; }
        public decimal SaldoFinal { get; set; }
        public IList<DetalleTransaccionDescripcion> Transacciones { get; set; }
    }

    public class DetalleTransaccion
    {
        public string Fecha { get; set; }
        public string Lugar { get; set; }
        public string Transaccion { get; set; }
        public string Descripcion { get; set; }
        public string Operacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Saldo { get; set; }
    }

    public class DetalleTransaccionDescripcion
    {
        public string Fecha { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public decimal Deposito { get; set; }
        public decimal Retiro { get; set; }        
        public decimal Saldo { get; set; }
        public string Transaccion { get; set; }
        public decimal Cantidad { get; set; }
    }
}
