using ServDocumentos.Core.Contratos.Repositorios.TCR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Factories.TCR.SQL
{
    public interface IUnitOfWorkCore : IDisposable
    {
        IRepositorioEstadoCuentaMensual RepositorioEstadoCuentaMensual { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
