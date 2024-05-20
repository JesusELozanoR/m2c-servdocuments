using Newtonsoft.Json;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class CampoTipoGetDto
    {
        /// <summary>
        /// id del Tipo de Campo
        /// </summary>
        [JsonProperty("Campo Tipo Id")]
        public int CampoTipoId { get; set; }

        /// <summary>
        /// Descripcion del Tipo Campo
        /// </summary>
        [JsonProperty("Campo Tipo")]
        public string CampoTipo { get; set; }

        /// <summary>
        /// Estado del Tipo Campo
        /// </summary>
        [JsonProperty("Estado")]
        public Estado Estado { get; set; }
    }
}
