using System;
using cmn.std.Log;
using System.Linq;
using Newtonsoft.Json;
using cmn.std.Parametros;
using System.Collections.Generic;
using ServDocumentos.Core.Helpers;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Enumeradores;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Dtos.DatosEngine;
using ServDocumentos.Core.Dtos.DatosSybase;
using ServDocumentos.Core.Dtos.DatosTcrCaja;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using Mambu = ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using sql = ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using comun = ServDocumentos.Core.Contratos.Factories.Comun;
using ServSeguros = ServDocumentos.Core.Dtos.DatosServSeguros;
using sybase = ServDocumentos.Core.Contratos.Factories.TCR.Sybase;

namespace ServDocumentos.Servicios.TCR
{
    public class ServicioDatosPlantillas : ServicioBase, IServicioDatosPlantillasObtencion
    {
        private const string Accept = "Accept";
        private const string AcceptApplication = "application/vnd.mambu.v2+json";
        public ServicioDatosPlantillas(sybase.IUnitOfWork uowSybase, sql.IUnitOfWork uowSQL, comun.IUnitOfWork unitOfWork, IFactService factoryPago, GestorLog gestorLog, IConfiguration configuration, GestorParametros gestorParametros) : base(uowSybase, uowSQL, unitOfWork, factoryPago, gestorLog, configuration, gestorParametros)
        {
        }

        public string ObtenerDatosPlantilla(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();

            Cliente cliente = null;
            if (solicitud.Empresa == Empresa.TCR.ToString())
            {

            }
            RespuestaOrdenPago ordenPago = FactoryPago.ServicioOrdenPago.ObtieneDatosOrdenPagoAsync(solicitud.NumeroCredito).Result;
            if (ordenPago.CodigoRespuesta == 0)
            {
                ordenPago = null;
            }
            cliente = UnitOfWorkSybase.RepositorioDatosPlantillas.ObtenerDatosPlantilla(solicitud.NumeroCredito, solicitud.NumerosClientes, solicitud.NumeroDividendos, ordenPago);
            // cliente = UnitOfWorkSybase.RepositorioDatosPlantillas.ObtenerDatosPlantilla(solicitud.NumeroCredito, ordenPago);
            if (cliente == null)
            {
                throw new BusinessException("UnitOfWorkSybase - No se obtuvo informacion.");
            }

            if (cliente.BanderaExpres > 0)
            {
                var credito = UnitOfWorkSQL.RepositorioDatosPlantillas.ObtenerDatosPlantilla(solicitud.NumeroCredito);
                if (credito == null)
                {
                    throw new BusinessException("UnitOfWorkSQL - No se obtuvo informacion.");
                }
                cliente.Credito.DatoSolicitudCredito = credito;
            }

            var referencia = FactoryPago.ServicioReferenciaPago.ObtieneReferenciaPagoAsync(solicitud.NumeroCredito, solicitud.Usuario).Result;
            cliente.Credito.NumeroReferencia = (referencia.NoReferenciaPaynet == null ? "" : referencia.NoReferenciaPaynet);
            cliente.Credito.ReferenciaWalmart = (referencia.NoReferenciaWalMart == null ? "" : referencia.NoReferenciaWalMart);
            cliente.Credito.CodigoBarrasWalmart = (referencia.Codigo128WalMart == null ? "" : referencia.Codigo128WalMart);
            cliente.CodigoBarras = (referencia.Codigo128Paynet == null ? "" : referencia.Codigo128Paynet);

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            var jsonCliente = JsonConvert.SerializeObject(cliente, settings);



            gestorLog.Salir();
            return jsonCliente;
        }
        public string ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud)
        {
            gestorLog.Entrar();

            var jsonCliente = "";

            gestorLog.Salir();
            return jsonCliente;
        }

        public string ObtenerDatosPlantillaAhorro(ObtenerDatosDto solicitud)
        {
            const string Accept = "Accept";
            const string AcceptApplication = "application/vnd.mambu.v2+json";

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
            var credito = HTTPClientWrapper<Mambu.Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;

            var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            var pagos = HTTPClientWrapper<List<Mambu.Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;

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
                MissingMemberHandling = MissingMemberHandling.Ignore
                ,
                NullValueHandling = NullValueHandling.Ignore
                ,
                FloatParseHandling = FloatParseHandling.Decimal
            };
            var solicitudEngine = JsonConvert.DeserializeObject<DatosSolicitud>(solicitud.JsonSolicitud, settings);

