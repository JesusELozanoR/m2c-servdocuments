using Newtonsoft.Json;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Grupo DTO
    /// </summary>
    public class GrupoDto
    {
        /// <summary>
        /// Encoded key
        /// </summary>
        public string EncodedKey { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        [JsonProperty ("_Datos_Moral")]
        public DatosMoral Datos_Moral { get; set; }
        /// <summary>
        /// Direccion del grupo
        /// </summary>
        [JsonProperty("_Direccion_Grupos")]
        public Direccion Direccion_Grupo { get; set; }
       
        /// <summary>
        /// Correo del grupo
        /// </summary>
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; } 
        /// <summary>
        /// Número de teléfono del grupo
        /// </summary>
        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }
        /// <summary>
        /// Número de casa del grupo
        /// </summary>
        [JsonProperty("homePhone")]
        public string HomePhone { get; set; }
        

    }
    public class DatosMoral
    {
        /// <summary>
        /// RFC
        /// </summary>
        [JsonProperty("_RFC")]
        public string RFC { get; set; }
    }

    public class Direccion
    {
        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Municipio_Direccion_Grupo")]
        public string Municipio_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Numero_Int_Direccion_Grupo")]
        public string Numero_Int_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Colonia_Direccion_Grupo")]
        public string Colonia_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Numero_Ext_Direccion_Grupo")]
        public string Numero_Ext_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Estado_Direccion_Grupo")]
        public string Estado_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("Calle_Direccion_Grupo")]
        public string Calle_Direccion_Grupo { get; set; }

        /// <summary>
        /// Municipio Direccion Grupo
        /// </summary>
        [JsonProperty("CP_Direccion_Grupo")]
        public string CP_Direccion_Grupo { get; set; }
    }

}
