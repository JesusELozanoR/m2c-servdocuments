using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Helpers;
using System.Collections.Generic;
using ServDocumentos.Core.Dtos.DatosEngine;
using ServDocumentos.Core.Dtos.Comun.Documento;

namespace ServDocumentos.Servicios.Invercarmex
{
    public class ServicioDatosPlantillas : ServicioBase, IServicioDatosPlantillasObtencion
    {
        private const string Accept = "Accept";
        private const string AcceptApplication = "application/vnd.mambu.v2+json";


        public ServicioDatosPlantillas(GestorLog gestorLog, IConfiguration configuration) : base(gestorLog, configuration)
        {
        }

        public string ObtenerDatosPlantilla(ObtenerDatosDto solicitud)
        {
            //Datos solictud Consultando Engine
            gestorLog.Entrar();

            //Configuración Engine
            var configuracionEngine = configuration.GetSection("EngineAPI");
            var clientId = configuracionEngine["client_id"];
            var password = configuracionEngine["password"];
            var username = configuracionEngine["username"];
            var grantType = configuracionEngine["grant_type"];
            var clientSecret = configuracionEngine["client_secret"];
            var resource = configuracionEngine["resource"];
            var urlEngine = configuracionEngine["Url"];
            var UrlToken = configuracionEngine["UrlToken"];
            var OcpApimSubscriptionKey = configuracionEngine["Ocp-Apim-Subscription-Key"];
            var Authorization = configuracionEngine["Authorization"];
            var ContentType = configuracionEngine["Content-Type"];
            var definitionId = configuracionEngine["definitionId"];

            Dictionary<string, string> headersEngineToken = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                {"Cookie","fpc=Asv483ojSehMv1xyBbm0zovPH21vAQAAAONd69YOAAAA"}
            };

            Dictionary<string, string> BodyToken = new Dictionary<string, string>
            {
                { "client_id",clientId },
                { "password",password},
                { "username", username },
                { "grant_type" ,grantType},
                { "client_secret",clientSecret },
                { "resource", resource}
            };

            var token = HTTPClientWrapper<RespuestaToken>.PostRequestEngine(string.Format(UrlToken), "", "", headersEngineToken, BodyToken).Result;

            Dictionary<string, string> headersEngine = new Dictionary<string, string>
            {
                { "Ocp-Apim-Subscription-Key",OcpApimSubscriptionKey },
                {"Authorization",Authorization + token.access_token},
                {"Content-Type",ContentType}
            };

            Dictionary<string, string> Body = new Dictionary<string, string>
            {
                { "definitionId",definitionId }
            };

            var solicitudEngine = HTTPClientWrapper<DatosCliente>.PostRequestEngine(urlEngine, "", "", headersEngine, Body).Result;

            //Configuracion Mambu
            var configuracionMambu = configuration.GetSection("MambuAPI");
            var usuario = configuracionMambu["Usuario"];
            var contraseña = configuracionMambu["Password"];
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditosGET"];
            var credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;

            var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            var pagos = HTTPClientWrapper<List<Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;

            solicitudEngine.Credito = credito;
            solicitudEngine.Pagos = pagos;

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud)
        {
            //Datos solictud enviadas por Json
            gestorLog.Entrar();

            var solicitudEngine = JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud);

            //Configuracion Mambu
            // var configuracionMambu = configuration.GetSection("MambuAPI");
            //var usuario = configuracionMambu["Usuario"];
            //var contraseña = configuracionMambu["Password"];
            //var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
            //Dictionary<string, string> headers = new Dictionary<string, string>
            //{
            //{ Accept, AcceptApplication }
            //};

            // var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditosGET"];
            //var credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;

            // var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            //var pagos = HTTPClientWrapper<List<Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;

            //  solicitudEngine.Credito = credito;
            //solicitudEngine.Pagos = pagos;

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaAhorro(ObtenerDatosDto solicitud)
        {
            return "";
        }

        public string ObtenerDatosPlantillaAhorroJson(ObtenerDatosJsonDto solicitud)
        {
            return "";
        }

        public string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();

            string jsonEstadoCuenta = string.Empty;

            gestorLog.Salir();
            return jsonEstadoCuenta;
        }

        public SolictudDocumentoDto AsignarValores(ObtenerPlantillasProcesoDto solicitud)
        {
            return null;
        }

        public DocData EstadoCuentaObtenerDatos(SolictudDocumentoDto documSolicitud)
        {
            return null;
        }
        public ServDocumentos.Core.Dtos.DatosSybase.EstadoCuenta ObtenerDatosJsonEstadoCuenta(SolictudDocumentoDto documSolicitud)
        {
            return null;
        }



    }
}
