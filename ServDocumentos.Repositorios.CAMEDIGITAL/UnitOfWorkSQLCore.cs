﻿using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.CAME.SQL;
using ServDocumentos.Core.Contratos.Repositorios.CAME;
using ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ServDocumentos.Repositorios.CAMEDIGITAL
{
    public class UnitOfWorkSQLCore : IUnitOfWorkCore
    {

        private IDbConnection connection = null;

        private readonly GestorLog gestorLog;
        private IDbTransaction transaction = null;

        public IRepositorioEstadoCuentaMensual repositorioEstadoCuentaMensual = null;

        private bool _disposed;
        public UnitOfWorkSQLCore(IConfiguration configuration, GestorLog gestorLog)
        {
            connection = new SqlConnection(configuration.GetConnectionString("BdCore"));
            this.gestorLog = gestorLog;
        }
        public IRepositorioEstadoCuentaMensual RepositorioEstadoCuentaMensual => repositorioEstadoCuentaMensual ?? (repositorioEstadoCuentaMensual = new RepositorioEstadoCuentaMensual(connection, () => transaction, gestorLog));


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

        ~UnitOfWorkSQLCore()
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