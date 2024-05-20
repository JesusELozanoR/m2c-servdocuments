using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Mambu
{
    /// <summary>
    /// Define los metodos del repositorio de cuentas de credito
    /// </summary>
    public interface IRepositorioCreditosApi
    {
        /// <summary>
        /// Obtener credito en base a su identificador
        /// </summary>
        /// <param name="id">Encoded key o Id del credito</param>
        /// <returns>Instancia de <see cref="Credito"/></returns>
        Task<Credito> GetById(string id);
        /// <summary>
        /// Obtener credito en base a su identificador
        /// </summary>
        /// <param name="id">Encoded key o Id del credito</param>
        /// <param name="detailsLevel">Nivel de detalle</param>
        /// <returns>Instancia de <see cref="Credito"/></returns>
        Task<Credito> GetById(string id, DetailsLevel detailsLevel);
        /// <summary>
        /// Obtener pagos de un credito
        /// </summary>
        /// <param name="accountId">Id o encoded key de la cuenta</param>
        /// <returns>Informacion de los pagos</returns>
        Task<PagosCameMambu> GetSchedules(string accountId);
    }
}
