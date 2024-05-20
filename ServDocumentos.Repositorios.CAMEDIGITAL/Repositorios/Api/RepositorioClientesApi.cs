using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Repositorios.Comun.Repositorios.MambuApi;

namespace ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.Api
{
    /// <summary>
    /// Respositorio de clientes CAME
    /// </summary>
    public class RepositorioClientesApi : RepositorioApiClientesBase
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
        public RepositorioClientesApi(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
            : base(gestorLog, gestorPeticiones, mambuOptions)
        {
        }
    }
}
