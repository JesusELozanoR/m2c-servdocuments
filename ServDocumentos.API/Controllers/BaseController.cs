using cmn.std.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;

namespace ServDocumentos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly GestorLog gestorLog;
        protected readonly comun.IServiceFactory factory;
        protected readonly Func<string, comun.IServiceFactory> serviceProvider;
        protected BaseController(Func<string, comun.IServiceFactory> serviceProvider, GestorLog gestorLog)
        {
            this.serviceProvider = serviceProvider;
            this.gestorLog = gestorLog;
        }
        protected BaseController(comun.IServiceFactory factory, GestorLog gestorLog)
        {
            this.factory = factory;
            this.gestorLog = gestorLog;
        }

    }
}