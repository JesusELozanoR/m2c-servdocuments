using cmn.std.Log;
using Comun.Peticiones.Extensores;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion de la funcionalidad del repositorio de ahorros
    /// </summary>
    public abstract class RepositorioApiAhorrosBase : IRepositorioAhorrosApi
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
        /// Url para buscar consultar cuentas de ahorro con filtros
        /// </summary>
        private const string AhorrosSearchUrl = "deposits:search";
        /// <summary>
        /// Url para obtener un ahorro en base a su id
        /// </summary>
        private const string UrlGetById = "deposits/{0}";
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
        public RepositorioApiAhorrosBase(GestorLog gestorLog, IGestorPeticiones gestorPeticiones, IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtiene una coleccion de ahorros
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Lista de cuentas de ahorro</returns>
        public async Task<IEnumerable<AhorroDto>> Search(SearchCriteria searchCriteria)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl($"{_config.Url}/{AhorrosSearchUrl}")
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConBody(JsonConvert.SerializeObject(searchCriteria))
                    .ConQueryStrings(searchCriteria?.Parameters ?? new Dictionary<string, string>())
                    .ComoPost()
                    .Retornar<IEnumerable<AhorroDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<AhorroDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
        /// <summary>
        /// Obtiene la informacion de un ahorro en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la cuenta</param>
        /// <returns>Cuenta de ahorro</returns>
        public async Task<AhorroDto> GetById(string id) => await GetById(id, DetailsLevel.BASIC);
        /// <summary>
        /// Obtiene la informacion de un ahorro en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la cuenta</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Cuenta de ahorro</returns>
        public async Task<AhorroDto> GetById(string id, DetailsLevel detailsLevel)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl(string.Format($"{_config.Url}/{UrlGetById}", id))
                    .ConQueryString("detailsLevel", detailsLevel.ToString())
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ComoGet()
                    .Retornar<AhorroDto>();
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
