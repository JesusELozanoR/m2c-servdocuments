using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Repositorios.Comun.Repositorios;
using System.Data;
using System.Data.SqlClient;

namespace ServDocumentos.Repositorios.Comun
{
    public class UnitOfWork : IUnitOfWork
    {

        private IDbConnection connection = null;
        /// <summary>
        /// Conexion a la base de datos cbk_core
        /// </summary>
        private IDbConnection connectionCore = null;

        private readonly GestorLog gestorLog;
        private IDbTransaction transaction = null;

        private IRepositorioDatosJson repositorioDatosJson = null;
        private IRepositorioProcesos repositorioProcesos = null;
        private IRepositorioSubProcesos repositorioSubProcesos = null;
        private IRepositorioSubProcesosPlantillas repositorioSubProcesosPlantillas = null;
        private IRepositorioPlantillas repositorioPlantillas = null;
        private IRepositorioPlantillasCampos repositorioPlantillasCampos = null;
        private IRepositorioCampos repositorioCampos = null;
        private IRepositorioCamposTipos repositorioCamposTipos = null;
        private IRepositorioDocumentos repositorioDocumentos = null;
        private IRepositorioGenerales repositorioGenerales = null;
        private IRepositorioClasificaciones repositorioClasificaciones = null;
        private IRepositorioProcesosCampos repositorioProcesosCampos = null;
        private IRepositorioSubProcesosCampos repositorioSubProcesosCampos = null;
        private IRepositorioEnviaCorreo repositorioEnviaCorreo = null;
        private IRepositorioPlantillaTipoSubProceso repositorioPlantillaTipoSubProceso = null;
        private IRepositorioBitacoraEstadoCuenta repositorioBitacoraEstadoCuenta = null;

        private bool _disposed;
        public UnitOfWork(IConfiguration configuration, GestorLog gestorLog)
        {
            connection = new SqlConnection(configuration.GetConnectionString("BdDocumentos"));
            connectionCore = new SqlConnection(configuration.GetConnectionString("BdCore"));
            this.gestorLog = gestorLog;
        }
        public IRepositorioDatosJson RepositorioDatosJson => repositorioDatosJson ?? (repositorioDatosJson = new RepositorioDatosJson(connection, () => transaction, gestorLog));
        public IRepositorioProcesos RepositorioProcesos => repositorioProcesos ?? (repositorioProcesos = new RepositorioProcesos(connection, () => transaction, gestorLog));
        public IRepositorioSubProcesos RepositorioSubProcesos => repositorioSubProcesos ?? (repositorioSubProcesos = new RepositorioSubProcesos(connection, () => transaction, gestorLog));
        public IRepositorioSubProcesosPlantillas RepositorioSubProcesosPlantillas => repositorioSubProcesosPlantillas ?? (repositorioSubProcesosPlantillas = new RepositorioSubProcesosPlantillas(connection, () => transaction, gestorLog));
        public IRepositorioPlantillas RepositorioPlantillas => repositorioPlantillas ?? (repositorioPlantillas = new RepositorioPlantillas(connection, () => transaction, gestorLog));
        public IRepositorioPlantillasCampos RepositorioPlantillasCampos => repositorioPlantillasCampos ?? (repositorioPlantillasCampos = new RepositorioPlantillasCampos(connection, () => transaction, gestorLog));
        public IRepositorioCampos RepositorioCampos => repositorioCampos ?? (repositorioCampos = new RepositorioCampos(connection, () => transaction, gestorLog));
        public IRepositorioCamposTipos RepositorioCamposTipos => repositorioCamposTipos ?? (repositorioCamposTipos = new RepositorioCamposTipos(connection, () => transaction, gestorLog));
        public IRepositorioDocumentos RepositorioDocumentos => repositorioDocumentos ?? (repositorioDocumentos = new RepositorioDocumentos(connection, () => transaction, gestorLog));
        public IRepositorioGenerales RepositorioGenerales => repositorioGenerales ?? (repositorioGenerales = new RepositorioGenerales(connection, () => transaction, gestorLog));
        public IRepositorioClasificaciones RepositorioClasificaciones => repositorioClasificaciones ?? (repositorioClasificaciones = new RepositorioClasificaciones(connection, () => transaction, gestorLog));
        public IRepositorioProcesosCampos RepositorioProcesosCampos => repositorioProcesosCampos ?? (repositorioProcesosCampos = new RepositorioProcesosCampos(connection, () => transaction, gestorLog));
        public IRepositorioSubProcesosCampos RepositorioSubProcesosCampos => repositorioSubProcesosCampos ??= new RepositorioSubProcesosCampos(connection, () => transaction, gestorLog);
        public IRepositorioEnviaCorreo RepositorioEnviaCorreo => repositorioEnviaCorreo ?? (repositorioEnviaCorreo = new RepositorioEnviaCorreo(connection, () => transaction, gestorLog));
        public IRepositorioPlantillaTipoSubProceso RepositorioPlantillaTipoSubProceso => repositorioPlantillaTipoSubProceso ?? (repositorioPlantillaTipoSubProceso = new RepositorioPlantillaTipoSubProceso(connection, () => transaction, gestorLog));
        public IRepositorioBitacoraEstadoCuenta RepositorioBitacoraEstadoCuenta =>
            repositorioBitacoraEstadoCuenta ?? (repositorioBitacoraEstadoCuenta = new RepositorioBitacoraEstadoCuenta(connectionCore, () => transaction, gestorLog));

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

        ~UnitOfWork()
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