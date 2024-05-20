using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Planilla
    {
        public DateTime Fecha { get; set; }
        public Int32 NumeroPlanilla { get; set; }
        public List<CuotasPlanilla> CuotasPlanilla { get; set; }
    }
}
