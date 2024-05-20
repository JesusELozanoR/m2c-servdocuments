using System;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class OrdenDePago
    {
        public int NumeroCliente { get; set; }
        public string NombreBanco { get; set; }
        public string NumeroConvenio { get; set; }
        public string NumeroCredito { get; set; }
        public double MontoOrdenPago { get; set; }
        public string ReferenciaPago { get; set; }
        public string NombreCompleto { get; set; }
        public string Concepto { get; set; }
        public int DiasVigencia { get; set; }
        public string FechaVigencia { get; set; }
        public string FechaOperacion { get; set; }
    }
}
