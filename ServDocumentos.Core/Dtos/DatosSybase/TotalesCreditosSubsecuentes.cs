using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class TotalesCreditosSubsecuentes
    {

        public decimal MontoActual { get; set; }
        public decimal CuotaActual { get; set; }
        public decimal MontoPresupuesto { get; set; }
        public decimal Liquidacion { get; set; }
        public decimal Ahorro { get; set; }
        public decimal SeguroBasico { get; set; }
        public decimal SeguroVoluntario { get; set; }
        public decimal MontoSolicitado { get; set; }
        public decimal TotalRecibir { get; set; }
        public decimal TotalCuota { get; set; }

    }
}
