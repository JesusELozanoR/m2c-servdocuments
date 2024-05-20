using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de grupos
    /// </summary>
    public interface IRepositorioGruposApi
    {
        /// <summary>
        /// Obtener grupo en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del grupo</param>
        /// <returns>Informacion del grupo</returns>
        Task<GrupoDto> GetById(string id);
        /// <summary>
        /// Obtener grupo en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del grupo</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Informacion del grupo</returns>
        Task<GrupoDto> GetById(string id, DetailsLevel detailsLevel);
    }
}
