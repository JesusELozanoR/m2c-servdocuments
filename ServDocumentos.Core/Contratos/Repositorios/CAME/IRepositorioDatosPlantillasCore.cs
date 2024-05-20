using ServDocumentos.Core.Dtos.DatosCore;
using System.Collections.Generic;
using sybase = ServDocumentos.Core.Dtos.DatosSybase;

namespace ServDocumentos.Core.Contratos.Repositorios.CAME
{
    public interface IRepositorioDatosPlantillasCore
    {
        DatosSolicitudCredito ObtenerDatosPlantilla(string numeroCredito);

        List<sybase.Dividendo> ObtenerTablaAmortizacion(string numeroCredito);
        string ObtenerGrupoId(string numeroCredito);


    }
}
