using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesosCamposController : BaseController
    {
        public ProcesosCamposController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un ProcesosCampos 
        /// </summary>
        /// <param name="ProcesosCampos Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un ProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un ProcesosCampos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "ProcesosCampos Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] ProcesoCampoInsDto procesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var ProcesosCamposs = factory.ServicioProcesosCampos.Agrega(procesoscampos);
                resultado = Ok(ProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un ProcesosCampos por su Nombre
        /// </summary>
        /// <param name="ProcesosCampos Elimina"> Nombre del ProcesosCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un ProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un ProcesosCampos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Ids", Name = "ProcesosCampos Elimina x Ids")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxIds([FromBody] ProcesoCampoDelDto procesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var ProcesosCamposs = factory.ServicioProcesosCampos.EliminaxIds(procesoscampos);
                resultado = Ok(ProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un ProcesosCampos por su Id
        /// </summary>
        /// <param name="ProcesosCampos EliminaxId"> Id del ProcesosCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un ProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un ProcesosCampos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("", Name = "ProcesosCampos Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] ProcesoCampoIdDto procesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var ProcesosCamposs = factory.ServicioProcesosCampos.Elimina(procesoscampos);
                resultado = Ok(ProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los ProcesosCamposs
        /// </summary>
        /// <param name="ProcesosCampos Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los ProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los ProcesosCampos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "ProcesosCampos Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoProcesoCampo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] ProcesoCampoGetDto procesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var ProcesosCamposs = factory.ServicioProcesosCampos.Obtiene(procesoscampos);
                resultado = Ok(ProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un ProcesosCampos
        /// </summary>
        /// <param name="ProcesosCampos Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un ProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un ProcesosCampos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "ProcesosCampos Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] ProcesoCampoUpdDto procesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var ProcesosCamposs = factory.ServicioProcesosCampos.Modifica(procesoscampos);
                resultado = Ok(ProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

    }
}
