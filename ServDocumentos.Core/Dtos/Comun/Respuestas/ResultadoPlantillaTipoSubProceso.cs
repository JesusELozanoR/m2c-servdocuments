using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
   public class ResultadoPlantillaTipoSubProceso
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCorta { get; set; }
        public bool Estado { get; set; }
    }
}
