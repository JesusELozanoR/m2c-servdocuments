using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Repositorios.TCR.Repositorios.Api;

namespace ServDocumentos.Repositorios.TCR
{
    /// <summary>
    /// Unidad de trabajo realcionadad al API de MAMBU TCR
    /// </summary>
    public class UnitOfWorkApi : IUnitOfWorkApi
    {
        /// <summary>
        /// Gestor log
        /// </summary>
        private readonly GestorLog _gestorLog;
        /// <summary>
        /// Gestor peticiones
        /// </summary>
        private readonly IGestorPeticiones _gestorPeticiones;
        /// <summary>
        /// Configuracion de MAMBU
        /// </summary>
        private IOptionsSnapshot<MambuConfig> _mambuOptions;
        /// <summary>
        /// Repositorio de productos de ahorro
        /// </summary>
        private IRepositorioProductosAhorroApi repositorioProductosAhorro = null;
        /// <summary>
        /// Repositorio de productos
        /// </summary>
        private IRepositorioAhorrosApi repositorioAhorros = null;
        /// <summary>
        /// Repositorio de clientes
        /// </summary>
        private IRepositorioClientesApi repositorioClientes = null;
        /// <summary>
        /// Repositorio de transacciones de ahorro
        /// </summary>
        private IRepositorioTransaccionesAhorroApi repositorioTransaccionesAhorro = null;
        /// <summary>
        /// Repositorio de canales de transaccion
        /// </summary>
        private IRepositorioCanalesTransaccionApi repositorioCanalesTransaccion = null;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public UnitOfWorkApi(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _mambuOptions = mambuOptions;
        }
        /// <summary>
        /// Repositorio de ahorros
        /// </summary>
        public IRepositorioAhorrosApi RepositorioAhorros => 
            repositorioAhorros ??= new RepositorioAhorrosApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de ahorros
        /// </summary>
        public IRepositorioClientesApi RepositorioClientes => 
            repositorioClientes ??= new RepositorioClientesApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de transacciones de ahorro
        /// </summary>
        public IRepositorioTransaccionesAhorroApi RepositorioTransaccionesAhorro =>
            repositorioTransaccionesAhorro ??= new RepositorioTransaccionesAhorroApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de canales de transaccion
        /// </summary>
        public IRepositorioCanalesTransaccionApi RepositorioCanalesTransaccion =>
            repositorioCanalesTransaccion ??= new RepositorioCanalesTransaccionApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de productos de ahorro
        /// </summary>
        public IRepositorioProductosAhorroApi RepositorioProductosAhorro =>
            repositorioProductosAhorro ??= new RepositorioProductosAhorroApi(_gestorLog, _gestorPeticiones, _mambuOptions);

        public IRepositorioCreditosApi RepositorioCreditos => throw new System.NotImplementedException();

        public IRepositorioTransaccionesCreditoApi RepositorioTransaccionesCredito => throw new System.NotImplementedException();

        public IRepositorioProductosCreditoApi RepositorioProductosCredito => throw new System.NotImplementedException();

        public IRepositorioGruposApi RepositorioGrupos => throw new System.NotImplementedException();

        public IRepositorioSucursalesApi RepositorioSucursales => throw new System.NotImplementedException();
    }
}
