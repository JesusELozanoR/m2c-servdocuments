using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.InverCamex;

namespace ServDocumentos.Servicios.Invercarmex
{
    public abstract class ServicioBase
    {
        protected readonly GestorLog gestorLog;
        protected readonly IConfiguration configuration;
        protected readonly IServiceFactory factory;

        protected ServicioBase(GestorLog gestorLog, IConfiguration configuration)
        {
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
    }
}
