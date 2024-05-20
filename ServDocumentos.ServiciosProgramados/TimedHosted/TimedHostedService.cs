
using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServDocumentos.Core.Contratos.Workers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServDocumentos.ServiciosProgramados.TimedHosted
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly GestorLog gestorLog;
        public IServiceProvider Services { get; }

        private const int TimerIntervalSeconds = 3600;
        private readonly IConfiguration configuration;

        public TimedHostedService(IServiceProvider services, IConfiguration configuration, GestorLog gestorLog)
        {
            Services = services;
            this.configuration = configuration;

            this.gestorLog = gestorLog;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            gestorLog.Registrar(Nivel.Information, "Start worker");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
              TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            gestorLog.Registrar(Nivel.Information, "Entra a DoWork");
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IWorkerDia>();

                scopedProcessingService.VerificaCadaDia();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            gestorLog.Registrar(Nivel.Information, "Stop worker");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    }
