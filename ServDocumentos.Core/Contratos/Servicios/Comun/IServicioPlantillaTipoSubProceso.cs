using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
   public interface IServicioPlantillaTipoSubProceso
    {
        public int Agrega(PlantillaTipoSubProcesoInsDto campo);

        public bool Modificar(PlantillaTipoSubProcesoUpdDto campo);

        public bool Eliminar(PlantillaTipoSubProcesoDelDto campo);

        public IEnumerable<ResultadoPlantillaTipoSubProceso> Obtener(PlantillaTipoSubProcesoGetDto campo);
    }
}
