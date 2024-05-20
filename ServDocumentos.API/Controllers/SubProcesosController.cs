using System;
using System.Collections.Generic;
using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.Core.Dtos.Comun.Documento;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SubProcesosController : BaseController
    {

        public SubProcesosController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un subproceso 
        /// </summary>
        /// <param name="Subproceso Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un subproceso</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "Subproceso Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] SubProcesoDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.Agrega(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un SubProceso por su Nombre
        /// </summary>
        /// <param name="Subproceso Elimina"> Nombre del SubProceso </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un subproceso</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Nombre", Name = "Subproceso Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] SubProcesoNombreDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.EliminaxNombre(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un SubProceso por su Id
        /// </summary>
        /// <param name="Subproceso EliminaxId"> Id del SubProceso </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un subproceso</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Id", Name = "Subproceso EliminaxId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxId([FromBody] SubProcesoIdDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.EliminaxId(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los SubProcesos
        /// </summary>
        /// <param name="Subproceso Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los subproceso</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "Subproceso Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<SubProcesoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene(SubProcesoGetDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.Obtiene(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un subProceso
        /// </summary>
        /// <param name="Subproceso Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "Subproceso Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] SubProcesoUpdDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.Modifica(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los SubProcesos por Clasificacion
        /// </summary>
        /// <param name="Subproceso x Clasificacion Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los subproceso</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los subproceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtienexClasificacion", Name = "Subproceso x Clasificacion Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<SubProcesoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtienexClasificacion(SubProcesoClasDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioSubProcesos.ObtienexClasificacion(subproceso);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

       
    }
}
