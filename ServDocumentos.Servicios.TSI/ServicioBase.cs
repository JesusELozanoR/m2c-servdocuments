using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.TCR;
using ServDocumentos.Core.Contratos.Factories.TCR.Sybase;

namespace ServDocumentos.Servicios.TSI
{
    public abstract class ServicioBase
    {
        protected readonly GestorLog gestorLog;
        protected readonly IConfiguration configuration;
        protected readonly IServiceFactory factory;

        protected IUnitOfWork UnitOfWork { get; private set; }

        protected ServicioBase(GestorLog gestorLog, IConfiguration configuration)
        {
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
    }
}
