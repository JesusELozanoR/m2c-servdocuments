using cmn.std.Log;
using Comun.Peticiones.Extensores;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion del repositorios de transacciones de ahorro
    /// </summary>
    public abstract class RepositorioApiTransaccionesAhorroBase : IRepositorioTransaccionesAhorroApi
    {
        /// <summary>
        /// Nombre de la configuracion
        /// </summary>
        public abstract Empresa Core { get; }
        /// <summary>
        /// Url para consultar las transacciones de una cuenta
        /// </summary>
        private const string GetAllUrl = "deposits/{0}/transactions";
        /// <summary>
        /// Url para buscar transacciones por filtro
        /// </summary>
        private const string SearchUrl = "deposits/transactions:search";
        /// <summary>
        /// Gestor log
        /// </summary>
        private readonly GestorLog _gestorLog;
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
        public RepositorioApiTransaccionesAhorroBase(GestorLog gestorLog, IGestorPeticiones gestorPeticiones, IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtener todas las transacciones de una cuenta de ahorro
        /// </summary>
        /// <param name="idCuenta">Id de la cuenta</param>
        /// <returns>Coleccion de transacciones</returns>
        public async Task<IEnumerable<TransaccionAhorroDto>> GetAll(string idCuenta)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl(string.Format($"{_config.Url}/{GetAllUrl}", idCuenta))
                    .ComoGet()
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .Retornar<IEnumerable<TransaccionAhorroDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<TransaccionAhorroDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
        /// <summary>
        /// Obtener transacciones de ahorro por filtro
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Coleccion de transacciones</returns>
        public async Task<IEnumerable<TransaccionAhorroDto>> Search(SearchCriteria searchCriteria)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl($"{_config.Url}/{SearchUrl}")
                    .ComoPost()
                    .ConQueryStrings(searchCriteria?.Parameters ?? new Dictionary<string, string>())
                    .ConBody(JsonConvert.SerializeObject(searchCriteria))
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .Retornar<IEnumerable<TransaccionAhorroDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<TransaccionAhorroDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }

        public async Task<IEnumerable<TransaccionAhorroDetalleDto>> SearchTransacctions(SearchCriteria searchCriteria)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl($"{_config.Url}/{SearchUrl}")
                    .ComoPost()
                    .ConQueryStrings(searchCriteria?.Parameters ?? new Dictionary<string, string>())
                    .ConBody(JsonConvert.SerializeObject(searchCriteria))
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .Retornar<IEnumerable<TransaccionAhorroDetalleDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<TransaccionAhorroDetalleDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
    }
}
