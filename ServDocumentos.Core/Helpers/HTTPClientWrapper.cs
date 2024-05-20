using Newtonsoft.Json;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Helpers
{
    public static class HTTPClientWrapper<T> where T : class
    {
        public static async Task<T> Get(string url)
        {
            return await Get(url, null, null, null);
        }
        public static async Task<T> Get(string url, string user, string password, Dictionary<string, string> headers)
        {
            T result = null;

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


                var response = httpClient.GetAsync(new Uri(url)).Result;
                try
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                        {
                            if (x.IsFaulted)
                                throw x.Exception;
                            try
                            {
                                JsonSerializerSettings settings = new JsonSerializerSettings
                                {
                                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                                };

                                result = JsonConvert.DeserializeObject<T>(x.Result, settings);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }

                        });
                    }
                    else
                    {

                        throw new ExternoTechnicalException(response.StatusCode.ToString());
                    }

                }
                catch (Exception ex)
                {


                    throw new ExternoTechnicalException(response.StatusCode.ToString());
                }




            }

            return result;
        }

        public static async Task<T> PostRequest(string apiUrl, T postObject)
        {
            T result = null;

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(apiUrl, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;

                    result = JsonConvert.DeserializeObject<T>(x.Result);

                });
            }

            return result;
        }

        public static async Task PutRequest(string apiUrl, T putObject)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(apiUrl, putObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }

        public static async Task<T> PostRequestEngine(string url, string user, string password, Dictionary<string, string> headers, Dictionary<string, string> paraBody)
        {
            T result = null;
            HttpContent content = null;


            using (var client = new HttpClient())
            {


                if (!string.IsNullOrEmpty(user) && !string.IsNullOrWhiteSpace(password))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                       Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}")));
                }


                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);

                    }
                }

                if (paraBody != null)
                {
                    content = new FormUrlEncodedContent(paraBody);

                }

                var response = await client.PostAsync(url, content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;

                    result = JsonConvert.DeserializeObject<T>(x.Result);

                });
            }

            return result;
        }

        public static async Task<T> PutRequest(string url, string user, string password, Dictionary<string, string> headers, string obj)
        {
            var data = new StringContent(obj, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            T result = null;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);

                }
            }

            var response = client.PostAsync(url, data).Result;
            try
            {
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        if (x.IsFaulted)
                            throw x.Exception;
                        try
                        {
                            JsonSerializerSettings settings = new JsonSerializerSettings
                            {
                                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                DateTimeZoneHandling = DateTimeZoneHandling.Local
                            };

                            result = JsonConvert.DeserializeObject<T>(x.Result, settings);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }

                    });

                }
                else 
                {
                    throw new ExternoTechnicalException(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new ExternoTechnicalException(response.StatusCode.ToString());
            }

            return result;
        }
        
        /// <summary>
        /// Diccionario que contiene los parámetros del query string. El key-pair representa el nombre del campo y su valor, respectivamente. 
        /// </summary>
        /// <param name="url"> Url de la api a llamar</param>
        /// <param name="parametros">Parárametros del query string. El key-value representa el campo y su valor</param>
        /// <returns>Valor genérico T</returns>
        public static async Task<T> Get(string url, Dictionary<string,string> parametros)
        {
            string cadena = "";
            if (parametros != null && parametros.Values.Any(v => !String.IsNullOrEmpty(v)))
            {
                url += "?";
                int pos = 0;

                foreach (var param in parametros)
                {
                    if (String.IsNullOrEmpty(param.Value))
                    {
                        pos++;
                        continue;
                    }
                    var vals = param.Value.Split(" ").Where(s => !String.IsNullOrEmpty(s)).ToList();
                    var querystring = string.Format("{0}={1}{2}", param.Key, String.Join("+", vals), pos < parametros.Count - 1 ? "&" : "");
                    cadena += querystring;
                    pos++;
                }
                if (cadena[cadena.Length - 1].Equals('&'))
                {
                    cadena = cadena.Substring(0, cadena.Length - 1);
                }

            }
            url += cadena;
            return await Get(url, null, null, null);
        }

        /// <summary>
        /// Metodo que ejecuta el llamado a un EndPoint de un API enviando en el body de la solicitud un JSON
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="headers"></param>
        /// <param name="paraBody"></param>
        /// <returns></returns>
        public static async Task<T> PostRequestEngineJson(string url, string user, string password, Dictionary<string, string> headers, string paraBody)
        {
            T result = null;
            HttpContent content = null;


            using (var client = new HttpClient())
            {


                if (!string.IsNullOrEmpty(user) && !string.IsNullOrWhiteSpace(password))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                       Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}")));
                }


                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);

                    }
                }

                if (paraBody != null)
                {
                    content = new StringContent(paraBody, Encoding.UTF8, "application/json");

                }

                var response = await client.PostAsync(url, content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;

                    result = JsonConvert.DeserializeObject<T>(x.Result);

                });
            }

            return result;
        }


    }
}
