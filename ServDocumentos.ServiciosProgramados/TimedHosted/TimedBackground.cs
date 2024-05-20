
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServDocumentos.Core.Contratos.Workers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServDocumentos.ServiciosProgramados.TimedHosted
    {
        public class TimedBackground : BackgroundService
        {
            private Timer _timer;

            public IServiceProvider Services { get; }

            private const int TimerIntervalSeconds = 3600;
            private readonly IConfiguration configuration;

            public TimedBackground(IServiceProvider services, IConfiguration configuration)
            {
                Services = services;
                this.configuration = configuration;
            }

            /* public Task StartAsync(CancellationToken cancellationToken)
             {
                 _timer = new Timer(DoWork, null, TimeSpan.Zero,
                   TimeSpan.FromDays(1));
                 return Task.CompletedTask;
             }

             private void DoWork(object state)
             {
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
                 _timer?.Change(Timeout.Infinite, 0);

                 return Task.CompletedTask;
             }

             public void Dispose()
             {
                 _timer?.Dispose();
             }
            */
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {

                await DoWork(stoppingToken);

            }
            private async Task DoWork(CancellationToken stoppingToken)
            {
                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IWorkerDia>();

                    scopedProcessingService.VerificaCadaDia();
                }
            }

            public override async Task StopAsync(CancellationToken stoppingToken)
            {
                await base.StopAsync(stoppingToken);
            }
        }
    }
