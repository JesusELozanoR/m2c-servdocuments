using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioSubProcesosPlantillas : ServicioBase, IServicioSubProcesosPlantillas
    {
        public ServicioSubProcesosPlantillas(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }       
        public Dictionary<int, string> Agrega(SubProcesoPlantillaInsDto subprocesoplantilla)
        {
            Dictionary<int, string> procesoId = UnitOfWork.RepositorioSubProcesosPlantillas.Agrega(subprocesoplantilla);
            return procesoId;
        }
        public bool Elimina(SubprocesoPlantillaDelDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesosPlantillas.Elimina(subproceso);
        }
        public bool EliminaxSubprocesoId(SubProcesoIdDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesosPlantillas.EliminaxSubprocesoId(subproceso);
        }
        public IEnumerable<ResultadoSubProcesoPlantilla> Obtiene(SubprocesoPlantillaGetDto subprocesoplantilla)
        {
            return UnitOfWork.RepositorioSubProcesosPlantillas.Obtiene(subprocesoplantilla);
        }
        public bool Modifica(SubProcesoPlantillaUpdDto subprocesoplantilla)
        {
            return UnitOfWork.RepositorioSubProcesosPlantillas.Modifica(subprocesoplantilla);
        }
    }
}
