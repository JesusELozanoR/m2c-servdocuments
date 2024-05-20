using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioCamposTipos
    {
        public ResultadoCampoTipo Agrega(CampoTipoInsDto campo);
        public ResultadoCampoTipo Modifica(CampoTipoUpdDto campo);
        public IEnumerable<ResultadoCampoTipo> Obtiene(CampoTipoGetDto campo);
    }
}
