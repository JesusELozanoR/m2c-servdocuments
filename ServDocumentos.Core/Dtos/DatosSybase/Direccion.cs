namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Direccion
    {
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Localidad { get; set; }
        public string Colonia { get; set; }
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string DireccionCompleta { get; set; }
        public string CP { get; set; }
        public int TiempoResidencia { get; set; }
        public string TipoDomicilio { get; set; }
        public string ReferenciasUbicacion { get; set; }
    }
}
