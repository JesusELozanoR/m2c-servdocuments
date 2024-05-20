namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class Persona
    {
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string ClaveElector { get; set; }
        public string FechaNacimiento { get; set; }
        public string Nacionalidad { get; set; }
        public string Telefono { get; set; }
        //public Direccion Direccion { get; set; }
        public string NombreCompletoInverso { get; set; }
        public string Sexo { get; set; }
        public string EstadoCivil { get; set; }
        public string TelefonoCelular { get; set; }
        public string LugarNacimiento { get; set; }
        public string TipoIdentificacion { get; set; }
        public string CorreoElectronico { get; set; }
        public string FormaMigratoria { get; set; }
        public string NumeroExtranjero { get; set; }
        public string DireccionExtranjero { get; set; }

        //Direccion
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

        //Direccion Negocio
        public string PaisNegocio { get; set; }
        public string ProvinciaNegocio { get; set; }
        public string CiudadNegocio { get; set; }
        public string LocalidadNegocio { get; set; }
        public string ColoniaNegocio { get; set; }
        public string CalleNegocio { get; set; }
        public string NumeroExteriorNegocio { get; set; }
        public string NumeroInteriorNegocio { get; set; }
        public string DireccionCompletaNegocio { get; set; }
        public string CPNegocio { get; set; }
        public int TiempoResidenciaNegocio { get; set; }
        public string TipoDomicilioNegocio { get; set; }
        public string ReferenciasUbicacionNegocio { get; set; }
    }
}
