using ServDocumentos.Core.Dtos.Comun.Documento;
using System;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioDocumentos
    {
        Int64 DocumentosRegistroGuarda(DocGuarda docGuarda);
    }
}
