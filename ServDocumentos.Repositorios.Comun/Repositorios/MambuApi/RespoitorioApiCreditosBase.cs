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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion de la funcionalidad del repositorio de creditos
    /// </summary>
    public abstract class RespoitorioApiCreditosBase : IRepositorioCreditosApi
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
        /// Url para obtener un creditoen base a su id
        /// </summary>
        private const string UrlGetById = "loans/{0}";
        /// <summary>
        /// URL para obtener los pagos programados
        /// </summary>
        private const string UrlGetSchedule = "loans/{0}/schedule";
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
        public RespoitorioApiCreditosBase(GestorLog gestorLog, IGestorPeticiones gestorPeticiones, IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtener credito en base a su identificador
        /// </summary>
        /// <param name="id">Encoded key o Id del credito</param>
        /// <returns>Instancia de <see cref="Credito"/></returns>
        public async Task<Credito> GetById(string id) => await GetById(id, DetailsLevel.BASIC);
        /// <summary>
        /// Obtener credito en base a su identificador
        /// </summary>
        /// <param name="id">Encoded key o Id del credito</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Instancia de <see cref="Credito"/></returns>
        public async Task<Credito> GetById(string id, DetailsLevel detailsLevel)
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
                    .Retornar<Credito>();
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
        /// <summary>
        /// Obtener pagos de un credito
        /// </summary>
        /// <param name="accountId">Id o encoded key de la cuenta</param>
        /// <returns>Coleccion de <see cref="PagosCameMambu"/></returns>
        public async Task<PagosCameMambu> GetSchedules(string accountId)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl(string.Format($"{_config.Url}/{UrlGetSchedule}", accountId))
                    .ComoGet()
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .Retornar<PagosCameMambu>();
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
