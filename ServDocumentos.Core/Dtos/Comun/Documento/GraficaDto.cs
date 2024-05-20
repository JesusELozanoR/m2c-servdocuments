using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class GraficaDto
    {
        public string Titulo { get; set; }
        public IEnumerable<GraficaDatosDto> Datos { get; set; }
    }
    public class GraficaDatosDto
    {
        public string Leyenda { get; set; }
        public double Valor { get; set; }
    }
}
