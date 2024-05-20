using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioCamposTipos
    {
        public ResultadoCampoTipo Agrega(CampoTipoInsDto campo);
        public ResultadoCampoTipo Modifica(CampoTipoUpdDto campo);
        public IEnumerable<ResultadoCampoTipo> Obtiene(CampoTipoGetDto campo);
    }
}
