using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioSubProcesosCampos
    {
        public int Agrega(SubProcesoCampoInsDto subprocesoscampos);
        public bool EliminaxIds(SubProcesoCampoDelDto subprocesoscampos);
        public bool Elimina(SubProcesoCampoIdDto subprocesoscampos);
        public IEnumerable<ResultadoSubProcesoCampo> Obtiene(SubProcesoCampoGetDto subprocesoscampos);
        public bool Modifica(SubProcesoCampoUpdDto subprocesoscampos);
        // public string Obtiene(int procesoId);
    }
}
