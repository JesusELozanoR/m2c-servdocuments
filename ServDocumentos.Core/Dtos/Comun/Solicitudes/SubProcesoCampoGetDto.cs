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
    public class SubProcesoCampoGetDto
    {
        /// <summary>
        /// Id del Proceso Campo
        /// </summary>
        [JsonProperty("SubProcesoCampoId")]
        public int SubProcesoCampoId { get; set; }

        /// <summary>
        /// Id del Proceso
        /// </summary>
        [JsonProperty("SubProcesoId")]
        public int SubProcesoId { get; set; }

        /// <summary>
        /// Id del  Campo
        /// </summary>
        [JsonProperty("CampoId")]
        public int CampoId { get; set; }

        /// <summary>
        /// Nombre del Campo
        /// </summary>
        [JsonProperty("CampoNombre")]
        public string CampoNombre { get; set; }

    }
}
