using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Enumeradores;
using ServDocumentos.Core.Mensajes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.Dtos.Comun.Solicitudes
{
    public class SolictudDocumentoDto
    {
        /// <summary>
        /// Nombre del proceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        //[EnumDataType(typeof(ProcesosName), ErrorMessage = "EL VALOR SNO ESTA PERMITIDO")]
        // [RequiredTipoProceso(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("procesoNombre")]
        public string ProcesoNombre { get; set; }
        /// <summary>
        /// Nombre del Subproceso
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("subProcesoNombre")]
        public string SubProcesoNombre { get; set; }
        /// <summary>
        /// Número de Crédito
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCredito")]
        public string NumeroCredito { get; set; }
        /// <summary>
        /// Número de Cliente
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("numeroCliente")]
        public string NumeroCliente { get; set; }
        /// <summary>
        /// Indica si el documento viene en archivos separados o individuales (true y false respectivamente)
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("separado")]
        public bool Separado { get; set; }
        /// <summary>
        /// Indica si los documentos se encuentra en un archivo comprimido en formato zip
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("comprimido")]
        public bool Comprimido { get; set; }
        /// <summary>
        /// Clave del usuario
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(MensajesDataAnnotations.Requerido), ErrorMessageResourceType = typeof(MensajesDataAnnotations))]
        [JsonProperty("usuario")]
        public string Usuario { get; set; }
        /// <summary>
        /// Indica si el archivos se codifica a Base64 para su entrega 
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty("Base64")]
        public bool Base64 { get; set; }
        /// <summary>
        /// Listado de Id's de las plantillas que se desean obtner. 
        /// </summary>
        // [HiddenInput(DisplayValue = false)]
        // [JsonIgnore]
        [JsonProperty("ListaPlantillasIds")]
        public List<int> ListaPlantillasIds { get; set; }
        public string TipoPersona { get; set; }
        public string TipoComprobante { get; set; }
        [JsonProperty("ImpresionesPlantillas")]
        public List<PlantillasImpresiones> ImpresionesPlantillas { get; set; }

        [JsonProperty("NumerosClientes")]
        public List<int> NumerosClientes { get; set; }
        [JsonProperty("NumerosDividendos")]
        public List<int> NumerosDividendos { get; set; }

        /// <summary>
        /// Indica si el archivos se codifica a Base64 para su entrega 
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("Alfresco")]
        public bool Alfresco { get; set; }
        public string ConCuentaAhorroLigada { get; set; }
        /// <summary>
        /// Tipo de gestor para guardar documentos
        /// </summary>
        private string _guardarDocumento { get; set; } = string.Empty;
        /// <summary>
        /// Tipo del gestor para guardar documentos
        /// </summary>
        public string GuardarDocumento
        {
            get => this._guardarDocumento;
            set
            {
                if (string.IsNullOrEmpty(value) && Alfresco)
                    this._guardarDocumento = "Digipro";
                else
                {
                    this._guardarDocumento = value;
                }
            }
        }
    }
}