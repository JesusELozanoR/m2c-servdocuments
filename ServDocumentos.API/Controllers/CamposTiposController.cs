using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CamposTiposController : BaseController
    {
        public CamposTiposController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un Tipo de Campo
        /// </summary>
        /// <param name="campotipo Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un Tipo de Campo</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "CampoTipo Agrega")]
        [ProducesResponseType(typeof(ResultadoCampoTipo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] CampoTipoInsDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioCamposTipos.Agrega(campotipo);
                resultado = Ok(result);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Modifica un Tipo de Campo 
        /// </summary>
        /// <param name="campotipo Modifica"> </param>
        /// <returns></returns>
        /// <response code="200">Modifica un Tipo de Campo</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Modifica", Name = "CampoTipo Modifica")]
        [ProducesResponseType(typeof(ResultadoCampoTipo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] CampoTipoUpdDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioCamposTipos.Modifica(campotipo);
                resultado = Ok(result);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }


        /// <summary>
        /// Obtiene los Tipos de Campos 
        /// </summary>
        /// <param name="campotipo buscar"> </param>
        /// <returns></returns>
        /// <response code="200">Busqueda de Tipos de Campo</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "CampoTipo Obtiene")]
        [ProducesResponseType(typeof(ResultadoCampoTipo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] CampoTipoGetDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioCamposTipos.Obtiene(campotipo);
                resultado = Ok(result);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }
    }
}
