using ServDocumentos.Core.Contratos.Repositorios.TCR;
using System;

namespace ServDocumentos.Core.Contratos.Factories.TCR.SQL
{
    public interface IUnitOfWorkMambu : IDisposable
    {
        IRepositorioEstadoCuentaMensual RepositorioEstadoCuentaMensual { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
