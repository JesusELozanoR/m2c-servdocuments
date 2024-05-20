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
    public class ServicioPlantillasCampos : ServicioBase, IServicioPlantillasCampos
    {
        public ServicioPlantillasCampos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        { }

        public List<int> Agrega(PlantillaCampoInsDto plantillacampo)
        {
            List<int> procesoErrId = UnitOfWork.RepositorioPlantillasCampos.Agrega(plantillacampo);
            return procesoErrId;
        }
        public bool Elimina(PlantillaCampoIdDto plantillacampo)
        {
            return UnitOfWork.RepositorioPlantillasCampos.Elimina(plantillacampo);
        }
        public bool EliminaxPlantilla(PlantillaIdDto plantilla)
        {
            return UnitOfWork.RepositorioPlantillasCampos.EliminaxPlantilla(plantilla);
        }
        public bool EliminaxCampo(PlantillaCampoIdcDto plantillacampo)
        {
            return UnitOfWork.RepositorioPlantillasCampos.EliminaxCampo(plantillacampo);
        }
        public IEnumerable<ResultadoPlantillaCampo> Obtiene(PlantillaCampoGetDto plantillacampo)
        {
            return UnitOfWork.RepositorioPlantillasCampos.Obtiene(plantillacampo);
        }
        public bool Modifica(PlantillaCampoUpdDto plantillacampo)
        {
            return UnitOfWork.RepositorioPlantillasCampos.Modifica(plantillacampo);
        }       
       public IEnumerable<ResultadoPlantillaCampo> ObtienexProceso(PlantillaCampoPSGet plantillacampo)
        {
            return UnitOfWork.RepositorioPlantillasCampos.ObtienexProceso(plantillacampo);
        }

    }
}
