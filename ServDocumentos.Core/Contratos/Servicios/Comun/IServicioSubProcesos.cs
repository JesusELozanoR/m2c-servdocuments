using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioSubProcesos
    {
        public int Agrega(SubProcesoDto subproceso);
        public bool EliminaxNombre(SubProcesoNombreDto subproceso);
        public bool EliminaxId(SubProcesoIdDto subproceso);
        public IEnumerable<SubProcesoc> Obtiene(SubProcesoGetDto subproceso);
        public bool Modifica(SubProcesoUpdDto subproceso);
        public IEnumerable<SubProcesoc> ObtienexClasificacion(SubProcesoClasDto subproceso);
        // public int AgregaxClasificacion(SubProcesoClasifIns subproceso);
    }
}
