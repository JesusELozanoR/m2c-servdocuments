using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioProcesos
    {
        public int Agrega(ProcesoDto proceso);
        public bool EliminaxNombre(ProcesoNombreDto proceso);
        public bool EliminaxId(ProcesoIdDto proceso);
        public IEnumerable<Procesoc> Obtiene();
        public bool Modifica(ProcesoUpdDto proceso);
        public IEnumerable<Procesoc> Obtener(ProcesoGetDto proceso);
    }
}
