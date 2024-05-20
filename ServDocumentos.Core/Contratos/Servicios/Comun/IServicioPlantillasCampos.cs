using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioPlantillasCampos
    {
        public List<int> Agrega(PlantillaCampoInsDto plantillacampo);
        public bool Elimina(PlantillaCampoIdDto plantillacampo); 
        public bool EliminaxPlantilla(PlantillaIdDto plantilla);
        public bool EliminaxCampo(PlantillaCampoIdcDto plantillacampo);
        public IEnumerable<ResultadoPlantillaCampo> Obtiene(PlantillaCampoGetDto plantillacampo);
        public bool Modifica(PlantillaCampoUpdDto plantillacampo);
        public IEnumerable<ResultadoPlantillaCampo> ObtienexProceso(PlantillaCampoPSGet plantillacampo);
    }
}
