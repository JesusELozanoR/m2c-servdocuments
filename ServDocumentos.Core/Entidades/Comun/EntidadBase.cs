using System;

namespace ServDocumentos.Core.Entidades.Comun
{
    public class EntidadBase
    {
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public bool Estado { get; set; }

    }
}
