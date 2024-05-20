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
    public class PlantillaTipoSubProcesoController : BaseController
    {

        public PlantillaTipoSubProcesoController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega PlantillaTipoSubProceso
        /// </summary>
        /// <param name="campotipo Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrego PlantillaTipoSubProceso</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un PlantillaTipoSubProceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "PlantillaTipoSubProceso Agrega")]
        [ProducesResponseType(typeof(ResultadoPlantillaTipoSubProceso), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] PlantillaTipoSubProcesoInsDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioPlantillaTipoSubProceso.Agrega(campotipo);
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
        /// Modifica PlantillaTipoSubProceso
        /// </summary>
        /// <param name="campotipo Modifica"> </param>
        /// <returns></returns>
        /// <response code="200">Modifico PlantillaTipoSubProceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar PlantillaTipoSubProceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("Modificar", Name = "PlantillaTipoSubProceso Modificar")]
        //[ProducesResponseType(typeof(ResultadoPlantillaTipoSubProceso), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modificar([FromBody] PlantillaTipoSubProcesoUpdDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioPlantillaTipoSubProceso.Modificar(campotipo);
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
        /// Obtiene PlantillaTipoSubProceso 
        /// </summary>
        /// <param name="campotipo Obtener"> </param>
        /// <returns></returns>
        /// <response code="200">Busqueda de PlantillaTipoSubProceso</response>
        /// <response code="409">Muestra los datos del error que impiden Obtener un PlantillaTipoSubProceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "PlantillaTipoSubProceso Obtiene")]
        [ProducesResponseType(typeof(ResultadoPlantillaTipoSubProceso), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] PlantillaTipoSubProcesoGetDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioPlantillaTipoSubProceso.Obtener(campotipo);
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
        /// Elimina PlantillaTipoSubProceso
        /// </summary>
        /// <param name="campotipo Eliminar"> </param>
        /// <returns></returns>
        /// <response code="200">Elimino PlantillaTipoSubProceso</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar PlantillaTipoSubProceso</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Eliminar", Name = "PlantillaTipoSubProceso Eliminar")]
        [ProducesResponseType(typeof(ResultadoPlantillaTipoSubProceso), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Eliminar([FromBody] PlantillaTipoSubProcesoDelDto campotipo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioPlantillaTipoSubProceso.Eliminar(campotipo);
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
