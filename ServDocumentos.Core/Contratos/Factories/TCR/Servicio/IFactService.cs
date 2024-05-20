using ServDocumentos.Core.Contratos.Repositorios.TCR;

namespace ServDocumentos.Core.Contratos.Factories.TCR.Servicio
{
    public interface IFactService
    {
        IServicioReferenciaPago ServicioReferenciaPago { get; }
        IServicioOrdenPago ServicioOrdenPago { get; }
    }

  
}
