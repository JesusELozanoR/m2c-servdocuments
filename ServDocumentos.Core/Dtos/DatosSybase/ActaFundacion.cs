using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class ActaFundacion
    {
        public string NombreCompleto { get; set; }
        public decimal MontoActual { get; set; }
        public decimal CuotaActual { get; set; }
        public decimal MontoPresupuesto { get; set; }
        public string ProgramaPrevencion { get; set; }
        public decimal MontoSolicitado { get; set; }
        public decimal TotalRecibir { get; set; }
        public decimal Cuota { get; set; }         
        
    }
}
