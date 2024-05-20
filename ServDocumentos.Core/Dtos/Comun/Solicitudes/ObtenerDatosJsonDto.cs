using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ObtenerDatosJsonDto : SolicitudBaseDto
    {
        /// <summary>
        /// Tipo de proceso a seguir, los valores permitodos son 1 y dos
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        // [EnumDataType(typeof(TipoProcesos), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProceso), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("proceso")]
        public int Proceso { get; set; }

        // <summary>
        // Empresa
        // </summary>
        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public string Empresa { get; set; }

        // <summary>
        // Empresa
        // </summary>
        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public string ProcesoNombre { get; set; }
        // <summary>
        // Empresa
        // </summary>
        [JsonIgnore]
        [HiddenInput(DisplayValue = false)]
        public string SubProcesoNombre { get; set; }
        /// <summary>
        /// Json Cliente engine
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("jsonSolicitud")]
        public string JsonSolicitud { get; set; }
    }
}
