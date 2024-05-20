using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Correo
{
  public  class CorreoDto
    {
        public string Url { get; set; }
        public string Asunto { get; set; }
        public string Correo { get; set; }
        public ICollection<string> CC { get; set; }
        public ICollection<Adjunto> Adjuntos { get; set; }
        public string Cuerpo { get; set; }
    }
    public class Adjunto
    {
        public string Nombre { get; set; }
        public byte[] Binario { get; set; }
    }
}
