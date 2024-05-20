using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Documento
{
    public class PlantillaArchivo
    {
        public string PlantillaNombre { get; set; }
        public byte[] Archivo { get; set; }
        public string AlfrescoId { get; set; }
        public string AlfrescoUrl { get; set; }
    }
}
