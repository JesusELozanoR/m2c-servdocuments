using ServDocumentos.Core.Contratos.Repositorios.Comun;
using System;

namespace ServDocumentos.Core.Contratos.Factories.Comun
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositorioDatosJson RepositorioDatosJson { get; }
        IRepositorioProcesos RepositorioProcesos { get; }
        IRepositorioSubProcesos RepositorioSubProcesos { get; }
        IRepositorioSubProcesosPlantillas RepositorioSubProcesosPlantillas { get; }
        IRepositorioPlantillas RepositorioPlantillas { get; }
        IRepositorioPlantillasCampos RepositorioPlantillasCampos { get; }
        IRepositorioCampos RepositorioCampos { get; }
        IRepositorioDocumentos RepositorioDocumentos { get; }
        IRepositorioGenerales RepositorioGenerales { get; }
        IRepositorioClasificaciones RepositorioClasificaciones { get; }
        IRepositorioProcesosCampos RepositorioProcesosCampos { get; }
        IRepositorioSubProcesosCampos RepositorioSubProcesosCampos { get; }
        IRepositorioCamposTipos RepositorioCamposTipos { get; }
        IRepositorioEnviaCorreo RepositorioEnviaCorreo { get; }
        IRepositorioPlantillaTipoSubProceso RepositorioPlantillaTipoSubProceso { get; }
        /// <summary>
        /// Repositorio bitacora estado cuenta
        /// </summary>
        IRepositorioBitacoraEstadoCuenta RepositorioBitacoraEstadoCuenta { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();

    }
}
