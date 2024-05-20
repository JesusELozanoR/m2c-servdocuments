using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using System;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DatosController : BaseController
    {
        public DatosController(Core.Contratos.Factories.Comun.IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        {
        }

        /// <summary>
        /// Servicio mediante el cual se obtienen los datos del cliente
        /// </summary>
        /// <param name="solicitud"> item en formato Json con datos de la solicitud</param>
        /// <returns></returns>
        /// <response code="200">Muestra los datos del cliente</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerDatos", Name = "PlantillaDatosDelCliente")]
        [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerDatos([FromBody] ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var datosJson = factory.ServicioDatosPlantillas.ObtenerDatos(solicitud);
                resultado = Ok(datosJson);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto  { Origen = MensajesServicios.ServicioDatos, Mensajes = new[] { buex.Message } });
            }
            catch (ExternoTechnicalException ctex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioDatos + "-" + MensajesServicios.ServicioExterno, Mensajes = new [] { ctex.ToString() } });
                
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioDatos, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio que elimina los datos guardados de la documentación de un crédito previamente solicitado
        /// </summary>
        /// <param name="solicitud">item en formato Json con los datos para resetear la solicitud</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="409">Muestra los datos del error que impiden procesar la solicitud </response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("ResetearDatos", Name = "EliminaDatosDelCliente")]        
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ResetearDatos([FromBody] SolicitudBaseDto solicitud)
        {
            gestorLog.Entrar();
            ActionResult resultado = Ok();
            try
            {
                factory.ServicioDatosPlantillas.ResetearDatos(solicitud);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new { Origen = MensajesServicios.ServicioDatos, Mensaje = buex.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDatos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }
        /// <summary>
        /// Migra todos los documentos con un id del alfresco a la DB con base64
        /// </summary>
        /// <param name="solicitud">Item en formato Json con los datos para realizar a la migracion</param>
        /// <returns>Entero con la cantidad total de registros modificados</returns>
        [HttpPost("MigrarDatosJsonBase64", Name = "MigrarDatosJsonBase64")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public IActionResult MigrarDatosJsonBase64([FromBody] LoginDto solicitud)
        {
            gestorLog.Entrar();
            try
            {
                return Ok(solicitud.Usuario == "jtinoco" && solicitud.Contrasena == "Tecas1357" ? $"Cantidad de registros afectados: { factory.ServicioDatosJson.MigrarDatosJsonBase64() }" : "Usuario y/o contraseña incorrectos");
            }
            catch (BusinessException buex)
            {
                return Conflict(new { Origen = MensajesServicios.ServicioDatos, Mensaje = buex.Message });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Origen = MensajesServicios.ServicioDatos, Mensaje = MensajesServicios.ErrorServidor, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
        }
    }
}