﻿using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class PlantillaCampoGetDto
    {
        /// <summary>
        /// Id de la Plantilla 
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("PlantillaId")]
        public int PlantillaId { get; set; }

        /// <summary>
        /// Id del Campo
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }
    }
}
