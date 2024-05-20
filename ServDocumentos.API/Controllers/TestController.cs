using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.ServiciosProgramados.TimedHosted;
using System;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private GestorParametros _gestorParametros;
        private readonly TimedHostedService _ws;

        private readonly GestorLog gestorLog;
        public TestController(IHostedService ws, GestorParametros gestorParametros, GestorLog gestorLog)
        {
            this.gestorLog = gestorLog;
            _gestorParametros = gestorParametros;
            _ws = ws as TimedHostedService;
        }

        /// <summary>
        /// Método para validar que los Servicios esten respondiendo
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Los servicios estan funcionando</response>        
        [HttpGet("", Name = "TestSerivciosDocumentos")]
        public ActionResult TestSerivciosDocumentos()
        {
            var mesAnterior = DateTime.Now.AddMonths(-1);
            return Ok(new { Fecha = DateTime.Now, Mensaje = MensajesServicios.ServiciosFuncionando, Version = MensajesServicios.Version });
        }

        [HttpGet("[action]")]
        public ActionResult ActualizarParametros()
        {
            try
            {
                _gestorParametros.VaciarCacheParametros();

                gestorLog.Registrar(Nivel.Information, "Se refresco parametros" );
                return Ok("Se vacio el cache de parametros");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("[action]")]
        public ActionResult IniciaWorker()
        {
            try
            {
                _ws.StartAsync(new System.Threading.CancellationToken());
                return Ok("Se inicio worker");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("[action]")]
        public ActionResult ReiniciaWorker()
        {
            try
            {
                _ws.StopAsync(new System.Threading.CancellationToken());
                _ws.StartAsync(new System.Threading.CancellationToken());
                return Ok("Se reinicio worker");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("[action]")]
        public ActionResult DetenerWorker()
        {
            try
            {
                _ws.StopAsync(new System.Threading.CancellationToken());
                return Ok("Se detuvo el worker");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
