using cmn.std.Log;
using cmn.std.Parametros;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ServDocumentos.Core;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosCore;
using ServDocumentos.Core.Dtos.DatosEngine;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using Mambu = ServDocumentos.Core.Dtos.DatosMambu;
using ServSeguros = ServDocumentos.Core.Dtos.DatosServSeguros;
using sql = ServDocumentos.Core.Contratos.Factories.CAME.SQL;
using sybase = ServDocumentos.Core.Dtos.DatosSybase;

namespace ServDocumentos.Servicios.CAMEDIGITAL
{
    public class ServicioDatosPlantillas : ServicioBase, IServicioDatosPlantillasObtencion
    {
        private const string Accept = "Accept";
        private const string AcceptApplication = "application/vnd.mambu.v2+json";


        public ServicioDatosPlantillas(GestorParametros gestorParametros, comun.IUnitOfWork unitOfWork, sql.IUnitOfWork uowSQL, GestorLog gestorLog, IConfiguration configuration) : base(gestorParametros, unitOfWork, uowSQL, gestorLog, configuration)
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
            var configuracionMambu = configuration.GetSection("MambuCameAPI");
            var usuario = configuracionMambu["Usuario"];
            var contraseña = configuracionMambu["Password"];
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClienteGET"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditoGET"];
            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            PagosCameMambu pagos = null;
            try
            {
                pagos = HTTPClientWrapper<PagosCameMambu>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la tabla de pagos ");
                else
                    throw;
            }

            solicitudEngine.Credito = credito;

            solicitudEngine.Pagos = new List<Pagos>();
            Core.Dtos.DatosMambu.Pagos p = null;
            if (pagos != null && pagos.installments.Count > 0)
            {
                foreach (var item in pagos.installments)
                {
                    p = new Core.Dtos.DatosMambu.Pagos()
                    {
                        dueDate = Convert.ToDateTime(item.dueDate),
                        encodedKey = item.encodedKey,
                        feesDue = item.fee.amount.due.ToString(),
                        feesPaid = item.fee.amount.paid.ToString(),
                        interestDue = item.interest.amount.due.ToString(),
                        interestPaid = item.interest.amount.paid.ToString(),
                        penaltyDue = item.interest.amount.due.ToString(),
                        penaltyPaid = item.interest.amount.paid.ToString(),
                        principalDue = item.principal.amount.due.ToString(),
                        principalPaid = item.principal.amount.paid.ToString(),
                        taxFeesDue = item.fee.tax.due.ToString(),
                        taxFeesPaid = item.fee.tax.paid.ToString(),
                        taxInterestDue = item.interest.tax.due.ToString(),
                        taxInterestPaid = item.interest.tax.paid.ToString(),
                        taxPenaltyDue = item.penalty.tax.due.ToString(),
                        repaymentUnappliedFeeDetails = null,
                        taxPenaltyPaid = item.penalty.tax.paid.ToString(),
                        parentAccountKey = item.number,
                    };
                    solicitudEngine.Pagos.Add(p);
                }
            }

            solicitudEngine.NumeroCliente = solicitud.NumeroCliente;
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

