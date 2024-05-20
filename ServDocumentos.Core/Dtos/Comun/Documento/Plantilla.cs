using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class Plantilla
    {
        public int PlantillaId { get; set; }
        public string PlantillaNombre { get; set; }
        public string Descripcion { get; set; }
        public string AlfrescoId { get; set; }
        public string AlfrescoURL { get; set; }
        public List<Campo> Campos { get; set; }
        public string Reca { get; set; }
        public string Tipo { get; set; }
        /// <summary>
        /// Plantilla en base 64
        /// </summary>
        public string Base64 { get; set; }
    }
}
