using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class GeneralesGetDto
    {
        /// <summary>
        /// Id de los generales
        /// </summary>
        [JsonProperty("GeneralId")]
        public int GeneralId { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        // [EnumDataType(typeof(TipoProcesoGet), ErrorMessageResourceName = nameof(MensajesDataAnnotations.EnumTipoProcesoGet), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("ProcesoId")]
        public int ProcesoId { get; set; }

        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

       
    }
}
