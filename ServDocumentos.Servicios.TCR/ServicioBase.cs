using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using sql = ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using sybase = ServDocumentos.Core.Contratos.Factories.TCR.Sybase;


namespace ServDocumentos.Servicios.TCR
{
    public abstract class ServicioBase
    {
        protected readonly GestorLog gestorLog;
        protected readonly GestorParametros gestorParametros;

        protected sybase.IUnitOfWork UnitOfWorkSybase { get; private set; }
        protected sql.IUnitOfWork UnitOfWorkSQL { get; private set; }
        protected sql.IUnitOfWorkCore UnitOfWorkSQLCore { get; private set; }
        protected sql.IUnitOfWorkMambu UnitOfWorkSQLMambu { get; private set; }
        protected comun.IUnitOfWork UnitOfWork { get; private set; }

        protected IFactService FactoryPago { get; private set; }
        protected readonly IConfiguration configuration;

        protected ServicioBase(GestorLog gestorLog)
        {
            this.gestorLog = gestorLog;
        }
        protected ServicioBase(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, IFactService factoryPago, GestorLog gestorLog)
        {
            UnitOfWorkSybase = uowSybase;
            UnitOfWorkSQL = uowSQL;
            FactoryPago = factoryPago;
            this.gestorLog = gestorLog;
        }

        protected ServicioBase(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, comun.IUnitOfWork uow, IFactService factoryPago, GestorLog gestorLog)
        {
            UnitOfWorkSybase = uowSybase;
            UnitOfWorkSQL = uowSQL;
            UnitOfWork = uow;
            FactoryPago = factoryPago;
            this.gestorLog = gestorLog;
        }

        protected ServicioBase(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, comun.IUnitOfWork uow, IFactService factoryPago, GestorLog gestorLog, IConfiguration configuration, GestorParametros gestorParametros)
        {
            UnitOfWorkSybase = uowSybase;
            UnitOfWorkSQL = uowSQL;
            UnitOfWork = uow;
            FactoryPago = factoryPago;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
            this.gestorParametros = gestorParametros;
        }

        protected ServicioBase(sql.IUnitOfWorkMambu uowSQLMambu, GestorLog gestorLog, IConfiguration configuration)
        {
            this.UnitOfWorkSQLMambu = uowSQLMambu;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
    }
}
