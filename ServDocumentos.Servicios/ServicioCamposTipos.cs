using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioCamposTipos : ServicioBase, IServicioCamposTipos
    {
        public ServicioCamposTipos(GestorLog gestorLog, IUnitOfWork unitOfWork) : base(gestorLog, unitOfWork)
        {
        }

        public ResultadoCampoTipo Agrega(CampoTipoInsDto campotipo)
        {
            return UnitOfWork.RepositorioCamposTipos.Agrega(campotipo);
        }
        public ResultadoCampoTipo Modifica(CampoTipoUpdDto campotipo)
        {
            return UnitOfWork.RepositorioCamposTipos.Modifica(campotipo);
        }
        public IEnumerable<ResultadoCampoTipo> Obtiene(CampoTipoGetDto campotipo)
        {
            return UnitOfWork.RepositorioCamposTipos.Obtiene(campotipo);
        }
    }
}
