using Newtonsoft.Json;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Helpers
{
    public static class HTTPClientApi
    {
        private enum tipoVerbo { GET, POST }
         static tipoVerbo Tipo { get; set; }


        public class JsonImagen
        {
            public string Url { get; set; }
            public string Usuario { get; set; }
            public string Password { get; set; }
            public string Metodo { get; set; }
            public string Parametros { get; set; }
            public string Header { get; set; }
            public string Body { get; set; }
        }

        public static async Task<HttpResponseMessage> SendRequest (string url)
        {
            return await SendRequest(url, null, null, null, null, null, null);
        }

        public static async Task<HttpResponseMessage> SendRequest(string url, string user, string password, string metodo, Dictionary<string, string> headers, Dictionary<string, string> parameters, string body)
        {
            if (metodo == "GET")
            {
                Tipo = tipoVerbo.GET;
            }
            else 
            {
                Tipo = tipoVerbo.POST;
            }
           
            return await  Execute(url, user, password, metodo, headers, parameters, body);

        }


       /*public static async Task<HttpResponseMessage> Post(string url, string user, string password, string metodo, Dictionary<string, string> headers, Dictionary<string, string> parameters, string body)
        {
            Tipo = tipoVerbo.POST;
            return await Execute( url, user, password, metodo, headers, parameters, body);

        }*/

        //No regresar un arreglo de bytes, regresar el HTTPclint Result


        private static async Task<HttpResponseMessage> Execute(string url, string user, string password, string metodo, Dictionary<string, string> headers, Dictionary<string, string> parameters, string body)
        {
            HttpResponseMessage result = null;

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            //HttpResponseMessage response = null;

            using (var httpClient = new HttpClient(handler))
            {

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrWhiteSpace(password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                       Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}")));
                }
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                if(Tipo== tipoVerbo.GET) {

                    result =  httpClient.GetAsync(new Uri(url)).Result;

                }

                else 
                {
                    JsonImagen ji = new JsonImagen();
                    ji.Usuario = user;
                    ji.Password = password;

                    var myContent = JsonConvert.SerializeObject(ji);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    result = httpClient.PostAsync(new Uri(url), byteContent).Result;
                }

            }

            return  result;
        }


        public static async Task<HttpResponseMessage> PostRequest(string url, string user, string password, Dictionary<string, string> headers, Dictionary<string, string> parameters, Dictionary<string, object> body)
        {
            HttpResponseMessage response = null;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            using (var httpClient = new HttpClient(handler))
            {

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrWhiteSpace(password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                       Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}")));
                }
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = httpClient.PostAsync(url, content).Result;
            }
            return response;
        }
    }
}
