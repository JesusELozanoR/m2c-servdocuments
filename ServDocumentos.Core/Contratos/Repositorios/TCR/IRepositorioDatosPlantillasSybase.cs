using ServDocumentos.Core.Dtos.DatosSybase;
using ServDocumentos.Core.Dtos.DatosTcrCaja;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Repositorios.TCR
{
    public interface IRepositorioDatosPlantillasSybase
    {
        Cliente ObtenerDatosPlantilla(string numeroCredito, List<int> numeroClientes, List<int> numeroDividendos, RespuestaOrdenPago ordenPago);
        EstadoCuenta ObtenerDatosEstadoCuenta(string numeroCredito);
    }
}
