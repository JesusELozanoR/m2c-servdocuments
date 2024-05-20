using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Servicios.Comun
{
    public  class ServicioSubProcesosCampos : ServicioBase, IServicioSubProcesosCampos
    {
        public ServicioSubProcesosCampos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(SubProcesoCampoInsDto subprocesocampo)
        {
            int generalesId = UnitOfWork.RepositorioSubProcesosCampos.Agrega(subprocesocampo);
            return generalesId;
        }
        public bool EliminaxIds(SubProcesoCampoDelDto subprocesocampo)
        {
            return UnitOfWork.RepositorioSubProcesosCampos.EliminaxIds(subprocesocampo);
        }
        public bool Elimina(SubProcesoCampoIdDto subprocesocampo)
        {
            return UnitOfWork.RepositorioSubProcesosCampos.Elimina(subprocesocampo);
        }
        public IEnumerable<ResultadoSubProcesoCampo> Obtiene(SubProcesoCampoGetDto subprocesocampo)
        {
            return UnitOfWork.RepositorioSubProcesosCampos.Obtiene(subprocesocampo);
        }
        public bool Modifica(SubProcesoCampoUpdDto subprocesocampo)
        {
            return UnitOfWork.RepositorioSubProcesosCampos.Modifica(subprocesocampo);
        }
        //public string Obtiene(int procesoId)
        //{
        //    return UnitOfWork.RepositorioSubProcesosCampos.Obtiene(procesoId);
        //}
    }
}
