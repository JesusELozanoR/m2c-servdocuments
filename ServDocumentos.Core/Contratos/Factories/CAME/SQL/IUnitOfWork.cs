using ServDocumentos.Core.Contratos.Repositorios.CAME;
using System;

namespace ServDocumentos.Core.Contratos.Factories.CAME.SQL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositorioDatosPlantillasCore RepositorioDatosPlantillas { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
