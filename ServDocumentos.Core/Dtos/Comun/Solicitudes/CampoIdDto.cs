﻿using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoIdDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }
    }
}
