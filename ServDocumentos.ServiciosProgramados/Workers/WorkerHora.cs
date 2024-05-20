using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Workers;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;


namespace ServDocumentos.ServiciosProgramados.Workers
{
   public class WorkerHora : IWorkerHora, IDisposable
    {
        private Timer _timer;
        public IServiceProvider Services { get; }
        private readonly GestorLog gestorLog;
        private readonly IConfiguration configuration;
        protected readonly comun.IServiceFactory _factory;
        public WorkerHora(IServiceProvider services, comun.IServiceFactory factory, GestorLog gestorLog, IConfiguration configuration)
        {
            Services = services;
            _factory = factory;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
        public void Dispose()
        {

        }

        public void VerificaCadaHora()
        {
            throw new NotImplementedException();
        }
    }
}
