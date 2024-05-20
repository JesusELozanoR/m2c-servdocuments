using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Comun
{
    public interface IHTTPClientWrapper<T>
    {
        Task<T> Get(string url);
        Task<T> Get(string url, string user, string password);
    }
}
