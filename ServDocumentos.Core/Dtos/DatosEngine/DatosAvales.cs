using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosAvales : DatosPersona
    {
        public string Numero { get; set; }
        public string Telefono { get; set; }        
        public string Estado { get; set; }      

    }
}