            //SE OBTIENE LOS DATOS PARA CONEXION A MAMBU
            var configuracionMambu = configuration.GetSection("MambuApiTCR");

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
            var ahorro = HTTPClientWrapper<Mambu.AhorroDto>.Get(string.Format(urlAhorro, solicitud.NumeroCredito), usuario, contraseña, headers).Result;


            //OBTENER COTITULARES POR CLIENTE
            ClientesMambuDto solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };
            List<string> lUsuarios = new List<string>();
            //se cambia el orden dependiendo del index mambu
            ahorro._Relaciones_Ahorro = ahorro._Relaciones_Ahorro.OrderBy(o => o._index).ToList();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "COTITULAR")
                    lUsuarios.Add(item._Encoded_Key_Relacionado);
            }
            solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuarios });
            var json = JsonConvert.SerializeObject(solCliente);
            var urlCotitulares = configuracionMambu["Url"] + configuracionMambu["ClientesPOST"];
            var cotitulares = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;
            //se cambia el orden dependiendo del index mambu
            cotitulares = cotitulares.OrderBy(i => ahorro._Relaciones_Ahorro.FindIndex(o => o._Encoded_Key_Relacionado == i.encodedKey)).ToList();
            //se trae los tutores
            List<string> lUsuariosTutores = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "PADRE O TUTOR")
                    lUsuariosTutores.Add(item._Encoded_Key_Relacionado);
            }
            solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };
            solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuariosTutores });
            json = JsonConvert.SerializeObject(solCliente);
            var tutores = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;

            //trea al representante legal
            List<string> lUsuariosRepresentantes = new List<string>();
            foreach (var item in ahorro._Relaciones_Ahorro)
            {
                if (item._Tipo_Relacion == "REPRESENTANTE LEGAL")
                    lUsuariosRepresentantes.Add(item._Encoded_Key_Relacionado);
            }
            solCliente = new ClientesMambuDto()
            {
                FilterCriteria = new List<filterCriteria>()
            };
            solCliente.FilterCriteria.Add(new filterCriteria { Field = "encodedKey", Operator = "IN", Value = lUsuariosRepresentantes });
            json = JsonConvert.SerializeObject(solCliente);
            var representantes = HTTPClientWrapper<List<Mambu.Cliente>>.PutRequest(string.Format(urlCotitulares), "", "", headers, json.ToString()).Result;



            usuario = configuracionMambu["Usuario"];
            contraseña = configuracionMambu["Password"];
            headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var cliente = (dynamic)null;

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

            //MAPEAR CAMPOS MAMBU A DTO
            solicitudEngine = Equivalencias(ahorro, cliente, cotitulares, solicitudEngine, solicitud.SubProcesoNombre);

            solicitudEngine = RepresentanteTutores(solicitudEngine, representantes, tutores, cliente);

            if (solicitud.SubProcesoNombre.ToLower() == "tcrpatrimonial")
            {
                if (representantes != null && representantes.Count != 0)
                {
                    solicitudEngine.Estado = representantes[0]?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";

                    if (representantes[0]._Direccion_Clientes != null && string.IsNullOrWhiteSpace(solicitudEngine.DireccionCompleta)) 
                    {
                        solicitudEngine.DireccionCompleta = OrdenaDireccionCompleta(representantes[0]);
                    }

                }
                else if (tutores != null && tutores.Count != 0)
                {
                    if (tutores[0]._Direccion_Clientes != null && string.IsNullOrWhiteSpace(solicitudEngine.DireccionCompleta))
                    {
                        solicitudEngine.DireccionCompleta = OrdenaDireccionCompleta(tutores[0]);
                    }
                }
                else if (cliente._Direccion_Clientes != null && string.IsNullOrWhiteSpace(solicitudEngine.DireccionCompleta))
                {
                    solicitudEngine.DireccionCompleta = OrdenaDireccionCompleta(cliente);
                }
            }

            settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver()
                ,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            solicitudEngine.solicitudJSON = JsonConvert.DeserializeObject<object>(solicitud.JsonSolicitud);

            var jsonCliente = JsonConvert.SerializeObject(solicitudEngine, settings);

            gestorLog.Salir();
            return jsonCliente;
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

        public string ObtenerDatosPlantillaInversion(ObtenerDatosDto solicitud)
        {
            return null;
        }
        public string ObtenerDatosPlantillaInversionJson(ObtenerDatosJsonDto solicitud)
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
            var configuracionTCRMambu = configuration.GetSection("MambuApiTCR");
            var usuario = configuracionTCRMambu["Usuario"];
            var contraseña = configuracionTCRMambu["Password"];
            var urlCliente = configuracionTCRMambu["Url"] + configuracionTCRMambu["clientesGET"];
            var urlCredito = configuracionTCRMambu["Url"] + configuracionTCRMambu["CreditoGET"];
            var urlPago = configuracionTCRMambu["Url"] + configuracionTCRMambu["PagosGet"];
            var urlSucursal = configuracionTCRMambu["Url"] + configuracionTCRMambu["SucursalGET"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };
            Mambu.Credito credito;
            try
            {
                credito = HTTPClientWrapper<Mambu.Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe el crédito ");
                else
                    throw;
            }

            Mambu.Cliente cliente;
            try
            {
                cliente = HTTPClientWrapper<Mambu.Cliente>.Get(string.Format(urlCliente, credito.accountHolderKey), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existen datos del cliente. ");
                else
                    throw;
            }

            Mambu.PagosCameMambu pagos;
            try
            {
                pagos = HTTPClientWrapper<Mambu.PagosCameMambu>.Get(string.Format(urlPago, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la tabla de pagos ");
                else
                    throw;
            }
            if (credito.assignedBranchKey == null)
            {
                throw new ServiceException("El crédito no tiene una sucursal asignada");
            }
            Mambu.SucursalDto sucursal = null;
            try
            {
                sucursal = HTTPClientWrapper<Mambu.SucursalDto>.Get(string.Format(urlSucursal, credito.assignedBranchKey), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No existe la sucursal");
                else
                    throw;
            }
            if (sucursal != null)
            {
                solicitudEngine.Sucursal = sucursal.name ?? "";
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

                solicitudEngine.Credito.NumeroCreditos = nuneroCreditos;
                solicitudEngine.Pagos = datosMambu.TablaAmortizacionPagare
                                        .Select(x => new DatosTablaAmortizacionFormatoReferencias
                                        {
                                            Fecha = x.Fecha,
                                            Monto = x.Monto
                                        }
                                        )
                                        .ToList();
                solicitudEngine.Vigencia = datosMambu.Vigencia;
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

            if (solicitud.ProcesoNombre == "CreditosTeCreemos" && solicitud.SubProcesoNombre == "TeCreemosPalNegocio")
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

            gestorLog.Salir();
            return jsonCliente;

        }
        public string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();

            var estadoCuenta = UnitOfWorkSybase.RepositorioDatosPlantillas.ObtenerDatosEstadoCuenta(solicitud.NumeroCredito);
            if (estadoCuenta == null)
            {
                throw new BusinessException("UnitOfWorkSybase - No se obtuvo informacion.");
            }

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            var jsonEstadoCuenta = JsonConvert.SerializeObject(estadoCuenta, settings);

            gestorLog.Salir();
            return jsonEstadoCuenta;
        }

        public SolictudDocumentoDto AsignarValores(ObtenerPlantillasProcesoDto solicitud)
        {
            gestorLog.Entrar();
            SolictudDocumentoDto resultado = new SolictudDocumentoDto
            {
                ProcesoNombre = solicitud.Proceso.ToString(),
                SubProcesoNombre = solicitud.SubProcesoNombre,
                NumeroCredito = solicitud.NumeroCredito,
                NumeroCliente = solicitud.NumeroCliente,
                Separado = false,
                Comprimido = false,
                Usuario = solicitud.Usuario
            };
            gestorLog.Salir();
            return resultado;
        }

        public DocData EstadoCuentaObtenerDatos(SolictudDocumentoDto documSolicitud)
        {
            gestorLog.Entrar();
            DocData datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            gestorLog.Salir();
            return datosDocumento;
        }

        public EstadoCuenta ObtenerDatosJsonEstadoCuenta(SolictudDocumentoDto documSolicitud)
        {
            gestorLog.Entrar();

            ObtenerDatosDto solicitud = new ObtenerDatosDto
            {
                // Proceso = datosDocumento.ProcesoId,
                NumeroCredito = documSolicitud.NumeroCredito,
                NumeroCliente = documSolicitud.NumeroCliente,
                Usuario = documSolicitud.Usuario
            };

            string datosJson = ObtenerDatosEstadoCuenta(solicitud);

            //Se deserializa el objeto para mapear la documentación
            EstadoCuenta estadoCuenta = JsonConvert.DeserializeObject<EstadoCuenta>(datosJson);

            gestorLog.Salir();

            return estadoCuenta;
        }

        private DatosSolicitud Equivalencias(Mambu.AhorroDto ahorro, Mambu.Cliente cliente, List<Mambu.Cliente> cotitulares, DatosSolicitud solicitudEngine, string subProceso)
        {
            gestorLog.Entrar();

            try
            {
                solicitudEngine.Ahorro = AhorroToDatosAhorro(ahorro);

                if (cliente.firstName != null)
                    solicitudEngine.NombreCompleto = $"{cliente.firstName} {cliente.lastName}";

                if (cliente.groupName != null)
                    solicitudEngine.NombreCompleto = $"{cliente.groupName}";

                DatosCotitulares cotitular = null;
                solicitudEngine.Cotitulares = new List<DatosCotitulares>();

                foreach (var item in cotitulares)
                {
                    cotitular = new DatosCotitulares();
                    cotitular.NombreCompleto = $"{item.firstName} {item.lastName}";
                    solicitudEngine.Cotitulares.Add(cotitular);
                };

                solicitudEngine.NumeroCliente = cliente.id;
                solicitudEngine.Ahorro.CuentaDeposito = string.IsNullOrEmpty(ahorro._Datos_Adicionales?.Cuenta_Deposito) ? "" : ahorro._Datos_Adicionales?.Cuenta_Deposito;
                solicitudEngine.Ahorro.FechaVencimiento = string.IsNullOrEmpty(ahorro.maturityDate) ? "" : ahorro.maturityDate;
                switch (subProceso)
                {
                    case "TCRPatrimonial":
                        {
                            solicitudEngine.Estado = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
                        }; break;
                    default:
                        break;
                }
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
                datosAhorro.GAT = ahorro._Datos_Adicionales?.Gat_Nominal ?? 0;
                datosAhorro.GatReal = ahorro._Datos_Adicionales?.Gat_Real ?? 0;
                datosAhorro.Contrato = ahorro.Id ?? "0";
                datosAhorro.Plazo = ahorro._Datos_Adicionales?._Plazo ?? 0;
                datosAhorro.NoConstancia = ahorro._Datos_Adicionales?.No_Constancia ?? 0;
                datosAhorro.Monto = Convert.ToDecimal(ahorro.InternalControls.RecommendedDepositAmount);
                datosAhorro.DiasTermino = ahorro._Datos_Adicionales?.Dias_Termino_Inversion ?? 0;
                datosAhorro.TipoPersona = ahorro._Datos_Adicionales?._TipoPersona;
            }
            catch (Exception)
            {

            }
            finally
            {
                gestorLog.Salir();
            }

            return datosAhorro;
        }

        public string ObtenerDatosPlantillaBC2020(ObtenerDatosDto solicitud)
        {
            return null;
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

        public string ObtenerDatosPlantillaAhorroTeChreoPatrimonial(ObtenerDatosDto solicitud)
        {
            throw new NotImplementedException();
        }

        public string ObtenerDatosPlantillaSeguros(ObtenerDatosDto solicitud)
        {
            throw new NotImplementedException();
        }

        private DatosSolicitud Equivalencias(Mambu.Credito credito, Mambu.PagosCameMambu pagos, string subProceso)
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
                if (listaTablaAmortizacion.Count == 0 && (subProceso.ToUpper() == "TECREEMOSPALNEGOCIO"))
                {
                    if (fechaAnterior == "")
                        fechaAnterior = credito.creationDate.AddDays(-1).ToString();
                    montoFees = credito.disbursementDetails.fees.Sum(i => i.amount);
                    monto = pagos.installments.Sum(x => x.principal.amount.due);
                }

                foreach (var item in pagos.installments)
                {
                    if (listaTablaAmortizacion.Count == 0 && (subProceso.ToUpper() == "TECREEMOSPALNEGOCIO"))
                    {
                        tablaAmortizacion = PagoCero(item, monto, out valor);

                        if (subProceso.ToUpper() == "TECREEMOSPALNEGOCIO")
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

                datosCredito = CreditoToDatosCredito(credito);
                datosCredito.CuotaMensual = listaPagare[0].Monto;
                datosCredito.FechaPrimerPago = listaPagare[0].Fecha;
                datosCredito.MontoTotal = listaTablaAmortizacion.Sum(i => i.PagoTotal);
                jsonDatosSolicitud.Vigencia = listaPagare[listaPagare.Count() - 1].Fecha;
                jsonDatosSolicitud.TablaAmortizacion = listaTablaAmortizacion;
                jsonDatosSolicitud.TablaAmortizacionPagare = listaPagare;
                jsonDatosSolicitud.Credito = datosCredito;
                jsonDatosSolicitud.Credito.MontoCreditoNeto = pagos.installments.Sum(x => x.principal.amount.due);

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

        private DatosTablaAmortizacion PagoCero(Mambu.Installment pago, double montoCredito, out double valor)
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

        private DatosTablaAmortizacion PagosToTablaAmortizacion(Mambu.Installment pago, double montoCredito, out double valor)
        {
            DatosTablaAmortizacion tablaAmortizacion = new DatosTablaAmortizacion();
            double valor1 = montoCredito;
            valor = 0;
            try
            {
                tablaAmortizacion.NumeroPago = Convert.ToInt16(pago.number);
                tablaAmortizacion.FechaLimitePago = Convert.ToDateTime(FechaOffset(pago.dueDate)).ToString("dd/MM/yyyy");
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

        private DatosTablaAmortizacionPagare PagosToTablaAmortizacionPagare(Mambu.Installment pago)
        {
            DatosTablaAmortizacionPagare pagare = new DatosTablaAmortizacionPagare();
            try
            {
                pagare.NumeroMensual = Convert.ToInt16(pago.number);
                pagare.Fecha = Convert.ToDateTime(FechaOffset(pago.dueDate)).ToString("dd/MM/yyyy");
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

        private DatosCredito CreditoToDatosCredito(Mambu.Credito credito)
        {
            DatosCredito datosCredito = new DatosCredito();
            try
            {

                datosCredito.NumeroCredito = credito.id;
                datosCredito.NumeroCliente = credito.accountHolderKey;
                datosCredito.FechaInicio = credito.creationDate.ToString("dd/MM/yyyy");
                datosCredito.Producto = credito.loanName;
                datosCredito.MontoCredito = Convert.ToDecimal(credito.loanAmount);
                datosCredito.EstatusCredito = credito.accountState;
                datosCredito.TasaInteresAnual = Convert.ToDecimal(credito.interestSettings.interestRate);
                datosCredito.TasaMoratoriaAnual = Convert.ToDecimal(credito.penaltySettings.penaltyRate * 360);

                datosCredito.PlazoGarcia = "";
                datosCredito.Plazo = credito.scheduleSettings.repaymentInstallments;
                datosCredito.FrecuenciaPago = credito.scheduleSettings.repaymentPeriodUnit;
                datosCredito.DiasFrecuenciaPago = credito.scheduleSettings.repaymentPeriodCount;
                datosCredito.FechaDesembolso = credito.disbursementDetails.expectedDisbursementDate.ToString("dd/MM/yyyy");

                if (datosCredito.FechaDesembolso == "01/01/0001")
                    datosCredito.FechaDesembolso = credito.creationDate.ToString("dd/MM/yyyy");

                if (credito._Datos_Adicionales_Credito != null)
                {
                    //datosCredito.MontoTotal = Convert.ToDecimal(credito._Datos_Adicionales_Credito.Total_Pagar);
                    if (credito._Datos_Adicionales_Credito.Monto_Seguro_Basico > 0)
                        datosCredito.PaqBasico = "X";
                    else
                        datosCredito.PaqBasico = " ";

                    if (credito._Datos_Adicionales_Credito.Tipo_Seguro_Voluntario == "PLATINO")
                        datosCredito.PaqPlatino = "X";
                    else
                        datosCredito.PaqPlatino = " ";

                    if (credito._Datos_Adicionales_Credito.Tipo_Seguro_Voluntario == "PREMIUM")
                        datosCredito.PaqPremium = "X";
                    else
                        datosCredito.PaqPremium = " ";
                    datosCredito.CAT = Convert.ToDecimal(credito._Datos_Adicionales_Credito.CAT_Datos_Adicionales);
                    datosCredito.ReferenciaNumeroRetiro = credito._Datos_Adicionales_Credito.Referencia_Retiro;

                }
                datosCredito.GATReal = 17;
                datosCredito.GATNominal = 17;
                datosCredito.MedioDisposicionEfectivo = "X";
                datosCredito.MedioDisposicionChequera = "";
                datosCredito.MedioDisposicionTransferencia = "";
                datosCredito.LugarEfectuarRetiroVentanilla = "X";
                datosCredito.EnvioEstadoCuentaDom = "X";
                datosCredito.EnvioEstadoCuentaEmail = "";

                if (credito._Referencias_Pago != null)
                {

                    foreach (var item in credito._Referencias_Pago)
                    {
                        switch (item.Referencia_Pago_Tipo)
                        {
                            case "BBVABANCOMER":
                                datosCredito.ReferenciaBBVA = item.Referencia_Pago_Valor;
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
                            case "BANAMEX":
                                datosCredito.ReferenciaCitibanamex = item.Referencia_Pago_Valor;
                                break;
                            case "OXXOPAY":
                                datosCredito.ReferenciaOXXO = item.Referencia_Pago_Valor;
                                break;
                            case "HSBC":
                                datosCredito.ReferenciaBANSEFI = item.Referencia_Pago_Valor;
                                break;
                            case "TCR":
                                datosCredito.ReferenciaTCR = item.Referencia_Pago_Valor;
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

        private string FechaOffset(string valor)
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
                solicitudEngine.BeneficiariosSeguros = beneficiarios;
            }
            gestorLog.Salir();
        }
        public string ObtenerDatosPlantillaAhorroJsonSinDatos(ObtenerDatosJsonDatosDto solicitud)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Genera una direccion con un orden definido para los datos *@+
        /// </summary>
        /// <param name="_calle"></param>
        /// <param name="_numext"></param>
        /// <param name="_numint"></param>
        /// <param name="_col"></param>
        /// <param name="_mun"></param>
        /// <param name="_edo"></param>
        /// <param name="_cp"></param>
        /// <returns>string con la direccion ordenada</returns>
        private string OrdenaDireccionCompleta(Mambu.Cliente cliente)
        {
            string calle = cliente?._Direccion_Clientes?.Calle_Direccion_Cliente ?? "";
            string mun = cliente?._Direccion_Clientes?.Municipio_Direccion_Cliente ?? "";
            string cp = cliente?._Direccion_Clientes?.CP_Direccion_Cliente ?? "";
            string col = cliente?._Direccion_Clientes?.Colonia_Direccion_Cliente ?? "";
            string edo = cliente?._Direccion_Clientes?.Estado_Direccion_Cliente ?? "";
            string numext = cliente?._Direccion_Clientes?.Numero_Ext_Direccion_Cliente ?? "";
            string numint = cliente?._Direccion_Clientes?.Numero_Int_Direccion_Cliente ?? "";
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
            return dir;
        }//fin OrdenaDireccionCompleta

        public string ObtenerDatosPlantillaMercado(ObtenerDatosJsonDto solicitud)
        {
            throw new NotImplementedException();
        }
    }
}
