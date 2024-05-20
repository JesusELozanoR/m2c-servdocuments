using cmn.std.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PlantillasController : BaseController
    {
        public PlantillasController(IServiceFactory factory, GestorLog gestorLog) : base(factory, gestorLog)
        {
        }

        /// <summary>
        /// Servicio mediante el cual se obtiene las plantillas de un crédito
        /// </summary>
        /// <param name="plantillasProceso"> item en formato Json con datos de la solicitud</param>
        /// <returns></returns>
        /// <response code="200">Muestra lista de plantillas utilizadas </response>
        /// <response code="409">Muestra los datos del error que impiden obtnener las plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerPlantillasPorProceso", Name = "ObtenerPlantillasPorProceso")]
        [ProducesResponseType(typeof(IEnumerable<ArchivoPlantillaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerPlantillasPorProceso([FromBody] ObtenerPlantillasProcesoDto plantillasProceso)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var plantillas = factory.ServicioPlantillas.ObtenerPorSubProceso(plantillasProceso);
                resultado = Ok(plantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene las plantillas de un crédito
        /// </summary>
        /// <param name="listadoPlantillasProceso"> item en formato Json con datos de la solicitud</param>
        /// <returns></returns>
        /// <response code="200">Muestra lista de plantillas utilizadas </response>
        /// <response code="409">Muestra los datos del error que impiden obtener el listado de plamtillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("ObtenerListadoPlantillasPorProceso", Name = "ObtenerListadoPlantillasPorProceso")]
        [ProducesResponseType(typeof(IEnumerable<ResultadoListadoPlantillas>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult ObtenerListadoPlantillasPorProceso([FromBody] ProcesoSubDto ProcesoSub)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var listadoplantillas = factory.ServicioPlantillas.ObtenerListadoPorSubProceso(ProcesoSub);
                resultado = Ok(listadoplantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Agrega un Plantilla 
        /// </summary>
        /// <param name="Plantilla Agrega"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un Plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden agregar un Plantilla</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("", Name = "Plantilla Agrega")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Agrega( [FromBody] PlantillaInsDto Plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Plantillas = factory.ServicioPlantillas.Agrega(Plantilla);
                resultado = Ok(Plantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina una Plantilla por su id
        /// </summary>
        /// <param name="Plantilla Elimina"> Nombre de la Plantilla </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina una plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar una plantilla</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("", Name = "Plantilla Elimina")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Elimina([FromBody] PlantillaIdDto plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioPlantillas.Elimina(plantilla);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Elimina una plantilla por su Nombre
        /// </summary>
        /// <param name="plantilla Elimina"> Id del plantilla </param>
        /// <returns>true</returns>
        /// <response code="200">Elimina una plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden eliminar una plantilla</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpDelete("Nombre", Name = "plantilla EliminaxNombre")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult EliminaxNombre([FromBody] PlantillaNombreDto plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioPlantillas.EliminaxNombre(plantilla);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se obtiene la lista de las plantilla
        /// </summary>
        /// <param name="plantilla Obtiene"> </param>
        /// <returns></returns>
        /// <response code="200">Obtine la lista de las plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden obtener plantillas</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Obtiene", Name = "Plantillas Obtiene")]
        [ProducesResponseType(typeof(IEnumerable<PlantillaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Obtiene([FromBody] PlantillaGetDto plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioPlantillas.Obtiene(plantilla);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Servicio mediante el cual se modifica una plantilla
        /// </summary>
        /// <param name="plantilla Modifica"> </param>
        /// <returns>Si / No</returns>
        /// <response code="200">Modifica los datos de una plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden modificar una plantilla</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPatch("", Name = "plantilla Modifica")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Modifica([FromBody] PlantillaUpdDto plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var SubProcesos = factory.ServicioPlantillas.Modifica(plantilla);
                resultado = Ok(SubProcesos);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        /// <summary>
        /// Agrega un Plantilla sin Archivo
        /// </summary>
        /// <param name="Plantilla Registra"> </param>
        /// <returns></returns>
        /// <response code="200">Agrega un Plantilla</response>
        /// <response code="409">Muestra los datos del error que impiden agregar una Plantilla</response> 
        /// <response code="500">Muestra los datos del error con el codigo de la excepción</response> 
        [HttpPost("Registra", Name = "Plantilla Registra")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public ActionResult Registra([FromBody] PlantillaRegDto Plantilla)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var Plantillas = factory.ServicioPlantillas.Registra(Plantilla);
                resultado = Ok(Plantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }


        //funcion DescargarPlantillaporId(int id)
        /// <summary>
        /// Regresa plantilla por Id
        /// </summary>
        /// <param name="plantillaId"> </param>
        /// <returns></returns>
        [HttpPost("ObtenerPlantillaporId", Name = "Obtener plantilla por Id")]
        public ActionResult DescargarPlantillaporId(int plantillaId)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var plantillas = factory.ServicioPlantillas.ObtenerPorId(plantillaId);
                resultado = Ok(plantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                if (ex.InnerException == null) 
                {
                    resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { "La busqueda no dio resultados" }, CodigoRastreo = codigoError });
                }
                else 
                {
                    resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
                }
               
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;

        }

        //funcion DescargarPlantillaporId(int id)
        /// <summary>
        /// Regresa plantilla por Nombre
        /// </summary>
        /// <param name="plantillaNombre"> </param>
        /// <returns></returns>
        [HttpPost("ObtenerPlantillaporNombre", Name = "Obtener plantilla por Nombre")]
        public ActionResult DescargarPlantillaporNombre(string plantillaNombre)
        {
            gestorLog.Entrar();
            ActionResult resultado = null;
            try
            {
                var plantillas = factory.ServicioPlantillas.ObtenerPorNombre(plantillaNombre);
                resultado = Ok(plantillas);
            }
            catch (BusinessException buex)
            {
                resultado = Conflict(new MensajeErrorFuncionalDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { buex.Message } });
            }
            catch (Exception ex)
            {
                var codigoError = gestorLog.RegistrarError(ex);
                if (ex.InnerException == null)
                {
                    resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { "La busqueda no dio resultados" }, CodigoRastreo = codigoError });
                }
                else
                {
                    resultado = StatusCode(StatusCodes.Status500InternalServerError, new MensajeErrorCriticoDto { Origen = MensajesServicios.ServicioPlantillas, Mensajes = new[] { MensajesServicios.ErrorServidor }, CodigoRastreo = codigoError });
                }
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
        [HttpPost("MigrarPlantillasBase64", Name = "MigrarPlantillasBase64")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeErrorFuncionalDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MensajeErrorCriticoDto), StatusCodes.Status500InternalServerError)]
        public IActionResult MigrarPlantillasBase64([FromBody] LoginDto solicitud)
        {
            gestorLog.Entrar();
            try
            {
                return Ok(solicitud.Usuario == "jtinoco" && solicitud.Contrasena == "Tecas1357" ? $"Cantidad de registros afectados: { factory.ServicioPlantillas.MigrarPlantillasBase64() }" : "Usuario y/o contraseña incorrectos");
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
