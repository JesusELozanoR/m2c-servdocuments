namespace ServDocumentos.Core.Dtos.Comun.Respuestas
{
    /// <summary>
    /// Respuesta del prcedimiento ObtenerBitacorasPorMes
    /// </summary>
    public class BitacoraEstadoCuentaRespuesta
    {
        /// <summary>
        /// Numero del cliente
        /// </summary>
        public string NumeroCliente { get; set; }
        /// <summary>
        /// Encoded key del cliente
        /// </summary>
        public string NumeroCuenta { get; set; }
    }
}
