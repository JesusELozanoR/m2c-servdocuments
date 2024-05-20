using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de ahorros
    /// </summary>
    public interface IRepositorioAhorrosApi
    {
        /// <summary>
        /// Obtiene una coleccion de ahorros
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Lista de cuentas de ahorro</returns>
        Task<IEnumerable<AhorroDto>> Search(SearchCriteria searchCriteria);
        /// <summary>
        /// Obtiene la informacion de un ahorro en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la cuenta</param>
        /// <returns>Cuenta de ahorro</returns>
        Task<AhorroDto> GetById(string id);
        /// <summary>
        /// Obtiene la informacion de un ahorro en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key de la cuenta</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Cuenta de ahorro</returns>
        Task<AhorroDto> GetById(string id, DetailsLevel detailsLevel);

    }
}
