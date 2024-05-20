using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioPlantillas
    {
        public List<ArchivoPlantillaDto> ObtenerPorSubProceso(ObtenerPlantillasProcesoDto plantillasProceso);
        public List<PlantillaDto> ObtenerListadoPorSubProceso(ProcesoSubDto ProcesoSub);
        public int Agrega(PlantillaInsDto plantilla);
        public bool EliminaxNombre(PlantillaNombreDto plantilla);
        public bool Elimina(PlantillaIdDto plantilla);
        public IEnumerable<PlantillaDto> Obtiene(PlantillaGetDto plantilla);
        public bool Modifica(PlantillaUpdDto plantilla);
        public int Registra(PlantillaRegDto plantilla);
         public ArchivoPlantillaDto  ObtenerPorId(int plantillaId);
        public ArchivoPlantillaDto ObtenerPorNombre(string plantillaNombre);
        int MigrarPlantillasBase64();
    }
}
