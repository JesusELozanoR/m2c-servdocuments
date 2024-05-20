using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioSubProcesosPlantillas
    {
        public Dictionary<int, string> Agrega(SubProcesoPlantillaInsDto subprocesoplantilla);
        public bool Elimina(SubprocesoPlantillaDelDto subprocesoplantilla);
        public bool EliminaxSubprocesoId(SubProcesoIdDto subproceso);
        public IEnumerable<ResultadoSubProcesoPlantilla> Obtiene(SubprocesoPlantillaGetDto subprocesoplantilla);
        public bool Modifica(SubProcesoPlantillaUpdDto subprocesoplantilla);
    }
}
