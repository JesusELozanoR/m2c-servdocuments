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
    [Produces("application/json")]
    [ApiController]
    public class ClasificacionesController : BaseController
    {
        public ClasificacionesController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega una Clasificacion
        /// </summary>
        /// <param name="Clasificaciones Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un Clasificaciones</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un Clasificaciones</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "Clasificaciones Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] ClasificacionInsDto clasificaciones)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Clasificacioness = factory.ServicioClasificaciones.Agrega(clasificaciones);
                resultado = Ok(Clasificacioness);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un Clasificaciones por su Nombre
        /// </summary>
        /// <param name="Clasificaciones Elimina"> Nombre del Clasificaciones </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un Clasificaciones</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un Clasificaciones</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Nombre", Name = "Clasificaciones Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] ClasificacionNombreDto clasificaciones)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Clasificacioness = factory.ServicioClasificaciones.EliminaxNombre(clasificaciones);
                resultado = Ok(Clasificacioness);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un Clasificaciones por su Id
        /// </summary>
        /// <param name="Clasificaciones EliminaxId"> Id del Clasificaciones </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un Clasificaciones</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un Clasificaciones</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Id", Name = "Clasificaciones EliminaxId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxId([FromBody] ClasificacionIdDto clasificaciones)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Clasificacioness = factory.ServicioClasificaciones.Elimina(clasificaciones);
                resultado = Ok(Clasificacioness);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los Clasificacioness
        /// </summary>
        /// <param name="Clasificaciones Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los Clasificaciones</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los Clasificaciones</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "Clasificaciones Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoClasificacion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] ClasificacionGetDto clasificaciones)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Clasificacioness = factory.ServicioClasificaciones.Obtiene(clasificaciones);
                resultado = Ok(Clasificacioness);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un Clasificaciones
        /// </summary>
        /// <param name="Clasificaciones Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un Clasificaciones</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un Clasificaciones</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "Clasificaciones Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] ClasificacionUpdDto clasificaciones)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Clasificacioness = factory.ServicioClasificaciones.Modifica(clasificaciones);
                resultado = Ok(Clasificacioness);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioClasificaciones, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }
    }
}
