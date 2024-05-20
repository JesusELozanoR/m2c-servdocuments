using ServDocumentos.Core.Contratos.Repositorios.TCR;
using System;

namespace ServDocumentos.Core.Contratos.Factories.TCR.SQL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositorioDatosPlantillasExpress RepositorioDatosPlantillas { get; }
        void BeginTransaction();
        void CommitChanges();
        void RollbackChanges();
    }
}
