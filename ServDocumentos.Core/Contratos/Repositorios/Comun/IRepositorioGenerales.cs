using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    public interface IRepositorioGenerales
    {
        public int Agrega(GeneralesInsDto generales);
        public bool EliminaxNombre(GeneralesNombreDto generales);
        public bool Elimina(GeneralesIdDto generales);        
        public IEnumerable<ResultadoGeneral> Obtiene(GeneralesGetDto generales);
        public bool Modifica(GeneralesUpdDto generales);
        public string Obtiene(int procesoId);

    }
}
