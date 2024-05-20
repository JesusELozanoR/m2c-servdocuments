using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosBeneficiarios : DatosPersona
    {
        public string Numero { get; set; }
        public string Telefono { get; set; }      
        public string Porcentaje { get; set; }
        public string Parentesco { get; set; }
        public string Alta { get; set; }
        public string Baja { get; set; }
    }
}
