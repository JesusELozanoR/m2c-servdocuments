using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.Api;

namespace ServDocumentos.Repositorios.CAMEDIGITAL
{
    /// <summary>
    /// Unidad de trabajo realcionadad al API de MAMBU CAME
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
        /// Repositorio de transacciones de ahorro
        /// </summary>
        private IRepositorioTransaccionesCreditoApi repositorioTransaccionesCredito = null;
        /// <summary>
        /// Repositorio de cuentas de credito
        /// </summary>
        private IRepositorioCreditosApi repositorioCreditos = null;
        /// <summary>
        /// Repositorio de productos de credito
        /// </summary>
        private IRepositorioProductosCreditoApi repositorioProductosCredito = null;
        /// <summary>
        /// Repositorio de grupos
        /// </summary>
        private IRepositorioGruposApi repositorioGrupos = null;
        /// <summary>
        /// Repositorio de sucursales
        /// </summary>
        private IRepositorioSucursalesApi repositorioSucursales = null;
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
        /// Repositorio de productos de ahorro
        /// </summary>
        public IRepositorioProductosAhorroApi RepositorioProductosAhorro => 
            repositorioProductosAhorro ??= new RepositorioProductosAhorroApi(_gestorLog, _gestorPeticiones, _mambuOptions);
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
        /// Repositorio de cuentas de credito
        /// </summary>
        public IRepositorioCreditosApi RepositorioCreditos => 
            repositorioCreditos ??= new RepositorioCreditosApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de transacciones de credito
        /// </summary>
        public IRepositorioTransaccionesCreditoApi RepositorioTransaccionesCredito => 
            repositorioTransaccionesCredito ??= new RepositorioTransaccionesCreditoApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de productos de credito
        /// </summary>
        public IRepositorioProductosCreditoApi RepositorioProductosCredito => 
            repositorioProductosCredito ??= new RepositorioProductosCreditoApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de grupos
        /// </summary>
        public IRepositorioGruposApi RepositorioGrupos => 
            repositorioGrupos ??= new RepositorioGruposApi(_gestorLog, _gestorPeticiones, _mambuOptions);
        /// <summary>
        /// Repositorio de sucursales
        /// </summary>
        public IRepositorioSucursalesApi RepositorioSucursales => 
            repositorioSucursales ??= new RepositorioSucursalesApi(_gestorLog, _gestorPeticiones, _mambuOptions);

    }
}
