using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosTablaAmortizacionPagare
    {
        public int NumeroMensual { get; set; }
        public string Fecha { get; set; }
        public decimal Monto { get; set; }
    }
}
