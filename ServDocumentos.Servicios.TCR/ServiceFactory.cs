using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using sql = ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using sybase = ServDocumentos.Core.Contratos.Factories.TCR.Sybase;
using ServDocumentos.Core.Contratos.Factories.Mambu;

namespace ServDocumentos.Servicios.TCR
{
    public class ServiceFactory : comun.IServiceFactoryComun
    {
        private readonly sybase.IUnitOfWork uowSybase = null;
        private readonly sql.IUnitOfWork uowSQL = null;
        private readonly sql.IUnitOfWorkCore uowSQLCore = null;
        private readonly sql.IUnitOfWorkMambu uowSQLMambu = null;
        private readonly comun.IUnitOfWork unitOfWork = null;
        private readonly IFactService factoryPago = null;
        private readonly IConfiguration configuration;
        private readonly GestorParametros gestorParametros;
        private readonly GestorLog gestorLog;
        private ServicioDatosPlantillas servicioDatosPlantillas = null;
        private ServicioEstadoCuentaMensual servicioEstadoCuentaMensual = null;
        /// <summary>
        /// Factory de repositorio del API de MAMBU
        /// </summary>
        private readonly IMambuRepositoryFactory _mambuFactory;

        public ServiceFactory(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, IFactService factoryPago, GestorLog gestorLog)
        {
            this.uowSybase = uowSybase;
            this.uowSQL = uowSQL;
            this.factoryPago = factoryPago;
            //this.factory = factory;
            this.gestorLog = gestorLog;
        }

        public ServiceFactory(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, comun.IUnitOfWork uow, IFactService factoryPago, GestorLog gestorLog)
        {
            this.uowSybase = uowSybase;
            this.uowSQL = uowSQL;
            this.unitOfWork = uow;
            this.factoryPago = factoryPago;
            //this.factory = factory;
            this.gestorLog = gestorLog;
        }

        public ServiceFactory(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, comun.IUnitOfWork uow, IFactService factoryPago, GestorLog gestorLog, IConfiguration configuration)
        {
            this.uowSybase = uowSybase;
            this.uowSQL = uowSQL;
            this.unitOfWork = uow;
            this.factoryPago = factoryPago;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        public ServiceFactory(
            sybase.IUnitOfWork uowSybase,
            sql.IUnitOfWork uowSQL,
            sql.IUnitOfWorkMambu uowSQLMambu,
            comun.IUnitOfWork uow,
            IFactService factoryPago,
            GestorLog gestorLog,
            IConfiguration configuration,
            GestorParametros gestorParametros,
            IMambuRepositoryFactory mambuFactory)
        {
            this.uowSybase = uowSybase;
            this.uowSQL = uowSQL;
            this.uowSQLMambu = uowSQLMambu;
            this.unitOfWork = uow;
            this.factoryPago = factoryPago;
            //this.factory = factory;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
            this.gestorParametros = gestorParametros;
            _mambuFactory = mambuFactory;
        }

        //public ServiceFactory(sql.IUnitOfWorkCore uowSQLCore,  GestorLog gestorLog, GestorBinarios gestorBinarios, IConfiguration configuration)
        //{
        //    this.uowSQLCore = uowSQLCore;
        //    this.gestorLog = gestorLog;
        //    this.gestorBinarios = gestorBinarios;
        //    this.configuration = configuration;
        //}


        // public IServicioDatosPlantillasObtencion ServicioDatosPlantillas => servicioDatosPlantillas ?? (servicioDatosPlantillas = new ServicioDatosPlantillas(uowSybase, uowSQL, factoryPago, gestorLog, gestorBinarios));

        public IServicioDatosPlantillasObtencion ServicioDatosPlantillas => servicioDatosPlantillas ?? (servicioDatosPlantillas = new ServicioDatosPlantillas(uowSybase, uowSQL, unitOfWork, factoryPago, gestorLog, configuration, gestorParametros));

        public IServicioEstadoCuentaMensual ServicioEstadoCuentaMensual => servicioEstadoCuentaMensual ?? (servicioEstadoCuentaMensual = new ServicioEstadoCuentaMensual(uowSQLMambu, gestorLog, configuration, _mambuFactory));
    }
}
