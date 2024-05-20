﻿using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class GeneralesUpdDto
    {
        /// <summary>
        /// Id del General
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("GeneralId")]
        public int GeneralId { get; set; }
        
        /// <summary>
        /// Id del Proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        // [EnumDataType(typeof(TipoProcesos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

        /// <summary>
        /// Valor del Campo
        /// </summary>
        [JsonProperty("Valor")]
        public string Valor { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }
    }
}
