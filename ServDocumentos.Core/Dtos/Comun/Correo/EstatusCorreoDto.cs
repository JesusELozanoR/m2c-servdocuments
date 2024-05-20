using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Correo
{
    public class EstatusCorreoDto
    {
        public string Estatus { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaEnvio { get; set; }
        
    }
}