            //cumun
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Decimal
            };
            var solicitudEngine = JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud, settings);

            //Configuracion Mambu
            var configuracionCAMEMambu = configuration.GetSection("MambuCameAPI");
            var usuario = ""; ///configuracionCAMEMambu["Usuario"];
            var contraseña = ""; // configuracionCAMEMambu["Password"];
            var urlCliente = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["clientesGET"];
            var urlCredito = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["CreditoGET"];
            var urlPago = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["PagosGet"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionCAMEMambu["HeaderAut"],configuracionCAMEMambu["HeaderValAut"]  },
                { configuracionCAMEMambu["HeaderCoo"],configuracionCAMEMambu["HeaderValCoo"]  }
            };

            Dictionary<string, string> headerPag = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionCAMEMambu["HeaderAut"],configuracionCAMEMambu["HeaderValAutPag"]  },
                { configuracionCAMEMambu["HeaderCoo"],configuracionCAMEMambu["HeaderValCooPag"]  }
            };

            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            Cliente cliente = null;
            try
            {
                cliente = HTTPClientWrapper<Cliente>.Get(string.Format(urlCliente, credito.accountHolderKey), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existen datos del cliente. ");
                else
                    throw;
            }
            bool conCuentaAhorroLigada = false;
            if (!string.IsNullOrEmpty(credito.settlementAccountKey))
            {
                AhorroDto ahorro = null;
                conCuentaAhorroLigada = true;
                try
                {
                    var urlAhorro = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["AhorroGET"];
                    ahorro = HTTPClientWrapper<AhorroDto>.Get(string.Format(urlAhorro, credito.settlementAccountKey), usuario, contraseña, headers).Result;
                    if (ahorro._Referencias_Deposito != null)
                    {
                        if (credito._Referencias_Pago != null)
                            credito._Referencias_Pago.Clear();
                        credito._Referencias_Pago = ahorro._Referencias_Deposito.Select(x => new ReferenciasPago()
                        {
                            Referencia_Pago_Tipo = x.Referencia_Deposito_Tipo,
                            Referencia_Pago_Valor = x.Referencia_Deposito_Valor
                        }).ToList();
                    }
                    else
                        conCuentaAhorroLigada = false;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No existen datos del cliente. ");
                    else
                        throw;
                }
            }

            PagosCameMambu pagos = null;
            try
            {
                pagos = HTTPClientWrapper<PagosCameMambu>.Get(string.Format(urlPago, solicitud.NumeroCredito), usuario, contraseña, headerPag).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la tabla de pagos ");
                else
                    throw;
            }

            var datosMambu = Equivalencias(credito, pagos, solicitud.SubProcesoNombre);
            if (datosMambu != null)
            {

                string frecuenciaPago = "";
                if (!string.IsNullOrEmpty(solicitudEngine.Credito?.FrecuenciaPago))
                {
                    frecuenciaPago = solicitudEngine.Credito?.FrecuenciaPago;
                }

                int plazoPago = 0;
                if (solicitudEngine.Credito?.Plazo > 0)
                {
                    plazoPago = solicitudEngine.Credito.Plazo;
                }

                int nuneroCreditos = 0;
                if (solicitudEngine.Credito?.NumeroCreditos > 0)
                    nuneroCreditos = solicitudEngine.Credito.NumeroCreditos;

                int cuotaAumento = 0;
                if (solicitudEngine.Credito?.CuotaAumento > 0)
                    cuotaAumento = (int)solicitudEngine.Credito.CuotaAumento;
                string paqBasico = solicitudEngine.Credito?.PaqBasico;
                string paqPlatino = solicitudEngine.Credito?.PaqPlatino;
                string paqPremium = solicitudEngine.Credito?.PaqPremium;

                solicitudEngine.Credito = datosMambu.Credito;

                solicitudEngine.Credito.ConCuentaAhorroLigada = conCuentaAhorroLigada;
                solicitudEngine.Credito.NumeroCreditos = nuneroCreditos;
                solicitudEngine.TablaAmortizacion = datosMambu.TablaAmortizacion;
                solicitudEngine.TablaAmortizacionPagare = datosMambu.TablaAmortizacionPagare;
                solicitudEngine.Credito.CuotaAumento = cuotaAumento;

                if (!string.IsNullOrEmpty(frecuenciaPago))
                    solicitudEngine.Credito.FrecuenciaPago = frecuenciaPago;

                if (plazoPago > 0)
                    solicitudEngine.Credito.Plazo = plazoPago;

                if (!string.IsNullOrEmpty(paqBasico))
                    solicitudEngine.Credito.PaqBasico = paqBasico;

                if (!string.IsNullOrEmpty(paqPlatino))
                    solicitudEngine.Credito.PaqPlatino = paqPlatino;

                if (!string.IsNullOrEmpty(paqPremium))
                    solicitudEngine.Credito.PaqPremium = paqPremium;

            }

            solicitudEngine.Telefono = cliente.mobilePhone;
            solicitudEngine.NumeroClienteMambu = cliente.id;
            solicitudEngine.NombreCompleto = cliente.firstName + " " + cliente.lastName;
            solicitudEngine.Nombre = cliente.firstName;
            solicitudEngine.NumeroCliente = cliente.id;
            List<string> subprocesos = new List<string> { "PALQUETRABAJA", "TECHREOPALNEGOCIO", "PALACASA", "PALEMPRESARIO", "CREDITOCASHI" };
            if (solicitud.ProcesoNombre == "CreditoCameDigital")
            {
                if (subprocesos.Contains(solicitud.SubProcesoNombre.ToUpper()))
                {
                    AjustePaquetesAsistencia(ref solicitudEngine, solicitud, cliente);
                }
                List<string> subprocesosDatosCliente = new List<string> { "PALQUETRABAJA", "PALACASA", "PALEMPRESARIO", "CREDITOCASHI"};
                if (subprocesosDatosCliente.Contains(solicitud.SubProcesoNombre.ToUpper()))
                {
                    ObtenerDatosCliente(ref solicitudEngine, cliente);
                }
            }

            if (solicitud.ProcesoNombre.ToUpper() == "CREDITOCAME" && solicitud.SubProcesoNombre.ToUpper() == "CREDITO_MERCADO")
            {
                AjustePaquetesAsistencia(ref solicitudEngine, solicitud, cliente);
            }

            settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            solicitudEngine.solicitudJSON = JsonConvert.DeserializeObject<object>(solicitud.JsonSolicitud);
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            solicitudEngine.CAT = Convert.ToDecimal(credito._Datos_Adicionales?.CAT_Datos_Adicionales ?? "0");
            gestorLog.Registrar(Nivel.Information, "Seguimiento CAT:" + solicitudEngine.CAT);
            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaAhorro(ObtenerDatosDto solicitud)
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
            var configuracionMambu = configuration.GetSection("MambuCameAPI");
            var usuario = configuracionMambu["Usuario"];
            var contraseña = configuracionMambu["Password"];
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClienteGET"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditoGET"];
            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            List<Pagos> pagos = null;
            try
            {
                pagos = HTTPClientWrapper<List<Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se obtuvo la lista de pagos.");
                else
                    throw;
            }

            solicitudEngine.Credito = credito;
            solicitudEngine.Pagos = pagos;

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaAhorroJson(ObtenerDatosJsonDto solicitud)
        {
            const string Accept = "Accept";
            const string AcceptApplication = "application/vnd.mambu.v2+json";

            gestorLog.Entrar();

            //DESERIALIZA JSON DE SOLICITUD
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Decimal
            };
            var solicitudEngine = JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud, settings);

            //SE OBTIENE LOS DATOS PARA CONEXION A MAMBU
            var configuracionMambu = configuration.GetSection("MambuCameAPI");

            var usuario = "";
            var contraseña = "";
            Dictionary<string, string> headers = new Dictionary<string, string>
            {

                { Accept, AcceptApplication },
                { configuracionMambu["HeaderAut"],configuracionMambu["HeaderValAut"]  },
                { configuracionMambu["HeaderCoo"],configuracionMambu["HeaderValCoo"]  },

            };
            //OBTENER AHORRO MAMBU
            var urlAhorro = configuracionMambu["Url"] + configuracionMambu["AhorroGET"];

            Mambu.AhorroDto ahorro = null;
            try
            {
                ahorro = HTTPClientWrapper<Mambu.AhorroDto>.Get(string.Format(urlAhorro, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
                //se cambia el orden dependiendo del index mambu
                ahorro._Relaciones_Ahorro = ahorro._Relaciones_Ahorro.OrderBy(o => o._index).ToList();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se obtuvo el credito.");
                else
                    throw;
            }


            ClientesMambuDto solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };
            var urlCotitulares = configuracionMambu["Url"] + configuracionMambu["ClientesPOST"];

            //OBTENER COTITULARES POR CLIENTE
            List<string> lUsuarios = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "COTITULAR" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuarios.Add(item._Encoded_Key_Relacionado);
            }

            List<Mambu.Cliente> cotitulares = new List<Mambu.Cliente>();
            if (lUsuarios.Count > 0)
            {

                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuarios });
                var json = JsonConvert.SerializeObject(solCliente);

                try
                {
                    cotitulares = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                    //se cambia el orden dependiendo del index mambu
                    cotitulares = cotitulares.OrderBy(i => ahorro._Relaciones_Ahorro.FindIndex(o => o._Encoded_Key_Relacionado == i.encodedKey)).ToList();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se obtuvo la lista de pagos.");
                    else
                        throw;
                }

            }


            //se trae los tutores
            List<string> lUsuariosTutores = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "PADRE O TUTOR" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuariosTutores.Add(item._Encoded_Key_Relacionado);
            }

            List<Mambu.Cliente> tutores = new List<Mambu.Cliente>();
            if (lUsuariosTutores.Count > 0)
            {
                solCliente = new ClientesMambuDto() { FilterCriteria = new List<filterCriteria>() };
                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuariosTutores });
                var json = JsonConvert.SerializeObject(solCliente);

                try
                {
                    tutores = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se obtuvo la lista de tutores.");
                    else
                        throw;
                }
            }


            //trea al representante legal
            List<string> lUsuariosRepresentantes = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "REPRESENTANTE LEGAL" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuariosRepresentantes.Add(item._Encoded_Key_Relacionado);
            }

            List<Mambu.Cliente> representantes = new List<Mambu.Cliente>();
            if (lUsuariosRepresentantes.Count > 0)
            {
                solCliente = new ClientesMambuDto() { FilterCriteria = new List<filterCriteria>() };
                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuariosRepresentantes });
                var json = JsonConvert.SerializeObject(solCliente);

                try
                {
                    representantes = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se obtuvo la lista de representantes.");
                    else
                        throw;
                }
            }


            usuario = configuracionMambu["Usuario"];
            contraseña = configuracionMambu["Password"];
            headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var cliente = (dynamic)null;
            //Mambu.Cliente cliente = new Mambu.Cliente();
            try
            {
                // VALIDAR SI ES GRUPAL O INDIVIDUAL
                if (ahorro.AccountHolderType == "CLIENT")
                {
                    //OBTNER CLIENTE MAMBU
                    var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
                    cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlCliente, solicitud.NumeroCliente), usuario, contraseña, headers).Result;
                }
                else
                {
                    //OBTENER GRUPOS MAMBU
                    var urlGrupo = configuracionMambu["Url"] + configuracionMambu["GroupsGET"];
                    cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlGrupo, ahorro.AccountHolderKey), usuario, contraseña, headers).Result;
                }

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se obtuvieron los datos del cliente.");
                else
                    throw;
            }

            //MAPEAR CAMPOS MAMBU A DTO
            solicitudEngine = Equivalencias(ahorro, cliente, cotitulares, solicitudEngine);

            solicitudEngine = RepresentanteTutores(solicitudEngine, representantes, tutores, cliente);

            if (solicitud.SubProcesoNombre == "CamePatrimonial")
            {
                //368-22.AjusteConstanciaInversionPatrimonialMainstreet
                solicitudEngine.Estado = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";

                if (tutores != null && tutores.Count != 0)
                {
                    solicitudEngine.NombreCompletoFirma = $"{cliente?.firstName} {cliente?.lastName}";
                }

                if (representantes != null && representantes.Count != 0)
                {
                    solicitudEngine.NombreCompletoFirma = solicitudEngine.NombreCompleto;

                    //368-22.AjusteConstanciaInversionPatrimonialMainstreet
                    solicitudEngine.Estado = representantes[0]?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";

                }

                if (!string.IsNullOrWhiteSpace(solicitudEngine.Ciudad))
                {
                    solicitudEngine.Estado = solicitudEngine.Ciudad;
                }
            }

            //MAPEO PARA LOS COMPROBANTES DE CLUBCAME Y CRECE+
            if ((solicitud.SubProcesoNombre.ToLower() == "clubcamecompinv") || (solicitud.SubProcesoNombre.ToLower() == "crecemascompinv") || (solicitud.SubProcesoNombre.ToLower() == "clubcameticketinv") || (solicitud.SubProcesoNombre.ToLower() == "crecemasticketinv")
                || (solicitud.SubProcesoNombre.ToLower() == "crecemasempresas"))
            {
                solicitudEngine = AjustarComprobantesClubCameCreceMas(solicitud, solicitudEngine, ahorro, usuario, contraseña, headers, configuracionMambu);
            }

            //MAPEO PARA EL REPORTE DE OPERACIONES DEL PRODUCTO X ADELA
            if (solicitud.SubProcesoNombre == "XAdela")
            {
                solicitudEngine = AjustarComprobantesXADELA(solicitud, solicitudEngine, ahorro, usuario, contraseña, headers, configuracionMambu);
            }

            settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            solicitudEngine.solicitudJSON = JsonConvert.DeserializeObject<object>(solicitud.JsonSolicitud);
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);


            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaInversion(ObtenerDatosDto solicitud)
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
            var configuracionMambu = configuration.GetSection("MambuCameAPI");
            var usuario = configuracionMambu["Usuario"];
            var contraseña = configuracionMambu["Password"];
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClienteGET"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditoGET"];
            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se obtuvieron los datos del credito.");
                else
                    throw;
            }

            var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            List<Pagos> pagos = null;
            try
            {
                pagos = HTTPClientWrapper<List<Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se obtuvieron los datos de los pagos.");
                else
                    throw;
            }

            solicitudEngine.Credito = credito;
            solicitudEngine.Pagos = pagos;

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaInversionJson(ObtenerDatosJsonDto solicitud)
        {
            //Datos solictud enviadas por Json
            gestorLog.Entrar();

            var solicitudEngine = JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud);

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Decimal
            };

            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud)
        {
            return "";
        }

        private DatosSolicitud RepresentanteTutores(DatosSolicitud solicitudEngine, List<Mambu.Cliente> representantes, List<Mambu.Cliente> tutores, Mambu.Cliente cliente)
        {
            solicitudEngine.NombreCompletoFirma = solicitudEngine.NombreCompleto;
            if (representantes != null && representantes.Count != 0)
            {
                solicitudEngine.NombreCompletoMoral = $"{representantes[0].firstName} {representantes[0].lastName}";
                solicitudEngine.NombreCompletoFirma = "";
                solicitudEngine.NombreCompletoTutor = "";
            }
            else
            {

                if (tutores != null && tutores.Count != 0)
                {
                    solicitudEngine.NombreCompletoMoral = "";
                    solicitudEngine.NombreCompletoFirma = "";
                    solicitudEngine.NombreCompletoTutor = $"{tutores[0].firstName} {tutores[0].lastName}";
                }
                else
                {
                    solicitudEngine.NombreCompletoMoral = "";
                    solicitudEngine.NombreCompletoTutor = "";
                }
            }

            DatosRepresentantes representante;
            solicitudEngine.Representantes = new List<DatosRepresentantes>();

            foreach (var item in representantes)
            {
                representante = new DatosRepresentantes();
                representante.NombreCompleto = $"{item.firstName} {item.lastName}";
                solicitudEngine.Representantes.Add(representante);
            };
            DatosTutores tutor;
            solicitudEngine.Tutores = new List<DatosTutores>();
            foreach (var item in tutores)
            {
                tutor = new DatosTutores();
                tutor.NombreCompleto = $"{item.firstName} {item.lastName}";
                solicitudEngine.Tutores.Add(tutor);
            };

            return solicitudEngine;
        }

        public SolictudDocumentoDto AsignarValores(ObtenerPlantillasProcesoDto solicitud)
        {
            return null;
        }

        public DocData EstadoCuentaObtenerDatos(SolictudDocumentoDto documSolicitud)
        {
            gestorLog.Entrar();
            DocData datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            gestorLog.Salir();
            return datosDocumento;
        }

        public sybase.EstadoCuenta ObtenerDatosJsonEstadoCuenta(SolictudDocumentoDto documSolicitud)
        {
            return null;
        }

        private DatosSolicitud Equivalencias(Credito credito, PagosCameMambu pagos, string subProceso)
        {
            gestorLog.Entrar();

            DatosSolicitud jsonDatosSolicitud = null;
            List<DatosTablaAmortizacion> listaTablaAmortizacion = null;
            DatosTablaAmortizacion tablaAmortizacion = null;
            DatosCredito datosCredito = null;
            List<DatosTablaAmortizacionPagare> listaPagare = null;
            DatosTablaAmortizacionPagare pagare = null;
            double valor = 0;
            double monto = 0;
            double montoFees = 0;
            try
            {
                string fechaAnterior = "";
                jsonDatosSolicitud = new DatosSolicitud();

                listaTablaAmortizacion = new List<DatosTablaAmortizacion>();
                listaPagare = new List<DatosTablaAmortizacionPagare>();
                monto = credito.loanAmount;
                List<string> subProcesosSumaTarifasACapital = new List<string> { "PALQUETRABAJA", "CAMEPALNEGOCIO", "TECHREOPALNEGOCIO", "PALACASA", "PALEMPRESARIO", "CREDITO_MERCADO", "CREDITOCASHI" };
                if (listaTablaAmortizacion.Count == 0 && (subProcesosSumaTarifasACapital.Contains(subProceso.ToUpper())))
                {
                    if (fechaAnterior == "")
                        fechaAnterior = credito.creationDate.AddDays(-1).ToString();
                    montoFees = credito.disbursementDetails.fees.Sum(i => i.amount);
                    //monto = monto + montoFees;
                    monto = pagos.installments.Sum(x => x.principal.amount.due);
                }

                foreach (var item in pagos.installments)
                {
                    List<string> subProcesosPagoCero = new List<string> { "PALQUETRABAJA", "CAMEPALNEGOCIO", "TECHREOPALNEGOCIO", "RENOVACIONINDIVIDUAL", "PALACASA", "PALEMPRESARIO", "CREDITO_MERCADO", "CREDITOCASHI" };
                    if (listaTablaAmortizacion.Count == 0 && (subProcesosPagoCero.Contains(subProceso.ToUpper())))
                    {
                        tablaAmortizacion = PagoCero(item, monto, out valor);

                        List<string> subProcesosSaldoInsolutoPrincipal = new List<string> { "PALQUETRABAJA", "CAMEPALNEGOCIO", "TECHREOPALNEGOCIO", "PALACASA", "PALEMPRESARIO", "CREDITO_MERCADO", "CREDITOCASHI" };
                        if (subProcesosSaldoInsolutoPrincipal.Contains(subProceso.ToUpper()))
                        {
                            tablaAmortizacion.SaldoInsolutoPricipal = (decimal)monto;
                        }

                        listaTablaAmortizacion.Add(tablaAmortizacion);
                    }

                    tablaAmortizacion = PagosToTablaAmortizacion(item, monto, out valor);
                    if (fechaAnterior != "")
                    {
                        tablaAmortizacion.FechaInicio = Convert.ToDateTime(fechaAnterior).AddDays(1).ToString("dd/MM/yyyy");
                    }
                    listaTablaAmortizacion.Add(tablaAmortizacion);
                    monto = valor;
                    pagare = PagosToTablaAmortizacionPagare(item);
                    listaPagare.Add(pagare);
                    fechaAnterior = item.dueDate;
                }

                List<string> subprocesosCreditoToDatosCreditoMercado = new List<string> { "CREDITO_MERCADO", "CREDITOCASHI" };
                if (subprocesosCreditoToDatosCreditoMercado.Contains(subProceso.ToUpper()))
                    datosCredito = CreditoToDatosCreditoMercado(credito);
                else
                    datosCredito = CreditoToDatosCredito(credito);

                datosCredito.CuotaMensual = listaPagare[0].Monto;
                datosCredito.FechaPrimerPago = listaPagare[0].Fecha;
                datosCredito.MontoTotal = listaTablaAmortizacion.Sum(i => i.PagoTotal);
                jsonDatosSolicitud.TablaAmortizacion = listaTablaAmortizacion;
                jsonDatosSolicitud.TablaAmortizacionPagare = listaPagare;
                jsonDatosSolicitud.Credito = datosCredito;
                jsonDatosSolicitud.Credito.MontoCreditoNeto = pagos.installments.Sum(x => x.principal.amount.due);
                jsonDatosSolicitud.Credito.FechaFinal = Convert.ToDateTime(pagos.installments.Select(s => s.dueDate).Last()).ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }


            return jsonDatosSolicitud;
        }

        private DatosTablaAmortizacion PagosToTablaAmortizacion(Installment pago, double montoCredito, out double valor)
        {
            DatosTablaAmortizacion tablaAmortizacion = new DatosTablaAmortizacion();
            double valor1 = montoCredito;
            valor = 0;
            try
            {
                tablaAmortizacion.NumeroPago = Convert.ToInt16(pago.number);
                tablaAmortizacion.FechaLimitePago = Convert.ToDateTime(fechaOffset(pago.dueDate)).ToString("dd/MM/yyyy");
                tablaAmortizacion.PagoInteres = Convert.ToDecimal(pago.interest.amount.due - pago.interest.tax.due);
                tablaAmortizacion.PagoPricipal = Convert.ToDecimal(pago.principal.amount.due);
                tablaAmortizacion.PagoTotal = Convert.ToDecimal(pago.principal.amount.due) + Convert.ToDecimal(pago.interest.amount.due) + Convert.ToDecimal(pago.fee.amount.due) + Convert.ToDecimal(pago.penalty.amount.due);
                tablaAmortizacion.SaldoInicial = Convert.ToDecimal(valor1);
                valor = montoCredito - pago.principal.amount.due;
                tablaAmortizacion.SaldoInsolutoPricipal = Convert.ToDecimal(valor);
                tablaAmortizacion.IVAInteres = Convert.ToDecimal(pago.interest.tax.due);
            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }
            return tablaAmortizacion;
        }

        private DatosTablaAmortizacion PagoCero(Installment pago, double montoCredito, out double valor)
        {
            DatosTablaAmortizacion tablaAmortizacion = new DatosTablaAmortizacion();
            double valor1 = montoCredito;
            valor = 0;
            try
            {
                tablaAmortizacion.NumeroPago = 0;
                tablaAmortizacion.FechaLimitePago = " ";
                tablaAmortizacion.PagoTotal = Convert.ToDecimal(pago.principal.amount.due) + Convert.ToDecimal(pago.interest.amount.due) + Convert.ToDecimal(pago.fee.amount.due) + Convert.ToDecimal(pago.penalty.amount.due);
                tablaAmortizacion.SaldoInicial = Convert.ToDecimal(montoCredito);
                valor = montoCredito - pago.principal.amount.due;
                tablaAmortizacion.SaldoInsolutoPricipal = Convert.ToDecimal(valor);
                tablaAmortizacion.PagoTotal = 0;
            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }
            return tablaAmortizacion;
        }

        private DatosTablaAmortizacionPagare PagosToTablaAmortizacionPagare(Installment pago)
        {
            DatosTablaAmortizacionPagare pagare = new DatosTablaAmortizacionPagare();
            try
            {
                pagare.NumeroMensual = Convert.ToInt16(pago.number);
                pagare.Fecha = Convert.ToDateTime(fechaOffset(pago.dueDate)).ToString("dd/MM/yyyy");
                // pagare.Monto = Convert.ToDecimal(pago.principal.amount.due);
                pagare.Monto = Convert.ToDecimal(pago.principal.amount.due) + Convert.ToDecimal(pago.interest.amount.due) + Convert.ToDecimal(pago.fee.amount.due) + Convert.ToDecimal(pago.penalty.amount.due);
            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }
            return pagare;
        }


        private DatosCredito CreditoToDatosCredito(Credito credito)
        {
            DatosCredito datosCredito = new DatosCredito();
            try
            {

                datosCredito.NumeroCredito = credito.id;
                // datosCredito.NumeroCreditos = 0;
                datosCredito.NumeroCliente = credito.accountHolderKey;
                datosCredito.FechaInicio = credito.creationDate.ToString("dd/MM/yyyy");
                datosCredito.Producto = credito.loanName;
                datosCredito.MontoCredito = Convert.ToDecimal(credito.loanAmount);
                datosCredito.EstatusCredito = credito.accountState;
                //se cambia el *12 porque ya tiene el anual
                datosCredito.TasaInteresAnual = Convert.ToDecimal(credito.interestSettings.interestRate);
                datosCredito.TasaMoratoriaAnual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 360);
                datosCredito.InteresMoratorioMensual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 30);
                datosCredito.PlazoGarcia = "";
                datosCredito.Plazo = credito.scheduleSettings.repaymentInstallments;
                datosCredito.FrecuenciaPago = credito.scheduleSettings.repaymentPeriodUnit;
                datosCredito.DiasFrecuenciaPago = credito.scheduleSettings.repaymentPeriodCount;
                datosCredito.FechaDesembolso = credito.disbursementDetails.expectedDisbursementDate.ToString("dd/MM/yyyy");

                if (datosCredito.FechaDesembolso == "01/01/0001")
                    datosCredito.FechaDesembolso = credito.creationDate.ToString("dd/MM/yyyy");

                if (credito._Datos_Adicionales != null)
                {
                    //datosCredito.MontoTotal = Convert.ToDecimal(credito._Datos_Adicionales.Total_Pagar);
                    if (credito._Datos_Adicionales.Monto_Seguro_Basico > 0)
                        datosCredito.PaqBasico = "X";
                    else
                        datosCredito.PaqBasico = " ";

                    if (credito._Datos_Adicionales.Tipo_Seguro_Voluntario == "PLATINO")
                        datosCredito.PaqPlatino = "X";
                    else
                        datosCredito.PaqPlatino = " ";

                    if (credito._Datos_Adicionales.Tipo_Seguro_Voluntario == "PREMIUM")
                        datosCredito.PaqPremium = "X";
                    else
                        datosCredito.PaqPremium = " ";
                    datosCredito.CAT = Convert.ToDecimal(credito._Datos_Adicionales.CAT_Datos_Adicionales);

                }
                datosCredito.GATReal = 17;
                datosCredito.GATNominal = 17;
                datosCredito.MedioDisposicionEfectivo = "X";
                datosCredito.MedioDisposicionChequera = "";
                datosCredito.MedioDisposicionTransferencia = "";
                datosCredito.LugarEfectuarRetiroVentanilla = "X";
                datosCredito.EnvioEstadoCuentaDom = "X";
                datosCredito.EnvioEstadoCuentaEmail = "";
                //datosCredito.TipoRenovacion = "Solo Capital";   


                if (credito._Referencias_Pago != null)
                {

                    foreach (var item in credito._Referencias_Pago)
                    {
                        switch (item.Referencia_Pago_Tipo)
                        {
                            case "BBVABANCOMER":
                            case "BANAMEX":
                                if (!string.IsNullOrEmpty(item.Referencia_Pago_Valor))
                                {
                                    datosCredito.ReferenciaBBVA = item.Referencia_Pago_Valor;
                                    datosCredito.ReferenciaCitibanamex = item.Referencia_Pago_Valor;
                                }
                                break;
                            case "BANBAJIO":
                            case "TELECOMM":
                            case "SCOTIABANK":
                                if (!string.IsNullOrEmpty(item.Referencia_Pago_Valor))
                                {
                                    datosCredito.ReferenciaBanBajio = item.Referencia_Pago_Valor;
                                    datosCredito.ReferenciaScontiabank = item.Referencia_Pago_Valor;
                                    datosCredito.ReferenciaTelecomm = item.Referencia_Pago_Valor;
                                }
                                break;
                            case "OXXOPAY":
                                datosCredito.ReferenciaOXXO = item.Referencia_Pago_Valor;
                                break;
                            case "HSBC":
                                datosCredito.ReferenciaBANSEFI = item.Referencia_Pago_Valor;
                                break;
                            case "CAME":
                                datosCredito.ReferenciaCame = item.Referencia_Pago_Valor;
                                break;
                            default:
                                break;
                        }
                    }
                }


            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }
            return datosCredito;
        }

        private DatosSolicitud Equivalencias(Mambu.AhorroDto ahorro, Mambu.Cliente cliente, List<Mambu.Cliente> cotitulares, DatosSolicitud solicitudEngine)
        {
            gestorLog.Entrar();

            try
            {

                solicitudEngine.Ahorro = AhorroToDatosAhorro(ahorro);

                if (cliente.firstName != null)
                    solicitudEngine.NombreCompleto = $"{cliente.firstName} {cliente.lastName}";

                if (cliente.groupName != null)
                    solicitudEngine.NombreCompleto = $"{cliente.groupName}";

                if (cliente._Direccion_Clientes != null)
                    solicitudEngine.Ciudad = $"{cliente._Direccion_Clientes.Municipio_Direccion_Cliente}";

                DatosCotitulares cotitular = null;
                solicitudEngine.Cotitulares = new List<DatosCotitulares>();

                foreach (var item in cotitulares)
                {
                    cotitular = new DatosCotitulares();
                    cotitular.NombreCompleto = $"{item.firstName} {item.lastName}";
                    solicitudEngine.Cotitulares.Add(cotitular);
                };

                solicitudEngine.NumeroCliente = cliente.id;
                solicitudEngine.Ahorro.CuentaDeposito = string.IsNullOrEmpty(ahorro._Datos_Adicionales_Cuenta?.Cuenta_Deposito) ? "" : ahorro._Datos_Adicionales_Cuenta?.Cuenta_Deposito;
                solicitudEngine.Ahorro.FechaVencimiento = string.IsNullOrEmpty(ahorro.maturityDate) ? "" : ahorro.maturityDate;
                solicitudEngine.Ahorro.CuentaAhorro = string.IsNullOrEmpty(ahorro.Id) ? "" : ahorro.Id;
                solicitudEngine.Ahorro.FechaApertura = string.IsNullOrEmpty(ahorro.CreationDate) ? "" : ahorro.CreationDate;
                solicitudEngine.Ahorro.InstruccionesTipoPlazo = string.IsNullOrEmpty(ahorro._Datos_Adicionales_Cuenta?._Instrucciones_Tipo_Plazo) ? "" : ahorro._Datos_Adicionales_Cuenta?._Instrucciones_Tipo_Plazo;
                solicitudEngine.Ahorro.InstruccionesVencimiento = string.IsNullOrEmpty(ahorro._Datos_Adicionales_Cuenta?._Instrucciones_Vencimiento) ? "" : ahorro._Datos_Adicionales_Cuenta?._Instrucciones_Vencimiento;
                solicitudEngine.Ahorro.FechaActivacion = string.IsNullOrEmpty(ahorro.ActivationDate) ? "" : ahorro.ActivationDate;

            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }

            return solicitudEngine;
        }

        private DatosAhorro AhorroToDatosAhorro(Mambu.AhorroDto ahorro)
        {
            gestorLog.Entrar();
            DatosAhorro datosAhorro = null;

            try
            {
                datosAhorro = new DatosAhorro();
                datosAhorro.TasaAhorro = Convert.ToDecimal(ahorro.InterestSettings?.InterestRateSettings?.InterestRate ?? "0");
                datosAhorro.GAT = ahorro._Datos_Adicionales_Cuenta?.Gat_Nominal ?? 0;
                datosAhorro.GatReal = ahorro._Datos_Adicionales_Cuenta?.Gat_Real ?? 0;
                datosAhorro.Contrato = ahorro.Id;
                datosAhorro.Plazo = ahorro._Datos_Adicionales_Cuenta?._Plazo ?? 0;
                datosAhorro.NoConstancia = ahorro._Datos_Adicionales_Cuenta?.No_Constancia ?? 0;
                datosAhorro.Monto = Convert.ToDecimal(ahorro.InternalControls.RecommendedDepositAmount);
                datosAhorro.DiasTermino = ahorro._Datos_Adicionales_Cuenta?.Dias_Termino_Inversion ?? 0;
                datosAhorro.TipoPlazo = ahorro._Datos_Adicionales_Cuenta._Tipo_Plazo;


            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new BusinessException("Hubo un problema al mapear los datos de Ahorro.");
            }

            return datosAhorro;
        }

        public string ObtenerDatosPlantillaBC2020(ObtenerDatosDto solicitud)
        {
            //Datos solictud enviadas por Json
            gestorLog.Entrar();

            var CoreBankAPI = configuration.GetSection("CoreBankAPI");
            //Configuracion Mambu
            var configuracionCAMEMambu = configuration.GetSection("MambuCameAPI");
            var usuario = "";//configuracionCAMEMambu["Usuario"];
            var contraseña = "";// configuracionCAMEMambu["Password"];
            var usuarioDoc = configuracionCAMEMambu["UsuarioCore"];
            var passwordDoc = configuracionCAMEMambu["PasswordCore"];
            var urlCliente = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["ClientesGET"];
            var urlCredito = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["CreditoGET"];
            var urlSucursal = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["SucursalGET"];
            var urlAhorro = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["AhorroGET"];
            var urlCotitulares = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["ClientesPOST"];
            var urlPagos = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["PagosGET"];
            var urlProductos = configuracionCAMEMambu["Url"] + Constantes.loansProductById;
            var urlRef = CoreBankAPI["Url"] + CoreBankAPI["ReferenciasGet"];
            string core = CoreBankAPI["Core"];
            var tipoOperacion = configuracionCAMEMambu["TipoOperacion"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionCAMEMambu["HeaderAut"],configuracionCAMEMambu["HeaderValAut"]  },
                { configuracionCAMEMambu["HeaderCoo"],configuracionCAMEMambu["HeaderValCoo"]  }
            };

            Dictionary<string, string> headerPag = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };


            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            // Obtiene Nombre Integrantes
            if (credito._Integrantes_Credito != null && credito._Integrantes_Credito.Count > 0)
            {
                foreach (IntegranteCredito integrante in credito._Integrantes_Credito)
                {
                    try
                    {
                        if (integrante.Rol_integrante == "Tesorero")
                        {
                            Mambu.Cliente ResIntegrante = HTTPClientWrapper<Cliente>.Get(string.Format(urlCliente, integrante.Cliente_Id), usuario, contraseña, headers).Result;
                            integrante.NombreCompleto = $"{ResIntegrante?.firstName ?? ""} {ResIntegrante?.lastName ?? ""}";
                            if (ResIntegrante._Direccion_Clientes != null)
                            {
                                integrante.Direccion = (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Calle_Direccion_Cliente) ? "" : "Calle ") + $"{ResIntegrante._Direccion_Clientes?.Calle_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Numero_Ext_Direccion_Cliente) ? "" : ", ") + $"{ResIntegrante._Direccion_Clientes?.Numero_Ext_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Numero_Int_Direccion_Cliente) ? "" : "-") + $"{ResIntegrante._Direccion_Clientes?.Numero_Int_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Colonia_Direccion_Cliente) ? "" : ", ") + $"{ResIntegrante._Direccion_Clientes?.Colonia_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.CP_Direccion_Cliente) ? "" : ", C.P. ") + $"{ResIntegrante._Direccion_Clientes?.CP_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Municipio_Direccion_Cliente) ? "" : ", ") + $"{ResIntegrante._Direccion_Clientes?.Municipio_Direccion_Cliente ?? ""} " +
                                    (string.IsNullOrWhiteSpace(ResIntegrante._Direccion_Clientes.Estado_Direccion_Cliente) ? "" : ", ") + $"{ResIntegrante._Direccion_Clientes?.Estado_Direccion_Cliente ?? ""} ";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message == "NotFound")
                            throw new ServiceException("No existe el integhrante del Credito.  ");
                        else
                            throw;
                    }

                }
            }

            Cliente cliente = null;
            try
            {
                string numeroCredito = string.Empty;
                numeroCredito = credito.accountHolderKey;

                if (solicitud.Subproceso == "CAME_INTERCICLO")
                {
                    if (credito._Relaciones_Credito == null ||
                        credito._Relaciones_Credito.Titular_Credito == null ||
                        string.IsNullOrWhiteSpace(credito._Relaciones_Credito.Titular_Credito))
                    {
                        throw new ServiceException("No existen datos del cliente real.");
                    }
                    numeroCredito = credito._Relaciones_Credito.Titular_Credito;
                }
                cliente = HTTPClientWrapper<Cliente>.Get(string.Format(urlCliente, numeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existen datos del cliente real.");
                else
                    throw;
            }
            if (credito.assignedBranchKey == null)
            {
                throw new ServiceException("El crédito no tiene una sucursal asignada");
            }
            SucursalDto sucursal = null;
            try
            {
                sucursal = HTTPClientWrapper<SucursalDto>.Get(string.Format(urlSucursal, credito.assignedBranchKey), usuario, contraseña, headers).Result;

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la sucursal");
                else
                    throw;
            }
            Mambu.AhorroDto ahorro = null;
            if (credito.settlementAccountKey == null)
            {
                throw new ServiceException("El crédito no tiene ligada una cuenta de ahorro");
            }
            try
            {
                //Validar autenticación, ya que el usuario docuentos no tiene acceso 
                ahorro = HTTPClientWrapper<Mambu.AhorroDto>.Get(string.Format(urlAhorro, credito.settlementAccountKey), usuario, contraseña, headers).Result;

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el ahorro");
                else
                    throw;
            }

            //OBTENER COTITULARES cambiar por id de cuenta ahorro

            ClientesMambuDto solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };

            List<string> lUsuarios = new List<string>();

            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "COTITULAR" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuarios.Add(item._Encoded_Key_Relacionado);
            }
            List<Mambu.Cliente> cotitulares = new List<Mambu.Cliente>();
            if (lUsuarios.Count > 0)
            {
                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuarios });
                var json = JsonConvert.SerializeObject(solCliente);
                try
                {
                    cotitulares = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se obtuvieron los cotitulares.");
                    else
                        throw;
                }
            }
            // loan product
            ProductosCreditoMambu producto = null;
            string comisioniva = "0";
            try
            {
                var urlT = string.Format(urlProductos, credito.productTypeKey);
                producto = HTTPClientWrapper<ProductosCreditoMambu>.Get(urlT, usuario, contraseña, headers).Result;
                comisioniva = producto.FeesSettings.Fees[0].PercentageAmount.ToString() ?? "0";
                gestorLog.Registrar(Nivel.Information, $"Comisiones - url:{urlT}, comision: {comisioniva}");
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                comisioniva = "0";
            }
            // FechaInicio = credito.disbursementDetails.expectedDisbursementDate,
            string numeroRef = ahorro.Id;
            var grupoId = UnitOfWorkSQL.RepositorioDatosPlantillas.ObtenerGrupoId(solicitud.NumeroCredito);
            var referencias = ObtenerReferecias(grupoId);
            sybase.Credito resCredito = new sybase.Credito
            {
                ComisionIva = comisioniva,
                VencidoIva = credito?.penaltySettings?.penaltyRate.ToString() ?? "0",
                InteresMoratorioMensual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 30),
                FechaInicio = credito.creationDate,
                NumeroCredito = credito.id,
                CAT = Convert.ToDecimal(credito._Datos_Adicionales?.CAT_Datos_Adicionales ?? "0"),
                TasaInteresAnual = Convert.ToDecimal(credito.interestSettings.interestRate),
                TasaMoratoriaAnual = Convert.ToDecimal(credito.penaltySettings.penaltyRate) * 360,
                MontoCredito = Convert.ToDecimal(credito.loanAmount),
                SegObligatorio = "X",
                SegOpcional = "X",
                GATNominal = ahorro._Datos_Adicionales_Cuenta?.Gat_Nominal ?? 0,
                GATReal = ahorro._Datos_Adicionales_Cuenta?.Gat_Real ?? 0,
                EnvioEstadoCuentaEmail = "X",
                LugarEfectuarRetiroVentanilla = "X",
                MedioDisposicionEfectivo = "X",
                Plazo = credito.scheduleSettings.repaymentInstallments,
                MontoTotalAhorro = Convert.ToDecimal(ahorro.InternalControls?.RecommendedDepositAmount ?? "0"),
                MontoTotalDepositar = Convert.ToDecimal(ahorro.InternalControls?.RecommendedDepositAmount ?? "0"),
                TipoOperacion = tipoOperacion,
                NumeroReferencia = numeroRef,
                ReferenciaTelecomm = referencias.ReferenciasPago.Where(r => r.corresponsal == "TELECOMM").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaWalmart = referencias.ReferenciasPago.Where(r => r.corresponsal == "WALMART").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaBanamex = referencias.ReferenciasPago.Where(r => r.corresponsal == "BANAMEX").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaBanbajio = referencias.ReferenciasPago.Where(r => r.corresponsal == "BANBAJIO").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaBbva = referencias.ReferenciasPago.Where(r => r.corresponsal == "BBVABANCOMER").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaHsbc = referencias.ReferenciasPago.Where(r => r.corresponsal == "HSBC").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaOxxopay = referencias.ReferenciasPago.Where(r => r.corresponsal == "OXXOPAY").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
                ReferenciaScotiabank = referencias.ReferenciasPago.Where(r => r.corresponsal == "SCOTIABANK").Select(r => r.referenciaPago).FirstOrDefault() ?? "NO APLICA",
            };

            if (sucursal != null)
            {
                resCredito.Sucursal = sucursal.name;
                if (sucursal.addresses != null && sucursal.addresses.Count != 0)
                {
                    resCredito.DireccionSucursal = sucursal.addresses[0]?.line1 ?? "";
                    resCredito.SucursalCiudad = sucursal.addresses[0]?.city ?? "";
                    resCredito.SucursalProvincia = sucursal.addresses[0]?.region ?? "";
                }
            }

            var dividendos = ObtenerTablaAmortizacionBC(solicitud.NumeroCredito);
            if (dividendos != null && dividendos.Count > 0)
            {
                // resCredito.Dividendos = dividendos;
                // int ultDividendo = (dividendos.Count - 1);
                //  resCredito.FechaFinal = dividendos[ultDividendo].FechaVencimiento.Substring(0, 10);
            }
            else if (solicitud.Subproceso != "CAME_INTERCICLO")
            {
                throw new ServiceException("No se encontro la tabla de amortizacion.");
            }

            PagosCameMambu pagos = null;
            try
            {
                pagos = HTTPClientWrapper<PagosCameMambu>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la tabla de pagos ");
                else
                    throw;
            }

            List<sybase.Dividendo> listaDividendos = new List<sybase.Dividendo>();
            if (pagos != null && pagos.installments.Count > 0)
            {
                decimal saldoCapital = 0; decimal pagoInteres = 0; decimal pagoIva = 0; decimal pagoComision = 0; decimal pagoSaldo = 0; decimal IvaComision = 0;
                string pagoAhorro = "";
                DateTime fechaVencimientoAnterior = Convert.ToDateTime( fechaOffset(credito?.disbursementDetails?.expectedDisbursementDate.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy") ) );
                sybase.Dividendo p = null;
                gestorLog.Registrar(Nivel.Information, "Fecha inicio desembolso credito: " + credito.id + ", expectedDisbursementDat e: " + (credito?.disbursementDetails?.expectedDisbursementDate.ToString("dd/MM/yyyy") ?? "No tiene fecha expectedDisbursementDate") + ", disbursementDate: "+(credito?.disbursementDetails?.disbursementDate.ToString("dd/MM/yyyy") ?? "No tiene fecha disbursementDate"));
                foreach (var item in pagos.installments)
                {
                    if (solicitud.Subproceso == "CAME_INTERCICLO")
                    {
                        pagoAhorro = "0";
                        pagoInteres = 0;
                        pagoIva = decimal.Parse(item.fee.tax.expected.ToString());
                        pagoComision = decimal.Parse((item.fee.amount.expected - item.fee.tax.expected).ToString());
                        pagoSaldo = (int.Parse(item.number) == 1 ? decimal.Parse(resCredito.MontoCredito.ToString()) : saldoCapital);
                        IvaComision = decimal.Parse(item.fee.tax.expected.ToString());
                    }
                    else
                    {
                        pagoAhorro = dividendos[0].Ahorro;
                        pagoInteres = decimal.Parse((item.interest.amount.expected - item.interest.tax.expected).ToString());
                        pagoIva = decimal.Parse(item.interest.tax.expected.ToString());
                        pagoComision = 0;
                        pagoSaldo = (int.Parse(item.number) == 1 ? decimal.Parse(resCredito.MontoCredito.ToString()) : saldoCapital);
                        IvaComision = decimal.Parse(item.fee.tax.expected.ToString());
                    }

                    p = new sybase.Dividendo()
                    {
                        NumeroDividendo = int.Parse(item.number),
                        FechaInicio = fechaVencimientoAnterior.ToString("dd/MM/yyyy") ,
                        FechaVencimiento = fechaOffset(item.dueDate),
                        Ahorro = pagoAhorro,
                        Cuota = decimal.Parse((item.principal.amount.expected + item.interest.amount.expected + item.fee.amount.expected + item.penalty.amount.expected + double.Parse(pagoAhorro)).ToString()),
                        Capital = decimal.Parse(item.principal.amount.expected.ToString()),
                        IntereS = pagoInteres,
                        Iva = pagoIva,
                        Comision = pagoComision,
                        Saldo = pagoSaldo,
                        ComisionIva = IvaComision
                    };

                    listaDividendos.Add(p);
                    fechaVencimientoAnterior = DateTime.Parse(p.FechaVencimiento).AddDays(1);
                    saldoCapital = p.Saldo - p.Capital;
                    if (int.Parse(item.number) == 1)
                    {
                        resCredito.FechaFinal = DateTime.ParseExact(p.FechaVencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");  // Fecha del Primer Pago
                    }
                }

                resCredito.Dividendos = (IEnumerable<sybase.Dividendo>)listaDividendos;
                if (solicitud.Subproceso != "CAME_INTERCICLO") { resCredito.MontoTotalAhorro = resCredito.Dividendos.Sum(item => decimal.Parse(item.Ahorro)); }
                resCredito.MontoTotal = resCredito.Dividendos.Sum(item => item.Cuota) - resCredito.MontoTotalAhorro;
                resCredito.MontoTotalDepositar = resCredito.MontoTotal + resCredito.MontoTotalAhorro;
                resCredito.Plazo = resCredito.Dividendos.Count();
                // resCredito.FechaFinal = fechaVencimientoAnterior.AddDays(-1).ToString("dd/MM/yyy");
                resCredito.FechaPago = resCredito.FechaInicio.ToString("dd/MM/yyyy");

            }


            string cteDireccionCompleta = "";
            if (cliente._Direccion_Clientes != null)
            {
                cteDireccionCompleta = (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Calle_Direccion_Cliente) ? "" : "Calle ") + $"{cliente._Direccion_Clientes?.Calle_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Numero_Ext_Direccion_Cliente) ? "" : ", ") + $"{cliente._Direccion_Clientes?.Numero_Ext_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Numero_Int_Direccion_Cliente) ? "" : "-") + $"{cliente._Direccion_Clientes?.Numero_Int_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Colonia_Direccion_Cliente) ? "" : ", ") + $"{cliente._Direccion_Clientes?.Colonia_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.CP_Direccion_Cliente) ? "" : ", C.P. ") + $"{cliente._Direccion_Clientes?.CP_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Municipio_Direccion_Cliente) ? "" : ", ") + $"{cliente._Direccion_Clientes?.Municipio_Direccion_Cliente ?? ""} " +
                    (string.IsNullOrWhiteSpace(cliente._Direccion_Clientes.Estado_Direccion_Cliente) ? "" : ", ") + $"{cliente._Direccion_Clientes?.Estado_Direccion_Cliente ?? ""} ";
            }
            // DireccionCompleta = $"{cliente._Direccion_Clientes.Calle_Direccion_Cliente} {cliente._Direccion_Clientes.Numero_Ext_Direccion_Cliente} {cliente._Direccion_Clientes.Numero_Int_Direccion_Cliente}",


            sybase.Cliente resCliente = new sybase.Cliente
            {
                NombreCompleto = $"{cliente.firstName} {cliente.lastName}",    // Presidente , Persona del Credito
                Credito = resCredito,
                EdoCtaPortalInternet = "X",
                Sucursal = "X",
                adherente = $"{cliente.firstName} {cliente.lastName}",
                NumeroCliente = int.Parse(cliente.id),
                MontoNeto = Convert.ToDecimal(credito.loanAmount),
                Periodo = credito.scheduleSettings.repaymentPeriodUnit,
                DireccionCompleta = cteDireccionCompleta,
                Ciudad = cliente._Direccion_Clientes.Municipio_Direccion_Cliente,
                Provincia = cliente._Direccion_Clientes.Estado_Direccion_Cliente,
                Colonia = cliente._Direccion_Clientes.Colonia_Direccion_Cliente,
                NombreSuscriptor1 = credito._Integrantes_Credito?.Where(e => e.Rol_integrante.Equals("Tesorero")).Select(e => e.NombreCompleto ?? string.Empty).FirstOrDefault(),
                DireccionSuscriptor1 = credito._Integrantes_Credito?.Where(e => e.Rol_integrante.Equals("Tesorero")).Select(e => e.Direccion ?? string.Empty).FirstOrDefault(),
            };

            resCliente.Ahorro = new sybase.Ahorro()
            {
                TasaInteresAnual = Convert.ToDecimal(ahorro.InterestSettings?.InterestRateSettings?.InterestRate ?? "0"),
                GATNominal = ahorro._Datos_Adicionales_Cuenta?.Gat_Nominal ?? 0,
                GATReal = ahorro._Datos_Adicionales_Cuenta?.Gat_Real ?? 0,
                MontoTotalAhorro = Convert.ToDecimal(ahorro.InternalControls?.RecommendedDepositAmount ?? "0"),
                MontoTotalDepositar = Convert.ToDecimal(ahorro.InternalControls?.RecommendedDepositAmount ?? "0")
            };

            if (cotitulares != null)
            {
                List<sybase.Cotitular> lCotitulares = new List<sybase.Cotitular>();
                foreach (var item in cotitulares)
                {
                    sybase.Cotitular cotitular = new sybase.Cotitular
                    {
                        NombreCompleto = $"{item.firstName} {item.lastName}"
                    };

                    lCotitulares.Add(cotitular);
                }
                resCliente.Cotitulares = lCotitulares;

            }

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            var jsonCliente = JsonConvert.SerializeObject(resCliente, settings);

            gestorLog.Salir();
            return jsonCliente;

        }

        private List<sybase.Dividendo> ObtenerTablaAmortizacionBC(string numeroCredito)
        {

            gestorLog.Entrar();

            List<sybase.Dividendo> dividendos = null;

            dividendos = UnitOfWorkSQL.RepositorioDatosPlantillas.ObtenerTablaAmortizacion(numeroCredito);

            if (dividendos.Count == 0 && dividendos == null)
            {
                throw new BusinessException("UnitOfWorkSybase - No se obtuvo informacion.");
            }


            gestorLog.Salir();

            return dividendos;
        }

        private ReferenciasCore ObtenerReferecias(string grupo)
        {
            var CoreBankAPI = configuration.GetSection("CoreBankAPI");
            var urlRef = CoreBankAPI["Url"] + CoreBankAPI["ReferenciasGet"];
            string core = CoreBankAPI["Core"];

            ReferenciasCore referencias = null;
            try
            {
                referencias = HTTPClientWrapper<ReferenciasCore>.Get(string.Format(urlRef, grupo, core)).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existen las referencias");
                else
                    throw;
            }
            return referencias;
        }

        private string fechaOffset(string valor)
        {
            string resultado = "";
            try
            {
                if (valor.Length >= 19)
                {
                    string sfecha;
                    if (valor.Contains("T"))
                    {
                        sfecha = valor.Substring(0, 19);
                    }
                    else
                    {
                        sfecha = valor;
                    }

                    DateTime fechaHoraFinal = System.DateTime.Parse(sfecha);
                    bool isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(fechaHoraFinal);
                    gestorLog.Registrar(Nivel.Debug, $"bevilad:isDaylight_1_:{isDaylight}");
                    //Codigo para eliminar horario de verano desde la fecha dada
                    DateTime FinHorarioVerano = new DateTime(2023, 3, 13);
                    int EsIgualMayorHorarioVerano = DateTime.Compare(fechaHoraFinal, FinHorarioVerano);

                    if ((EsIgualMayorHorarioVerano >= 0))
                    {
                        isDaylight = false;
                    }
                    gestorLog.Registrar(Nivel.Debug, $"bevilad:isDaylight_2_:{isDaylight}");
                    gestorLog.Registrar(Nivel.Debug, $"bevilad:Compare({fechaHoraFinal},{FinHorarioVerano})={EsIgualMayorHorarioVerano}:{JsonConvert.SerializeObject(TimeZoneInfo.Local)}");
                    //fin

                    if (isDaylight)
                    {
                        fechaHoraFinal = fechaHoraFinal.AddHours(5);
                    }
                    else { fechaHoraFinal = fechaHoraFinal.AddHours(6); }
                    resultado = fechaHoraFinal.ToString("yyyy-MM-dd");
                    gestorLog.Registrar(Nivel.Debug, $"bevilad:Resultado:{resultado}");
                }
                else { resultado = valor; }
            }
            catch (Exception)
            {
                resultado = "";
            }
            return resultado;
        }

        public SolictudDocumentoDto AsignarValores(EstadoCuentaMensualSol solicitud)
        {
            gestorLog.Entrar();
            SolictudDocumentoDto resultado = new SolictudDocumentoDto
            {
                ProcesoNombre = solicitud.Proceso.ToString(),
                SubProcesoNombre = solicitud.SubProceso,
                NumeroCredito = solicitud.NumeroCuenta,
                NumeroCliente = solicitud.NumeroCliente,
                Comprimido = solicitud.Comprimido,
                Base64 = solicitud.Base64,
                Separado = false
            };
            gestorLog.Salir();
            return resultado;
        }

        public int EstadoCuentaMensualProcesa(DateTime fecha)
        {
            throw new NotImplementedException();
        }

        public IList<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud)
        {
            throw new NotImplementedException();
        }

        public DocData EstadoCuentaMensualObtenerDatos(SolictudDocumentoDto documSolicitud)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Actualiza los datos de la solicitud de acuerdo a la información proveniente del cliente, tal
        /// como los paquetes de los seguros.
        /// </summary>
        /// <param name="solicitudEngine"></param>
        /// <param name="cliente"></param>
        private void AjustePaquetesAsistencia(ref DatosSolicitud solicitudEngine, ObtenerDatosJsonDto solicitud, Mambu.Cliente cliente)
        {
            gestorLog.Entrar();
            string valorPaqBasico = "";
            string valorPaqPremium = "";
            string valorPaqPlatino = "";

            List<Parametro> parametros = gestorParametros.RecuperarPorGrupo("ServSeguros");
            valorPaqBasico = parametros.Where(x => x.Codigo == "PaqBasico").Select(x => x.Valor).SingleOrDefault();
            valorPaqPlatino = parametros.Where(x => x.Codigo == "PaqPlatino").Select(x => x.Valor).SingleOrDefault();
            valorPaqPremium = parametros.Where(x => x.Codigo == "PaqPremium").Select(x => x.Valor).SingleOrDefault();

            solicitudEngine.ApellidoPaterno = cliente._Datos_Personales_Clientes?.Primer_Apellido ?? "";
            solicitudEngine.ApellidoMaterno = cliente._Datos_Personales_Clientes?.Segundo_Apellido ?? "";
            solicitudEngine.FechaNacimiento = cliente.birthDate;

            var solDetalles = new List<ServSeguros.Seguro>();
            var beneficiarios = new List<ServSeguros.Beneficiario>();
            var servSegurosConf = configuration.GetSection("ServSeguros");
            string urlCliente = servSegurosConf["Url"] + servSegurosConf["ObtenerSolicitudDetalle"];
            var parameters = new Dictionary<string, string>() { { "CodigoCredito", solicitud.NumeroCredito } };
            try
            {
                solDetalles = HTTPClientWrapper<List<ServSeguros.Seguro>>.Get(urlCliente, parameters).Result;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new ServiceException("Error al intentar consultar los datos del seguro del cliente.");
            }

            if (solDetalles.Count > 0)
            {
                string paqBasico = "";
                string paqPremium = "";
                string paqPlatino = "";
                foreach (ServSeguros.Seguro sol in solDetalles)
                {
                    if (sol.DescripcionTipoSeguro == valorPaqBasico)
                    {
                        paqBasico = "X";
                        if (sol.Beneficiarios.Count > 0)
                        {
                            foreach (var beneficiario in sol.Beneficiarios)
                            {
                                if (beneficiario != null)
                                {
                                    beneficiario.PaternoMaternoNombre = $"{beneficiario?.ApellidoPaterno ?? ""} {beneficiario?.ApellidoMaterno ?? ""} {beneficiario?.Nombre ?? ""}";
                                    beneficiarios.Add(beneficiario);
                                }
                            }
                            beneficiarios = beneficiarios.OrderBy(b => b.ApellidoPaterno).ToList();
                        }
                    }
                    else if (sol.DescripcionTipoSeguro == valorPaqPlatino)
                    {
                        paqPlatino = "X";
                    }
                    else if (sol.DescripcionTipoSeguro == valorPaqPremium)
                    {
                        paqPremium = "X";
                    }
                }
                solicitudEngine.PaqBasico = paqBasico;
                solicitudEngine.PaqPremium = paqPremium;
                solicitudEngine.PaqPlatino = paqPlatino;
                List<string> subprocesos = new List<string> { "PALQUETRABAJA", "PALACASA", "CREDITOCASHI" };
                if (subprocesos.Contains(solicitud.SubProcesoNombre.ToUpper()))
                {
                    solicitudEngine.Credito.PaqBasico = paqBasico;
                    solicitudEngine.Credito.PaqPremium = paqPremium;
                    solicitudEngine.Credito.PaqPlatino = paqPlatino;
                    solicitudEngine.FechaNacimiento = DateTime.Parse(solicitudEngine.FechaNacimiento).ToShortDateString();
                    solicitudEngine.Beneficiarios = beneficiarios.Select(b => new DatosBeneficiarios
                    {
                        ApellidoPaterno = b.ApellidoPaterno,
                        ApellidoMaterno = b.ApellidoMaterno,
                        Nombre = b.Nombre,
                        Parentesco = b.DescripcionParentesco,
                        FechaNacimiento = DateTime.Parse(b.FechaNacimiento).ToShortDateString(),
                        Porcentaje = b.Porcentaje
                    }).ToList();
                }
                else
                {
                    solicitudEngine.BeneficiariosSeguros = beneficiarios;
                }
            }
            gestorLog.Salir();
        }

        public void ObtenerDatosCliente(ref DatosSolicitud solicitudEngine, Cliente cliente)
        {
            TextInfo textInfo = new CultureInfo("es-MX", false).TextInfo;
            solicitudEngine.NombreCompleto = $"{textInfo.ToTitleCase(cliente.firstName ?? "")} {textInfo.ToTitleCase(cliente.lastName ?? "")}";
            solicitudEngine.ApellidoPaterno = textInfo.ToTitleCase(cliente._Datos_Personales_Clientes?.Primer_Apellido ?? "");
            solicitudEngine.ApellidoMaterno = textInfo.ToTitleCase(cliente._Datos_Personales_Clientes?.Segundo_Apellido ?? "");
            solicitudEngine.Nombre = textInfo.ToTitleCase(cliente.firstName ?? "");
            if (cliente?._Direccion_Clientes != null)
            {
                solicitudEngine.Ciudad = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";
                solicitudEngine.Colonia = cliente?._Direccion_Clientes?.Colonia_Direccion_Cliente ?? "";
                solicitudEngine.Estado = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
                solicitudEngine.Municipio = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";

                string dir = $"{cliente._Direccion_Clientes?.Calle_Direccion_Cliente}, {cliente._Direccion_Clientes?.Numero_Ext_Direccion_Cliente}, " +
                    $"{cliente._Direccion_Clientes?.Numero_Int_Direccion_Cliente}, {cliente._Direccion_Clientes?.Colonia_Direccion_Cliente}, {cliente._Direccion_Clientes?.Municipio_Direccion_Cliente}, " +
                    $"{cliente._Direccion_Clientes?.Estado_Direccion_Cliente}, C.P: {cliente._Direccion_Clientes?.CP_Direccion_Cliente}";

                solicitudEngine.DireccionCompleta = dir.Trim();
            }
        }

        public string ObtenerDatosPlantillaAhorroTeChreoPatrimonial(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();
            // ** Subproceso TeChreoPatrimonial1 **

            // SE OBTIENE LOS DATOS PARA CONEXION A MAMBU
            var configuracionMambu = configuration.GetSection("MambuCameAPI");
            var usuario = "";
            var contraseña = "";
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionMambu["HeaderAut"],configuracionMambu["HeaderValAut"]  },
                { configuracionMambu["HeaderCoo"],configuracionMambu["HeaderValCoo"]  },
            };

            var solicitudEngine = new DatosSolicitud();

            // Obtener ahorro mambu
            var urlAhorro = configuracionMambu["Url"] + configuracionMambu["AhorroGET"];
            Mambu.AhorroDto ahorro = null;
            try
            {
                ahorro = HTTPClientWrapper<Mambu.AhorroDto>.Get(string.Format(urlAhorro, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
                if (ahorro != null)
                {
                    ahorro._Relaciones_Ahorro = ahorro._Relaciones_Ahorro.OrderBy(o => o._index).ToList();
                }
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se pudo obtener la información de la cuenta de ahorro.");
                else
                {
                    throw;
                }
            }

            // Obtener datos mambu del cliente
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
            var urlGrupo = configuracionMambu["Url"] + configuracionMambu["GroupsGET"];

            Mambu.Cliente cliente = null;
            try
            {
                if (ahorro?.AccountHolderType?.ToLower() == "group")
                {
                    cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlGrupo, ahorro.AccountHolderKey), usuario, contraseña, headers).Result;
                }
                else
                {
                    cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlCliente, solicitud.NumeroCliente), usuario, contraseña, headers).Result;
                }
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se pudo obtener la información del cliente.");
                else
                {
                    throw;
                }
            }

            // OBTENER COTITULARES POR CLIENTE
            ClientesMambuDto solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };
            var urlCotitulares = configuracionMambu["Url"] + configuracionMambu["ClientesPOST"];

            List<string> lUsuarios = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "COTITULAR" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuarios.Add(item._Encoded_Key_Relacionado);
            }

            List<Mambu.Cliente> cotitulares = new List<Mambu.Cliente>();
            if (lUsuarios.Count > 0)
            {
                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuarios });
                var json = JsonConvert.SerializeObject(solCliente);
                try
                {
                    cotitulares = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                    //se cambia el orden dependiendo del index mambu
                    if (cotitulares != null)
                    {
                        cotitulares = cotitulares
                            .Select(cot =>
                            {
                                cot.firstName = cot.firstName.ToUpper();
                                cot.lastName = cot.lastName.ToUpper();
                                return cot;
                            })
                            .OrderBy(i => ahorro._Relaciones_Ahorro.FindIndex(o => o._Encoded_Key_Relacionado == i.encodedKey))
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se pudo obtener la información de cotitulares.");
                    else
                        throw;
                }

            }

            #region Representante Legal
            List<string> lUsuariosRepresentantes = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "REPRESENTANTE LEGAL" && !string.IsNullOrWhiteSpace(item._Encoded_Key_Relacionado))
                    lUsuariosRepresentantes.Add(item._Encoded_Key_Relacionado);
            }

            List<Mambu.Cliente> representantes = new List<Mambu.Cliente>();
            if (lUsuariosRepresentantes.Count > 0)
            {
                solCliente = new ClientesMambuDto() { FilterCriteria = new List<filterCriteria>() };
                solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuariosRepresentantes });
                var json = JsonConvert.SerializeObject(solCliente);

                try
                {
                    representantes = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se obtuvo la lista de representantes.");
                    else
                        throw;
                }
            }
            #endregion

            #region Depositos
            List<RespTransacionesDepDto> listDepts = new List<RespTransacionesDepDto>();

            SolTransacionesDepDto deposits = new SolTransacionesDepDto()
            {
                FilterCriteria = new List<searchCriteria>(),
            };

            var urlTransactions = configuracionMambu["Url"] + "/deposits/transactions:search?detailsLevel=FULL";

            deposits.FilterCriteria.Add(new searchCriteria { Field = "parentAccountKey", Operator = "EQUALS", Value = ahorro.EncodedKey });
            deposits.FilterCriteria.Add(new searchCriteria { Field = "type", Operator = "EQUALS", Value = "DEPOSIT" });
            deposits.FilterCriteria.Add(new searchCriteria { Field = "adjustmentTransactionKey", Operator = "EMPTY" });
            deposits.SortingCriteria = new sortingCriteria() { Field = "creationDate", Order = "DESC" };

            var body = JsonConvert.SerializeObject(deposits, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });


            try
            {
                listDepts = HTTPClientWrapper<List<RespTransacionesDepDto>>.PutRequest(urlTransactions, "", "", headers, body).Result;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se pudo obtener la información de depositos.");
                else
                    throw;
            }
            #endregion 

            #region InteresAplicado
            List<RespTransacionesDepDto> listInteresA = new List<RespTransacionesDepDto>();

            SolTransacionesDepDto interestA = new SolTransacionesDepDto()
            {
                FilterCriteria = new List<searchCriteria>(),
            };

            interestA.FilterCriteria.Add(new searchCriteria { Field = "parentAccountKey", Operator = "EQUALS", Value = ahorro.EncodedKey });
            interestA.FilterCriteria.Add(new searchCriteria { Field = "type", Operator = "EQUALS", Value = "INTEREST_APPLIED" });
            interestA.FilterCriteria.Add(new searchCriteria { Field = "adjustmentTransactionKey", Operator = "EMPTY" });
            interestA.SortingCriteria = new sortingCriteria() { Field = "creationDate", Order = "DESC" };

            var bodyIA = JsonConvert.SerializeObject(interestA, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            try
            {
                listInteresA = HTTPClientWrapper<List<RespTransacionesDepDto>>.PutRequest(urlTransactions, "", "", headers, bodyIA).Result;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se pudo obtener la información de interés aplicado.");
                else
                    throw;
            }
            #endregion 


            // Mapeo a DatosSolicitud
            solicitudEngine = Equivalencias(ahorro, cliente, cotitulares, solicitudEngine, solicitud.Subproceso);

            if (solicitud.Subproceso.ToLower() == "clubcame" || solicitud.Subproceso.ToLower() == "crecemas")
            {
                solicitudEngine.Ahorro.Monto = listDepts.Sum(x => x.Amount);
            }

            if ((solicitud.Subproceso.ToLower() == "creditogarantizado"))
            {
                if (listDepts.Count < 1)
                {
                    throw new BusinessException("Error, no se tiene datos de algún deposito");
                }

                if (listInteresA.Count < 1)
                {
                    throw new BusinessException("Error, no se tiene datos de algún interés aplicado");
                }

                solicitudEngine = AjustarCreditoGarantizado(solicitudEngine, ahorro, cliente, listDepts, listInteresA);
            }
			if (solicitud.Subproceso.ToLower() == "bancadigitalempresarial")
            {
                solicitudEngine = AjustarBancaDigitalEmpresarial(ahorro, solicitudEngine, representantes, cliente);
            }
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }
        private DatosSolicitud Equivalencias(Mambu.AhorroDto ahorro, Mambu.Cliente cliente, List<Mambu.Cliente> cotitulares, DatosSolicitud solicitudEngine, string subProceso)
        {
            gestorLog.Entrar();

            try
            {
                switch (subProceso)
                {
                    case "TeChreoPatrimonial1":
                    case "TeChreoPatrimonial2":
                    case "ClubCAME":
                    case "CreceMas":
                    case "KjaMas":

                        solicitudEngine.Ahorro = AhorroToDatosAhorro(ahorro);
                        if (cliente.firstName != null)
                            solicitudEngine.NombreCompleto = $"{cliente?.firstName ?? ""} {cliente?.lastName ?? ""}";
                        solicitudEngine.Ciudad = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";
                        DatosCotitulares cotitular = null;
                        solicitudEngine.Cotitulares = new List<DatosCotitulares>();

                        foreach (var item in cotitulares)
                        {
                            cotitular = new DatosCotitulares();
                            cotitular.NombreCompleto = $"{item?.firstName ?? ""} {item?.lastName ?? ""}";
                            solicitudEngine.Cotitulares.Add(cotitular);
                        };

                        solicitudEngine.NumeroCliente = cliente.id;
                        solicitudEngine.Ahorro.CuentaDeposito = string.IsNullOrEmpty(ahorro._Datos_Adicionales_Cuenta?.Cuenta_Deposito) ? "" : ahorro._Datos_Adicionales_Cuenta?.Cuenta_Deposito;
                        solicitudEngine.Ahorro.FechaVencimiento = string.IsNullOrEmpty(ahorro.maturityDate) ? "" : ahorro.maturityDate;
                        solicitudEngine.Ahorro.FechaActivacion = string.IsNullOrEmpty(ahorro.ActivationDate) ? "" : ahorro.ActivationDate;

                        if (cliente?._Direccion_Clientes != null)
                        {
                            string calle = cliente._Direccion_Clientes?.Calle_Direccion_Cliente;
                            string mun = cliente._Direccion_Clientes?.Municipio_Direccion_Cliente;
                            string cp = cliente._Direccion_Clientes?.CP_Direccion_Cliente;
                            string col = cliente._Direccion_Clientes?.Colonia_Direccion_Cliente;
                            string edo = cliente._Direccion_Clientes?.Estado_Direccion_Cliente;
                            string numext = cliente._Direccion_Clientes?.Numero_Ext_Direccion_Cliente;
                            string numint = cliente._Direccion_Clientes?.Numero_Int_Direccion_Cliente;
                            //  0     1       2     3     4   5    6
                            var dirData = new string[] { calle, numext, numint, col, mun, edo, cp };
                            // contruir la dirección completa
                            string dir = "";
                            for (int i = 0; i < dirData.Length; i++)
                            {
                                var item = dirData[i];
                                if (String.IsNullOrEmpty(item)) continue;
                                switch (i)
                                {
                                    case 0: // calle
                                    case 1: // numext
                                    case 2: // numint
                                    case 3: // col
                                    case 4: // mun
                                    case 5:// edo
                                        {
                                            string data = item.Trim();
                                            dir += $"{data}, ";
                                            break;
                                        }
                                    case 6:// cp
                                        {
                                            string data = item.Trim();
                                            dir += $"C.P. {data}, ";
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            dir = dir.Trim();
                            int commaPos = dir.Length - 1;
                            // sustituir ultima ',' por '.'
                            dir = dir.Substring(0, commaPos) + '.';
                            solicitudEngine.DireccionCompleta = dir;
                            solicitudEngine.Estado = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
                        }
                        break;
                    case "BoomCardPaquete":
                        if (!string.IsNullOrEmpty(ahorro.ActivationDate))
                        {
                            DateTime fechaApertura = Convert.ToDateTime(ahorro.ActivationDate);
                            solicitudEngine.FechaApertura = fechaApertura.ToString("dd/MM/yyyy");
                        }
                        else
                            solicitudEngine.FechaApertura = "";
                        solicitudEngine.Ahorro = AhorroToDatosAhorro(ahorro);
                        solicitudEngine.Ahorro.FechaApertura = solicitudEngine.FechaApertura;
                        solicitudEngine.Credito = new DatosCredito { NumeroCredito = string.IsNullOrEmpty(ahorro.Id) ? "" : ahorro.Id };
                        solicitudEngine.NumeroCliente = string.IsNullOrEmpty(cliente.id) ? "" : cliente.id;
                        solicitudEngine.Nombre = string.IsNullOrEmpty(cliente.firstName) ? "" : cliente.firstName;
                        solicitudEngine.ApellidoPaterno = string.IsNullOrEmpty(cliente._Datos_Personales_Clientes.Primer_Apellido) ? "" : cliente._Datos_Personales_Clientes.Primer_Apellido;
                        solicitudEngine.ApellidoMaterno = string.IsNullOrEmpty(cliente._Datos_Personales_Clientes.Segundo_Apellido) ? "" : cliente._Datos_Personales_Clientes.Segundo_Apellido;
                        solicitudEngine.FechaNacimiento = string.IsNullOrEmpty(cliente.birthDate) ? "" : cliente.birthDate;

                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new BusinessException("Hubo un problema al consturir las equivalencias para cliente y ahorro.");
            }

            return solicitudEngine;
        }

        private DatosSolicitud Equivalencias(Mambu.Cliente cliente, ServSeguros.CPADto seguroResponse, DatosSolicitud solicitudEngine, string subProceso)
        {
            gestorLog.Entrar();
            bool band = false;
            try
            {
                switch (subProceso)
                {
                    case "TechreoSeguroVida":
                        {
                            foreach (ServSeguros.CPABeneficiario beneficiario in seguroResponse.Beneficiarios)
                            {
                                beneficiario.NombreCompletoReves = $"{beneficiario.ApellidoPaterno ?? ""} {beneficiario.ApellidoMaterno ?? ""} {beneficiario.Nombres ?? ""}".Trim();
                            }
                            solicitudEngine.Seguro = new List<ServSeguros.CPASeguro>(3);
                            for (int i = 0; i < solicitudEngine.Seguro.Capacity; i++)
                            {
                                solicitudEngine.Seguro.Add(new ServSeguros.CPASeguro());
                            }

                            for (int i = 0; i < seguroResponse?.Seguros?.Count; i++)
                            {
                                if (seguroResponse.Seguros[i].CodigoSeguroDescripcion == "Vida")
                                {
                                    solicitudEngine.Seguro[0] = seguroResponse.Seguros[i];
                                    band = true;
                                }
                                else if (seguroResponse.Seguros[i].CodigoSeguroDescripcion == "Tu salud")
                                {
                                    solicitudEngine.Seguro[1] = seguroResponse.Seguros[i];
                                    band = true;
                                }
                                else if (seguroResponse.Seguros[i].CodigoSeguroDescripcion == "Gastos funerarios")
                                {
                                    solicitudEngine.Seguro[2] = seguroResponse.Seguros[i];
                                    band = true;
                                }
                            }
                            if (!band)
                            {
                                throw new BusinessException("No se encontro información de seguros.");
                            }
                            solicitudEngine.NumeroCliente = cliente.id;
                            solicitudEngine.Nombre = cliente.firstName ?? "";
                            solicitudEngine.ApellidoPaterno = cliente._Datos_Personales_Clientes?.Primer_Apellido ?? "";
                            solicitudEngine.ApellidoMaterno = cliente._Datos_Personales_Clientes?.Segundo_Apellido ?? "";
                            solicitudEngine.FechaNacimiento = cliente.birthDate ?? "";
                            //solicitudEngine.Seguro = seguroResponse.Seguros;
                            solicitudEngine.Beneficiario = seguroResponse.Beneficiarios;
                        }; break;
                    default: break;
                }
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new Exception("Hubo un problema al construir las equivalencias.");
            }
            finally
            {
                gestorLog.Salir();
            }
            return solicitudEngine;
        }
        public string ObtenerDatosPlantillaSeguros(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();
            var configuracionMambu = configuration.GetSection("MambuCameAPI");
            var usuario = "";
            var contraseña = "";
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionMambu["HeaderAut"],configuracionMambu["HeaderValAut"]  },
                { configuracionMambu["HeaderCoo"],configuracionMambu["HeaderValCoo"]  },
            };
            var solicitudEngine = new DatosSolicitud();
            // Obtener datos mambu del cliente
            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
            Mambu.Cliente cliente = null;
            try
            {
                cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlCliente, solicitud.NumeroCliente), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se pudo obtener la información del cliente.");
                else
                {
                    throw;
                }
            }
            //OBTENIENDO SEGUROS Y BENEFICIARIOS
            var servSegurosConf = configuration.GetSection("ServSeguros");
            string urlSeguros = servSegurosConf["Url"] + servSegurosConf["ConsultaSegurosPorPaquete"];
            var parameters = new Dictionary<string, string>();
            parameters.Add("codigoCredito", solicitud.NumeroCredito);
            parameters.Add("clienteId", solicitud.NumeroCliente);
            ServSeguros.CPADto solRequest = null;
            try
            {
                solRequest = HTTPClientWrapper<ServSeguros.CPADto>.Get(urlSeguros, parameters).Result;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new ServiceException("Error al intentar consultar los datos del seguro del cliente.");
            }


            //asignando seguros y beneficiarios a solicitudEngine
            solicitudEngine = Equivalencias(cliente, solRequest, solicitudEngine, solicitud.Subproceso);
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);
            gestorLog.Salir();
            return jsonCliente;
        }
        public string ObtenerDatosPlantillaAhorroJsonSinDatos(ObtenerDatosJsonDatosDto solicitud)
        {
            gestorLog.Entrar();

            dynamic jsonData = solicitud.JsonSolicitudData;

            if (solicitud.SubProcesoNombre.ToLower() == "coftechreo" && solicitud.Gat != null)
            {

                #region IndiceInflacionObtenerActual

                var calculoGat = configuration.GetSection("CoreBank");
                string urlInflacion = calculoGat["Url"] + calculoGat["InflacionActual"];
                var parameters = new Dictionary<string, string>();
                parameters.Add("CoreId", calculoGat["IndiceInflacionObtenerActual_CoreId"]);
                ObtenerDatosRespuestaInflacionActual infRequest = null;

                try
                {
                    infRequest = HTTPClientWrapper<ObtenerDatosRespuestaInflacionActual>.Get(urlInflacion, parameters).Result;

                    if (infRequest == null)
                    {
                        throw new BusinessException("Error al intentar manipular el indice de inflacion.");
                    }

                    gestorLog.Registrar(Nivel.Information, $"coftechreo:indiceInflacion:{infRequest.indiceInflacion}");

                }
                catch (BusinessException ex)
                {
                    gestorLog.RegistrarError(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    gestorLog.RegistrarError(ex);
                    throw new ServiceException("Error al intentar consultar los datos del indice de inflacion.");
                }

                #endregion

                #region CalcularGATSinTabla

                string urlCalculoGat = calculoGat["Url"] + calculoGat["CalculoGat"];
                DatosRespuestaCalculoGat gat = null;
                var body = new Dictionary<string, object>()
                {
                    { "depositoInicial",solicitud.Gat.DepositoInicial },
                    { "tasaAnual",solicitud.Gat.TasaAnual / 100},
                    { "plazo", 12},
                    { "numeroPagos", 30},
                    { "indiceInflacion", infRequest.indiceInflacion / 100 }
                };

                try
                {
                    var result = HTTPClientApi.PostRequest(urlCalculoGat, string.Empty, string.Empty, null, null, body).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var content = result.Content.ReadAsStringAsync();
                        gat = JsonConvert.DeserializeObject<DatosRespuestaCalculoGat>(content.Result);

                        if (gat == null)
                        {
                            throw new BusinessException("Error al intentar manipular los datos del GAT.");
                        }

                        gestorLog.Registrar(Nivel.Information, $"coftechreo:Gat:{JsonConvert.SerializeObject(gat)}");

                    }
                    else
                    {
                        throw new ExternoTechnicalException(result.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    gestorLog.RegistrarError(ex);
                    throw new ServiceException("Error al intentar consultar el GAT.");
                }
                #endregion
                jsonData.ahorro.gat = Math.Truncate(gat.gatNominal * 10000) / 100;
                jsonData.ahorro.gatreal = Math.Truncate(gat.gatReal * 10000) / 100;

            }

            var json = JsonConvert.SerializeObject(jsonData, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new LowerCaseNamingStrategy()
                    {
                        ProcessDictionaryKeys = true
                    }
                },
                Formatting = Formatting.Indented
            });

            gestorLog.Registrar(Nivel.Information, json);

            gestorLog.Salir();
            return json;
        }

        private DatosSolicitud AjustarComprobantesClubCameCreceMas(ObtenerDatosJsonDto solicitud, DatosSolicitud solicitudEngine, AhorroDto ahorro, string usuario, string contraseña, Dictionary<string, string> headers, IConfigurationSection configuracionMambu)
        {
            gestorLog.Entrar();

            //se obtiene el producto de mambu que corresponde a la cuenta de deposito
            var urLProductosDeposito = configuracionMambu["Url"] + configuracionMambu["DepositProductsGET"];
            if (string.IsNullOrEmpty(urLProductosDeposito))
            {
                throw new BusinessException("La URL obtenida del archivo de configuración para la consulta del tipo de producto es NULL o vacía");
            }

            string ProductTypeKey = ahorro?.ProductTypeKey ?? "";
            string UrlFinal = string.Format(urLProductosDeposito, ProductTypeKey);

            try
            {
                Mambu.ProductoDto ProductoMambu = HTTPClientWrapper<Mambu.ProductoDto>.Get(UrlFinal, usuario, contraseña, headers).Result;
                solicitudEngine.Ahorro.ProductoAhorro = ProductoMambu?.Name ?? "";
            }
            catch (Exception ex)
            {
                string datosUrlAuthentication = $"encodekey:{ProductTypeKey}, UrlConf: {urLProductosDeposito}, Url: {UrlFinal}, User: {usuario}, Pass: {contraseña}, headers: {JsonConvert.SerializeObject(headers)}";
                gestorLog.Registrar(Nivel.Information, datosUrlAuthentication);
                gestorLog.RegistrarError(ex);
                throw new ServiceException($"Error en el servicio de consulta de productos:[{ProductTypeKey}]");
            }

            //se obtiene y asigna la hora de creacion
            solicitudEngine.Ahorro.HoraApertura = ahorro?.CreationDate.Substring(11, 8) ?? "";

            //se obtiene el folio de la solicitud
            string folio = "";
            if (!string.IsNullOrEmpty(solicitud.JsonSolicitud))
            {
                var FolioMonto = (JObject)JsonConvert.DeserializeObject(solicitud.JsonSolicitud);
                folio = FolioMonto.SelectToken("FolioOperacion").ToString();
            }

            //se obtienen la transacciones de la cuenta de deposito por su folio
            var urLTransaccionesCuenta = configuracionMambu["Url"] + configuracionMambu["DepositsTransactionsSearch"];
            if (string.IsNullOrEmpty(urLTransaccionesCuenta))
            {
                throw new BusinessException("La URL obtenida del archivo de configuración para la consulta de las transacciones es NULL o vacía");
            }


            string encodedKeyCuenta = ahorro?.EncodedKey ?? "";
            string bodyTransactions = $"{{\"filterCriteria\":[{{\"field\":\"parentAccountKey\",\"operator\":\"EQUALS\",\"value\":\"{encodedKeyCuenta}\"}},{{\"field\":\"type\",\"operator\":\"EQUALS\",\"value\":\"DEPOSIT\"}},{{\"field\":\"adjustmentTransactionKey\",\"operator\":\"EMPTY\"}}],\"sortingCriteria\":{{\"field\":\"creationDate\",\"order\":\"DESC\"}}}}";
            Mambu.TransaccionAhorroDto transaccion = new TransaccionAhorroDto();
            List<Mambu.TransaccionAhorroDto> Transacciones = new List<TransaccionAhorroDto>();

            try
            {
                Transacciones = HTTPClientWrapper<List<Mambu.TransaccionAhorroDto>>.PostRequestEngineJson(urLTransaccionesCuenta, usuario, contraseña, headers, bodyTransactions).Result;
                transaccion = Transacciones.FirstOrDefault(x => x.Id == folio);
                if (transaccion != null)
                {
                    solicitudEngine.Ahorro.Transaccion = new DatosTransaccion();
                    solicitudEngine.Ahorro.Transaccion.Id = transaccion?.Id ?? "";
                    solicitudEngine.Ahorro.Transaccion.EncodedKey = transaccion?.EncodedKey ?? "";
                    solicitudEngine.Ahorro.Transaccion.FechaTransaccion = transaccion?.ValueDate.ToString() ?? "";
                    solicitudEngine.Ahorro.Transaccion.Monto = transaccion?.Amount ?? 0;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                string datosUrlAuthentication = $"encodekey: {encodedKeyCuenta}, UrlConf: {urLTransaccionesCuenta}, Url: {string.Format(urLProductosDeposito, ProductTypeKey)}, User: {usuario}, Pass: {contraseña},BodyTransactions: {bodyTransactions}, headers: {JsonConvert.SerializeObject(headers)}";
                gestorLog.Registrar(Nivel.Information, datosUrlAuthentication);
                gestorLog.RegistrarError(ex);
                if (transaccion == null)
                {
                    throw new BusinessException($"Error no se encontro información para el folio de operación [{folio}]", ex.InnerException);
                }
                else
                {
                    throw new ServiceException($"Error al intentar consultar las transacciones para la cuenta [{encodedKeyCuenta}]", ex.InnerException);
                }
            }

            var jsonObject = JObject.Parse(solicitud.JsonSolicitud);
            if (solicitud.SubProcesoNombre.ToLower() == "crecemasempresas" && !jsonObject.ContainsKey("MontoRecibirFinalInversion"))
            {
                ProcesarCrecemasEmpresas(solicitud, solicitudEngine, Transacciones, transaccion);
            }

            string InsTipoPlazo = string.IsNullOrEmpty(solicitudEngine.Ahorro?.InstruccionesTipoPlazo) ? "" : solicitudEngine.Ahorro?.InstruccionesTipoPlazo;
            try
            {
                //Ajustes para InstruccionesTipoPlazo
                if (!string.IsNullOrEmpty(InsTipoPlazo))
                {
                    switch (InsTipoPlazo.ToLower())
                    {
                        case "retirar": solicitudEngine.Ahorro.InstruccionesTipoPlazo = "Si"; break;
                        case "no retirar": solicitudEngine.Ahorro.InstruccionesTipoPlazo = "No"; break;
                        default: throw new BusinessException("El valor de InstruccionesTipoPlazo no corresponde a los valores \"Retirar\", \"No Retirar\". ");
                    }
                }
                else
                {
                    throw new BusinessException("El valor de InstruccionesTipoPlazo es null o vacío.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            //Ajustes para TipoPlazo
            int? plazo = solicitudEngine.Ahorro?.Plazo;
            string tipoPlazo = string.IsNullOrEmpty(solicitudEngine.Ahorro?.TipoPlazo) ? "" : solicitudEngine.Ahorro?.TipoPlazo;
            try
            {
                if (plazo != null)
                {
                    if (tipoPlazo != "")
                    {
                        switch (tipoPlazo.ToLower())
                        {
                            case "dias": if (plazo == 1) { solicitudEngine.Ahorro.TipoPlazo = "Día"; } else { solicitudEngine.Ahorro.TipoPlazo = "Días"; }; break;
                            case "meses": if (plazo == 1) { solicitudEngine.Ahorro.TipoPlazo = "Mes"; } else { solicitudEngine.Ahorro.TipoPlazo = "Meses"; }; break;
                            default: throw new BusinessException("El valor de TipoPlazo no corresponde a los valores \"Meses\", \"Dias\".");
                        }
                    }
                    else
                    {
                        throw new BusinessException("El valor de tipoPlazo es null o vacío.");
                    }
                }
                else
                {
                    throw new BusinessException("El valor de Plazo es null.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            gestorLog.Salir();
            //se retorna el objeto con los datos agregados
            return solicitudEngine;
        }
        private DatosSolicitud AjustarComprobantesXADELA(ObtenerDatosJsonDto solicitud, DatosSolicitud solicitudEngine, AhorroDto ahorro, string usuario, string contraseña, Dictionary<string, string> headers, IConfigurationSection configuracionMambu)
        {
            gestorLog.Entrar();

            try
            {
                //se obtiene datos de reporte
                solicitudEngine.Ahorro.HoraApertura = ahorro?.CreationDate.Substring(11, 8) ?? "";
                solicitudEngine.Ahorro.Monto = ahorro?._Datos_Adicionales_Cuenta.MontoReal;
                solicitudEngine.Ahorro.TasaAhorro = ahorro?._Datos_Adicionales_Cuenta.TasaIntAnualFijaReal;
                solicitudEngine.Ahorro.FechaApertura = Convert.ToDateTime(ahorro?.CreationDate).ToString("dd/MM/yyyy");

            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw new BusinessException("Hubo un problema al mapear los datos de Ahorro.");
            }
            gestorLog.Salir();
            //se retorna el objeto con los datos agregados
            return solicitudEngine;
        }

        public string ObtenerDatosPlantillaMercado(ObtenerDatosJsonDto solicitud)
        {
            //Datos solictud enviadas por Json
            gestorLog.Entrar();

            //cumun
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                FloatParseHandling = FloatParseHandling.Decimal
            };
            var solicitudEngine = new DatosSolicitud(); //JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud, settings);

            //Configuracion Mambu
            var configuracionCAMEMambu = configuration.GetSection("MambuCameAPI");
            var usuario = ""; ///configuracionCAMEMambu["Usuario"];
            var contraseña = ""; // configuracionCAMEMambu["Password"];
            var urlCliente = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["clientesGET"];
            var urlCredito = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["CreditoGET"];
            var urlPago = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["PagosGet"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionCAMEMambu["HeaderAut"],configuracionCAMEMambu["HeaderValAut"]  },
                { configuracionCAMEMambu["HeaderCoo"],configuracionCAMEMambu["HeaderValCoo"]  }
            };

            Dictionary<string, string> headerPag = new Dictionary<string, string>
            {
                { Accept, AcceptApplication },
                { configuracionCAMEMambu["HeaderAut"],configuracionCAMEMambu["HeaderValAutPag"]  },
                { configuracionCAMEMambu["HeaderCoo"],configuracionCAMEMambu["HeaderValCooPag"]  }
            };

            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            Cliente cliente = null;
            try
            {
                cliente = HTTPClientWrapper<Cliente>.Get(string.Format(urlCliente, credito.accountHolderKey), usuario, contraseña, headers).Result;

                if (cliente.firstName != null)
                    solicitudEngine.NombreCompleto = $"{cliente?.firstName ?? ""} {cliente?.lastName ?? ""}";
                solicitudEngine.Ciudad = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";
                solicitudEngine.Colonia = cliente?._Direccion_Clientes?.Colonia_Direccion_Cliente ?? "";
                solicitudEngine.Estado = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
                solicitudEngine.Municipio = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";
                if (cliente?._Direccion_Clientes != null)
                {
                    string calle = cliente._Direccion_Clientes?.Calle_Direccion_Cliente;
                    string mun = cliente._Direccion_Clientes?.Municipio_Direccion_Cliente;
                    string cp = cliente._Direccion_Clientes?.CP_Direccion_Cliente;
                    string col = cliente._Direccion_Clientes?.Colonia_Direccion_Cliente;
                    string edo = cliente._Direccion_Clientes?.Estado_Direccion_Cliente;
                    string numext = cliente._Direccion_Clientes?.Numero_Ext_Direccion_Cliente;
                    string numint = cliente._Direccion_Clientes?.Numero_Int_Direccion_Cliente;
                    //  0     1       2     3     4   5    6
                    var dirData = new string[] { calle, numext, numint, col, mun, edo, cp };
                    // contruir la dirección completa
                    string dir = "";
                    for (int i = 0; i < dirData.Length; i++)
                    {
                        var item = dirData[i];
                        if (String.IsNullOrEmpty(item)) continue;
                        switch (i)
                        {
                            case 0: // calle
                            case 1: // numext
                            case 2: // numint
                            case 3: // col
                            case 4: // mun
                            case 5:// edo
                                {
                                    string data = item.Trim();
                                    dir += $"{data}, ";
                                    break;
                                }
                            case 6:// cp
                                {
                                    string data = item.Trim();
                                    dir += $"C.P. {data}, ";
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    dir = dir.Trim();
                    int commaPos = dir.Length - 1;
                    // sustituir ultima ',' por '.'
                    dir = dir.Substring(0, commaPos) + '.';
                    solicitudEngine.DireccionCompleta = dir;
                    solicitudEngine.Estado = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existen datos del cliente. ");
                else
                    throw;
            }
            bool conCuentaAhorroLigada = false;
            if (!string.IsNullOrEmpty(credito.settlementAccountKey))
            {
                AhorroDto ahorro = null;
                conCuentaAhorroLigada = true;
                try
                {
                    var urlAhorro = configuracionCAMEMambu["Url"] + configuracionCAMEMambu["AhorroGET"];
                    ahorro = HTTPClientWrapper<AhorroDto>.Get(string.Format(urlAhorro, credito.settlementAccountKey), usuario, contraseña, headers).Result;
                    if (ahorro._Referencias_Deposito != null)
                    {
                        if (credito._Referencias_Pago != null)
                            credito._Referencias_Pago.Clear();
                        credito._Referencias_Pago = ahorro._Referencias_Deposito.Select(x => new ReferenciasPago()
                        {
                            Referencia_Pago_Tipo = x.Referencia_Deposito_Tipo,
                            Referencia_Pago_Valor = x.Referencia_Deposito_Valor
                        }).ToList();
                    }
                    else
                        conCuentaAhorroLigada = false;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No existen datos del cliente. ");
                    else
                        throw;
                }
            }

            PagosCameMambu pagos = null;
            try
            {
                pagos = HTTPClientWrapper<PagosCameMambu>.Get(string.Format(urlPago, solicitud.NumeroCredito), usuario, contraseña, headerPag).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la tabla de pagos ");
                else
                    throw;
            }

            var datosMambu = Equivalencias(credito, pagos, solicitud.SubProcesoNombre);
            if (datosMambu != null)
            {

                string frecuenciaPago = "";
                if (!string.IsNullOrEmpty(solicitudEngine.Credito?.FrecuenciaPago))
                {
                    frecuenciaPago = solicitudEngine.Credito?.FrecuenciaPago;
                }

                int plazoPago = 0;
                if (solicitudEngine.Credito?.Plazo > 0)
                {
                    plazoPago = solicitudEngine.Credito.Plazo;
                }

                int nuneroCreditos = 0;
                if (solicitudEngine.Credito?.NumeroCreditos > 0)
                    nuneroCreditos = solicitudEngine.Credito.NumeroCreditos;

                int cuotaAumento = 0;
                if (solicitudEngine.Credito?.CuotaAumento > 0)
                    cuotaAumento = (int)solicitudEngine.Credito.CuotaAumento;
                string paqBasico = solicitudEngine.Credito?.PaqBasico;
                string paqPlatino = solicitudEngine.Credito?.PaqPlatino;
                string paqPremium = solicitudEngine.Credito?.PaqPremium;

                solicitudEngine.Credito = datosMambu.Credito;

                solicitudEngine.Credito.ConCuentaAhorroLigada = conCuentaAhorroLigada;
                solicitudEngine.Credito.NumeroCreditos = nuneroCreditos;
                solicitudEngine.TablaAmortizacion = datosMambu.TablaAmortizacion;
                solicitudEngine.TablaAmortizacionPagare = datosMambu.TablaAmortizacionPagare;
                solicitudEngine.Credito.CuotaAumento = cuotaAumento;

                if (!string.IsNullOrEmpty(frecuenciaPago))
                    solicitudEngine.Credito.FrecuenciaPago = frecuenciaPago;

                if (plazoPago > 0)
                    solicitudEngine.Credito.Plazo = plazoPago;

                if (!string.IsNullOrEmpty(paqBasico))
                    solicitudEngine.Credito.PaqBasico = paqBasico;

                if (!string.IsNullOrEmpty(paqPlatino))
                    solicitudEngine.Credito.PaqPlatino = paqPlatino;

                if (!string.IsNullOrEmpty(paqPremium))
                    solicitudEngine.Credito.PaqPremium = paqPremium;

            }

            solicitudEngine.Telefono = cliente.mobilePhone;
            solicitudEngine.NumeroClienteMambu = cliente.id;
            solicitudEngine.NombreCompleto = cliente.firstName + " " + cliente.lastName;
            solicitudEngine.Nombre = cliente.firstName;
            solicitudEngine.NumeroCliente = cliente.id;


            if (solicitud.ProcesoNombre.ToUpper() == "CREDITOCAME" && solicitud.SubProcesoNombre.ToUpper() == "CREDITO_MERCADO")
            {
                AjustePaquetesAsistencia(ref solicitudEngine, solicitud, cliente);
            }

            settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            //solicitudEngine.solicitudJSON = JsonConvert.DeserializeObject<object>(solicitud.JsonSolicitud);
            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
        }

        private DatosCredito CreditoToDatosCreditoMercado(Credito credito)
        {
            DatosCredito datosCredito = new DatosCredito();
            try
            {

                datosCredito.NumeroCredito = credito.id;
                // datosCredito.NumeroCreditos = 0;
                datosCredito.NumeroCliente = credito.accountHolderKey;
                datosCredito.FechaInicio = credito.creationDate.ToString("dd/MM/yyyy");
                datosCredito.Producto = credito.loanName;
                datosCredito.MontoCredito = Convert.ToDecimal(credito.loanAmount);
                datosCredito.EstatusCredito = credito.accountState;
                //se cambia el *12 porque ya tiene el anual
                datosCredito.TasaInteresAnual = Convert.ToDecimal(credito.interestSettings.interestRate);
                datosCredito.TasaMoratoriaAnual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 360);
                datosCredito.InteresMoratorioMensual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 30);
                datosCredito.PlazoGarcia = "";
                datosCredito.Plazo = credito.scheduleSettings.repaymentInstallments;
                datosCredito.FrecuenciaPago = credito.scheduleSettings.repaymentPeriodUnit;
                datosCredito.DiasFrecuenciaPago = credito.scheduleSettings.repaymentPeriodCount;
                datosCredito.FechaDesembolso = credito.disbursementDetails.expectedDisbursementDate.ToString("dd/MM/yyyy");

                if (datosCredito.FechaDesembolso == "01/01/0001")
                    datosCredito.FechaDesembolso = credito.creationDate.ToString("dd/MM/yyyy");

                if (credito._Datos_Adicionales != null)
                {
                    //datosCredito.MontoTotal = Convert.ToDecimal(credito._Datos_Adicionales.Total_Pagar);
                    if (credito._Datos_Adicionales.Monto_Seguro_Basico > 0)
                        datosCredito.PaqBasico = "X";
                    else
                        datosCredito.PaqBasico = " ";

                    if (credito._Datos_Adicionales.Tipo_Seguro_Voluntario == "PLATINO")
                        datosCredito.PaqPlatino = "X";
                    else
                        datosCredito.PaqPlatino = " ";

                    if (credito._Datos_Adicionales.Tipo_Seguro_Voluntario == "PREMIUM")
                        datosCredito.PaqPremium = "X";
                    else
                        datosCredito.PaqPremium = " ";
                    datosCredito.CAT = Convert.ToDecimal(credito._Datos_Adicionales.CAT_Datos_Adicionales);

                }
                datosCredito.GATReal = 17;
                datosCredito.GATNominal = 17;
                datosCredito.MedioDisposicionEfectivo = "X";
                datosCredito.MedioDisposicionChequera = "";
                datosCredito.MedioDisposicionTransferencia = "";
                datosCredito.LugarEfectuarRetiroVentanilla = "X";
                datosCredito.EnvioEstadoCuentaDom = "X";
                datosCredito.EnvioEstadoCuentaEmail = "";
                //datosCredito.TipoRenovacion = "Solo Capital";   


                if (credito._Referencias_Pago != null)
                {

                    foreach (var item in credito._Referencias_Pago)
                    {
                        switch (item.Referencia_Pago_Tipo)
                        {
                            case "BBVABANCOMER":
                                datosCredito.ReferenciaBBVA = item.Referencia_Pago_Valor;
                                break;
                            case "BANAMEX":
                                datosCredito.ReferenciaCitibanamex = item.Referencia_Pago_Valor;
                                break;
                            case "BANBAJIO":
                                datosCredito.ReferenciaBanBajio = item.Referencia_Pago_Valor;
                                break;
                            case "TELECOMM":
                                datosCredito.ReferenciaTelecomm = item.Referencia_Pago_Valor;
                                break;
                            case "SCOTIABANK":
                                datosCredito.ReferenciaScontiabank = item.Referencia_Pago_Valor;
                                break;
                            case "OXXOPAY":
                                datosCredito.ReferenciaOXXO = item.Referencia_Pago_Valor;
                                break;
                            case "HSBC":
                                datosCredito.ReferenciaBANSEFI = item.Referencia_Pago_Valor;
                                break;
                            case "CAME":
                                datosCredito.ReferenciaCame = item.Referencia_Pago_Valor;
                                break;
                            case "OPENPAY":
                                datosCredito.ReferenciaWalmart = item.Referencia_Pago_Valor;
                                break;
                            default:
                                break;
                        }
                    }
                }


            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }
            return datosCredito;
        }
        private DatosSolicitud AjustarCreditoGarantizado(DatosSolicitud solicitudEngine, AhorroDto ahorro, Mambu.Cliente cliente, List<RespTransacionesDepDto> listDepts, List<RespTransacionesDepDto> listInteresA)
        {
            gestorLog.Entrar();

            try
            {
                solicitudEngine.Ahorro = new DatosAhorro();

                RespTransacionesDepDto tmp = null;

                tmp = listDepts.LastOrDefault();

                solicitudEngine.Ahorro.Transaccion = new DatosTransaccion();
                solicitudEngine.Ahorro.Transaccion.Id = tmp.Id.ToString();
                solicitudEngine.Ahorro.Transaccion.EncodedKey = tmp.EncodedKey;
                solicitudEngine.Ahorro.Transaccion.Monto = tmp.Amount;

                solicitudEngine.Ahorro.FechaApertura = tmp.ValueDate.ToString() ?? "";
                solicitudEngine.Ahorro.HoraApertura = tmp.ValueDate.Substring(11, 8) ?? "";

                solicitudEngine.Ahorro.CreditoGarantizado = tmp.Amount * Convert.ToDecimal(0.75);
                solicitudEngine.Ahorro.CuentaAhorro = string.IsNullOrEmpty(ahorro.Id) ? "" : ahorro.Id;
                solicitudEngine.Ahorro.Rendimiento = listInteresA.FirstOrDefault().Amount;

                solicitudEngine.Ahorro.FechaActivacion = string.IsNullOrEmpty(ahorro.ActivationDate) ? "" : ahorro.ActivationDate;
                solicitudEngine.Ahorro.TasaAhorro = Convert.ToDecimal(ahorro.InterestSettings?.InterestRateSettings?.InterestRate ?? "0");
                solicitudEngine.Ahorro.GAT = ahorro._Datos_Adicionales_Cuenta?.Gat_Nominal ?? 0;
                solicitudEngine.Ahorro.GatReal = ahorro._Datos_Adicionales_Cuenta?.Gat_Real ?? 0;
                solicitudEngine.NombreCompleto = $"{cliente?.firstName ?? ""} {cliente?.lastName ?? ""}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Credito Garantizado: {ex.Message}");
            }
            gestorLog.Salir();

            return solicitudEngine;
        }


        private DatosSolicitud AjustarBancaDigitalEmpresarial(Mambu.AhorroDto ahorro, DatosSolicitud solicitudEngine, List<Mambu.Cliente> representantes, Mambu.Cliente cliente)
        {
            string aux = string.Empty;
            string valor = string.Empty;

            aux = ahorro?.AccountHolderType?.ToLower();

            switch (aux)
            {
                case "client":
                    valor = aux;
                    solicitudEngine.NombreCompletoFirma = $"{cliente?.firstName ?? ""} {cliente?.lastName ?? ""}"?.Trim() ?? string.Empty;
                    break;

                case "group":
                    valor = aux;
                    solicitudEngine.NombreCompletoFirma = ObtenerClientKey_RepresentanteLegalPM(cliente.groupMembers);
                    break;

                default:
                    break;
            }

            solicitudEngine.Ahorro = new DatosAhorro();
            solicitudEngine.Ahorro.TipoPersona = valor;
            solicitudEngine.Ahorro.FechaActivacion = string.IsNullOrEmpty(ahorro.ActivationDate) ? "" : ahorro.ActivationDate;

            DatosRepresentantes representante;
            solicitudEngine.Representantes = new List<DatosRepresentantes>();

            foreach (var item in representantes)
            {
                representante = new DatosRepresentantes();
                representante.NombreCompleto = $"{item.firstName} {item.lastName}";
                solicitudEngine.Representantes.Add(representante);
            };

            return solicitudEngine;
        }

        private string ObtenerClientKey_RepresentanteLegalPM(List<Member> groupMembers)
        {
            gestorLog.Entrar();

            string representante = string.Empty;

            try
            {
                foreach (var item in groupMembers)
                {
                    foreach (var rol in item.roles)
                    {
                        if (rol.roleName.ToUpper() == "REPRESENTANTE LEGAL")
                        {
                            representante = item.clientKey;
                            return ObtenerNombreRepresentanteLegal_PM(representante);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                gestorLog.RegistrarError(ex);
                new Exception("Error al intentar obtener el Representante Legal");
            }
            finally
            {
                gestorLog.Salir();
            }

            return representante;
        }

        private string ObtenerNombreRepresentanteLegal_PM(string clientKey)
        {
            gestorLog.Entrar();

            string result = string.Empty;

            if(!string.IsNullOrWhiteSpace(clientKey))
            {
                var configuracionMambu = configuration.GetSection("MambuCameAPI");
                var usuario = "";
                var contraseña = "";
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { Accept, AcceptApplication },
                    { configuracionMambu["HeaderAut"],configuracionMambu["HeaderValAut"]  },
                    { configuracionMambu["HeaderCoo"],configuracionMambu["HeaderValCoo"]  },
                };

                var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];

                Mambu.Cliente cliente = null;
                try
                {
                    cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlCliente, clientKey), usuario, contraseña, headers).Result;
                    result = $"{cliente.firstName} {cliente.lastName}";
                }
                catch (Exception ex)
                {
                    gestorLog.RegistrarError(ex);
                    if (ex.InnerException.Message == "NotFound")
                        throw new ServiceException("No se pudo obtener la información del Representante Legal.");
                    else
                    {
                        throw;
                    }
                }
            }

            gestorLog.Salir();

            return result;

        }

        /// <summary>
        /// Procesa el monto final a recibir en subproceso CreceMasEmpresas
        /// </summary>
        /// <param name="solicitud">Solicitud orignal</param>
        /// <param name="solicitudEngine">solicitud con datos</param>
        /// <param name="transacciones">Transacciones</param>
        private void ProcesarCrecemasEmpresas(ObtenerDatosJsonDto solicitud, DatosSolicitud solicitudEngine, List<TransaccionAhorroDto> transacciones, TransaccionAhorroDto transaccionActual)
        {
            if (string.IsNullOrEmpty(solicitudEngine.Ahorro.FechaVencimiento))
                throw new BusinessException("La cuenta no tiene fecha de vencimiento");

            var depositos = transacciones
                .Where(t => t.Type == Mambu.Enums.TipoTransaccionAhorro.DEPOSIT)
                .OrderBy(d => d.ValueDate)
                .ToList();

            var primeraTransaccion = depositos.First();
            var fechaInicial = primeraTransaccion.ValueDate.Date;
            var fechaFinal = Convert.ToDateTime(solicitudEngine.Ahorro.FechaVencimiento).Date;
            var montoInicial = primeraTransaccion.AccountBalances.TotalBalance;
            decimal tasa = (decimal)solicitudEngine.Ahorro.TasaAhorro / 100;

            var fechas = Enumerable.Range(0, (int)(fechaFinal - fechaInicial).TotalDays)
                                    .Select(offset => fechaInicial.AddDays(offset));

            decimal rendimiento = 0.00m;
            decimal montoFinal = montoInicial;
            var interesDiario = montoFinal * tasa / 360;
            var movimientos = depositos.Skip(1).ToList();

            foreach (var fecha in fechas)
            {
                if (fecha.Day == 1)
                {
                    montoFinal += Math.Round(rendimiento, 4);
                    interesDiario = montoFinal * tasa / 360;
                    rendimiento = 0.00m;
                }

                var incremento = movimientos
                    .Where(t => t.ValueDate <= transaccionActual.ValueDate && t.ValueDate.Date == fecha)
                    .Sum(t => t.Amount);

                montoFinal += incremento;
                interesDiario = montoFinal * tasa / 360;
                rendimiento += interesDiario;
            }

            var jsonObject = JsonConvert.DeserializeObject<JObject>(solicitud.JsonSolicitud);
            jsonObject["MontoRecibirFinalInversion"] = (montoFinal + rendimiento).ToString("#,##0.00");
            solicitud.JsonSolicitud = jsonObject.ToString();
        }

    }
}
