using ServDocumentos.Core.Contratos.Servicios.Comun;

namespace ServDocumentos.Core.Contratos.Factories.Comun
{
    public interface IServiceFactoryComun
    {
        IServicioDatosPlantillasObtencion ServicioDatosPlantillas { get; }
        IServicioEstadoCuentaMensual ServicioEstadoCuentaMensual { get; }
    }
}
