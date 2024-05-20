using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Repositorios.Comun.Repositorios.MambuApi;

namespace ServDocumentos.Repositorios.TCR.Repositorios.Api
{
    /// <summary>
    /// Respositorio de transacciones de ahorro TCR
    /// </summary>
    public class RepositorioTransaccionesAhorroApi : RepositorioApiTransaccionesAhorroBase
    {
        /// <summary>
        /// Core del repositorio
        /// </summary>
        public override Empresa Core { get; } = Empresa.TCR;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public RepositorioTransaccionesAhorroApi(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
            : base(gestorLog, gestorPeticiones, mambuOptions)
        {
        }
    }
}
