using System;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class DocGuarda
    {
        public string NumeroCredito { get; set; }
        public int ClienteNumero { get; set; }
        public string DatosJsonId { get; set; }
        public string DatosJaonUrl { get; set; }
        public Int64 DocumentoId { get; set; }
        public string Usuario { get; set; }

        public DocGuarda()
        {
            NumeroCredito = "";
            ClienteNumero = 0;
            DatosJsonId = "";
            DatosJaonUrl = "";
            Usuario = "";
        }
    }
}
