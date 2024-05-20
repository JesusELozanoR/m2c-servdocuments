using ServDocumentos.Core.Contratos.Repositorios.CAME;
using System;

namespace ServDocumentos.Core.Contratos.Factories.CAME.SQL
{
    public interface IUnitOfWorkMambu : IDisposable
    {
        IRepositorioEstadoCuentaMensual RepositorioEstadoCuentaMensual { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
