using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
     public class DatosAhorro
	{
		public string NumeroCuentaInicial { get; set; }
		public string RazonSocial { get; set; }
		public string Giro { get; set; }
		public string FechaConstitucion { get; set; }
		
		public string NumeroSerieFirmaElectronica { get; set; }
		public string PeriodoInversion3m { get; set; }
		public string PeriodoInversion6m { get; set; }
		public string PeriodoInversion12m { get; set; }
		public string PeriodoInversion18m { get; set; }
		public string PeriodoInversion24m { get; set; }
		public string PeriodoInversionOtro { get; set; }
		public string PeriodoInversion { get; set; }
		public string DiasPreestablecidos { get; set; }
		public string RenovacionAutoSI { get; set; }
		public string RenovacionAutoNO { get; set; }
		public string ReinversionAutoSI { get; set; }
		public string ReinversionAutoNO { get; set; }
		public string ManejoCuentaFirmaIndistintas { get; set; }
		public string ManejoCuentaFirmaMancomunadas { get; set; }
		public string ManejoCuentaFirmaIndividual { get; set; }

		public decimal? GAT { get; set; }
		public decimal? GatReal { get; set; }
		public decimal? TasaAhorro { get; set; }
		public int? Plazo { get; set; }
		public int? NoConstancia { get; set; }
		public string Contrato { get; set; }
		public decimal? Monto { get; set; }
		public int? DiasTermino { get; set; }

        public string CuentaDeposito { get; set; }
		public string FechaVencimiento { get; set; }

		public string FechaActivacion { get; set; }

		public string TipoPersona { get; set; }
		public string TipoPlazo { get; set; }
		public string TipoCuenta { get; set; }

		public string TipoCombrobante { get; set; }
		public string ProductoAhorro { get; set; }
		public string CuentaAhorro { get; set; }
		public string FechaApertura { get; set; }
		public string InstruccionesTipoPlazo { get; set; }
		public string InstruccionesVencimiento { get; set; }
		public string HoraApertura { get; set; }
		public DatosTransaccion Transaccion { get; set; }
		public decimal CreditoGarantizado { get; set; }
		public decimal Rendimiento { get; set; }
	}

}
