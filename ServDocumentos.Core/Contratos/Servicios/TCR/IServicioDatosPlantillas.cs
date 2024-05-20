using ServDocumentos.Core.Dtos.Comun.Solicitudes;

namespace ServDocumentos.Core.Contratos.Servicios.TCR
{
    public interface IServicioDatosPlantillas
    {
        string ObtenerDatosPlantilla(ObtenerDatosDto solicitud);
        
        void ResetearDatos(SolicitudBaseDto solicitud);
        string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud);
    }
}

