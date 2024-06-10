using cmn.core.GestorRepositorioDocumentos;
using cmn.core.InspectorMensajes;
using cmn.std.Binarios;
using cmn.std.Log;
using cmn.std.Parametros;
using cmn.std.Test.App.ServiceExtension;
using Comun.Peticiones.Extensores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using ServDocumentos.API.Extensores;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using ServDocumentos.Core.Contratos.Workers;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Servicios.Comun.Factories;
using ServDocumentos.ServiciosProgramados.TimedHosted;
using ServDocumentos.ServiciosProgramados.Workers;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using sql = ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using sqlCame = ServDocumentos.Core.Contratos.Factories.CAME.SQL;
using sybase = ServDocumentos.Core.Contratos.Factories.TCR.Sybase;

namespace ServDocumentos.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
         {
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // services.AddSingleton<IConfiguration>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); services.AddHttpContextAccessor();
            services.AddTransient<ServDocumentos.Core.Contratos.Factories.Comun.IServiceFactory, ServDocumentos.Servicios.Comun.ServiceFactory>();
            //services.AddTransient<IServiceFactory, ServiceFactory>();


            services.AddSingleton<GestorParametros, GestorParametros>(serviceProvider =>
            {
                return new GestorParametros(Configuration.GetConnectionString("BDControl"),
                                            "GestionDocumentos");
            });

            services.AddTransient<GestorDocumentos, GestorDocumentos>();

            services.AddTransient<GestorLog, GestorLog>();
            services.AddTransient<GestorBinarios>(s => new GestorBinarios(AccesoTipo.DOTCMIS));
            services.AddTransient<sybase.IUnitOfWork, ServDocumentos.Repositorios.TCR.UnitOfWorkSybase>();
            services.AddTransient<sql.IUnitOfWork, ServDocumentos.Repositorios.TCR.UnitOfWorkSQL>();
            services.AddTransient<sql.IUnitOfWorkCore, ServDocumentos.Repositorios.TCR.UnitOfWorkSQLCore>();
            services.AddTransient<sql.IUnitOfWorkMambu, ServDocumentos.Repositorios.TCR.UnitOfWorkSQLMambu>();
            services.AddTransient<IFactService, ServDocumentos.Repositorios.TCR.FactoryReferenciaPago>();
            services.AddTransient<sqlCame.IUnitOfWork, ServDocumentos.Repositorios.CAMEDIGITAL.UnitOfWorkSQL>();

            services.AddTransient<sqlCame.IUnitOfWorkMambu, ServDocumentos.Repositorios.CAMEDIGITAL.UnitOfWorkSQLMambu>();
            services.AddTransient<comun.IUnitOfWork, ServDocumentos.Repositorios.Comun.UnitOfWork>();
            services.AddTransient<comun.IServiceFactoryComun, ServDocumentos.Servicios.TSI.ServiceFactory>();
            services.AddTransient<comun.IServiceFactoryComun, ServDocumentos.Servicios.TCR.ServiceFactory>();
            services.AddTransient<comun.IServiceFactoryComun, ServDocumentos.Servicios.CAMEDIGITAL.ServiceFactory>();

            services.AddTransient<ServDocumentos.Servicios.TSI.ServiceFactory>();
            services.AddTransient<ServDocumentos.Servicios.TCR.ServiceFactory>();
            services.AddTransient<ServDocumentos.Servicios.Comun.ServiceFactory>();
            services.AddTransient<ServDocumentos.Servicios.CAMEDIGITAL.ServiceFactory>();

            

            services.AddTransient<Func<string, comun.IServiceFactoryComun>>(serviceProvider => key =>
            {
                string Empresa = key == "" ? Configuration.GetValue<string>("Instancia") : key;
                return Empresa switch
                {
                    "TCR" => serviceProvider.GetService<ServDocumentos.Servicios.TCR.ServiceFactory>(),
                    "TSI" => serviceProvider.GetService<ServDocumentos.Servicios.TSI.ServiceFactory>(),
                    "CAME" => serviceProvider.GetService<ServDocumentos.Servicios.CAMEDIGITAL.ServiceFactory>(),
                    _ => throw new Exception("Proceso invalido")
                };

                //"" => serviceProvider.GetService<ServDocumentos.Servicios.AhorroCAME.ServiceFactory>(),
                //    "" => serviceProvider.GetService<ServDocumentos.Servicios.AhorroTCR.ServiceFactory>(),
                //    "" => serviceProvider.GetService<ServDocumentos.Servicios.Invercarmex.ServiceFactory>(),
                //    "" => throw new Exception("Proceso invalido"),



            });

            services.AddAutoMapper(typeof(Program));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                            .Select(e => e.Value.Errors.First().ErrorMessage).ToArray();
                    MensajeErrorFuncionalDto MensajeError = new MensajeErrorFuncionalDto()
                    {
                        Origen = "Servicio " + actionContext.RouteData.Values["controller"].ToString(),
                        Mensajes = errors
                    };

                    return new ConflictObjectResult(MensajeError);
                };
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Servicio Documentos" ,
                    Description = "Servicio Documentos CAME",
                    //Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "Talking Dotnet", Email = "contact@talkingdotnet.com" }
                });
                c.ExampleFilters();

                var CurrentPatch = Environment.CurrentDirectory;
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var baseDir = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(CurrentPatch, xmlFile);
                c.IncludeXmlComments(xmlPath);

                //INCLUYE LOS COMENTARIOS DE LOS MODELOS QUE SE ENCUENTRAN EN EL PROYECTO CORE                                
                var ArchivoComentariosModelos = Path.Combine(CurrentPatch, "ServDocumentos.Core.xml");
                c.IncludeXmlComments(ArchivoComentariosModelos);
            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            services.AddTransient<IWorkerDia, WorkerDia>();
            services.AddTransient<IWorkerHora, WorkerHora>();
            services.AddSingleton<IHostedService, TimedHostedService>();
            //services.AddHostedService<TimedHostedService>();

            // Factory del API de MAMBU
            services.AddTransient<IMambuRepositoryFactory, MambuRepositoryFactory>();
            services.AddGestorPeticiones(Configuration);
            services.AgregarConfiguraciones(Configuration);

            services.AddOptionsTestApp(options =>
            {
                options.NombreAplicativoPrincipal = "Servicio Documentos";
                options.LibreriasAmostrar = new string[] { "cmn.std.Test.App.dll",
                "cmn.std.Log.dll",
                "cmn.core.InspectorMensajes.dll"};
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMiddleware<InspectorMensajesMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

#if DEBUG

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Values Api V1");
                c.RoutePrefix = "swagger";
            });
#else

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/ServicioDocumentos/swagger/v1/swagger.json", "Values Api V1");
                c.RoutePrefix = "swagger";
            });

#endif

        }
    }
}
