using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaTipoSubProcesoGetDto
    {
        /// <summary>
        /// id del Tipo de Campo
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Descripcion del Tipo Campo
        /// </summary>
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Descripcion del Tipo Campo
        /// </summary>
        [JsonProperty("DescripcionCorta")]
        public string DescripcionCorta { get; set; }

    }
}
