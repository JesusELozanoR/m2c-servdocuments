using cmn.std.Log;
using System;
using System.Data;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public abstract class RepositorioBase
    {
        protected readonly Func<IDbTransaction> transaction = null;
        protected readonly GestorLog gestorLog;
        protected IDbConnection connection { get; private set; }
        protected IDbTransaction Transaction => transaction();
        protected RepositorioBase(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.gestorLog = gestorLog;
        }

    }
}
