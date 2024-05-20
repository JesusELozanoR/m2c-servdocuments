using ServDocumentos.Core.Entidades.Comun;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioDatosJson
    {
        void Insertar(DatosJson datosJson);
        DatosJson Obtener(string credito);
        void Eliminar(DatosJson datosJson);
        IEnumerable<string> ObtenerListadoIdSinBase64();
        int ActualizarMigracionBase64(string base64, string idAlfresco);
    }
}
