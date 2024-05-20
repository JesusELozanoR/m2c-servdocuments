using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosCotitulares : DatosPersonaAdicional
    {
        public string Numero { get; set; }
        public string NumeroCliente { get; set; }
        public string Alta { get; set; }
        public string Baja { get; set; }
        public string Modificacion { get; set; }
        public string NumeroSerieFirmaElectronica { get; set; }
        public string ConsentimientoCredito { get; set; }
        public string PaisNacimiento { get; set; }


        public DatosDireccion DomicilioParticular { get; set; }
        public DatosDireccion DomicilioCorrespondencia { get; set; }
        public DatosDireccion DomicilioFiscal { get; set; }
        public DatosDireccion DomicilioExtranjero { get; set; }
        

    }
}
