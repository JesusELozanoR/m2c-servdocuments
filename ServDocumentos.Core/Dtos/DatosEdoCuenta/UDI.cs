using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEdoCuenta
{
    public class UDI
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
        public float Valor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
