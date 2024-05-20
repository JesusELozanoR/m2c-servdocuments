using Serilog;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Seguro
    {
        public long ClienteId { get; set; }
        public string Certificado { get; set; }
        public string Paquete { get; set; }
        public int Tipo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int TipoSeguroId { get; set; }
    }
}
