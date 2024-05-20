using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoListadoPlantillas
    {
        /// <summary>
        /// Listado de Plantillas
        /// </summary>
        [JsonProperty("listaPlantillas")]
        public List<PlantillaDto> listaPlantillas { get; set; }

    }
}
