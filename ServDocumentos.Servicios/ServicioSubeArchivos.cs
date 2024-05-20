using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Entidades.Comun;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioSubeArchivos : ServicioBase, IServicioSubeArchivos
    {

        string UrlBase = string.Empty;
        string NombreCarpeta = string.Empty;

        public ServicioSubeArchivos(GestorLog gestorLog, IUnitOfWork unitOfWork, IConfiguration configuration, Func<string, IServiceFactoryComun> serviceProvider, IServiceFactory factory) : base(gestorLog, unitOfWork, configuration, serviceProvider, factory)
        {
        }

        public RespuestaBinarios SubeArchivos(MemoryStream Archivo, string nombreArchivo)
        {
            gestorLog.Entrar();
            gestorLog.Registrar(Nivel.Information, "Alfresco ya no se encuentra disponible");
            RespuestaBinarios respuestaBinarios = new RespuestaBinarios();
            gestorLog.Salir();
            return respuestaBinarios;
        }


        private void DatoBinarios()
        {
            gestorLog.Entrar();

            var DatosBinarios = configuration.GetSection("DatosBinarios");
            UrlBase = DatosBinarios["UrlBase"].ToString();
            NombreCarpeta = DatosBinarios["NombreCarpeta"];

            gestorLog.Salir();
        }
    }
}
