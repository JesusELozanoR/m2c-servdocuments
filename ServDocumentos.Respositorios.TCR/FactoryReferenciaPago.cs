using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Repositorios.TCR.ServicioTcrCaja;
using System.ServiceModel;

namespace ServDocumentos.Repositorios.TCR
{
    public class FactoryReferenciaPago : IFactService
    {
        protected readonly IConfiguration configuration;
        public IServicioReferenciaPago servicioReferenciaPago = null;
        public FactoryReferenciaPago(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }
        public IServicioReferenciaPago ServicioReferenciaPago
        {
            get
            {
                var configuracionMambu = configuration.GetSection("ServicioTcrCaja");
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                basicHttpBinding.Security.Mode = BasicHttpSecurityMode.None;
                EndpointAddress endpointAddress = new EndpointAddress(configuracionMambu["EndpointUrl"]);
                return servicioReferenciaPago ?? (servicioReferenciaPago = new TcrCajaServiceClient(basicHttpBinding,endpointAddress));
            }
        }

       // protected readonly IConfiguration configuration;
        public IServicioOrdenPago servicioOrdenPago = null;
       
        public IServicioOrdenPago ServicioOrdenPago
        {
            get
            {
                var configuracionMambu = configuration.GetSection("ServicioTcrCaja");
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                basicHttpBinding.Security.Mode = BasicHttpSecurityMode.None;
                EndpointAddress endpointAddress = new EndpointAddress(configuracionMambu["EndpointUrl"]);
                return servicioOrdenPago ?? (servicioOrdenPago = new TcrCajaServiceClient(basicHttpBinding, endpointAddress));
            }
        }

    }


   
}
