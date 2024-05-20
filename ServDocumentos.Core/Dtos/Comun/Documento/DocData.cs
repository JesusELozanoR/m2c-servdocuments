using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class DocData
    {
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; }
        public string Empresa { get; set; }
        public string Correo { get; set; }
        public int SubProcesoId { get; set; }
        public string SubProcesoNombre { get; set; }
        public IEnumerable<Plantilla> Plantillas { get; set; }
        public IEnumerable<Campo> Campos { get; set; }
    }
}
