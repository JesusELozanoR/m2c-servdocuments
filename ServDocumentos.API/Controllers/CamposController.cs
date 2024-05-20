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
    public class CamposController : BaseController
    {
        public CamposController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        { }

        /// <summary>
        /// Agrega un Campo 
        /// </summary>
        /// <param name="Campo Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un Campo</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "Campo Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega([FromBody] CampoInsDto Campo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Campos = factory.ServicioCampos.Agrega(Campo);
                resultado = Ok(Campos);
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
        /// Elimina un Campo por su Nombre
        /// </summary>
        /// <param name="Campo Elimina"> Nombre del Campo </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un Campo</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Nombre", Name = "Campo Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] CampoNombreDto Campo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Campos = factory.ServicioCampos.EliminaxNombre(Campo);
                resultado = Ok(Campos);
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
        /// Elimina un Campo por su Id
        /// </summary>
        /// <param name="Campo EliminaxId"> Id del Campo </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina un Campo</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Id", Name = "Campo EliminaxId")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxId([FromBody] CampoIdDto Campo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Campos = factory.ServicioCampos.EliminaxId(Campo);
                resultado = Ok(Campos);
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
        /// Servicio mediante el cual se obtiene la lista de los Campos
        /// </summary>
        /// <param name="Campo Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de los Campo</response>
        /// <response code="409">Muestra los datos del error que impiden obtneer los campos</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "Campo Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoCampo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] CampoGetDto campo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Campos = factory.ServicioCampos.Obtiene(campo);
                resultado = Ok(Campos);
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
        /// Servicio mediante el cual se modifica un Campo
        /// </summary>
        /// <param name="Campo Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de un proceso</response>
        /// <response code="409">Muestra los datos del error que impiden modificar un campo</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "Campo Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] CampoUpdDto Campo)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Campos = factory.ServicioCampos.Modifica(Campo);
                resultado = Ok(Campos);
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
