using ServDocumentos.Core.Contratos.Repositorios.TCR;
using System;

namespace ServDocumentos.Core.Contratos.Factories.TCR.Sybase
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositorioDatosPlantillasSybase RepositorioDatosPlantillas { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
