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
   public class ServicioPlantillaTipoSubProceso : ServicioBase, IServicioPlantillaTipoSubProceso
    {
        public ServicioPlantillaTipoSubProceso(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }

        public int Agrega(PlantillaTipoSubProcesoInsDto campotipo)
        {
            return UnitOfWork.RepositorioPlantillaTipoSubProceso.Agrega(campotipo);
        }

        public bool Modificar(PlantillaTipoSubProcesoUpdDto campotipo)
        {
            return UnitOfWork.RepositorioPlantillaTipoSubProceso.Modificar(campotipo);
        }

        public IEnumerable<ResultadoPlantillaTipoSubProceso> Obtener(PlantillaTipoSubProcesoGetDto campotipo)
        {
            return UnitOfWork.RepositorioPlantillaTipoSubProceso.Obtener(campotipo);
        }

        public bool Eliminar(PlantillaTipoSubProcesoDelDto campotipo)
        {
            return UnitOfWork.RepositorioPlantillaTipoSubProceso.Eliminar(campotipo);
        }
    }
}
