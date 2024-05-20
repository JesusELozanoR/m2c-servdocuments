using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class SolicitudGrupalBCCreditosSubsecuentes
    {
        public DateTime DesembolsoFecha { get; set; }
        public string Hora { get; set; }
        public int DiaDesembolso { get; set; }
        public int AnioDesembolso { get; set; }
        public string MesDesembolso { get; set; }
        public int NumeroGrupo { get; set; }
        public string NombreGrupo { get; set; }
        public string DiaPagoSolicitud { get; set; }
        public int NoIntegrantes { get; set; }
        public string Oficina { get; set; }
        public decimal MontoAnterior { get; set; }
        public decimal CuotaAnterior { get; set; }
        public decimal ValorCuota { get; set; }
        public string SemaforoColor { get; set; }
        public string Escenario { get; set; }
        public string CuotaUltimaPagada { get; set; }
        public decimal SaldoActualLiquidar { get; set; }
        public decimal AhorroActual { get; set; }
        public decimal DiferenciaAhorro { get; set; }
        public int CuotasRefinanciar { get; set; }
        public string Cuota { get; set; }
        public string SaldoLiquidar { get; set; }
        public decimal SaldoAhorro { get; set; }
        public string FrecuenciaSemanal { get; set; }
        public string FrecuenciaCatorcenal { get; set; }
        public string Plazo { get; set; }
        public string EscenarioCategoria { get; set; }
        public string AhorroInicialIncremento { get; set; }
        public string AhorroInicialDescontado { get; set; }
        public decimal AhorroInicial { get; set; }
        public string LiqNoRenovadosProrrateo { get; set; }
        public string LiqNoRenovadosIguales { get; set; }
        public DateTime FechaDesembolso { get; set; }
        public string LiquidacionCorrecta { get; set; }
        public string LiquidacionEditada { get; set; }
        public string DiaPago { get; set; }
        public string HoraPago { get; set; }
        public string LugarDesembolsoDescripcion { get; set; }
        public int SolicitudId { get; set; }
        public IEnumerable<CreditosSubsecuentes> CredSubsecuentes { get; set; }
        public TotalesCreditosSubsecuentes TotalCredSubs { get; set; }
    }
}
