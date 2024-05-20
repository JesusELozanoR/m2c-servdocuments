using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
   public  class AhorroIndividuales
    {
        public int NumeroCliente { get; set; }        
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NombreCompleto { get; set; }
        public string DireccionCompleta { get; set; }
        public string FechaNacimiento { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string Telefono { get; set; }
        public string CuentaAhorro { get; set; }
        public decimal Monto { get; set; }
        public decimal Ingresos { get; set; }
        public string ProductoAhorro { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal TasaAhorro { get; set; }
        public decimal GAT { get; set; }
        public string NoTarjeta { get; set; }
        public decimal GATReal { get; set; }
        public Seguro SeguroBasico { get; set; }
        public Seguro SeguroVoluntario { get; set; }
        public PlanConexion PlanConexion { get; set; }
        public IEnumerable<Beneficiario> Beneficiarios { get; set; }
        public decimal SaldoPromedio { get; set; }
        public int MovimientosEsperados { get; set; }
        public string Ocupacion { get; set; }
        public string Trabajo { get; set; }
        public string Rol { get; set; }
        public decimal Basico { get; set; }
        public decimal Voluntario { get; set; }
        public decimal Paquete { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal Ahorro { get; set; }
        public decimal MontoLiq { get; set; }
        public decimal MontoRecivir { get; set; }
        public string Vigencia { get; set; }     
        public string Descripcion { get; set; }
        public string Sexo { get; set; }
        public string FormaMigratoria { get; set; }
        public string NumeroExtrajero { get; set; }
        public string DireccionExtrajero { get; set; }
        public string Nacionalidad { get; set; }

    }
}
