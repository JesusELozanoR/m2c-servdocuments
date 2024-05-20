namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Producto obtenido del API de MAMBU
    /// </summary>
    public class ProductoDto
    {
        /// <summary>
        /// Encoded key del producto
        /// </summary>
        public string EncodedKey { get; set; }
        /// <summary>
        /// Id del producto
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Name { get; set; }
    }
}
