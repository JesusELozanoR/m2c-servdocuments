using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
  public  class TablaAcelerada
    {
        public int Dividendo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaVencimiento { get; set; }
        public decimal SaldoInsoluto { get; set; }
        public decimal Cuota { get; set; }
        public decimal Capital  { get; set; }
        public decimal Interes { get; set; }
        public decimal IVA { get; set; }
        public decimal SaldoNeto { get; set; }

    }
}
