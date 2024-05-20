using System;
using System.Collections.Generic;
using System.Text;
using ServDocumentos.Core.Dtos.DatosEngine;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class ClienteJson: DatosPersonaAdicional
    {
        public string NumeroCliente { get; set; }
        public string NombreCliente { get; set; }
        public DatosDireccion DomicilioParticular { get; set; }
        public DatosDireccion DomicilioCorrespondencia { get; set; }
        public DatosDireccion DomicilioFiscal { get; set; }
        public DatosDireccion DomicilioExtranjero { get; set; }
        public string CuentaBancaria { get; set; }
        public string Banco { get; set; }
        public string CuentaCLABE { get; set; }
        public string Folio { get; set; }
        public string Sucursal { get; set; }
        public string Ciudad { get; set; }
        public string PromotorId { get; set; }
        public string NombreCompletoPromotor { get; set; }
        public string FechaApertura { get; set; }
        public string NumeroNomina { get; set; }
        public string Puesto { get; set; }
        public string PersonalidadJuridica { get; set; }
        public string DirecionRegional { get; set; }
        public string DirecionSucursal { get; set; }
        public string DirecionCorporativo { get; set; }
        public string NombreAseguradora { get; set; }
        public string NombreSeguro { get; set; }
        public string Correo { get; set; }
        public string DireccionCompletaCorporativo { get; set; }
        public string Colonia { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }

        public List<DatosBeneficiarios> Beneficiarios { get; set; }
        public List<DatosAvales> AvalesObligados { get; set; }
        public List<DatosCotitulares> Cotitulares { get; set; }
        public List<DatosRepresentantes> Representantes { get; set; }

        public DatosCredito Credito { get; set; }
        public List<DatosTablaAmortizacion> TablaAmortizacion { get; set; }
        public List<DatosTablaAmortizacionPagare> TablaAmortizacionPagare { get; set; }

        public DatosAhorro Ahorro { get; set; }
        public DatosInvercamex Invercamex { get; set; }
    }
}
