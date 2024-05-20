using Newtonsoft.Json;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Helpers;
using ServDocumentos.Core.Mensajes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace ServDocumentos.Servicios.Comun
{
    class ServicioCampoImagen
    {


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

        private const string Accept = "Accept";
        private const string AcceptApplication = "application/vnd.mambu.v2+json";


        public byte [] Imagenbyte { get; set; }

        public void SetImagen(string tipo,string imagen)
        {
            try { 
            switch (tipo)
            {
                //Si es base 64
                case "Base64":
                        Imagenbyte = Convert.FromBase64String(imagen);
                       
                        break;
                //si es imagen url en carpeta local o desde web
                case "Url":

                    
                    using (var webClient = new WebClient())
                    {
                       Imagenbyte = webClient.DownloadData(imagen);
                    }


                    break;

                //Si es arreglo de bytes
                case "Api":

                    JsonImagen _jsonImagen = JsonConvert.DeserializeObject<JsonImagen>(imagen);


                        Dictionary<string, string> headers = new Dictionary<string, string>
                        {
                        { Accept, AcceptApplication }
                        };

                        Dictionary<string, string> parameters = new Dictionary<string, string>
                        {
                        { Accept, AcceptApplication }
                        };

                        Dictionary<string, string> body = new Dictionary<string, string>
                        {
                            { "definitionId","" }
                        };

                       
                        try
                        {

                            //var result=HTTPClientApi<HttpResponseMessage>.Get(_jsonImagen.Url, "", "", "POST",headers,parameters,"").Result;
                            //el result no tiene conversion

                            var result = HTTPClientApi.SendRequest(_jsonImagen.Url, "", "", "POST", headers, parameters, "").Result;


                            Imagenbyte = result.Content.ReadAsByteArrayAsync().Result;

                        }
                        catch (Exception ex)
                        {
                          
                                Imagenbyte = Convert.FromBase64String(MensajesServicios.ImagenDinamicaError);
                               

                        }

                    break;

            }
            }
            catch(Exception ex) 
            {

               Imagenbyte = Convert.FromBase64String(MensajesServicios.ImagenDinamicaError);

            }

        }
    }

}
