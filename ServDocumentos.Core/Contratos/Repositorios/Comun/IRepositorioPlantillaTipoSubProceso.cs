using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
   public interface IRepositorioPlantillaTipoSubProceso
    {
        int Agrega(PlantillaTipoSubProcesoInsDto campo);

        bool Modificar(PlantillaTipoSubProcesoUpdDto campo);

        bool Eliminar(PlantillaTipoSubProcesoDelDto campo);

        public IEnumerable<ResultadoPlantillaTipoSubProceso> Obtener(PlantillaTipoSubProcesoGetDto campo);

    }
}
