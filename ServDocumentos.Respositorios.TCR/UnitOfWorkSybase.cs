using AdoNetCore.AseClient;
using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.TCR.Sybase;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Repositorios.TCR.Repositorios.Sybase;
using System.Data;

namespace ServDocumentos.Repositorios.TCR
{
    public sealed class UnitOfWorkSybase : IUnitOfWork
    {

        private AseConnection connection = null;
        public string connectionAsync = null;

        private readonly GestorLog gestorLog;
        private IDbTransaction transaction = null;

        public IRepositorioDatosPlantillasSybase repositorioDatosPlantillas = null;
        private bool _disposed;
        public UnitOfWorkSybase(IConfiguration configuration, GestorLog gestorLog)
        {
            connection = new AseConnection(configuration.GetConnectionString("BdSyBase"));
            this.gestorLog = gestorLog;
        }
        public IRepositorioDatosPlantillasSybase RepositorioDatosPlantillas => repositorioDatosPlantillas ?? (repositorioDatosPlantillas = new RepositorioDatosPlantillas(connection, () => transaction, gestorLog));


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

        ~UnitOfWorkSybase()
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
