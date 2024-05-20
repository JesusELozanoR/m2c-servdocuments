using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
   public class NombrePorDividendo
    {
        public int Id { get; set; }
        public int NumeroDividendo { get; set; }     
        public string NombreCompleto { get; set; }       
        public decimal Cuota { get; set; }
        public decimal Ahorro { get; set; }
        public decimal Total { get; set; }
    }
}
