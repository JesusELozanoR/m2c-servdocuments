using System;
using System.Collections.Generic;
using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using sql = ServDocumentos.Core.Contratos.Factories.TCR.SQL;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using System.Threading.Tasks;
using ServDocumentos.Core.Dtos.DatosEdoCuenta;
using Newtonsoft.Json;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Helpers;
using System.Linq;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;

namespace ServDocumentos.Servicios.TCR
{
    public class ServicioEstadoCuentaMensual : ServicioBase, IServicioEstadoCuentaMensual, IDisposable
    {
        private const string CoreId = "TCR";
        /// <summary>
        /// Unidad de trabajo MAMBU API
        /// </summary>
        private readonly IUnitOfWorkApi _unitOfWorkApi;
        /// <summary>
        /// Canales de transaccion
        /// </summary>
        private IEnumerable<CanalTransaccionDto> _canlesTrasanccion;
        /// <summary>
        /// Cantidad de transacciones por peticion  a mambu (Maximo 1000)
        /// </summary>
        private const int LimiteTransacciones = 50;
        /// <summary>
        /// Productos permitidos para generar estados de cuenta
        /// </summary>
        private List<ProductoDto> _productos = new List<ProductoDto>();
        public ServicioEstadoCuentaMensual(
            sql.IUnitOfWorkMambu uowSQLMambu,
            GestorLog gestorLog,
            IConfiguration configuration,
            IMambuRepositoryFactory mambuFactory) : base(uowSQLMambu, gestorLog,  configuration)
        {
            _unitOfWorkApi = mambuFactory.Crear(Core.Enumeradores.Empresa.TCR);
        }

        public void Dispose()
        {
        }

        //public SolictudDocumentoDto AsignarValores(EstadoCuentaMensualSol solicitud)
        //{
        //    gestorLog.Entrar();
        //    SolictudDocumentoDto resultado = new SolictudDocumentoDto
        //    {
        //        ProcesoNombre = solicitud.Proceso.ToString(),
        //        SubProcesoNombre = solicitud.SubProceso,
        //        NumeroCredito = solicitud.NumeroCredito,
        //        NumeroCliente = solicitud.NumeroCliente,
        //    };
        //    gestorLog.Salir();
        //    return resultado;
        //}

        public int EstadoCuentaMensualProcesa(DateTime fecha)
        {
            gestorLog.Entrar();
            int resultado = UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.EstadoCuentaMensualProcesa(fecha);
            gestorLog.Salir();
            return resultado;
        }
        public async Task<int> EstadoCuentaMensualProcesaAsync(DateTime fecha)
        {
            gestorLog.Entrar();
            int resultado = await UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.EstadoCuentaMensualProcesaAsync(fecha);
            gestorLog.Salir();
            return resultado;
        }
        public List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            List<EstadoCuentaMensualProcResp> resultado = UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.EstadosCuentaMensualObtiene(solicitud);
            gestorLog.Salir();
            return resultado;
        }

