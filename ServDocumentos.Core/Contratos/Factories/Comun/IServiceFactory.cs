using ServDocumentos.Core.Contratos.Servicios.Comun;

namespace ServDocumentos.Core.Contratos.Factories.Comun
{
    public interface IServiceFactory
    {
        IServicioDatosJson ServicioDatosJson { get; }
        IServicioProcesos ServicioProcesos { get; }
        IServicioSubProcesos ServicioSubProcesos { get; }
        IServicioSubProcesosPlantillas ServicioSubProcesosPlantillas { get; }
        IServicioPlantillasCampos ServicioPlantillasCampos { get; }
        IServicioCampos ServicioCampos { get; }
        IServicioCamposTipos ServicioCamposTipos { get; }
        IServicioPlantillas ServicioPlantillas { get; }
        IServicioDocumentos ServicioDocumentos { get; }
        IServicioDatosPlantillas ServicioDatosPlantillas { get; }
        IServicioSubeArchivos ServicioSubeArchivos { get; }
        IServicioGenerales ServicioGenerales { get; }
        IServicioClasificaciones ServicioClasificaciones { get; }
        IServicioProcesosCampos ServicioProcesosCampos { get; }
        IServicioSubProcesosCampos ServicioSubProcesosCampos { get; }
        IServicioPlantillaTipoSubProceso ServicioPlantillaTipoSubProceso { get; }

        IServicioBitacoraEstadoCuenta ServicioBitacoraEstadoCuenta { get; }
    }
}
