using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de transacciones de credito
    /// </summary>
    public interface IRepositorioTransaccionesCreditoApi
    {
        /// <summary>
        /// Obtener transacciones de credito por filtro
        /// </summary>
        /// <param name="searchCriteria">Criterios de busqueda</param>
        /// <returns>Coleccion de transacciones</returns>
        Task<IEnumerable<TransaccionCreditoDto>> Search(SearchCriteria searchCriteria);
    }
}
