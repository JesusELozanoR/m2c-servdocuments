using System;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Solicitud
    {
        public int NumeroSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public decimal SolucionHabitacional { get; set; }
        public decimal SubsidioFederal { get; set; }
        public decimal SubsidioEstatal { get; set; }
        public decimal SubsidioTotal { get; set; }
        public string NombreAPV { get; set; }
        public decimal PorcentajeAhorroPrevio { get; set; }
        public decimal PromedioMensual { get; set; }
        public string TelefonoAclaraciones { get; set; }
        public string DireccionAclaraciones { get; set; }
        public string DireccionSucursal { get; set; }
        public string DireccionRegional { get; set; }
        public string DireccionCorporativo { get; set; }
        public decimal ComisionMontoBloqueado { get; set; }
        public string NumeroReferencia { get; set; }
        public int TipoDesembolso { get; set; }
        public string NombreBanco { get; set; }
        public DateTime FechaDesembolsoRenovacion { get; set; }
    }
}