        public string ObtenerDatosEstadoCuenta(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            ObtenerCanales();
            ObtenerProductos();
            string resultado;
            var cliente = _unitOfWorkApi.RepositorioClientes.GetById(documSolicitud.NumeroCliente, DetailsLevel.FULL).Result;
            if (cliente == null)
                throw new BusinessException($"El número de cliente: {documSolicitud.NumeroCliente} no existe.");

            var cuenta = _unitOfWorkApi.RepositorioAhorros.GetById(documSolicitud.NumeroCuenta, DetailsLevel.FULL).Result;
            if (cuenta == null)
                throw new BusinessException($"El número de cuenta: {documSolicitud.NumeroCuenta} no existe.");

            if (!_productos.Any(p => p.EncodedKey == cuenta.ProductTypeKey))
                throw new BusinessException($"El numero de cuenta: {cuenta.Id} no corresponde a un producto valido");

            var inicioMes = new DateTime(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month, 02, 00, 00, 01);
            var finalMes = new DateTime(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month, DateTime.DaysInMonth(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month)).AddDays(1);

            var encodedKeyTutor = cuenta._Relaciones_Ahorro.FirstOrDefault(c => c._Tipo_Relacion.ToUpper() == "PADRE O TUTOR")?._Encoded_Key_Relacionado;
            var direccionCliente = cliente._Direccion_Clientes;
            if (!string.IsNullOrEmpty(encodedKeyTutor))
            {
                var tutor = _unitOfWorkApi.RepositorioClientes.GetById(encodedKeyTutor, DetailsLevel.FULL).Result;
                direccionCliente = tutor._Direccion_Clientes;
            }

            var estadoCuenta = new EstadoCuenta()
            {
                NumeroCuenta = cuenta.Id,
                NumeroCliente = cliente.id,
                NombreCliente = $"{cliente.firstName} {cliente.middleName} {cliente.lastName}".Replace("  ", " ").Trim(),
                RFC = cliente._Datos_Personales_Clientes?.RFC_Datos_Personales_Cliente,
                Telefono = cliente.mobilePhone,
                Moneda = cuenta.CurrencyCode,
                TasaAnual = Convert.ToDecimal(cuenta.InterestSettings.InterestRateSettings.InterestRate),
                Estado = cuenta.AccountState,
                PeriodoDel = inicioMes.ToString("dd-MM-yyyy"),
                PeriodoAl = finalMes.ToString("dd-MM-yyyy"),
                Ejecutivo = string.Empty,
                GATNominal = cuenta._Datos_Adicionales?.Gat_Nominal.GetValueOrDefault(0) ?? 0.00M,
                GATReal = cuenta._Datos_Adicionales?.Gat_Real.GetValueOrDefault(0) ?? 0.00M,
                Correo = cliente.emailAddress,
                SaldoMinimo = 0,
                Comisiones = 0,
                OtrosCargos = 0,
                Direccion1 = $"{direccionCliente?.Calle_Direccion_Cliente} {direccionCliente?.Numero_Ext_Direccion_Cliente}, {direccionCliente?.Numero_Int_Direccion_Cliente}".Replace("  ", " ").Trim(new char[] { ' ', ',' }),
                Direccion2 = $"{direccionCliente?.Colonia_Direccion_Cliente}, {direccionCliente?.CP_Direccion_Cliente}".Trim(new char[] { ' ', ',' }),
                Direccion3 = direccionCliente?.Municipio_Direccion_Cliente,
                Direccion4 = direccionCliente?.Estado_Direccion_Cliente
            };

            estadoCuenta.Transacciones = ObtenerTransacciones(cuenta.EncodedKey, documSolicitud.Fecha).ToList();

            var cuentaActivaEnPeriodo = Convert.ToDateTime(cuenta.ActivationDate) <= finalMes && Convert.ToDateTime(cuenta.ActivationDate) >= inicioMes;

            if (cuentaActivaEnPeriodo)
            {
                estadoCuenta.SaldoInicial = 0;
                estadoCuenta.SaldoDiario = estadoCuenta.Transacciones.FirstOrDefault()?.Cantidad ?? 0;
            }
            else
            {
                var searchCriteria = new SearchCriteria();
                searchCriteria.AgregarFiltro("parentAccountKey", "EQUALS", cuenta.EncodedKey);
                searchCriteria.AgregarFiltro("valueDate", "BEFORE", inicioMes.AddDays(-1).ToString("yyyy-MM-dd"));
                searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");

                searchCriteria.AgregarOrdenamiento("valueDate", "DESC");
                searchCriteria.AgregarLimite(LimiteTransacciones);
                var ultimasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.Search(searchCriteria).Result;
                var transaccion = ultimasTransacciones.FirstOrDefault(t => t.Type.HasValue && !t.Type.Value.ToString().Contains("ADJUST"));
                if (transaccion != null)
                    estadoCuenta.SaldoInicial = transaccion.AccountBalances?.TotalBalance ?? 0.00m;
                else
                {
                    while (true)
                    {
                        searchCriteria.AgregarOffset(ultimasTransacciones.Count());
                        ultimasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.Search(searchCriteria).Result;
                        transaccion = ultimasTransacciones.FirstOrDefault(t => t.Type.HasValue && !t.Type.Value.ToString().Contains("ADJUST"));
                        if (transaccion != null)
                        {
                            estadoCuenta.SaldoInicial = transaccion.AccountBalances?.TotalBalance ?? 0.00m;
                            break;
                        }
                        if (ultimasTransacciones.Count() < LimiteTransacciones)
                        {
                            estadoCuenta.SaldoInicial = 0.00m;
                            break;
                        }
                    }
                }
                estadoCuenta.SaldoDiario = estadoCuenta.SaldoInicial;
            }

            estadoCuenta.Rendimientos = estadoCuenta.Transacciones.Where(t => t.Descripcion.Contains("Interés Aplicado")).Sum(t => t.Cantidad);
            estadoCuenta.ImpuestosRetenidos = estadoCuenta.Transacciones.Where(t => t.Descripcion.Contains("ISR")).Sum(t => t.Cantidad);
            estadoCuenta.DepositosD = estadoCuenta.Transacciones.Where(t => t.Transaccion.Contains("Depósito")).Sum(t => t.Cantidad);
            estadoCuenta.RetirosD = estadoCuenta.Transacciones.Where(t => t.Transaccion.Contains("Retiro")).Sum(t => t.Cantidad) * -1;
            estadoCuenta.Depositos = estadoCuenta.Transacciones.Count(t => t.Transaccion.Contains("Depósito"));
            estadoCuenta.Retiros = estadoCuenta.Transacciones.Count(t => t.Transaccion.Contains("Retiro"));
            estadoCuenta.SaldoFinal = (estadoCuenta.SaldoInicial + estadoCuenta.DepositosD) - estadoCuenta.RetirosD;

            ObtenerValorUdiPesos(estadoCuenta, documSolicitud.Fecha);
            resultado = JsonConvert.SerializeObject(estadoCuenta);
            gestorLog.Salir();
            return resultado;
        }

