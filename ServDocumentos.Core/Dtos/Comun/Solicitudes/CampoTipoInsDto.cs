﻿using Newtonsoft.Json;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoTipoInsDto
    {
        /// <summary>
        /// Nombre del Tipo de Campo 
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("CampoTipo")]
        public string CampoTipo { get; set; }

        /// <summary>
        /// Nombre del Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }
    }
}
