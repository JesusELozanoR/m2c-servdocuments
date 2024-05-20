
using ServDocumentos.Core.Dtos.DatosExpress;
using System;

namespace ServDocumentos.Core.Contratos.Repositorios.TCR
{
    public interface IRepositorioDatosPlantillasExpress
    {
        DatosSolicitudCredito ObtenerDatosPlantilla(string numeroCredito);
    }
}
