namespace ServDocumentos.Core.Entidades.Comun
{
    public class Plantillas : EntidadBase
    {
        public string PlantillaId { get; set; }
        public string PlantillaNombre { get; set; }
        public string Descripcion { get; set; }
        public string Version { get; set; }
        public string AlfrescoId { get; set; }
        public string AlfrescoURL { get; set; }
        /// <summary>
        /// Plantilla en base 64
        /// </summary>
        public string Base64 { get; set; }
    }
}
