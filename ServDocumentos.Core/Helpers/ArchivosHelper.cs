using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Helpers
{
    public class ArchivosHelper
    {
        public static async Task<string> DescargarArchivoTexto(string url)
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            using (var client = new HttpClient(clientHandler))
            {

                using var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsStringAsync();
                }
            }
            return null;
        }
        public static byte[] LeerArchivoBytes(Stream input)
        {
            using MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

    }
}
