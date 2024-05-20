using cmn.std.Log;
using Comun.Peticiones.Extensores;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion del repositorios de sucursales
    /// </summary>
    public abstract class RepositorioApiSucursalesBase : IRepositorioSucursalesApi
    {
        /// <summary>
        /// Nombre de la configuracion
        /// </summary>
        public abstract Empresa Core { get; }
        /// <summary>
        /// Gestor log
        /// </summary>
        private readonly GestorLog _gestorLog;
        /// <summary>
        /// Url para obtener una sucursal en base a su id
        /// </summary>
        private const string UrlGetById = "branches/{0}";
        /// <summary>
        /// Nombre del header necesario para las peticiones de mambu
        /// </summary>
        private const string AcceptsHeaderName = "Accept";
        /// <summary>
        /// Nombre del valor del header necesario en mambu
        /// </summary>
        private const string AcceptsHeaderValue = "application/vnd.mambu.v2+json";
        /// <summary>
        /// Gestor peticiones
        /// </summary>
        private readonly IGestorPeticiones _gestorPeticiones;
        /// <summary>
        /// Configuracion de mambu
        /// </summary>
        private readonly MambuConfig _config;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public RepositorioApiSucursalesBase(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtener sucursal en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la sucursal</param>
        /// <returns>Informacion de la  sucursal</returns>
        public async Task<SucursalDto> GetById(string id) => await GetById(id, DetailsLevel.BASIC);
        /// <summary>
        /// Obtener sucursal en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la sucursal</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Informacion de la sucursal</returns>
        public async Task<SucursalDto> GetById(string id, DetailsLevel detailsLevel)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl(string.Format($"{_config.Url}/{UrlGetById}", id))
                    .ComoGet()
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConQueryString("detailsLevel", detailsLevel.ToString())
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .Retornar<SucursalDto>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return null;
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
    }
}
