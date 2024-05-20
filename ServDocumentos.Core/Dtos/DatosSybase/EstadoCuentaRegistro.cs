using System;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class EstadoCuentaRegistro
    {
        public DateTime FechaMovimiento { get; set; }
        public string Concepto { get; set; }
        public decimal Pago { get; set; }
        public decimal Cargos { get; set; }
        public decimal Capital { get; set; }
        public decimal ComisionMora { get; set; }
        public decimal InteresMora { get; set; }
        public decimal Interes { get; set; }
        public decimal Iva { get; set; }
        public decimal Seguro { get; set; }
        public decimal GastoSupervision { get; set; }
        public decimal Saldo { get; set; }
    }
}
