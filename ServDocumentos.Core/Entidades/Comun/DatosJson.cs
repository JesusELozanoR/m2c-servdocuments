namespace ServDocumentos.Core.Entidades.Comun
{
    public class DatosJson : EntidadBase
    {
        public long Id { get; set; }
        public string Credito { get; set; }
        public string AlfrescoId { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        /// <summary>
        /// Contiene la informacion del documento en formato json
        /// </summary>
        public string Json { get; set; }
    }
}
