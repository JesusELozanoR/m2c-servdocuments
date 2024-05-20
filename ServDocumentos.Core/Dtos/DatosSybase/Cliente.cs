using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Cliente : Persona
    {
        public int NumeroCliente { get; set; }
        public int NumeroDependientes { get; set; }
        public decimal Ingresos { get; set; }
        public string Ocupacion { get; set; }
        public string Trabajo { get; set; }
        public decimal UtilidadNegocio { get; set; }
        public int NumeroEmpleado { get; set; }
        public string Empresa { get; set; }
        public string CodigoBarras { get; set; }
        public Credito Credito { get; set; }
        public Ahorro Ahorro { get; set; }
        public IEnumerable<Beneficiario> Beneficiarios { get; set; }
        public IEnumerable<Aval> Avales { get; set; }
        //public Direccion DireccionNegocio { get; set; }
        public string TipoCliente { get; set; }
        public string TipoVivienda { get; set; }
        public RepresentanteLegal RepresentanteLegal { get; set; }
        //public Grupo Grupo { get; set; }
        public Seguro SeguroBasico { get; set; }
        public Seguro SeguroVoluntario { get; set; }
        public PlanConexion PlanConexion { get; set; }
        public IEnumerable<AhorroIndividuales> AhorroIndividual { get; set; }
        public string NombreSuscriptor1 { get; set; }
        public string DireccionSuscriptor1 { get; set; }
        public List<Cotitular> Cotitulares { get; set; }
        public string EdoCtaPortalInternet { get; set; }
        public string Sucursal { get; set; }
        public string adherente { get; set; }



        public decimal MontoNeto { get; set; }
        public string Periodo { get; set; }
        public List<Cotitular> Tutores { get; set; }
        public int BanderaAcelerada { get; set; }
        public int BanderaCreditosSubsecuentes { get; set; }
        public int BanderaExpres { get; set; }
        public int BanderaOrdenPago { get; set; }
        public string Edad { get; set; }
        public string PaisNacimiento { get; set; }
    }
}
