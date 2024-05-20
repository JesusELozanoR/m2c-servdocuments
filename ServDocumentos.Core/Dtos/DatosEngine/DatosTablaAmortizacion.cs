using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosTablaAmortizacion
    {
        public int NumeroPago { get; set; }
        public string FechaLimitePago { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal PagoInteres { get; set; }
        public decimal IVAInteres { get; set; }
        public decimal PagoPricipal { get; set; }
        public decimal PagoTotal { get; set; }
        public decimal SaldoInsolutoPricipal { get; set; }
        public string FechaInicio { get; set; }
    }
       
}
