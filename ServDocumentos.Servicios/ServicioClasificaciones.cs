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
    public class ServicioClasificaciones : ServicioBase, IServicioClasificaciones
    {
        public ServicioClasificaciones(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(ClasificacionInsDto clasificaciones)
        {
            int generalesId = UnitOfWork.RepositorioClasificaciones.Agrega(clasificaciones);
            return generalesId;
        }
        public bool EliminaxNombre(ClasificacionNombreDto clasificaciones)
        {
            return UnitOfWork.RepositorioClasificaciones.EliminaxNombre(clasificaciones);
        }
        public bool Elimina(ClasificacionIdDto clasificaciones)
        {
            return UnitOfWork.RepositorioClasificaciones.Elimina(clasificaciones);
        }
        public IEnumerable<ResultadoClasificacion> Obtiene(ClasificacionGetDto clasificaciones)
        {
            return UnitOfWork.RepositorioClasificaciones.Obtiene(clasificaciones);
        }
        public bool Modifica(ClasificacionUpdDto clasificaciones)
        {
            return UnitOfWork.RepositorioClasificaciones.Modifica(clasificaciones);
        }
        //public string Obtiene(int procesoId)
        //{
        //    return UnitOfWork.RepositorioClasificaciones.Obtiene(procesoId);
        //}
    }
}
