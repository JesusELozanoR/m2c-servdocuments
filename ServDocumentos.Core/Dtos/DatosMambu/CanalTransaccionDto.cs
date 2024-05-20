namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Canal de transaccion
    /// </summary>
    public class CanalTransaccionDto
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Encoded key
        /// </summary>
        public string EncodedKey { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
    }
}
