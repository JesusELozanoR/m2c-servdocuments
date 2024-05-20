using ServDocumentos.Core.Dtos.DatosMambu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de productos
    /// </summary>
    public interface IRepositorioCanalesTransaccionApi
    {
        /// <summary>
        /// Obtener todos los canales de transaccion
        /// </summary>
        /// <returns>Coleccion de canales de transaccion</returns>
        Task<IEnumerable<CanalTransaccionDto>> GetAll();
    }
}
