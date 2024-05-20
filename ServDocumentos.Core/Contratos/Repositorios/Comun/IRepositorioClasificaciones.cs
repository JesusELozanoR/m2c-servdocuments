using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioClasificaciones
    {
        public int Agrega(ClasificacionInsDto clasificaciones);
        public bool EliminaxNombre(ClasificacionNombreDto clasificaciones);
        public bool Elimina(ClasificacionIdDto clasificaciones);
        public IEnumerable<ResultadoClasificacion> Obtiene(ClasificacionGetDto clasificaciones);
        public bool Modifica(ClasificacionUpdDto clasificaciones);
        // public string Obtiene(int procesoId);
    }
}
