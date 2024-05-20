using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de transacciones de ahorro
    /// </summary>
    public interface IRepositorioTransaccionesAhorroApi
    {
        /// <summary>
        /// Obtener todas las transacciones de una cuenta de ahorro
        /// </summary>
        /// <param name="idCuenta">Id de la cuenta</param>
        /// <returns>Coleccion de transacciones</returns>
        Task<IEnumerable<TransaccionAhorroDto>> GetAll(string idCuenta);
        /// <summary>
        /// Obtener transacciones de ahorro por filtro
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Coleccion de transacciones</returns>
        Task<IEnumerable<TransaccionAhorroDto>> Search(SearchCriteria searchCriteria);

        /// <summary>
        /// Obtener transacciones de ahorro por filtro
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Coleccion de transacciones</returns>
        Task<IEnumerable<TransaccionAhorroDetalleDto>> SearchTransacctions(SearchCriteria searchCriteria);
        
    }
}
