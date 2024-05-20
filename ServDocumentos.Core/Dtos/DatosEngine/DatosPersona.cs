using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosPersona
    {       		
		public string NombreCompleto { get; set; }
		public string Nombre { get; set; }
		public string ApellidoPaterno { get; set; }
		public string ApellidoMaterno { get; set; }
		public string FechaNacimiento { get; set; }
		public string DireccionCompleta { get; set; }
		public string CorreoElectronico { get; set; }
	}
}
