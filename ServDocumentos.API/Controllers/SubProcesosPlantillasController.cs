using System;
using System.Collections.Generic;
using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.Core.Contratos.Factories.Comun;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubProcesosPlantillasController : BaseController
    {
        public SubProcesosPlantillasController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega Plantillas a un Subproceso
        /// </summary>
        /// <param name="SubProcesoPlantillas Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un SubProcesoPlantillas</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un proceso plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "SubProcesoPlantillas Agrega")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] SubProcesoPlantillaInsDto SubProcesoPlantillas)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesoPlantillass = factory.ServicioSubProcesosPlantillas.Agrega(SubProcesoPlantillas);
                if (SubProcesoPlantillass == null || SubProcesoPlantillass.Count == 0)
                {
                    resultado = Ok(true);
                }
                else
                {
                    resultado = Conflict(
                        new MensajeErrorFuncionalDto
                        {
                            Origen = MensajesServicios.ServicioSubProcesosPlantillas,
                            Mensajes = new[] { "Resgistros con error : " + string.Join(", ", SubProcesoPlantillass) }
                        }
                    );
                }
                // resultado = Ok(SubProcesoPlantillass);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un SubProcesoPlantillas por Ids
        /// </summary>
        /// <param name="SubProcesoPlantillas Elimina"> Nombre del SubProcesoPlantillas </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un SubProcesoPlantillas</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un proceso plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("", Name = "SubProcesoPlantillas Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] SubprocesoPlantillaDelDto subprocesoplantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesoPlantillass = factory.ServicioSubProcesosPlantillas.Elimina(subprocesoplantilla);
                resultado = Ok(SubProcesoPlantillass);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        
        /// <summary>
        /// Elimina un SubProcesoPlantillas por su Id
        /// </summary>
        /// <param name="SubProcesoPlantillas EliminaxSubprocesoId"> Id del SubProcesoPlantillas </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un SubProcesoPlantillas</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un proceso plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("SubprocesoId", Name = "SubProcesoPlantillas EliminaxSubprocesoId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxSubprocesoId([FromBody] SubProcesoIdDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesoPlantillass = factory.ServicioSubProcesosPlantillas.EliminaxSubprocesoId(subproceso);
                resultado = Ok(SubProcesoPlantillass);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los SubProcesoPlantillass
        /// </summary>
        /// <param name="SubProcesoPlantillas Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los SubProcesoPlantillas</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los proceso plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "SubProcesoPlantillas Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoSubProcesoPlantilla>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody]  SubprocesoPlantillaGetDto subproceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesoPlantillass = factory.ServicioSubProcesosPlantillas.Obtiene(subproceso);
                resultado = Ok(SubProcesoPlantillass);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un SubProcesoPlantillas
        /// </summary>
        /// <param name="SubProcesoPlantillas Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un proceso plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "SubProcesoPlantillas Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] SubProcesoPlantillaUpdDto SubProcesoPlantillas)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesoPlantillass = factory.ServicioSubProcesosPlantillas.Modifica(SubProcesoPlantillas);
                resultado = Ok(SubProcesoPlantillass);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioSubProcesosPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

    }
}
