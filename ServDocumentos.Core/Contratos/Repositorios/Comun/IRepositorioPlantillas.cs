using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Entidades.Comun;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioPlantillas
    {
        IEnumerable<Plantillas> ObtenerPorSubProceso(ObtenerPlantillasProcesoDto plantillasProceso);
        IEnumerable<PlantillaDto> ObtenerListadoPorSubProceso(ProcesoSubDto ProcesoSub);
        DocData DocumentoDatosPorSubProceso(SolictudDocumentoDto solicitud);
        int Agrega(PlantillaInsDto plantilla, PlantillaArchivo archivo);
        bool EliminaxNombre(PlantillaNombreDto plantilla);
        bool Elimina(PlantillaIdDto plantilla);
        IEnumerable<PlantillaDto> Obtiene(PlantillaGetDto plantilla);
        bool Modifica(PlantillaUpdDto plantilla);
        int Registra(PlantillaRegDto plantilla, PlantillaArchivo archivo);
        Plantillas ObtenerPorId(int plantillaId);
        Plantillas ObtenerPorNombre(string plantillaNombre);
        IEnumerable<string> ObtenerListadoIdSinBase64();
        int ActualizarMigracionBase64(string base64, string idAlfresco);
        IEnumerable<Bancos> ObtieneBancos();
    }
}
