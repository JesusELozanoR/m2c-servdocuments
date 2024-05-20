using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Helpers;
using System.Collections.Generic;

namespace ServDocumentos.Servicios.Ticket
{
    public class ServicioDatosPlantillas : ServicioBase, IServicioDatosPlantillasObtencion
    {
        private const string Accept = "Accept";
        private const string AcceptApplication = "application/vnd.mambu.v2+json";
        public ServicioDatosPlantillas(GestorLog gestorLog, IConfiguration configuration) : base(gestorLog, configuration)
        {
        }
        public string ObtenerDatosPlantilla(ObtenerDatosDto solicitud)
        {
           
            return (null);
        }

        string IServicioDatosPlantillasObtencion.ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud)
        {
            throw new System.NotImplementedException();
        }

        string IServicioDatosPlantillasObtencion.ObtenerDatosPlantilla(ObtenerDatosDto solicitud)
        {
            throw new System.NotImplementedException();
        }

        string IServicioDatosPlantillasObtencion.ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud)
        {
            throw new System.NotImplementedException();
        }
    }
}
