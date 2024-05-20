using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de sucursales
    /// </summary>
    public interface IRepositorioSucursalesApi
    {
        /// <summary>
        /// Obtener sucursal en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la sucursal</param>
        /// <returns>Informacion de la sucursal</returns>
        Task<SucursalDto> GetById(string id);
        /// <summary>
        /// Obtener sucursal en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la sucursal</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Informacion de la sucursal</returns>
        Task<SucursalDto> GetById(string id, DetailsLevel detailsLevel);
    }
}
