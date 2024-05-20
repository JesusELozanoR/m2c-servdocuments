using ServDocumentos.Core.Dtos.DatosMambu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de productos de deposito
    /// </summary>
    public interface IRepositorioProductosAhorroApi
    {
        /// <summary>
        /// Consultar productos
        /// </summary>
        /// <returns>Coleccion de productos</returns>
        Task<IEnumerable<ProductoDto>> ObtenerProductos();
        /// <summary>
        /// Obtener producto en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del producto</param>
        /// <returns>Informacion del producto</returns>
        Task<ProductoDto> GetById(string id);
    }
}
