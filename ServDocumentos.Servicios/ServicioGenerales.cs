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
    public class ServicioGenerales : ServicioBase, IServicioGenerales
    {
        public ServicioGenerales(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }
        public int Agrega(GeneralesInsDto generales)
        {
            int generalesId = UnitOfWork.RepositorioGenerales.Agrega(generales);
            return generalesId;
        }
        public bool EliminaxNombre(GeneralesNombreDto generales)
        {
            return UnitOfWork.RepositorioGenerales.EliminaxNombre(generales);
        }
        public bool Elimina(GeneralesIdDto generales)
        {
            return UnitOfWork.RepositorioGenerales.Elimina(generales);
        }
        public IEnumerable<ResultadoGeneral> Obtiene(GeneralesGetDto generales)
        {
            return UnitOfWork.RepositorioGenerales.Obtiene(generales);
        }
        public bool Modifica(GeneralesUpdDto generales)
        {
            return UnitOfWork.RepositorioGenerales.Modifica(generales);
        }
        public string Obtiene(int procesoId)
        {
            return UnitOfWork.RepositorioGenerales.Obtiene(procesoId);
        }
    }
}