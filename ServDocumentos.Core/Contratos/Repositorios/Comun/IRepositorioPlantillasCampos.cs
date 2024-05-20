using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioPlantillasCampos
    {
        List<int> Agrega(PlantillaCampoInsDto plantillacampo);
        public bool Elimina(PlantillaCampoIdDto plantillacampo);
        public bool EliminaxPlantilla(PlantillaIdDto plantilla);
        public bool EliminaxCampo(PlantillaCampoIdcDto plantillacampo);
        public IEnumerable<ResultadoPlantillaCampo> Obtiene(PlantillaCampoGetDto plantillacampo);
        bool Modifica(PlantillaCampoUpdDto plantillacampo);
        public IEnumerable<ResultadoPlantillaCampo> ObtienexProceso(PlantillaCampoPSGet plantillacampo);
    }
}
