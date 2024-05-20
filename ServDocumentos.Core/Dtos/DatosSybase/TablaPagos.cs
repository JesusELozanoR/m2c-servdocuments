using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
  public   class TablaPagos
    {
        public string FechaMovimiento { get; set; }
        public string Concepto { get; set; }
        public decimal Pagos { get; set; }
        public decimal Cargos { get; set; }
        public decimal Capital { get; set; }
        public decimal ComisionMora { get; set; }
        public decimal IntereseMora { get; set; }
        public decimal Interes { get; set; }
        public decimal IVA { get; set; }
        public decimal Seguro { get; set; }
        public string GastoSupervision { get; set; }
        public decimal Saldo { get; set; }
    }
}
