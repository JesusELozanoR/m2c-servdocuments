using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServDocumentos.Core.Contratos.Workers;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.ServiciosProgramados.TimedHosted;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;

namespace ServDocumentos.ServiciosProgramados.Workers
{
    public class WorkerDia : IWorkerDia, IDisposable
    {
        private Dictionary<string, string> parametrosConfig;
        private bool banderaCorre = true;
        private readonly GestorParametros _gestorParametros;
        public IWorkerHora Services { get; }
        private readonly GestorLog gestorLog;
        private readonly IConfiguration configuration;
        protected readonly comun.IServiceFactory _factory;

        private readonly TimedHostedService _ws;
        public WorkerDia(IHostedService ws, IWorkerHora services, comun.IServiceFactory factory, GestorLog gestorLog, IConfiguration configuration, GestorParametros gestorParametros)
        {

            _ws = ws as TimedHostedService;
            _gestorParametros = gestorParametros;
            Services = services;
            _factory = factory;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
        }
        public void Dispose()
        {

        }
        public int ParametroStringInt(string nombre)
        {
            parametrosConfig.TryGetValue(nombre, out string stringOut);
            stringOut = string.IsNullOrEmpty(stringOut) ? "0" : stringOut;

            int.TryParse(stringOut, out int intOut);
            return intOut;

        }
        public void VerificaCadaDia()
        {
            try
            {
                var dia = DateTimeOffset.Now.Day;


                //obtine el valor de parametros
                List<Parametro> parametros = _gestorParametros.RecuperarPorGrupo("SERVICIOEDOCTA");
                parametrosConfig = parametros.Distinct().ToDictionary(x => x.Codigo, x => x.Valor);

                int diaInicio = ParametroStringInt("diaInicio");
                int diaFin = ParametroStringInt("diaFin");

                var horaInicia = ParametroStringInt("horaInicio");
                var horaFin = ParametroStringInt("horaFin");
                var minutos = ParametroStringInt("minutosEspera");

                if (diaFin == 0 && diaInicio == 0 && horaFin == 0 && horaInicia == 0 && minutos == 0)
                {
                    gestorLog.Registrar(Nivel.Information, "Se para el service worker por falta de parametros");

                    _ws.StopAsync(new System.Threading.CancellationToken());
                }
                gestorLog.Registrar(Nivel.Information, "Version: " + MensajesServicios.Version);
                
                gestorLog.Registrar(Nivel.Information, "Entra para verificar cada dia, Fecha: " + DateTimeOffset.Now + " dia: " + dia + ", Del " + diaInicio + " al " + diaFin);

                //si el dia es antes del 5 se ejecuta
                if ( dia >= diaInicio && dia <= diaFin)
                {
                    banderaCorre = true;
                    gestorLog.Registrar(Nivel.Information, "Entra para verificar cada hora");

                   _ = VerificaCadaHora();
                }



             
            }
            catch (Exception ex)
            {

                gestorLog.RegistrarError(ex);
            }
            
        }

        public async Task<int> VerificaCadaHora()
        {
            var servCorreo = configuration.GetSection("ServicioCorreoNotificaciones");


            //obtine el valor de parametros
            List<Parametro> parametros = _gestorParametros.RecuperarPorGrupo("SERVICIOEDOCTA");
            parametrosConfig = parametros.Distinct().ToDictionary(x => x.Codigo, x => x.Valor);

            var hora = DateTimeOffset.Now.Hour;

            var horaInicia = ParametroStringInt("horaInicio");
            var horaFin = ParametroStringInt("horaFin");
            gestorLog.Registrar(Nivel.Information, "hora: "+hora+ ", Del: "+ horaInicia+ " Al: "+horaFin);

            while (hora >= horaInicia && hora < horaFin)
            {
                if (banderaCorre)
                {
                    banderaCorre = false;
                    _ = EnviarEmail(configuration.GetValue<string>("Instancia"));
                }
                Thread.Sleep(60 * 60 * 1000);
                hora = DateTimeOffset.Now.Hour;
            }


            return 1;
        }

