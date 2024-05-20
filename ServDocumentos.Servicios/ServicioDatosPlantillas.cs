using System;
using cmn.std.Log;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.Core.Excepciones;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Entidades.Comun;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioDatosPlantillas : ServicioBase, IServicioDatosPlantillas
    {
        public ServicioDatosPlantillas(GestorLog gestorLog, IUnitOfWork unitOfWork, IConfiguration configuration, Func<string, IServiceFactoryComun> serviceProvider, IServiceFactory factory) : base(gestorLog, unitOfWork, configuration, serviceProvider, factory)
        {
        }

        public string ObtenerDatosPlantilla(ObtenerDatosJsonDto solicitud)
        {
            gestorLog.Entrar();

            string jsonCliente = string.Empty;
            var datosJson = factory.ServicioDatosJson.Obtener(solicitud.NumeroCredito);

#if DEBUG
            datosJson = null;
#endif

            if (datosJson == null || string.IsNullOrEmpty(datosJson.Json))
            {

                jsonCliente = ConsultarDatosPlantilla(solicitud);

                DatosJson datos = new DatosJson
                {
                    Credito = solicitud.NumeroCredito,
                    UsuarioCreacion = solicitud.Usuario,
                    Json = jsonCliente
                };

                factory.ServicioDatosJson.Insertar(datos);
            }
            else
                jsonCliente = datosJson.Json;

            gestorLog.Salir();
            return jsonCliente;
        }
        public string ObtenerDatosPlantilla(ObtenerDatosDto solicitud)
        {
            gestorLog.Entrar();

            string jsonCliente = string.Empty;
            var datosJson = factory.ServicioDatosJson.Obtener(solicitud.NumeroCredito);
#if DEBUG
            datosJson = null;
#endif
            if (datosJson == null || string.IsNullOrEmpty(datosJson.Json))
            {
                jsonCliente = ConsultarDatosPlantilla(solicitud);

                DatosJson datos = new DatosJson
                {
                    Credito = solicitud.NumeroCredito,
                    UsuarioCreacion = solicitud.Usuario,
                    Json = jsonCliente
                };

                factory.ServicioDatosJson.Insertar(datos);
            }
            else
                jsonCliente = datosJson.Json;

            gestorLog.Salir();
            return jsonCliente;
        }

        public void ResetearDatos(SolicitudBaseDto solicitud)
        {
            gestorLog.Entrar();
            var datosJson = factory.ServicioDatosJson.Obtener(solicitud.NumeroCredito);
            factory.ServicioDatosJson.Eliminar(new DatosJson { Credito = solicitud.NumeroCredito, UsuarioModificacion = solicitud.Usuario });

            if (datosJson == null)
                throw new BusinessException($"{MensajesServicios.CreditoNoEncontrado}{solicitud.NumeroCredito}");

            gestorLog.Salir();
        }

        public string ObtenerDatosEstadoCuenta(ObtenerDatosDto solicitud)

        {
            gestorLog.Entrar();

            string jsonEstadoCuenta = string.Empty;
            jsonEstadoCuenta = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosEstadoCuenta(solicitud);

            gestorLog.Salir();
            return jsonEstadoCuenta;
        }

        public string ObtenerDatosPlantilla(ObtenerDatosJsonDatosDto solicitud)
        {
            gestorLog.Entrar();

            string jsonCliente = string.Empty;
            string procesoNombre = string.Empty;

            procesoNombre = solicitud.ProcesoNombre.ToLower();

            switch (solicitud.Empresa)
            {
                case "CAME":
                    switch (procesoNombre)
                    {
                        case "ahorrocame":
                            jsonCliente = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorroJsonSinDatos(solicitud);
                            break;
                    }
                    break;
            }

            gestorLog.Salir();
            return jsonCliente;
        }

        /// <summary>
        /// Obtiene los datos de la plantilla
        /// </summary>
        /// <param name="solicitud">Informacion de la solicitud</param>
        /// <returns></returns>
        private string ConsultarDatosPlantilla(ObtenerDatosJsonDto solicitud)
        {
            switch (solicitud.Empresa)
            {
                case "TCR":
                    switch (solicitud.ProcesoNombre)
                    {
                        case "AhorroTcr":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorroJson(solicitud);
                        //case "Invercamex":
                        //    jsonCliente = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaInversionJson(solicitud);
                        //    break;
                        case "CreditosTeCreemos":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaInversionJson(solicitud);
                        default:
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
                    }
                case "CAME":
                    switch (solicitud.ProcesoNombre)
                    {
                        case "AhorroCame":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorroJson(solicitud);
                        case "Invercamex":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaInversionJson(solicitud);
                        case "CreditoCAME":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaMercado(solicitud);
                        default:
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
                    }
                default:
                    return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
            }
        }
        /// <summary>
        /// Obtiene los datos de la plantilla
        /// </summary>
        /// <param name="solicitud">Informacion de la solicitud</param>
        /// <returns></returns>
        private string ConsultarDatosPlantilla(ObtenerDatosDto solicitud)
        {
            switch (solicitud.Empresa)
            {
                case "TCR":
                    switch (solicitud.Proceso)
                    {
                        case "AhorroTcr":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorro(solicitud);
                        // Se comento porque el requerimiento palnegocio tecreemos se cancelo.
                        //case "CreditosTeCreemos":
                        //    return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaInversion(solicitud);
                        default:
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
                    }
                case "CAME":
                    switch (solicitud.Proceso)
                    {
                        case "SegurosCame":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaSeguros(solicitud);
                        case "Techreo":
                        case "AhorroCame":
                            if (solicitud.Subproceso == "BoomCardPaquete" || solicitud.Subproceso == "TeChreoPatrimonial1" || solicitud.Subproceso == "TeChreoPatrimonial2" || solicitud.Subproceso == "ClubCAME" || solicitud.Subproceso == "CreceMas" || (solicitud.Subproceso.ToLower() == "creditogarantizado") ||  solicitud.Subproceso.ToLower() == "credgaranti-legal"
                                || solicitud.Subproceso.ToLower() == "bancadigitalempresarial" || solicitud.Subproceso == "KjaMas")
                                return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorroTeChreoPatrimonial(solicitud);
                            else
                                return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaAhorro(solicitud);
                        case "Invercamex":
                            return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaInversion(solicitud);
                        default:
                            if (solicitud.Subproceso == "CAME_GRUPAL" || solicitud.Subproceso == "CAME_INTERCICLO")
                                return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantillaBC2020(solicitud);
                            else
                                return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
                    }
                default:
                    return serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
            }
        }

        public string  ObtenerDatos(ObtenerDatosDto solicitud)
        {
           

            gestorLog.Entrar();
            var datosJson = factory.ServicioDatosJson.Obtener(solicitud.NumeroCredito);
         
            if (datosJson == null)
                throw new BusinessException($"{MensajesServicios.CreditoNoEncontrado}{solicitud.NumeroCredito}");

            gestorLog.Salir();
            return datosJson.Json; 
        }
    }
}
