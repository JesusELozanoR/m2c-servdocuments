using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoCampoTipo
    {
        public int CampoTipoId { get; set; }
        public string CampoTipo { get; set; }
        public bool Estado { get; set; }
    }
}