        public async Task<int>  ActualizaEstadoCuentaAsync(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            var resultado = await UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.ActualizaDatosEstadoCuentaAsync(documSolicitud);
            gestorLog.Salir();
            return resultado;
        }

        public int ActualizaEstadoCuenta(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            var resultado =  UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.ActualizaDatosEstadoCuenta(documSolicitud);
            gestorLog.Salir();
            return resultado;
        }
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(DateTime fecha)
        {
            gestorLog.Entrar();
            var resultado = UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.ObtenerBitacorasEstadoCuenta(fecha);
            gestorLog.Salir();
            return resultado;
        }
        /// <summary>
        /// Inserta un estado de cuenta
        /// </summary>
        /// <param name="numeroCliente">Numero de cliente</param>
        /// <param name="numeroCuenta">Numero de cuenta</param>
        /// <param name="fecha">Fecha de generacion</param>
        /// <param name="estado">Estado</param>
        /// <param name="correo">Correo al que se enviara el email</param>
        public bool InsertarEstadoDeCuenta(string numeroCliente, string numeroCuenta, DateTime fecha, string estado, string correo)
        {
            gestorLog.Entrar();
            var resultado = UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.InsertarEstadoDeCuenta(numeroCliente, numeroCuenta, fecha, estado, correo);
            gestorLog.Salir();
            return resultado;
        }

