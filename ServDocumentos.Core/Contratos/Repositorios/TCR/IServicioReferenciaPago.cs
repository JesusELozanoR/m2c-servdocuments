using ServDocumentos.Core.Dtos.DatosTcrCaja;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.TCR
{
    public interface IServicioReferenciaPago
    {
        Task<RespuestaReferencia> ObtieneReferenciaPagoAsync(string numCredito, string usuario);
    }
    
}
