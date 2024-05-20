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
    public class ProcesoUpdDto
    {
        /// <summary>
        /// id del proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Nombre de la Empresa
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [EnumDataType(typeof(EmpresaSel), ErrorMessageResourceName = nameof(MensajesDataAnnotations.Empresa), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Empresa")]
        public EmpresaSel Empresa { get; set; }

        /// <summary>
        /// Nombre del Proceso
        /// </summary>
        [JsonProperty("ProcesoNombre")]
        public string ProcesoNombre { get; set; }

        /// <summary>
        /// Descripcion del Proceso
        /// </summary>
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Id de la Clasificacion
        /// </summary>
        [JsonProperty("ClasificacionId")]
        public int ClasificacionId { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("Usuario")]
        public string Usuario { get; set; }
        
    }
}
