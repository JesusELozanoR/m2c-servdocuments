namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Aval : Persona
    {
        public string TipoAval { get; set; }
        public string NumeroDependientes { get; set; }
        public decimal Ingresos { get; set; }
        public string Ocupacion { get; set; }
        public string Trabajo { get; set; }
        public int NumeroCliente { get; set; }
    }
}
