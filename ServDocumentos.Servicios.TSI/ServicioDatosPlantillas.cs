using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Helpers;
using System;
using System.Collections.Generic;

namespace ServDocumentos.Servicios.TSI
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
            gestorLog.Entrar();
            var configuracionMambu = configuration.GetSection("MambuAPI");
            var usuario = configuracionMambu["Usuario"];
            var contraseña = configuracionMambu["Password"];
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { Accept, AcceptApplication }
            };

            var urlCliente = configuracionMambu["Url"] + configuracionMambu["ClientesGET"];
            Cliente cliente = null;
            try
            {
                cliente = HTTPClientWrapper<Cliente>.Get(string.Format(urlCliente, solicitud.NumeroCliente), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se encontraron datos del cliente.");
                else
                    throw;
            }

            var urlCredito = configuracionMambu["Url"] + configuracionMambu["CreditosGET"];
            Credito credito = null;
            try
            {
                credito = HTTPClientWrapper<Credito>.Get(string.Format(urlCredito, solicitud.NumeroCredito), usuario, contraseña, headers).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "NotFound")
                    throw new ServiceException("No se encontraron datos del crédito.");
                else
                    throw;
            }

            //var urlPagos = configuracionMambu["Url"] + configuracionMambu["PagosGET"];
            //List<Pagos> pagos = null;
            //try
            //{
            //    pagos = HTTPClientWrapper<List<Pagos>>.Get(string.Format(urlPagos, solicitud.NumeroCredito), usuario, contraseña, null).Result;
            //}
            //catch (Exception ex)
            //{
            //    if (ex.InnerException.Message == "NotFound")
            //        throw new ServiceException("No se encontraron datos de los Pagos.");
            //    else
            //        throw;
            //}

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

            //solicitudEngine.Pagos = new List<Pagos>();
            cliente.Pagos = new List<Pagos>();
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
                        //penaltyDue = item.interest.amount.due.ToString(),
                        penaltyDue = item.penalty.amount.due.ToString(),
                        //penaltyPaid = item.interest.amount.paid.ToString(),
                        penaltyPaid = item.penalty.amount.paid.ToString(),
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

                    cliente.Pagos.Add(p);
                }
            }

            cliente.Credito = credito;
            //cliente.Pagos = pagos;

            cliente.Credito.TasaMoratoriaAnual = credito._Datos_Adicionales.Tasa_Moratoria_Anual;
            cliente.Credito.CuotaMensual = credito._Datos_Adicionales.Monto_Cuota;
            cliente.Credito.InteresMoratorioMensual = credito._Datos_Adicionales.Tasa_Moratoria_Anual / 12;

            cliente.Credito._Datos_Adicionales.Referencia_Pago = credito._Datos_Adicionales?.Referencia_Pago;
            cliente.Credito._Datos_Adicionales.Referencia_Pago_OXXO = credito._Datos_Adicionales?.Referencia_Pago_OXXO;

            if (credito._Referencias_Pago != null)
            {
                foreach (var item in credito._Referencias_Pago)
                {
                    if (!string.IsNullOrEmpty(item.Referencia_Pago_Valor))
                    {
                        if (item.Referencia_Pago_Tipo.ToUpper().Equals("OPENPAY"))
                        {
                            cliente.Credito.ReferenciaPagoOPENPAY = item.Referencia_Pago_Valor;
                        }

                        if (item.Referencia_Pago_Tipo.ToUpper().Equals("BBVABANCOMER"))
                        {
                            cliente.Credito.ReferenciaPagoBBVA = item.Referencia_Pago_Valor;
                        }

                        if (item.Referencia_Pago_Tipo.ToUpper().Equals("NUMERO"))
                        {
                            cliente.Credito.ReferenciaPagoNumero = item.Referencia_Pago_Valor;
                        }
                    }
                }
            }

            var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
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
            return "";
        }
        public string ObtenerDatosPlantillaAhorroJson(ObtenerDatosJsonDto solicitud)
        {
            return "";
        }
        public string ObtenerDatosPlantillaInversion(ObtenerDatosDto solicitud)
        {
            return null;
        }
        public string ObtenerDatosPlantillaInversionJson(ObtenerDatosJsonDto solicitud)
        {
            return null;
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

        public string ObtenerDatosPlantillaBC2020(ObtenerDatosDto solicitud)
        {
            return null;

        }

        public SolictudDocumentoDto AsignarValores(EstadoCuentaMensualSol solicitud)
        {
            throw new NotImplementedException();
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

        public string ObtenerDatosPlantillaAhorroTeChreoPatrimonial(ObtenerDatosDto solicitud)
        {
            throw new NotImplementedException();
        }

        public string ObtenerDatosPlantillaSeguros(ObtenerDatosDto solicitud)
        {
            throw new NotImplementedException();
        }
        public string ObtenerDatosPlantillaAhorroJsonSinDatos(ObtenerDatosJsonDatosDto solicitud)
        {
            throw new NotImplementedException();
        }
        
        public string ObtenerDatosPlantillaMercado(ObtenerDatosJsonDto solicitud)
        {
            throw new NotImplementedException();
        }
    }
}
