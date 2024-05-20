using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SubProcesoPlantilla
    {

        /// <summary>
        /// id de la Plantilla
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        /// <summary>
        /// RECA
        /// </summary>
        [JsonProperty("RECA")]
        public string RECA { get; set; }

        /// <summary>
        /// Tipo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }
    }
}
