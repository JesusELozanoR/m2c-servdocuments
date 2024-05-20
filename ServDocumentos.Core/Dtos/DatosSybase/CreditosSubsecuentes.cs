using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class CreditosSubsecuentes : TotalesCreditosSubsecuentes
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public string SeguroVoluntarioId { get; set; }
        public string SeguroBasicoId { get; set; }
    }
}
