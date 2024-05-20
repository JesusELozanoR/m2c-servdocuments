using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DocumentosController : BaseController
    {
        public DocumentosController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog) { }
        /// <summary>
        /// Servicio mediante el cual se obtienen los documentos legales de un crédito en formato PDF o ZIP
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerDocumentos", Name = "ObtenerArchivos")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerDocumentos([FromBody] SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioDocumentos.ObtieneDocumento(solicitud);
                resultado = Ok(result);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen la url para consultar los Documentos
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerRutaDocumentos", Name = "ObtenerRutaArchivos")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerRutaDocumentos([FromBody] SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoRuta = factory.ServicioDocumentos.ObtieneRutaDocumento(solicitud);
                resultado = Ok(resultadoRuta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen el Estado de Cuenta
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerEstadoCuenta", Name = "ObtenerEstadoCuenta")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerEstadoCuenta([FromBody] ObtenerPlantillasProcesoDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoEstadoCuenta = factory.ServicioDocumentos.ObtieneEstadoCuenta(solicitud);
                resultado = Ok(resultadoEstadoCuenta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Proceso Mensual para la seleccion de los creditos a crear estado de cuenta.
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("EstadosCuentaMensualProcesa", Name = "EstadosCuentaMensualProcesa")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EstadosCuentaMensualProcesa([FromBody] EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoEstadoCuenta = factory.ServicioDocumentos.EstadosCuentaMensualProcesa(solicitud);
                resultado = Ok(resultadoEstadoCuenta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Obtiene la lista de los creditos para la creacion de su estado de cuenta.
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("EstadosCuentaMensualObtiene", Name = "EstadosCuentaMensualObtiene")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EstadosCuentaMensualObtiene([FromBody] EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoEstadoCuenta = factory.ServicioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuenta(solicitud.Empresa, solicitud.Fecha, solicitud.Elementos);
                resultado = Ok(resultadoEstadoCuenta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen el Estado de Cuenta Mensual de los creditos activos
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("EstadoCuentaMensualGenera", Name = "EstadoCuentaMensualGenera")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EstadoCuentaMensualGenera([FromBody] EstadoCuentaMensualSol solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoEstadoCuenta = factory.ServicioDocumentos.EstadoCuentaMensualGenera(solicitud);
                resultado = Ok(resultadoEstadoCuenta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen el Estado de Cuenta Mensual de los creditos activos
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("EstadoCuentaMensualEnivarCorreo", Name = "EstadoCuentaMensualEnivarCorreo")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EstadoCuentaMensualEnivarCorreo([FromBody] EstadoCuentaMensualSol solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoEstadoCuenta = await factory.ServicioDocumentos.EstadoCuentaMensualEnviaAsync(solicitud);
                resultado = Ok(resultadoEstadoCuenta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen el estado de cuenta para productos digitales de crédito individual y grupal.
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos del documento generado.</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud.</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción.</response> 
        [HttpPost("EstadoCuentaCredito", Name = "EstadoCuentaCredito")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public IActionResult EstadoCuentaCredito([FromBody] EstadoCuentaCreditoSolDto solicitud)
        {
            gestorLog.Entrar();
            try
            {
                return Ok(factory.ServicioDocumentos.EstadoCuentaCredito(solicitud));
            }
            catch (BusinessException buex)
            {
                return Conflict((Origen: MensajesServicios.ServicioDocumentos, Mensaje: buex.Message));
            }
            catch (ServiceException servEx)
            {
                return StatusCode(StatusCodes.Status404NotFound, (Origen: MensajesServicios.ServicioDocumentos, Mensaje: servEx.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (Origen: MensajesServicios.ServicioDocumentos, Mensaje: MensajesServicios.ErrorServidor, CodigoRastreo: gestorLog.RegistrarError(ex)));
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen una Url para consultar el Documento creado con base un Json
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerRutaDocumentosJson", Name = "ObtenerRutaDocumentosJson")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerRutaDocumentosJson([FromBody] SolictudDocumentoJsonDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoRuta = factory.ServicioDocumentos.ObtieneRutaDocumentoJson(solicitud);
                resultado = Ok(resultadoRuta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }
        /// <summary>
        /// Servicio mediante el cual se obtienen los documentos legales de un crédito creado con base un Json en formato PDF o ZIP
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerDocumentosJson", Name = "ObtenerDocumentosJson")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerDocumentosJson([FromBody] SolictudDocumentoJsonDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoRuta = factory.ServicioDocumentos.ObtenerDocumentosJson(solicitud);
                resultado = Ok(resultadoRuta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen los documentos legales de un crédito con valores Ejemplo
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("VistaPrevia", Name = "ObtenerDocumentosVistaPrevia")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerDocumentosVistaPrevia([FromBody] SolictudVistaPreviaDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoRuta = factory.ServicioDocumentos.VistaPrevia(solicitud);
                resultado = Ok(resultadoRuta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
        /// <summary>
        /// Servicio mediante el cual se obtienen los documentos del recibo de pago
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerReciboPago", Name = "ObtenerReciboPago")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerReciboPago([FromBody] SolicitudDocumentoReciboJson solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var resultadoRuta = factory.ServicioDocumentos.ObtenerRutaReciboJson(solicitud);
                resultado = Ok(resultadoRuta);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }

        /// <summary>
        /// Servicio mediante el cual se obtienen los documentos legales
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos de los documentos generadors</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerDatosPlantillaAhorroJsonSinDatos", Name = "ObtenerDatosPlantillaAhorroJsonSinDatos")]
        [ProducesResponseType(typeof(ResultadoDocumentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerDatosPlantillaAhorroJsonSinDatos([FromBody] SolictudDocumentoJsonDatosDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var result = factory.ServicioDocumentos.ObtenerDocumentosJsonDatos(solicitud);
                resultado = Ok(result);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = buex.Message });
            }
            catch (ServiceException servEx)
            {
                resultado = StatusCode(StatusCodes.Status404NotFound, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = servEx.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDocumentos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }
    }
}
