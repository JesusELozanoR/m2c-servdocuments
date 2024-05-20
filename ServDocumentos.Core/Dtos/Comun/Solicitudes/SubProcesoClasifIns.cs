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
    public class SubProcesoClasifIns
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
        /// Nombre del SubProceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("SubProcesoNombre")]
        public string SubProcesoNombre { get; set; }

        /// <summary>
        /// Descripcion del SubProceso
        /// </summary>
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Id de la Clasificacion
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ClasificacionId")]
        public int ClasificacionId { get; set; }
    }
}
