using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.DatosCore;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolictudDocumentoJsonDatosDto : SolictudDocumentoBaseDto
    {
        /// <summary>
        /// Datos para ser llenados 
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("jsonSolicitudData")]
        public ExpandoObject JsonSolicitudData { get; set; }
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        public DatosSolicitudGat Gat { get; set; }
    }
}
