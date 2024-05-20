using System;

namespace ServDocumentos.Core.Dtos.Comun.Correo
{
    /// <summary>
    /// DTO para actualizar una bitacora de estado de cuenta
    /// </summary>
    public class ActualizaEstadoCuentaDto
    {
        /// <summary>
        /// Numero de la cuenta (Id o Encoded key)
        /// </summary>
        public string NumeroCuenta { get; set; }
        /// <summary>
        /// Numero del cliente (Id o Encoded key)
        /// </summary>
        public string NumeroCliente { get; set; }
        /// <summary>
        /// Estatus del estado de cuenta
        /// </summary>
        public string Estatus { get; set; }
        /// <summary>
        /// Observaciones adicionales
        /// </summary>
        public string Observaciones { get; set; }
        /// <summary>
        /// Fecha en que se genera el estado de cuenta
        /// </summary>
        public DateTime? Fecha { get; set; }
        /// <summary>
        /// Fecha en la que se envio el estado de cuenta
        /// </summary>
        public DateTime? FechaEnvio { get; set; }
        /// <summary>
        /// Empresa a la que pertenece la cuenta
        /// </summary>
        public string Empresa { get; set; }

    }
}
