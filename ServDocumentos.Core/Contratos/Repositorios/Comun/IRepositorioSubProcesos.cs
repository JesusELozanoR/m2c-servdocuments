using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioSubProcesos
    {
        int Agrega(SubProcesoDto proceso);
        bool EliminaxNombre(SubProcesoNombreDto proceso);
        bool EliminaxId(SubProcesoIdDto proceso);
        IEnumerable<SubProcesoc> Obtiene(SubProcesoGetDto subproceso);
        bool Modifica(SubProcesoUpdDto proceso);
        IEnumerable<SubProcesoc> ObtienexClasificacion(SubProcesoClasDto subproceso);
    }
}
