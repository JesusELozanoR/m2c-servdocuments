namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class PlanConexion
    {
        
        public int PaqueteId { get; set; }
        public long ClienteId { get; set; }
        public decimal MontoPaquete { get; set; }
        public string CodigoCompra { get; set; }
    }
}
