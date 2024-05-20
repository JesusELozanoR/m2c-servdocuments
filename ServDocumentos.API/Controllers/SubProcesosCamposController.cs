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
    public class SubProcesosCamposController : BaseController
    {
        public SubProcesosCamposController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un SubProcesosCampos 
        /// </summary>
        /// <param name="SubProcesosCampos Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un SubProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un SubProcesos Campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "SubProcesosCampos Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] SubProcesoCampoInsDto subprocesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesosCamposs = factory.ServicioSubProcesosCampos.Agrega(subprocesoscampos);
                resultado = Ok(SubProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un SubProcesosCampos por su Nombre
        /// </summary>
        /// <param name="SubProcesosCampos Elimina"> Nombre del SubProcesosCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un SubProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un SubProcesos Campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("ids", Name = "SubProcesosCampos Elimina x Ids")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxIds([FromBody] SubProcesoCampoDelDto subprocesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesosCamposs = factory.ServicioSubProcesosCampos.EliminaxIds(subprocesoscampos);
                resultado = Ok(SubProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un SubProcesosCampos por su Id
        /// </summary>
        /// <param name="SubProcesosCampos EliminaxId"> Id del SubProcesosCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un SubProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un SubProcesos Campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Id", Name = "SubProcesosCampos EliminaxId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxId([FromBody] SubProcesoCampoIdDto subprocesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesosCamposs = factory.ServicioSubProcesosCampos.Elimina(subprocesoscampos);
                resultado = Ok(SubProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los SubProcesosCamposs
        /// </summary>
        /// <param name="SubProcesosCampos Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los SubProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los SubProcesos Campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "SubProcesosCampos Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoSubProcesoCampo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] SubProcesoCampoGetDto subprocesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesosCamposs = factory.ServicioSubProcesosCampos.Obtiene(subprocesoscampos);
                resultado = Ok(SubProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un SubProcesosCampos
        /// </summary>
        /// <param name="SubProcesosCampos Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un SubProcesosCampos</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un SubProcesos Campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "SubProcesosCampos Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] SubProcesoCampoUpdDto subprocesoscampos)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesosCamposs = factory.ServicioSubProcesosCampos.Modifica(subprocesoscampos);
                resultado = Ok(SubProcesosCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }
    }
}
