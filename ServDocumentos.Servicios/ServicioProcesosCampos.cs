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
    public class ServicioProcesosCampos : ServicioBase, IServicioProcesosCampos
    {
        public ServicioProcesosCampos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(ProcesoCampoInsDto generales)
        {
            int generalesId = UnitOfWork.RepositorioProcesosCampos.Agrega(generales);
            return generalesId;
        }
        public bool EliminaxIds(ProcesoCampoDelDto generales)
        {
            return UnitOfWork.RepositorioProcesosCampos.EliminaxIds(generales);
        }
        public bool Elimina(ProcesoCampoIdDto generales)
        {
            return UnitOfWork.RepositorioProcesosCampos.Elimina(generales);
        }
        public IEnumerable<ResultadoProcesoCampo> Obtiene(ProcesoCampoGetDto generales)
        {
            return UnitOfWork.RepositorioProcesosCampos.Obtiene(generales);
        }
        public bool Modifica(ProcesoCampoUpdDto generales)
        {
            return UnitOfWork.RepositorioProcesosCampos.Modifica(generales);
        }
        //public string Obtiene(int procesoId)
        //{
        //    return UnitOfWork.RepositorioProcesosCampos.Obtiene(procesoId);
        //}
    }
}
