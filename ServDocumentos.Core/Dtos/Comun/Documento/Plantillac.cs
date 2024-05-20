using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class Plantillac
    {
        public int PlantillaId { get; set; }
        public string PlantillaNombre { get; set; }
        public string Descripcion { get; set; }
        public string AlfrescoId { get; set; }
        public string AlfrescoURL { get; set; }
        public List<Campo> Campos { get; set; }
        public string Reca { get; set; }
        public string Tipo { get; set; }
    }
}
