using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class SubProcesoc
    {
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; }
        public string ProcesoDescripcion { get; set; }
        public int SubProcesoId { get; set; } 
        public string SubProcesoNombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int ClasificacionId { get; set; }
        public string Clasificacion { get; set; }
    }
}
