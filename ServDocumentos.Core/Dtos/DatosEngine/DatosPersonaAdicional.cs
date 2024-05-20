using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public     class DatosPersonaAdicional: DatosPersona
	{      
        public string LugarNacimiento { get; set; }
        public string Edad { get; set; }
        public string Genero { get; set; }
        public string EstadoCivil { get; set; }
        public string TelefonoFijo { get; set; }
        public string TelefonoOficina { get; set; }
        public string TelefonoCelular { get; set; }
        public string Identificacion { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string Nacionalidad { get; set; }
        public string FormaMigratoria { get; set; }
        public string Profesion { get; set; }
		public string Trabajo { get; set; }
		public Decimal Ingresos { get; set; }
		public string DireccionCompletaExtranjero { get; set; }
		public string TelefonoExtranjero { get; set; }
		public string RFCExtranjero { get; set; }
		public Decimal SaldoPromedio { get; set; }
		public string MovimientosEsperados { get; set; }
		public string NivelEstudio { get; set; }
		public string TiempoRadicandoDomicilio { get; set; }
		public string AntigüedadEmpleoNegocio { get; set; }
		public decimal IngresoMensual { get; set; }
		public decimal ImporteInicialDepositado { get; set; }
		public string OrigenRecursos { get; set; }
		public string CargoPoliticoUltimos12Meses { get; set; }
		public string Cargo { get; set; }
		public string RelacionPolitico12Meses { get; set; }
		public string NombreFuncionario { get; set; }
		public string ParentescoPolitico { get; set; }
		public string ResidenciaOtroPais { get; set; }
		public string NumeroIdentificacionFiscal { get; set; }
		public string PaisAsignado { get; set; }
		public string PropietarioRecursos { get; set; }
		public string QuienEsPropietarioRecursos { get; set; }
		public string RelacionDueno { get; set; }
		public string ProveedorRecursos { get; set; }
		public string QuienEsProveedorRecursos { get; set; }

		public string RelacionProveedor { get; set; }
		public string DestinoRecursos { get; set; }
		public string IncrementoInversionMensual { get; set; }
		public decimal MontoIncrementoInversionMensual { get; set; }
		public string RetirosMensuales { get; set; }
		public decimal MontoRetirosMensuales { get; set; }
		public string ActuaCuentaPropia { get; set; }

    }
}
