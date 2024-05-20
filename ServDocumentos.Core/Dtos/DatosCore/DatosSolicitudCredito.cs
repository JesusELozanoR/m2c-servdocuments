using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosCore
{
    public class DatosSolicitudCredito
    {
        public long Id { get; set; }
        public long ClienteId { get; set; }
        public int ProductoId { get; set; }
        public int? ApvId { get; set; }
        public string Etapa { get; set; }
        public decimal MontoSolicitado { get; set; }
        public string FrecuenciaSolicitado { get; set; }
        public int PlazoSolicitado { get; set; }
        public decimal MontoAutorizado { get; set; }
        public string FrecuenciaAutorizado { get; set; }
        public int PlazoAutorizado { get; set; }
        public decimal MontoCarterizado { get; set; }
        public string FrecuenciaCarterizado { get; set; }
        public int PlazoCarterizado { get; set; }
        public long CreditoTcrId { get; set; }
        public string CodigoCreditoTcr { get; set; }
        public string EstadoSolicitud { get; set; }
        public bool Bloqueada { get; set; }
        public string UsuarioBloqueo { get; set; }
        public DateTime FechaBloqueo { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public DateTime FechaUltimaMod { get; set; }
        public int UsuarioUltimaModId { get; set; }
        public bool Estado { get; set; }
        public string DestinoCredito { get; set; }
        public short AvalRequerido { get; set; }
        public short DiaPago { get; set; }
        public int UsuarioBloqueoId { get; set; }
        public bool RequiereConyuge { get; set; }
        public int UsuarioSolicitud { get; set; }
        public DateTime? FechaDesembolso { get; set; }
        public decimal VentaProducto { get; set; }
        public long MotivoCorreoId { get; set; }
        public byte OrigenSolicitud { get; set; }
        public short DiaPagoSolicitado { get; set; }
        public string FormaDesembolso { get; set; }
        public string Vendedor { get; set; }
        public int PlazoAnalisis { get; set; }
        public decimal CuotaAnalisis { get; set; }
        public DateTime? FechaRecepcionDocs { get; set; }
        public string Modelos { get; set; }
        public string FormiikId { get; set; }
        public long RangoCalificacionId { get; set; }
        public string TipoRelacion { get; set; }
        public string EtapaAnterior { get; set; }
        public string NumCreditoAnterior { get; set; }
        public string OrigenDesembolso { get; set; }
        public decimal CuotaCapacidadPago { get; set; }
        public int? PeriodoGracia { get; set; }
        public byte Tipo { get; set; }

    }
}
