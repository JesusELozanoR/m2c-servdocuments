using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosTransaccion
    {
        public string Id { get; set; }
        public string EncodedKey { get; set; }
        public string FechaTransaccion { get; set; }
        public decimal Monto { get; set; }
    }
}
