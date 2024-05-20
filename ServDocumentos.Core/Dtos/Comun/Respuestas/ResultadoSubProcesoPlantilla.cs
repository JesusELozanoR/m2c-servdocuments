using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoSubProcesoPlantilla
    {
		public int SubpPlantId { get; set; }
		public int SubProcesoId { get; set; }
		public string SubProcesoNombre { get; set; }
		public int PlantillaId { get; set; }
		public string PlantillaNombre { get; set; }
		public string Descripcion { get; set; }
		public string AlfrescoId { get; set; }
		public string AlfrescoUrl { get; set; }
		public bool Estado { get; set; }
		public string RECA { get; set; }
		public string Tipo { get; set; }

		public string TipoDescripcion { get; set; }

		public string TipoDescripcionCorta { get; set; }

		public string PlantillaFechaModificacion { get; set; }
	}
}
