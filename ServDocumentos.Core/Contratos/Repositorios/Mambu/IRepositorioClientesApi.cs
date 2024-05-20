using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de clientes
    /// </summary>
    public interface IRepositorioClientesApi
    {
        /// <summary>
        /// Obtener cliente en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del cliente</param>
        /// <returns>Informacion del cliente</returns>
        Task<Cliente> GetById(string id);
        /// <summary>
        /// Obtener cliente en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del cliente</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Informacion del cliente</returns>
        Task<Cliente> GetById(string id, DetailsLevel detailsLevel);
    }
}
