using ServDocumentos.Core.Dtos.DatosExpress;
using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Credito : Solicitud
    {
        public string VencidoIva { get; set; }
        public string NumeroCredito { get; set; }
        public string FrecuenciaPago { get; set; }
        public int Plazo { get; set; }
        public decimal CAT { get; set; }
        public decimal TasaInteresAnual { get; set; }
        public decimal MontoCredito { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal GastoOriginacion { get; set; }
        public decimal InteresMoratorio { get; set; }
        public string CuentaAhorro { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public decimal GastoSupervision { get; set; }
        public decimal PagoFonagavip { get; set; }
        public decimal AhorroPrevio { get; set; }
        public DateTime FechaDesembolso { get; set; }
        public string EstadoCredito { get; set; }
        public DateTime FechaInicio { get; set; }
        public string FechaFinal { get; set; }
        public string TipoOperacion { get; set; }
        public string Sucursal { get; set; }
        public string SucursalCiudad { get; set; }
        public string SucursalProvincia { get; set; }
        public string LugarDesembolso { get; set; }
        public int NumeroGrupo { get; set; }
        public string NombreGrupo { get; set; }
        public string NombreRepresentante { get; set; }
        public int MovimientosEsperados { get; set; }
        public decimal SaldoPromedio { get; set; }
        public string DestinoCredito { get; set; }
        public string DestinoCreditoDescripcion { get; set; }
        public string TipoPlazo { get; set; }
        public decimal ComisionCredito { get; set; }
        public decimal TasaInteresAnualIva { get; set; }
        public decimal TasaInteresInicialIva { get; set; }
        public string FechaPago { get; set; }
        public decimal InteresMoratorioMensual { get; set; }
        public decimal CuotaMensual { get; set; }
        public IEnumerable<Dividendo> Dividendos { get; set; }
        //public List<EstadoCuentaRegistro> EstadoCuenta { get; set; }
        public string FormaDesembolso { get; set; }
        public DatosSolicitudCredito DatoSolicitudCredito { get; set; }
        public Int32 PorAhorroPrevMonto { get; set; }
        public string TipoSaldo { get; set; }
        public decimal AsistenciaTecnica { get; set; }
        public List<decimal> Disposiciones { get; set; }
        //public List<Planilla> PlanillasPago { get; set; }
        public List<OrdenDePago> OrdenesPago { get; set; }
        public decimal? GastoSupervisionPorc { get; set; }
        public string NombreLargoAseguradora { get; set; }
        public string NombreCortoAseguradora { get; set; }
        public string NombreAseguradora { get; set; }
        public string DireccionAseguradora { get; set; }
        public string RFCAseguradora { get; set; }
        public string NumeroCreditoAnterior { get; set; }
        public string ProductoAnterior { get; set; }
        public decimal MontoIncentivo { get; set; }   
        public decimal MontoSegundaDispersion { get; set; }
        public string ReferenciaWalmart { get; set; }
        public string CodigoBarrasWalmart { get; set; }
        public string TipoCredito { get; set; }
        public string Producto { get; set; }
        public string DireccionCompletaCorporativo { get; set; }
        public string HoraReunion { get; set; }
        public string DiaReunion { get; set; }
        public int PlazoLiquidacion { get; set; }
        public string ProductoDescripcion { get; set; }
        public string CertificadoBC { get; set; }
        public string CertificadoB52 { get; set; }

        public IEnumerable<Comisiones> Comisiones { get; set; }
        public IEnumerable<PagoPorDividendo> PagoPorDividendo { get; set; }
        public IEnumerable<TablaAcelerada> TablaAcelerada { get; set; }
        public IEnumerable<ActaFundacion> TablaActaFundacion { get; set; }

        public decimal TasaMoratoriaAnual { get; set; }
        public string SegObligatorio { get; set; }
        public string SegOpcional { get; set; }
        public decimal? GATNominal { get; set; }
        public decimal? GATReal { get; set; }
        public string EnvioEstadoCuentaEmail { get; set; }
        public string LugarEfectuarRetiroVentanilla { get; set; }
        public string MedioDisposicionEfectivo { get; set; }
        public string ComisionIva { get; set; }
        public string ReferenciaTelecomm { get; set; }
        public string ReferenciaBbva { get; set; }
        public string ReferenciaBanbajio { get; set; }
        public string ReferenciaScotiabank { get; set; }
        public string ReferenciaHsbc { get; set; }
        public string ReferenciaOxxopay { get; set; }
        public string ReferenciaBanamex { get; set; }

        public decimal MontoTotalAhorro { get; set; }
        public decimal MontoTotalDepositar { get; set; }
        public SolicitudGrupalBCCreditosSubsecuentes BCCreditosSubsecuentes { get; set; }
    }
}