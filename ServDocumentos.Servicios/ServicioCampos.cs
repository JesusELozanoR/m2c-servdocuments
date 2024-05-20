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
    public class ServicioCampos : ServicioBase, IServicioCampos
    {
        public ServicioCampos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }

        public int Agrega(CampoInsDto campo)
        {
            int procesoId = UnitOfWork.RepositorioCampos.Agrega(campo);
            return procesoId;
        }
        public bool EliminaxNombre(CampoNombreDto campo)
        {
            return UnitOfWork.RepositorioCampos.EliminaxNombre(campo);
        }
        public bool EliminaxId(CampoIdDto campo)
        {
            return UnitOfWork.RepositorioCampos.EliminaxId(campo);
        }
        public IEnumerable<ResultadoCampo> Obtiene(CampoGetDto campo)
        {
            return UnitOfWork.RepositorioCampos.Obtiene(campo);
        }
        public bool Modifica(CampoUpdDto campo)
        {
            return UnitOfWork.RepositorioCampos.Modifica(campo);
        }
    }
}
