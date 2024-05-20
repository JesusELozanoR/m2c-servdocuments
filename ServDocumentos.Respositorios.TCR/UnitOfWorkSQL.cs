using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Repositorios.TCR.Repositorios.SQL;
using System.Data;
using System.Data.SqlClient;

namespace ServDocumentos.Repositorios.TCR
{
    public class UnitOfWorkSQL : IUnitOfWork
    {

        private IDbConnection connection = null;

        private readonly GestorLog gestorLog;
        private IDbTransaction transaction = null;

        public IRepositorioDatosPlantillasExpress repositorioDatosPlantillas = null;

        private bool _disposed;
        public UnitOfWorkSQL(IConfiguration configuration, GestorLog gestorLog)
        {
            connection = new SqlConnection(configuration.GetConnectionString("BdCrediExpress"));
            this.gestorLog = gestorLog;
        }
        public IRepositorioDatosPlantillasExpress RepositorioDatosPlantillas => repositorioDatosPlantillas ?? (repositorioDatosPlantillas = new RepositorioDatosPlantillas(connection, () => transaction, gestorLog));


        public void Dispose()
        {
            dispose(true);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                        transaction = null;
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWorkSQL()
        {
            dispose(false);
        }
        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        public void CommitChanges()
        {
            transaction.Commit();
            transaction = null;
        }

        public void RollbackChanges()
        {
            transaction.Rollback();
            transaction = null;
        }


    }
}
