using ServDocumentos.Core.Entidades.Comun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioSubeArchivos
    {
        RespuestaBinarios SubeArchivos(MemoryStream Archivo, string nombreArchivo);       
    }
}