        public async Task<int> EnviarEmail(string emp)
        {
            try
            {

                gestorLog.Registrar(Nivel.Information, "Entra para enviar el email");
                var servCorreo = configuration.GetSection("ServicioCorreoNotificaciones");
                var empresa = emp;
                var subProc = "EstadosCuenta" + empresa;
                var proc = empresa == "CAME" ? "AhorroCame" : "AhorroTcr";
                var mesAnterior = DateTime.Now.AddMonths(-1);
                //Se procesan
                EstadosCuentaMensualProcSol sol = new EstadosCuentaMensualProcSol
                {
                    Empresa = empresa,
                    Fecha = mesAnterior
                };


                //obtine el valor de parametros
                List<Parametro> parametros = _gestorParametros.RecuperarPorGrupo("SERVICIOEDOCTA");
                parametrosConfig = parametros.Distinct().ToDictionary(x => x.Codigo, x => x.Valor);

                gestorLog.Registrar(Nivel.Information, "Procesa los estados de cuenta");
                var numeroProcesados = _factory.ServicioDocumentos.EstadosCuentaMensualProcesa(sol);
                var countProcesos = ParametroStringInt("numeroProcesar");

                gestorLog.Registrar(Nivel.Information, "Se insertaron: " + numeroProcesados + " en la bitacora");

                var hora = DateTimeOffset.Now.Hour;


                var horaInicia = ParametroStringInt("horaInicio");
                var horaFin = ParametroStringInt("horaFin");

                int diaInicio = ParametroStringInt("diaInicio");
                int diaFin = ParametroStringInt("diaFin");
                var dia = DateTimeOffset.Now.Day;
                var flagWhile = true;

                while (hora >= horaInicia && hora < horaFin && dia >= diaInicio && dia <= diaFin && flagWhile)
                {
                    EstadosCuentaMensualProcSol solObtiene = new EstadosCuentaMensualProcSol
                    {
                        Empresa = empresa,
                        Fecha = mesAnterior,
                        Elementos = countProcesos
                    };

                    gestorLog.Registrar(Nivel.Information, "Obtiene de la tabla de estados de cuenta");
                    var listaCuentas = _factory.ServicioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuenta(empresa, mesAnterior, countProcesos);
                    gestorLog.Registrar(Nivel.Information, "Se van a procesar: " + listaCuentas.Count());
                    foreach (var item in listaCuentas)
                    {
                        EstadoCuentaMensualSol solEnvio = new EstadoCuentaMensualSol
                        {
                            NumeroCliente = item.NumeroCliente,
                            NumeroCuenta = item.NumeroCuenta,
                            Proceso = proc,
                            SubProceso = subProc,
                            Empresa = empresa,
                            Fecha = mesAnterior
                        };
                        gestorLog.Registrar(Nivel.Information, "Cuenta: " + item.NumeroCuenta + ", Cliente: " + item.NumeroCliente);
                        try
                        {

                            _ = _factory.ServicioDocumentos.EstadoCuentaMensualEnviaAsync(solEnvio);
                        }
                        catch (Exception ex)
                        {
                            gestorLog.RegistrarError(ex);
                        }

                    }
                     hora = DateTimeOffset.Now.Hour;
                    dia = DateTimeOffset.Now.Day;
                    if (listaCuentas.Count() == 0)
                    {
                        flagWhile = false;
                        break;
                    }
                    else
                    {

                    var minutos = ParametroStringInt("minutosEspera");
                    Task.Delay(minutos * 60 * 1000).Wait();
                    }

                    
                }

                banderaCorre = true;
                return 1;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
            }
            return 1;
        }

        public void LogSimple(string mensaje)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, "logsimple.txt");
            string[] start = { DateTime.Now + ": " + mensaje };
            File.AppendAllLines(path, start);
        }
        //public void chartPrueba()
        //{

        //    Thread thread = new Thread(() =>
        //    {
        //        try
        //        {
        //            Func<ChartPoint, string> labelPoint = chartPoint =>
        //                    string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
        //            var myChart = new LiveCharts.Wpf.PieChart
        //            {
        //                DisableAnimations = true,
        //                Width = 1000,
        //                Height = 1000,
        //                Series = new SeriesCollection
        //    {
        //       new PieSeries
        //             {
        //                 Title = "Maria",
        //                 Values = new ChartValues<double> {3},
        //                 PushOut = 15,
        //                 DataLabels = true,
        //                 LabelPoint = labelPoint
        //             },
        //             new PieSeries
        //             {
        //                 Title = "Charles",
        //                 Values = new ChartValues<double> {4},
        //                 DataLabels = true,
        //                 LabelPoint = labelPoint
        //             },
        //    }
        //            };
        //            myChart.LegendLocation = LegendLocation.Bottom;
        //            var viewbox = new Viewbox();
        //            viewbox.Child = myChart;
        //            viewbox.Measure(myChart.RenderSize);
        //            viewbox.Arrange(new Rect(new System.Windows.Point(0, 0), myChart.RenderSize));
        //            myChart.Update(true, true); //force chart redraw
        //            viewbox.UpdateLayout();
        //            var encoder = new PngBitmapEncoder();
        //            var bitmap = new RenderTargetBitmap((int)myChart.ActualWidth, (int)myChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        //            bitmap.Render(myChart);
        //            var frame = BitmapFrame.Create(bitmap);
        //            encoder.Frames.Add(frame);
        //            var path = Path.Combine(Directory.GetCurrentDirectory(), @"img", "p.png");
        //            using (var stream = File.Create(path)) encoder.Save(stream);

        //        }
        //        catch (Exception ex)
        //        {

        //            throw;
        //        }

        //    });
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();


        //    /*  
        //    */
        //    /*  Chart chart1 = new Chart();
        //      chart1.Series.Clear();
        //      chart1.Legends.Clear();

        //      //Add a new Legend(if needed) and do some formating
        //      chart1.Legends.Add("MyLegend");
        //      chart1.Legends[0].LegendStyle = LegendStyle.Table;
        //      chart1.Legends[0].Docking = Docking.Bottom;
        //      chart1.Legends[0].Alignment = StringAlignment.Center;
        //      chart1.Legends[0].Title = "MyTitle";
        //      chart1.Legends[0].BorderColor = Color.Black;

        //      //Add a new chart-series
        //      string seriesname = "MySeriesName";
        //      chart1.Series.Add(seriesname);
        //      //set the chart-type to "Pie"
        //      chart1.Series[seriesname].ChartType = SeriesChartType.Pie;

        //      //Add some datapoints so the series. in this case you can pass the values to this method
        //      chart1.Series[seriesname].Points.AddXY("MyPointName", 1);
        //      chart1.Series[seriesname].Points.AddXY("MyPointName1", 2);
        //      chart1.Series[seriesname].Points.AddXY("MyPointName2", 3);
        //      chart1.Series[seriesname].Points.AddXY("MyPointName3", 4);
        //      chart1.Series[seriesname].Points.AddXY("MyPointName4", 5);

        //      var path = Path.Combine(Directory.GetCurrentDirectory(), @"img/p.png");
        //      chart1.(path, ChartImageFormat.Png);*/
        //}

    }
}
