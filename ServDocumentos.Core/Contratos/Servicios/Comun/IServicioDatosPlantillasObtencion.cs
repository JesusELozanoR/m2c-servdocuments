using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosSybase;
using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioDatosPlantillasObtencion
    {
        string ObtenerDatosPlantilla(ObtenerDatosDto solicitud);
        

        string ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud);
        string ObtenerDatosPlantillaAhorro(ObtenerDatosDto solicitud);
        string ObtenerDatosPlantillaAhorroJson(ObtenerDatosJsonDto solicitud);
        string ObtenerDatosPlantillaInversion(ObtenerDatosDto solicitud);
        string ObtenerDatosPlantillaInversionJson(ObtenerDatosJsonDto solicitud);
        DocData EstadoCuentaObtenerDatos(SolictudDocumentoDto documSolicitud);
        string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud);
        EstadoCuenta ObtenerDatosJsonEstadoCuenta(SolictudDocumentoDto documSolicitud);

        SolictudDocumentoDto AsignarValores(ObtenerPlantillasProcesoDto solicitud);
        SolictudDocumentoDto AsignarValores(EstadoCuentaMensualSol solicitud);

        string ObtenerDatosPlantillaBC2020(ObtenerDatosDto solicitud);
        string ObtenerDatosPlantillaAhorroTeChreoPatrimonial(ObtenerDatosDto solicitud);
        string ObtenerDatosPlantillaSeguros(ObtenerDatosDto solicitud);
        string ObtenerDatosPlantillaAhorroJsonSinDatos(ObtenerDatosJsonDatosDto solicitud);
        string ObtenerDatosPlantillaMercado(ObtenerDatosJsonDto solicitud);
    }
}
