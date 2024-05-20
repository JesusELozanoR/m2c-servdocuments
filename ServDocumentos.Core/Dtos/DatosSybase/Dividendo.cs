namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Dividendo
    {
        public int NumeroDividendo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaVencimiento { get; set; }
        public string Ahorro { get; set; }
        public decimal Cuota { get; set; }
        public decimal Capital { get; set; }
        public decimal IntereS { get; set; }
        public decimal Iva { get; set; }
        public decimal Comision { get; set; }
        public decimal Seguros { get; set; }
        public decimal GastoSupervision { get; set; }
        public decimal Saldo { get; set; }
        public decimal ComisionIva { get; set; }
    }
}
