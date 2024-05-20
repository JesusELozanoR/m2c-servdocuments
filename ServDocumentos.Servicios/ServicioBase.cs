using cmn.std.Binarios;
using cmn.std.Log;
using cmn.std.Parametros;
//using cmn.std.Utilitarios.Compresion;
using Microsoft.Extensions.Configuration;
using System;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;

namespace ServDocumentos.Servicios.Comun
{
    public abstract class ServicioBase
    {
        protected readonly GestorLog gestorLog;
        //protected readonly GestorZip gestorZip;
        protected readonly comun.IServiceFactory factory;
        protected readonly GestorParametros gestorParametros;
        protected readonly Func<string, comun.IServiceFactoryComun> serviceProvider;
        protected readonly IConfiguration configuration;
        protected readonly GestorBinarios gestorBinarios;

        protected comun.IUnitOfWork UnitOfWork { get; private set; }

        protected ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
        }
        protected ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork, GestorBinarios gestorBinarios)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
            this.gestorBinarios = gestorBinarios;
        }

        protected ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork, Func<string, comun.IServiceFactoryComun> serviceProvider)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
            this.serviceProvider = serviceProvider;
        }
        protected ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork, Func<string, comun.IServiceFactoryComun> serviceProvider, comun.IServiceFactory factory)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
            this.serviceProvider = serviceProvider;
            this.factory = factory;
        }

        public ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork, IConfiguration configuration, Func<string, comun.IServiceFactoryComun> serviceProvider, comun.IServiceFactory factory)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.factory = factory;
        }
        public ServicioBase(GestorLog gestorLog, comun.IUnitOfWork unitOfWork, IConfiguration configuration, Func<string, comun.IServiceFactoryComun> serviceProvider, comun.IServiceFactory factory, GestorParametros gestorParametros)
        {
            this.gestorLog = gestorLog;
            UnitOfWork = unitOfWork;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.factory = factory;
            this.gestorParametros = gestorParametros;
        }
    }
}
