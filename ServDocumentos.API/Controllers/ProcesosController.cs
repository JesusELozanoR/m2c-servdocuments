using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ProcesosController : BaseController
    {
        public ProcesosController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un proceso 
        /// </summary>
        /// <param name="Proceso Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "Proeso Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] ProcesoDto proceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.Agrega(proceso);
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un Proceso por su Nombre
        /// </summary>
        /// <param name="Proceso Elimina"> Nombre del Proceso </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Nombre", Name = "Proceso Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] ProcesoNombreDto proceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.EliminaxNombre(proceso);
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un Proceso por su Id
        /// </summary>
        /// <param name="Proceso EliminaxId"> Id del Proceso </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Id", Name = "Proceso EliminaxId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxId([FromBody]  ProcesoIdDto proceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.EliminaxId(proceso);
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los Procesos
        /// </summary>
        /// <param name="Proceso Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los proceso</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpGet("", Name = "Proceso Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<Procesoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene()
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.Obtiene();
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un Proceso
        /// </summary>
        /// <param name="Proceso Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "Proceso Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] ProcesoUpdDto proceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.Modifica(proceso);
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        // <summary>
        /// Servicio mediante el cual se obtiene la lista de los Procesos
        /// </summary>
        /// <param name="Proceso Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los proceso</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los proceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtener", Name = "Proceso Obtener")]
        [ProducesResponseType(typeof(IEnumerable<Procesoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtener([FromBody] ProcesoGetDto proceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Procesos = factory.ServicioProcesos.Obtener(proceso);
                resultado = Ok(Procesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioProcesos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

    }
}
