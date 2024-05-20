using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Repositorios.Comun.Repositorios.MambuApi;

namespace ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.Api
{
    /// <summary>
    /// Respositorio de productos de credito CAME
    /// </summary>
    public class RepositorioProductosCreditoApi : RepositorioApiProductosCreditoBase
    {
        /// <summary>
        /// Core del repositorio
        /// </summary>
        public override Empresa Core { get; } = Empresa.CAME;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public RepositorioProductosCreditoApi(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
            : base(gestorLog, gestorPeticiones, mambuOptions)
        {
        }
    }
}
