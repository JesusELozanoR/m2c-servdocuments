using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioProcesos : ServicioBase, IServicioProcesos
    {
        public ServicioProcesos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(ProcesoDto proceso)
        {
            int procesoId = UnitOfWork.RepositorioProcesos.Agrega(proceso);            
            return procesoId;
        }
        public bool EliminaxNombre(ProcesoNombreDto proceso) {
            return UnitOfWork.RepositorioProcesos.EliminaxNombre(proceso);
        }
        public bool EliminaxId(ProcesoIdDto proceso) {
            return UnitOfWork.RepositorioProcesos.EliminaxId(proceso);
        }
        public IEnumerable<Procesoc> Obtiene() {
            return UnitOfWork.RepositorioProcesos.Obtiene();
        }
        public bool Modifica(ProcesoUpdDto proceso) {
            return UnitOfWork.RepositorioProcesos.Modifica(proceso);            
        }
        public IEnumerable<Procesoc> Obtener(ProcesoGetDto proceso)
        {
            return UnitOfWork.RepositorioProcesos.Obtener(proceso);
        }
    }
}
