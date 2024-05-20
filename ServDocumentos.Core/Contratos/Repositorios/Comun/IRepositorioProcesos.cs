using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioProcesos
    {
        int Agrega(ProcesoDto proceso);
        bool EliminaxNombre(ProcesoNombreDto proceso);
        bool EliminaxId(ProcesoIdDto proceso);
        IEnumerable<Procesoc> Obtiene();
        bool Modifica(ProcesoUpdDto proceso);
        IEnumerable<Procesoc> Obtener(ProcesoGetDto proceso);

    }
}
