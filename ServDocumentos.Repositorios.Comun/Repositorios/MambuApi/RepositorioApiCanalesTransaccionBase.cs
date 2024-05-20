using cmn.std.Log;
using Comun.Peticiones.Extensores;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion del repositorios de canales de transaccion
    /// </summary>
    public abstract class RepositorioApiCanalesTransaccionBase : IRepositorioCanalesTransaccionApi
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
        /// Url para obtener todos los canales
        /// </summary>
        private const string UrlGetAll = "organization/transactionChannels";
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
        public RepositorioApiCanalesTransaccionBase(GestorLog gestorLog, IGestorPeticiones gestorPeticiones, IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Obtener todos los canales de transaccion
        /// </summary>
        /// <returns>Coleccion de canales de transaccion</returns>
        public async Task<IEnumerable<CanalTransaccionDto>> GetAll()
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl($"{_config.Url}/{UrlGetAll}")
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .ComoGet()
                    .Retornar<IEnumerable<CanalTransaccionDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<CanalTransaccionDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
    }
}
