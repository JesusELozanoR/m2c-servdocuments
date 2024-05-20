using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoProcesoCampo
    {
        public int ProcesoCampoId { get; set; }
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; }
        public int CampoId { get; set; }
        public string CampoNombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string DatoCampo { get; set; }
        public string DatoConjunto { get; set; }
        public string DatoConjuntoGrupal { get; set; }
        public string Ejemplo { get; set; }
        public bool Estado { get; set; }
    }
}
