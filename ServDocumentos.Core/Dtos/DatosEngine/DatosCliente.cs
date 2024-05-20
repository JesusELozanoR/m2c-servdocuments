
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using Mambu = ServDocumentos.Core.Dtos.DatosMambu;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosCliente : DatosPersona
    {
        public string NumeroCliente { get; set; }

        public string instanceId { get; set; }
        public string tenantId { get; set; }
        public string agentId { get; set; }
        public string agentUsername { get; set; }
        public string status { get; set; }        
        public List<DatosBeneficiarios> Beneficiarios  { get; set; }
        public Mambu.Credito Credito { get; set; }
        public List<Mambu.Pagos> Pagos { get; set; }
        public Mambu.AhorroDto Ahorro { get; set; }

    }
}
