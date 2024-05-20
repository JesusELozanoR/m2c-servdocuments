using System;
using System.Collections.Generic;
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
    public class PlantillasCamposController : BaseController
    {
        public PlantillasCamposController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un Plantillas Campos 
        /// </summary>
        /// <param name="PlantillasCampos Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un PlantillasCampos</response>
        /// <response code="409">Muestra los datos del error que impiden agregar plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "PlantillasCampos Agrega")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] PlantillaCampoInsDto plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCampos = factory.ServicioPlantillasCampos.Agrega(plantillacampo);
                if (PlantillasCampos == null || PlantillasCampos.Count == 0)
                {
                    resultado = Ok(true);
                }
                else {
                    resultado = Conflict(
                        new MensajeErrorFuncionalDto
                        {
                            Origen = MensajesServicios.ServicioPlantillasCampos,
                            Mensajes = new[] { "Resgistros con error : " + string.Join(", ", PlantillasCampos) }
                        }
                    ); 
                }
                // resultado = Ok(PlantillasCampos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un PlantillasCampos por su id
        /// </summary>
        /// <param name="PlantillasCampos Elimina"> </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("id", Name = "PlantillasCampos Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] PlantillaCampoIdDto plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.Elimina(plantillacampo);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina los campos por el id de la plantilla 
        /// </summary>
        /// <param name="PlantillasCampos EliminaxPlantilla"> Id del PlantillasCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un PlantillasCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Plantilla", Name = "PlantillasCampos EliminaxPlantilla")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxPlantilla([FromBody] PlantillaIdDto plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.EliminaxPlantilla(plantilla);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina un PlantillasCampos por su Id
        /// </summary>
        /// <param name="PlantillasCampos EliminaxId"> Id del PlantillasCampos </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un PlantillasCampos</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Campo", Name = "PlantillasCampos EliminaxCampoId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxCampo([FromBody] PlantillaCampoIdcDto plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.EliminaxCampo(plantillacampo);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los PlantillasCamposs
        /// </summary>
        /// <param name="PlantillasCampos Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los PlantillasCampos</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "PlantillasCampos Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoPlantillaCampo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene(PlantillaCampoGetDto plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.Obtiene( plantillacampo);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica un PlantillasCampos
        /// </summary>
        /// <param name="PlantillasCampos Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "PlantillasCampos Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] PlantillaCampoUpdDto plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.Modifica(plantillacampo);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de los PlantillasCamposs por el Proces0 Subproceso
        /// </summary>
        /// <param name="PlantillasCampos Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los PlantillasCampos</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer plantillas campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtienexProceso", Name = "PlantillasCampos ObtienexProceso")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoPlantillaCampo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtienexProceso(PlantillaCampoPSGet plantillacampo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var PlantillasCamposs = factory.ServicioPlantillasCampos.ObtienexProceso(plantillacampo);
                resultado = Ok(PlantillasCamposs);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillasCampos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

    }
}