        private void ObtenerValorUdiPesos(EstadoCuenta estadoCuenta, DateTime fecha)
        {
            var CoreBankAPI = configuration.GetSection("CoreBank");
            var url = CoreBankAPI["Url"] + CoreBankAPI["UDIObtenerPorFecha"];
            var body = new Dictionary<string, object>()
            {
                { "coreId", CoreId },
                { "fecha", fecha }
            };

            var result = HTTPClientApi.PostRequest(url, string.Empty, string.Empty, null, null, body).Result;

            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync();
                var udi = JsonConvert.DeserializeObject<UDI>(content.Result);

                //if (udi.Valor <= 0) udi.Valor =   7.112667F;
                if (udi != null && udi.Valor > 0)
                    estadoCuenta.UdiPesos = Convert.ToDecimal(udi.Valor * 25000.00);
            }
        }
        /// <summary>
        /// Obtener transacciones de una cuenta
        /// </summary>
        /// <param name="encodedKey">Encoded key de la cuenta</param>
        /// <param name="fecha">Fecha para obtener las transacciones </param>
        /// <returns>Coleccion de transacciones</returns>
        private IEnumerable<DetalleTransaccion> ObtenerTransacciones(string encodedKey, DateTime fecha)
        {
            var inicioMes = new DateTime(fecha.Year, fecha.Month, 02);
            var finalMes = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)).AddDays(1);
            var searchCriteria = new SearchCriteria();
            searchCriteria.AgregarFiltro("parentAccountKey", "EQUALS", encodedKey);
            searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");
            searchCriteria.AgregarFiltroBetween("valueDate", inicioMes.ToString("yyyy-MM-dd"), finalMes.ToString("yyyy-MM-dd"));
            searchCriteria.AgregarOrdenamiento("valueDate", "ASC");
            searchCriteria.AgregarLimite(LimiteTransacciones);

            var transacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.Search(searchCriteria).Result.ToList();

            if (transacciones.Count() == LimiteTransacciones)
            {
                while (true)
                {
                    try
                    {
                        searchCriteria.AgregarOffset(transacciones.Count());
                        var nuevasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.Search(searchCriteria).Result;
                        transacciones.AddRange(nuevasTransacciones);
                        if (nuevasTransacciones.Count() < LimiteTransacciones)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            return transacciones
                .Where(t => t.Type.HasValue && !t.Type.Value.ToString().Contains("ADJUST"))
                .Select(t =>
                {
                    var descripcion = t.TransactionDetails != null ? _canlesTrasanccion
                        .FirstOrDefault(c => c.EncodedKey == t.TransactionDetails.transactionChannelKey)?.Name ?? t.TypeDescription : t.TypeDescription;
                    return new DetalleTransaccion()
                    {
                        Cantidad = t.Amount,
                        Fecha = t.ValueDate.ToString("MM/dd/yyyy"),
                        Lugar = "CDMX",
                        Transaccion = t.Amount < 0.0M ? "Retiro" : "Depósito",
                        Saldo = t.AccountBalances.TotalBalance,
                        Operacion = t.Amount < 0.0M ? "-" : "+",
                        Descripcion = $"{descripcion} {t.Notes}".Trim()
                    };
                })
            .ToList();
        }
        /// <summary>
        /// Obtener canales de transaccion
        /// </summary>
        private void ObtenerCanales()
        {
            if (_canlesTrasanccion != null && _canlesTrasanccion.Any())
                return;
            _canlesTrasanccion = _unitOfWorkApi.RepositorioCanalesTransaccion.GetAll().Result;
        }
        /// <summary>
        /// Obtener productos
        /// </summary>
        private void ObtenerProductos()
        {
            if (_productos != null && _productos.Any())
                return;

            var idProductosPermitidos = new string[] { "Patrimonial", "Ahorrale" };
            foreach (var idProducto in idProductosPermitidos)
            {
                var producto = _unitOfWorkApi.RepositorioProductosAhorro.GetById(idProducto).Result;
                if (producto != null)
                    _productos.Add(producto);
            };
        }

        public string EstadoCuentaCredito(EstadoCuentaCreditoSolDto documSolicitud)
        {
            throw new NotImplementedException();
        }

        public string ObtenerDatosEstadoCuentaDescripcion(EstadoCuentaMensualSol documSolicitud)
        {
            throw new NotImplementedException();
        }

        public string ObtenerDatosEstadoCuentaGrupal(EstadoCuentaMensualSol documSolicitud)
        {
            throw new NotImplementedException();
        }
    }
}
