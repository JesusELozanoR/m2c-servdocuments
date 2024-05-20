using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;

namespace ServDocumentos.Servicios.AhorroCAME
{
          
   public class ServiceFactory : IServiceFactoryComun
   {
            private readonly IUnitOfWork factory = null;
            private readonly GestorLog gestorLog;
            private readonly IConfiguration configuration;

            private ServicioDatosPlantillas servicioDatosPlantillas = null;
            public ServiceFactory(IUnitOfWork factory, GestorLog gestorLog, IConfiguration configuration)
            {
                this.factory = factory;
                this.gestorLog = gestorLog;
                this.configuration = configuration;
            }
            public IServicioDatosPlantillasObtencion ServicioDatosPlantillas => servicioDatosPlantillas ?? (servicioDatosPlantillas = new ServicioDatosPlantillas(gestorLog, configuration));
   }
}
