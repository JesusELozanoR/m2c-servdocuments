using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class Procesoc
    {
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; }
        public string Descripcion { get; set; }
        public string Empresa { get; set; }
        public int ClasificacionId { get; set; }
        public string Clasificacion { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
