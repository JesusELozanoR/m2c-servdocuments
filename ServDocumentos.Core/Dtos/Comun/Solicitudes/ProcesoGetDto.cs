using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ProcesoGetDto
    {
        /// <summary>
        /// Nombre de la Empresa
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [EnumDataType(typeof(Empresa), ErrorMessageResourceName = nameof(MensajesDataAnnotations.Empresa), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Empresa")]
        public Empresa Empresa { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Nombre del Proceso
        /// </summary>
        [JsonProperty("ProcesoNombre")]
        public string ProcesoNombre { get; set; }

        /// <summary>
        /// Id de la Clasificacion
        /// </summary>
        [JsonProperty("ClasificacionId")]
        public int ClasificacionId { get; set; }

        /// <summary>
        /// Nombre de la Clasificacion
        /// </summary>
        [JsonProperty("ClasificacionNombre")]
        public string ClasificacionNombre { get; set; }


    }
}
