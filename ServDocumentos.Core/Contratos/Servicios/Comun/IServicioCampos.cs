using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioCampos
    {
        public int Agrega(CampoInsDto campo);
        public bool EliminaxNombre(CampoNombreDto campo);
        public bool EliminaxId(CampoIdDto campo);
        public IEnumerable<ResultadoCampo> Obtiene(CampoGetDto campo);
        public bool Modifica(CampoUpdDto campo);
    }
}
