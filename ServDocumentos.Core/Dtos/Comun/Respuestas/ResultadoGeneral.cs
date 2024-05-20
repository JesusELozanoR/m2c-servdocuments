using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoGeneral
    {
        public int GeneralId { get; set; }
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; }
        public string CampoNombre { get; set; }
        public string Valor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}
