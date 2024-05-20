using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.DatosCore;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class ObtenerDatosJsonDatosDto : SolicitudBaseDto
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
        [JsonProperty("jsonSolicitudData")]
        public ExpandoObject JsonSolicitudData { get; set; }
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        public DatosSolicitudGat Gat { get; set; }
    }
}
