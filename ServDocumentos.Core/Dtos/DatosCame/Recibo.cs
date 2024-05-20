using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosCame
{
    public class Recibo
    {
        public long NumeroCliente { get; set; }
        public string NombreCompleto { get; set; }
        public long NumeroGrupo { get; set; }
        public string NombreGrupo { get; set; }
        public string Folio { get; set; }
        public decimal Monto { get; set; }
        public string Cantidad { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string LugaPago { get; set; }
        public string BanamexCta { get; set; }
        public string ReferenciaBanamex { get; set; }
        public string BancomerCta { get; set; }
        public string ReferenciasBancomer { get; set; }
        public string Gestor { get; set; }
        public string NumeroContador { get; set; }
        public string BarcodeOxxo { get; set; }
    }
}
