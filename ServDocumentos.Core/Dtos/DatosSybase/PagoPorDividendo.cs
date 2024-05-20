using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
   public  class PagoPorDividendo
    {
       
        public int NumeroDividendo { get; set; }      
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal TotalCuota { get; set; }
        public decimal TotalAhorro { get; set; }
        public decimal TotalAcumulado { get; set; }
        public IEnumerable<NombrePorDividendo> ClientePorDividendo { get; set; }
    }
}
