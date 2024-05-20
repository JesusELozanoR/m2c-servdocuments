using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using sql = ServDocumentos.Core.Contratos.Factories.CAME.SQL;

using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using cmn.std.Parametros;
using ServDocumentos.Core.Contratos.Factories.Mambu;

namespace ServDocumentos.Servicios.CAMEDIGITAL
{
   public class ServiceFactory : IServiceFactoryComun
    {
        private readonly IUnitOfWork factory = null;
        private readonly GestorLog gestorLog;
        private readonly GestorParametros gestorParametros;
        private readonly IConfiguration configuration;
        private readonly sql.IUnitOfWork uowSQL = null;
        private readonly sql.IUnitOfWorkMambu uowSQLMambu = null;

        private readonly comun.IUnitOfWork UnitOfWork = null;
        private ServicioDatosPlantillas servicioDatosPlantillas = null;
        private ServicioEstadoCuentaMensual servicioEstadoCuentaMensual = null;
        /// <summary>
        /// Factory de repositorio del API de MAMBU
        /// </summary>
        private readonly IMambuRepositoryFactory _mambuFactory;
        public ServiceFactory(IUnitOfWork factory, GestorLog gestorLog, IConfiguration configuration)
        {
            this.factory = factory;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        public ServiceFactory(sql.IUnitOfWorkMambu uowSQLMambu ,  IUnitOfWork factory, sql.IUnitOfWork uowSQL, GestorLog gestorLog, IConfiguration configuration)
        {
            this.uowSQLMambu = uowSQLMambu;
            this.UnitOfWork = factory;
            this.factory = factory;
            this.uowSQL = uowSQL;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        public ServiceFactory(
            GestorParametros gestorParametros,
            sql.IUnitOfWorkMambu uowSQLMambu,
            IUnitOfWork factory,
            sql.IUnitOfWork uowSQL,
            GestorLog gestorLog, 
            IConfiguration configuration, 
            IMambuRepositoryFactory mambuFactory)
        {
            this.gestorParametros = gestorParametros;
            this.uowSQLMambu = uowSQLMambu;
            this.UnitOfWork = factory;
            this.factory = factory;
            this.uowSQL = uowSQL;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
            _mambuFactory = mambuFactory;
        }

      /*  public ServiceFactory(sql.IUnitOfWorkMambu uowSQLMambu, GestorLog gestorLog, IConfiguration configuration)
        {
            this.uowSQLMambu = uowSQLMambu;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }*/
        public IServicioDatosPlantillasObtencion ServicioDatosPlantillas => servicioDatosPlantillas ?? (servicioDatosPlantillas = new ServicioDatosPlantillas(gestorParametros, UnitOfWork, uowSQL, gestorLog, configuration));

        public IServicioEstadoCuentaMensual ServicioEstadoCuentaMensual => servicioEstadoCuentaMensual ?? (servicioEstadoCuentaMensual = new ServicioEstadoCuentaMensual(uowSQLMambu, UnitOfWork, gestorLog, configuration, _mambuFactory));
    }
}
