namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Beneficiario : Persona
    {
        public long NumeroCliente { get; set; }
        public decimal Porcentaje { get; set; }
        public string TipoBeneficiario { get; set; }
        public string Relacion { get; set; }
        public int NumIntegrante { get; set; }
        public string Observacion { get; set; }
    }
}
