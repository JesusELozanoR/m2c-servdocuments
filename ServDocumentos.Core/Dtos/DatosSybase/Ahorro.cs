using System;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Ahorro
    {
        public string CuentaAhorro { get; set; }
        public string ProductoAhorro { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal? TasaAhorro { get; set; }
        public decimal? GAT { get; set; }
        public string ConTarjeta { get; set; }
        public string NoTarjeta { get; set; }
        //public decimal? GatReal { get; set; }
        public decimal? GATReal { get; set; }
        public decimal? TasaInteresAnual { get; set; }
        public decimal? GATNominal { get; set; }
        public decimal MontoTotalAhorro { get; set; }
        public decimal MontoTotalDepositar { get; set; }
    }
}
