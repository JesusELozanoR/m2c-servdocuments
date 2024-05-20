using ServDocumentos.Core.Entidades.Comun;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioDatosJson
    {
        void Insertar(DatosJson datosJson);
        DatosJson Obtener(string credito);
        void Eliminar(DatosJson datosJson);
        int MigrarDatosJsonBase64();
    }
}
