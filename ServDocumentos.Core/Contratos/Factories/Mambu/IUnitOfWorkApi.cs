using ServDocumentos.Core.Contratos.Repositorios.Mambu;

namespace ServDocumentos.Core.Contratos.Factories.Mambu
{
    /// <summary>
    /// Unidad de trabajo realcionadas al API de MAMBU
    /// </summary>
    public interface IUnitOfWorkApi
    {
        /// <summary>
        /// Repositorio de productos de ahorro
        /// </summary>
        public IRepositorioProductosAhorroApi RepositorioProductosAhorro { get; }
        /// <summary>
        /// Repositorio de cuentas de ahorro
        /// </summary>
        public IRepositorioAhorrosApi RepositorioAhorros { get; }
        /// <summary>
        /// Repositorio de clientes
        /// </summary>
        public IRepositorioClientesApi RepositorioClientes { get; }
        /// <summary>
        /// Repositorio de transacciones de ahorro
        /// </summary>
        public IRepositorioTransaccionesAhorroApi RepositorioTransaccionesAhorro { get; }
        /// <summary>
        /// Repositorio de canales de transaccion
        /// </summary>
        public IRepositorioCanalesTransaccionApi RepositorioCanalesTransaccion { get; }
        /// <summary>
        /// Repositorio de cuentas de credito
        /// </summary>
        public IRepositorioCreditosApi RepositorioCreditos { get; }
        /// <summary>
        /// Repositorio de transacciones de credito
        /// </summary>
        public IRepositorioTransaccionesCreditoApi RepositorioTransaccionesCredito{ get; }
        /// <summary>
        /// Repositorio de productos de credito
        /// </summary>
        public IRepositorioProductosCreditoApi RepositorioProductosCredito { get; }
        /// <summary>
        /// Repositorio de grupos
        /// </summary>
        public IRepositorioGruposApi RepositorioGrupos{ get; }
        /// <summary>
        /// Repositorio de sucursales
        /// </summary>
        public IRepositorioSucursalesApi RepositorioSucursales { get; }
    }
}
