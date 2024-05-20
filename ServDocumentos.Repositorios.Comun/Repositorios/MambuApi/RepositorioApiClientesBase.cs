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
    /// Abstraccion del repositorios de clientes
    /// </summary>
    public abstract class RepositorioApiClientesBase : IRepositorioClientesApi
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
        /// Url para obtener un cliente en base a su id
        /// </summary>
        private const string UrlGetById = "clients/{0}";
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
        public RepositorioApiClientesBase(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtener cliente en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del cliente</param>
        /// <returns>Informacion del cliente</returns>
        public async Task<Cliente> GetById(string id) => await GetById(id, DetailsLevel.BASIC);
        /// <summary>
        /// Obtener cliente en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del cliente</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Informacion del cliente</returns>
        public async Task<Cliente> GetById(string id, DetailsLevel detailsLevel)
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
                    .Retornar<Cliente>();
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
