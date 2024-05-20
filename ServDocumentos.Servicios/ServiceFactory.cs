using AutoMapper;
using cmn.core.GestorRepositorioDocumentos;
using cmn.std.Binarios;
using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using System;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;

namespace ServDocumentos.Servicios.Comun
{
    public class ServiceFactory : comun.IServiceFactory
    {
        private ServicioDatosJson servicioDatosJson = null;
        private ServicioProcesos servicioProcesos = null;
        private ServicioSubProcesos servicioSubProcesos = null;
        private ServicioSubProcesosPlantillas servicioSubProcesosPlantillas = null;
        private ServicioPlantillasCampos servicioPlantillasCampos = null;
        private ServicioCampos servicioCampos = null;
        private ServicioCamposTipos servicioCamposTipos = null;
        private ServicioPlantillas servicioPlantillas = null;
        private ServicioDocumentos servicioDocumentos = null;
        private ServicioDatosPlantillas servicioDatosPlantillas = null;
        private ServicioSubeArchivos servicioSubeArchivos = null;
        private ServicioGenerales servicioGenerales = null;
        private ServicioClasificaciones servicioClasificaciones = null;
        private ServicioProcesosCampos servicioProcesosCampos = null;
        private ServicioSubProcesosCampos servicioSubProcesosCampos = null;
        private ServicioPlantillaTipoSubProceso servicioPlantillaTipoSubProceso = null;
        private ServicioBitacoraEstadoCuenta servicioBitacoraEstadoCuenta = null;

        //private comun.IServiceFactory factory;
        private readonly comun.IUnitOfWork unitOfWork;
        private readonly GestorLog gestorLog = null;
        private readonly GestorBinarios gestorBinarios;
        private readonly Func<string, comun.IServiceFactoryComun> serviceProvider = null;
        private readonly GestorParametros gestorParametros;
        private readonly IMapper _mapper;
        private readonly IConfiguration configuration;
        private readonly GestorDocumentos _gestorDocumentos;
        private readonly IMambuRepositoryFactory _mambuFactory;
        public ServiceFactory(comun.IUnitOfWork unitOfWork, GestorLog gestorLog)
        {
            this.unitOfWork = unitOfWork;
            this.gestorLog = gestorLog;
        }
        public ServiceFactory(comun.IUnitOfWork unitOfWork, GestorLog gestorLog, Func<string, comun.IServiceFactoryComun> serviceProvider)
        {
            this.unitOfWork = unitOfWork;
            this.gestorLog = gestorLog;
            this.serviceProvider = serviceProvider;
        }

        public ServiceFactory(comun.IUnitOfWork unitOfWork, GestorLog gestorLog, IConfiguration configuration, Func<string, comun.IServiceFactoryComun> serviceProvider)
        {
            this.unitOfWork = unitOfWork;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }
        public ServiceFactory(
            comun.IUnitOfWork unitOfWork,
            GestorLog gestorLog,
            GestorBinarios gestorBinarios,
            IConfiguration configuration,
            Func<string, comun.IServiceFactoryComun> serviceProvider,
            GestorParametros gestorParametros,
            IMapper mapper,
            GestorDocumentos gestorDocumentos,
            IMambuRepositoryFactory mambuFactory)
        {
            this.unitOfWork = unitOfWork;
            this.gestorLog = gestorLog;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.gestorParametros = gestorParametros;
            this.gestorBinarios = gestorBinarios;
            _mapper = mapper;
            _gestorDocumentos = gestorDocumentos;
            _mambuFactory = mambuFactory;
        }
        //public ServiceFactory(comun.IUnitOfWork unitOfWork, GestorLog gestorLog, GestorBinarios gestorBinarios, Func<string, comun.IServiceFactoryComun> serviceProvider, comun.IServiceFactory factory)
        //{
        //    this.unitOfWork = unitOfWork;
        //    this.gestorLog = gestorLog;
        //    this.gestorBinarios = gestorBinarios;
        //    this.serviceProvider = serviceProvider;
        //    this.factory = factory;
        //}

        public IServicioDatosJson ServicioDatosJson => servicioDatosJson ??= new ServicioDatosJson(gestorLog, unitOfWork, gestorBinarios);
        public IServicioProcesos ServicioProcesos => servicioProcesos ??= new ServicioProcesos(gestorLog, unitOfWork);
        public IServicioSubProcesos ServicioSubProcesos => servicioSubProcesos ??= new ServicioSubProcesos(gestorLog, unitOfWork);
        public IServicioSubProcesosPlantillas ServicioSubProcesosPlantillas => servicioSubProcesosPlantillas ??= new ServicioSubProcesosPlantillas(gestorLog, unitOfWork);
        public IServicioPlantillasCampos ServicioPlantillasCampos => servicioPlantillasCampos ??= new ServicioPlantillasCampos(gestorLog, unitOfWork);
        public IServicioCampos ServicioCampos => servicioCampos ??= new ServicioCampos(gestorLog, unitOfWork);
        public IServicioCamposTipos ServicioCamposTipos => servicioCamposTipos ??= new ServicioCamposTipos(gestorLog, unitOfWork);
        public IServicioPlantillas ServicioPlantillas => servicioPlantillas ??= new ServicioPlantillas(gestorLog, unitOfWork, gestorBinarios);
        public IServicioDocumentos ServicioDocumentos => servicioDocumentos ??= new ServicioDocumentos(gestorLog, unitOfWork, configuration, serviceProvider, this, gestorParametros, _mapper, _gestorDocumentos, _mambuFactory);
        public IServicioDatosPlantillas ServicioDatosPlantillas => servicioDatosPlantillas ??= new ServicioDatosPlantillas(gestorLog, unitOfWork, configuration, serviceProvider, this);
        public IServicioSubeArchivos ServicioSubeArchivos => servicioSubeArchivos ??= new ServicioSubeArchivos(gestorLog, unitOfWork, configuration, serviceProvider, this);
        public IServicioGenerales ServicioGenerales => servicioGenerales ??= new ServicioGenerales(gestorLog, unitOfWork);
        public IServicioClasificaciones ServicioClasificaciones => servicioClasificaciones ??= new ServicioClasificaciones(gestorLog, unitOfWork);
        public IServicioProcesosCampos ServicioProcesosCampos => servicioProcesosCampos ??= new ServicioProcesosCampos(gestorLog, unitOfWork);
        public IServicioSubProcesosCampos ServicioSubProcesosCampos => servicioSubProcesosCampos ??= new ServicioSubProcesosCampos(gestorLog, unitOfWork);
        public IServicioPlantillaTipoSubProceso ServicioPlantillaTipoSubProceso => servicioPlantillaTipoSubProceso ??= new ServicioPlantillaTipoSubProceso(gestorLog, unitOfWork);
        public IServicioBitacoraEstadoCuenta ServicioBitacoraEstadoCuenta => servicioBitacoraEstadoCuenta ??= new ServicioBitacoraEstadoCuenta(gestorLog, unitOfWork);

    }
}
