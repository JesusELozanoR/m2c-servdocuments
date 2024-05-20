using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    
        public class DatosTutores : DatosPersonaAdicional
        {
            public string Numero { get; set; }
            public string PersonalidadJuridica { get; set; }
            public string FolioIdentificacion { get; set; }
            public string NumeroEscritura { get; set; }
            public string FechaOtorgamiento { get; set; }
            public string TipoPoder { get; set; }
            public string NombreNotario { get; set; }
            public string NumeroNotario { get; set; }
            public string Firma { get; set; }

            public DatosDireccion Domicilio { get; set; }

            public string RepresentanteLegal { get; set; }
            public string NumeroInscripcion { get; set; }
            public string TipoFirma { get; set; }


            public string NumeroSerieFirmaElectronica { get; set; }
            public string ConsentimientoCredito { get; set; }
            public string PaisNacimiento { get; set; }
            public string ConsentimientoDatosPersonales { get; set; }
        }
    }
