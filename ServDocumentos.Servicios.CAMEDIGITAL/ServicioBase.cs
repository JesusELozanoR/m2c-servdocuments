using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.CAME;
using sql = ServDocumentos.Core.Contratos.Factories.CAME.SQL;

using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using cmn.std.Parametros;

namespace ServDocumentos.Servicios.CAMEDIGITAL
{
   public abstract class ServicioBase
    {
        protected readonly GestorLog gestorLog;
        protected readonly GestorParametros gestorParametros;
        protected readonly IConfiguration configuration;
        protected readonly IServiceFactory factory;
        protected sql.IUnitOfWork UnitOfWorkSQL { get; private set; }
        protected sql.IUnitOfWorkCore UnitOfWorkSQLCore { get; private set; }
        protected sql.IUnitOfWorkMambu UnitOfWorkSQLMambu { get; private set; }

        protected comun.IUnitOfWork UnitOfWork { get; private set; }
        protected ServicioBase(GestorLog gestorLog, IConfiguration configuration)
        {
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        protected ServicioBase( sql.IUnitOfWork uowSQL, GestorLog gestorLog,  IConfiguration configuration)
        {            
            UnitOfWorkSQL = uowSQL;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        protected ServicioBase(sql.IUnitOfWorkMambu uowSQLMambu, GestorLog gestorLog, IConfiguration configuration)
        {
            this.UnitOfWorkSQLMambu = uowSQLMambu;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        
        protected ServicioBase(sql.IUnitOfWorkMambu uowSQLMambu, comun.IUnitOfWork uow, GestorLog gestorLog, IConfiguration configuration)
        {
            this.UnitOfWorkSQLMambu = uowSQLMambu;
            this.UnitOfWork = uow;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        protected ServicioBase(comun.IUnitOfWork uow,sql.IUnitOfWork uowSQL, GestorLog gestorLog, IConfiguration configuration)
        {
            this.UnitOfWork = uow;
            UnitOfWorkSQL = uowSQL;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }

        protected ServicioBase(GestorParametros gestorParametros ,comun.IUnitOfWork uow,sql.IUnitOfWork uowSQL, GestorLog gestorLog, IConfiguration configuration)
        {
            this.gestorParametros = gestorParametros;
            this.UnitOfWork = uow;
            UnitOfWorkSQL = uowSQL;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
    }
}
