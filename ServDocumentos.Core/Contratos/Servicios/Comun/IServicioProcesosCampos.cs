using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioProcesosCampos
    {

        public int Agrega(ProcesoCampoInsDto procesoscampos);
        public bool EliminaxIds(ProcesoCampoDelDto procesoscampos);
        public bool Elimina(ProcesoCampoIdDto procesoscampos);
        public IEnumerable<ResultadoProcesoCampo> Obtiene(ProcesoCampoGetDto procesoscampos);
        public bool Modifica(ProcesoCampoUpdDto procesoscampos);
        // public string Obtiene(int subprocesoCampo procesoId);
    }
}
