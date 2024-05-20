using ServDocumentos.Core.Dtos.Comun.Solicitudes;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioDatosPlantillas
    {
        string ObtenerDatosPlantilla(ObtenerDatosDto solicitud);
       

        void ResetearDatos(SolicitudBaseDto solicitud);
        string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud);

        string ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud);

        string ObtenerDatosPlantilla(ObtenerDatosJsonDatosDto solicitud);

        string ObtenerDatos(ObtenerDatosDto solicitud);
    }
}
