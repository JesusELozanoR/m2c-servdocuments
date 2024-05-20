using ServDocumentos.Core.Dtos.DatosTcrCaja;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.TCR
{
    public interface IServicioOrdenPago
    {
        Task<RespuestaOrdenPago> ObtieneDatosOrdenPagoAsync(string numCredito);
    }
}

