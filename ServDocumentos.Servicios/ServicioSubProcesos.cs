using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioSubProcesos : ServicioBase, IServicioSubProcesos
    {
        public ServicioSubProcesos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(SubProcesoDto subproceso)
        {
            int procesoId = UnitOfWork.RepositorioSubProcesos.Agrega(subproceso);
            return procesoId;
        }
        public bool EliminaxNombre(SubProcesoNombreDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesos.EliminaxNombre(subproceso);
        }
        public bool EliminaxId(SubProcesoIdDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesos.EliminaxId(subproceso);
        }
        public IEnumerable<SubProcesoc> Obtiene(SubProcesoGetDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesos.Obtiene(subproceso);
        }
        public bool Modifica(SubProcesoUpdDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesos.Modifica(subproceso);
        }

        public IEnumerable<SubProcesoc> ObtienexClasificacion(SubProcesoClasDto subproceso)
        {
            return UnitOfWork.RepositorioSubProcesos.ObtienexClasificacion(subproceso);
        }

    }
}
