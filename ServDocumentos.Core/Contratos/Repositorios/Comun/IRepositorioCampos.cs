using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioCampos
    {
        int Agrega(CampoInsDto campo);
        bool EliminaxNombre(CampoNombreDto campo);
        bool EliminaxId(CampoIdDto campo);
        IEnumerable<ResultadoCampo> Obtiene(CampoGetDto campos);
        bool Modifica(CampoUpdDto campo);
    }
}
