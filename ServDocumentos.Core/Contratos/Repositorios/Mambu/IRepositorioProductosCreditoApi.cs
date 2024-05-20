using ServDocumentos.Core.Dtos.DatosMambu;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de productos de credito
    /// </summary>
    public interface IRepositorioProductosCreditoApi
    {
        /// <summary>
        /// Obtener producto en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del producto</param>
        /// <returns>Informacion del producto</returns>
        Task<ProductoDto> GetById(string id);
    }
}
