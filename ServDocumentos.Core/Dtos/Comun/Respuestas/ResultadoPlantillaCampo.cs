﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    public class ResultadoPlantillaCampo
    {
		public int PlantillaId { get; set; }
		public string PlantillaNombre { get; set; }
		public int CampoId { get; set; }
		public string CampoNombre { get; set; }
		public string Descripcion { get; set; }
		public string Tipo { get; set; }
		public string DatoCampo { get; set; }
		public string DatoConjunto { get; set; }
		public string DatoConjuntoGrupal { get; set; }
		public bool Estado { get; set; }
	}
}
