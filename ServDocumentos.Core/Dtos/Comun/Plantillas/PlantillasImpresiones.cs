using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Plantillas
{
    public class PlantillasImpresiones
    {
        /// <summary>
        /// id de la Plantilla
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Numero Impresion Plantilla
        /// </summary>
        [JsonProperty("NumeroImpresion")]
        public int NumeroImpresion { get; set; }
    }
}
