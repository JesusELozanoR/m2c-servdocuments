using ServDocumentos.Core.Enumeradores;

namespace ServDocumentos.Core.Contratos.Factories.Mambu
{
    /// <summary>
    /// Factory de <see cref="IUnitOfWorkApi"/>
    /// </summary>
    public interface IMambuRepositoryFactory
    {
        /// <summary>
        /// Crear una instancia de <see cref="IUnitOfWorkApi"/>
        /// </summary>
        /// <param name="core">Empresa de la que se va crear la instancia</param>
        /// <returns>Instancia de <see cref="IUnitOfWorkApi"/></returns>
        IUnitOfWorkApi Crear(Empresa core);
        /// <summary>
        /// Crear una instancia de <see cref="IUnitOfWorkApi"/>
        /// </summary>
        /// <param name="core">Empresa de la que se va crear la instancia</param>
        /// <returns>Instancia de <see cref="IUnitOfWorkApi"/></returns>
        IUnitOfWorkApi Crear(string core);
    }
}
