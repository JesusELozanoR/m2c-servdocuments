using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServDocumentos.Core.Configuraciones;

namespace ServDocumentos.API.Extensores
{
    /// <summary>
    /// Extension para <see cref="IServiceCollection"/>
    /// </summary>
    public static class ConfiguracionExtension
    {
        /// <summary>
        /// Nombre de la configuracion de CAME mambu
        /// </summary>
        private const string NombreCAMEConfiguracion = "MambuCameAPI";
        /// <summary>
        /// Nombre de la configuracion de TCR mambu
        /// </summary>
        private const string NombreTCRConfiguracion = "MambuApiTCR";
        /// <summary>
        /// Agregar configuracion de MAMBU
        /// </summary>
        /// <param name="services">Coleccion de servicios de la applicacion</param>
        /// <param name="configuration">Configuraciones</param>
        /// <returns>Coleccion de servicio</returns>
        public static IServiceCollection AgregarConfiguraciones(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MambuConfig>(Core.Enumeradores.Empresa.CAME.ToString(), configuration.GetSection(NombreCAMEConfiguracion));
            services.Configure<MambuConfig>(Core.Enumeradores.Empresa.TCR.ToString(), configuration.GetSection(NombreTCRConfiguracion));
            return services;
        }
    }
}
