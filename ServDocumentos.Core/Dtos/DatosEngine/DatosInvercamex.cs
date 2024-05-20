using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosInvercamex
    {
        public string Folio { get; set; }
        public string TipoRenovacion { get; set; }
        public string ConsentimientoCredito { get; set; }
        public string ActividadObjetoSocial { get; set; }
        public string NumeroEscrituraConstitutiva { get; set; }
        public string PaisConstitucion { get; set; }
        public string NombreNotario { get; set; }
        public string NumeroInscripcionRegistro { get; set; }
        public string RepresentanteLegal { get; set; }
        public DatosDireccion DomicilioComercial { get; set; }

        public List<RefereciaBancaria> RefereciaBancaria { get; set; }
        public List<RefereciaComercial> RefereciaComercial { get; set; }
        public List<EstructuraAccionaria> EstructuraAccionaria { get; set; }
        public List<EstructuraCorporativa> EstructuraCorporativa { get; set; }
        public List<ConsejoAdministracion> ConsejoAdministracion { get; set; }


        public string TipoSolicitud { get; set; }

      



    }
}
