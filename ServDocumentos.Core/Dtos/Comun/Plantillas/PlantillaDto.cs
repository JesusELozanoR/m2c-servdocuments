using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Plantillas
{
    public class PlantillaDto
    {
        /// <summary>
        /// id de la Plantilla
        /// </summary>
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        /// <summary>
        /// Nombre de la Plantilla
        /// </summary>
        [JsonProperty("PlantillaNombre")]
        public string PlantillaNombre { get; set; }

        /// <summary>
        /// Descripción de la Plantilla
        /// </summary>
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Version de la Plantilla
        /// </summary>
        [JsonProperty("Version")]
        public string Version { get; set; }

        /// <summary>
        /// Descripción Documento de la Plantilla
        /// </summary>
        [JsonProperty("DescripcionDocumentos")]
        public string DescripcionDocumentos { get; set; }

        /// <summary>
        /// id de la plantilla en repositorio de Alfresco
        /// </summary>
        [JsonProperty("AlfrescoId")]
        public string AlfrescoId { get; set; }

        /// <summary>
        /// Url para accesar a la plantilla de Alfresco
        /// </summary>
        [JsonProperty("AlfrescoURL")]
        public string AlfrescoURL { get; set; }

        /// <summary>
        /// Estado de la Plantilla
        /// </summary>
        [JsonProperty("Estado")]
        bool  Estado { get; set; }


    }
}
