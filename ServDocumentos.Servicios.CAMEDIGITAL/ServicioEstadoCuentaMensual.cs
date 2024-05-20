using cmn.std.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosEdoCuenta;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sql = ServDocumentos.Core.Contratos.Factories.CAME.SQL;

namespace ServDocumentos.Servicios.CAMEDIGITAL
{
    public class ServicioEstadoCuentaMensual : ServicioBase, IServicioEstadoCuentaMensual, IDisposable
    {
        /// <summary>
        /// Core del servicio
        /// </summary>
        private const string CoreId = "CME";
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
            IUnitOfWork unitOfWork,
            GestorLog gestorLog,
            IConfiguration configuration,
            IMambuRepositoryFactory mambuFactory)
            : base(uowSQLMambu, unitOfWork, gestorLog, configuration)
        {
            _unitOfWorkApi = mambuFactory.Crear(Core.Enumeradores.Empresa.CAME);
        }

        public void Dispose() { }

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
                CuentaCLABE = cuenta._Datos_Adicionales_Cuenta?._Cuenta_Clabe ?? string.Empty,
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
                GATNominal = cuenta._Datos_Adicionales_Cuenta?.Gat_Nominal.GetValueOrDefault(0) ?? 0.00M,
                GATReal = cuenta._Datos_Adicionales_Cuenta?.Gat_Real.GetValueOrDefault(0) ?? 0.00M,
                Correo = cliente.emailAddress,
                SaldoMinimo = 0,
                Comisiones = 0,
                OtrosCargos = 0,
                Direccion1 = $"{direccionCliente?.Calle_Direccion_Cliente} {direccionCliente?.Numero_Ext_Direccion_Cliente}, {direccionCliente?.Numero_Int_Direccion_Cliente}".Replace("  ", " ").Trim(new char[] { ' ', ',' }),
                Direccion2 = $"{direccionCliente?.Colonia_Direccion_Cliente}, {direccionCliente?.CP_Direccion_Cliente}".Trim(new char[] { ' ', ',' }),
                Direccion3 = direccionCliente?.Municipio_Direccion_Cliente,
                Direccion4 = direccionCliente?.Estado_Direccion_Cliente
            };

            estadoCuenta.Transacciones = ObtenerTransaccionesAhorro(cuenta.EncodedKey, documSolicitud.Fecha).ToList();

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

        public async Task<int> ActualizaEstadoCuentaAsync(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            var resultado = await UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.ActualizaDatosEstadoCuentaAsync(documSolicitud);
            gestorLog.Salir();
            return resultado;
        }

        public int ActualizaEstadoCuenta(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            var resultado = UnitOfWorkSQLMambu.RepositorioEstadoCuentaMensual.ActualizaDatosEstadoCuenta(documSolicitud);
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
            try
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
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
            }
        }
        /// <summary>
        /// Obtener estado de cuenta de credito
        /// </summary>
        /// <param name="documSolicitud">Informacion de la solictud</param>
        /// <returns>Json con la informacion del estado de cuenta</returns>
        public string EstadoCuentaCredito(EstadoCuentaCreditoSolDto documSolicitud)
        {
            gestorLog.Entrar();
            string resultado;

            var cliente = _unitOfWorkApi.RepositorioClientes.GetById(documSolicitud.NumeroCliente, DetailsLevel.FULL).Result;
            if (cliente == null)
                throw new BusinessException($"El número de cliente: {documSolicitud.NumeroCliente} no existe.");

            var cuenta = _unitOfWorkApi.RepositorioCreditos.GetById(documSolicitud.NumeroCredito, DetailsLevel.FULL).Result;
            if (cuenta == null)
                throw new BusinessException($"El número de credito: {documSolicitud.NumeroCredito} no existe.");

            if (cuenta.accountHolderKey != cliente.encodedKey)
                throw new BusinessException($"El número de credito: {documSolicitud.NumeroCredito} no pertenece al cliente: {documSolicitud.NumeroCliente}");

            var producto = _unitOfWorkApi.RepositorioProductosCredito.GetById(cuenta.productTypeKey).Result;
            var pagos = _unitOfWorkApi.RepositorioCreditos.GetSchedules(cuenta.encodedKey).Result?.installments ?? new List<Installment>();
            var transacciones = ObtenerTransaccionesCredito(cuenta.encodedKey).ToList();
            var grupo = "N/A";
            if (!string.IsNullOrEmpty(cuenta._Datos_Adicionales?.Grupo))
                grupo = _unitOfWorkApi.RepositorioGrupos.GetById(cuenta._Datos_Adicionales?.Grupo).Result?.GroupName ?? string.Empty;

            var nombreSucursal = "CORPORATIVO";
            var telefonoSucursal = "8006679900";
            var direccionSucursal = "Avenida Colonia del Valle 615, Colonia Del Valle [CLI].P. 03100, Alcaldía Benito Juárez, Ciudad de México.";

            if (!string.IsNullOrEmpty(cliente.assignedBranchKey))
            {
                var sucursal = _unitOfWorkApi.RepositorioSucursales.GetById(cliente.assignedBranchKey, DetailsLevel.FULL).GetAwaiter().GetResult();
                nombreSucursal = sucursal.name;
                telefonoSucursal = sucursal.phoneNumber;
                var direccion = sucursal.addresses.FirstOrDefault();
                if (direccion != null)
                    direccionSucursal = $"{direccion.line1}, {direccion.city} C.P. {direccion.postcode}, {direccion.region} {direccion.country}";
            }

            EstadoCuentaCreditoDto estadoCuenta = new EstadoCuentaCreditoDto()
            {
                NumeroCliente = cliente.id,
                NumeroCredito = cuenta.id,
                TipoOperacion = producto.Id,
                MontoCredito = Convert.ToDecimal(cuenta.loanAmount),
                EstadoCredito = ObtenerEstatusDeCuentaAmigable(cuenta.accountState),
                CAT = string.IsNullOrEmpty(cuenta._Datos_Adicionales?.CAT_Datos_Adicionales) ? 0.00m : Convert.ToDecimal(cuenta._Datos_Adicionales?.CAT_Datos_Adicionales),
                TasaInteres = decimal.Round(Convert.ToDecimal(cuenta.interestSettings?.interestRate ?? 0.00), 2),
                InteresMoratorio = decimal.Round(Convert.ToDecimal(cuenta.penaltySettings?.penaltyRate ?? 0.00), 2),
                NombreGrupo = grupo,
                NombreCompleto = $"{cliente.firstName} {cliente.middleName} {cliente.lastName}".Trim(),
                Telefono = cliente.mobilePhone,
                Sucursal = nombreSucursal,
                TelefonoSucursal = telefonoSucursal,
                DireccionSucursal = direccionSucursal,
            };

            if (cliente._Direccion_Clientes != null)
            {
                var direccionCliente = cliente._Direccion_Clientes;
                estadoCuenta.Direccion = $"{direccionCliente.Calle_Direccion_Cliente} {direccionCliente.Numero_Ext_Direccion_Cliente}, {direccionCliente.Numero_Int_Direccion_Cliente}".Trim(new char[] { ' ', ',' });
                estadoCuenta.Colonia = $"{direccionCliente.Colonia_Direccion_Cliente}, {direccionCliente.CP_Direccion_Cliente}".Trim(new char[] { ' ', ',' });
                estadoCuenta.Ciudad = direccionCliente.Municipio_Direccion_Cliente;
                estadoCuenta.Estado = direccionCliente.Estado_Direccion_Cliente;
            }

            var now = DateTime.Now;
            estadoCuenta.Transacciones = MapearTransaccionesCredito(transacciones).ToList();
            var fechaInicio = transacciones.FirstOrDefault(t => string.IsNullOrEmpty(t.AdjustmentTransactionKey) && t.Type == TipoTransaccionCredito.DISBURSEMENT)?.ValueDate.ToString("dd/MM/yyyy");
            estadoCuenta.Periodo = $"DEL: {fechaInicio} AL: {now:dd/MM/yyyy}";
            estadoCuenta.FechaInicio = fechaInicio;
            estadoCuenta.FechaLimitePago = Convert.ToDateTime(pagos.Max(p => p.dueDate)).ToUniversalTime().ToString("dd/MM/yyyy");
            var pagoMinimo = pagos.OrderBy(p => Convert.ToDateTime(p.dueDate)).FirstOrDefault();
            var cuotaAnual = pagos.Where(p => Convert.ToDateTime(p.dueDate).ToUniversalTime() >= now.ToUniversalTime()).OrderBy(p => Convert.ToDateTime(p.dueDate).ToUniversalTime()).FirstOrDefault();

            estadoCuenta.SaldoVencido = pagos.Where(p => p.state.ToUpper() == "LATE").Sum(p =>
            {
                return Convert.ToDecimal(p.interest.amount.due + p.principal.amount.due + p.fee.amount.due + p.penalty.amount.due);
            });

            estadoCuenta.SaldoInsoluto = Convert.ToDecimal(pagos.Where(p => p.state.ToUpper() != "PAID").Sum(p => p.principal.amount.expected));
            estadoCuenta.CuotaActual = cuotaAnual == null ? 0.00m : Convert.ToDecimal(cuotaAnual.interest.amount.due + cuotaAnual.principal.amount.due + cuotaAnual.fee.amount.due
                + cuotaAnual.penalty.amount.due);
            estadoCuenta.PagoMinimo = Convert.ToDecimal(pagoMinimo.interest.amount.expected + pagoMinimo.principal.amount.expected);
            estadoCuenta.TotPagos = estadoCuenta.Transacciones.Sum(t => t.Pagos);
            estadoCuenta.TotCargos = estadoCuenta.Transacciones.Sum(t => t.Cargos);
            estadoCuenta.TotCapital = estadoCuenta.Transacciones.Sum(t => t.Capital);
            estadoCuenta.TotComisionMora = estadoCuenta.Transacciones.Sum(t => t.ComisionMora);
            estadoCuenta.TotInteresMora = estadoCuenta.Transacciones.Sum(t => t.InteresMora);
            estadoCuenta.TotInteres = estadoCuenta.Transacciones.Sum(t => t.Interes);
            estadoCuenta.TotIVA = estadoCuenta.Transacciones.Sum(t => t.IVA);
            estadoCuenta.TotSeguros = estadoCuenta.Transacciones.Sum(t => t.Seguro);
            estadoCuenta.TotGastoSupervision = "NO APLICA";

            ObtenerPagoParaLiquidar(estadoCuenta);
            resultado = JsonConvert.SerializeObject(estadoCuenta);
            gestorLog.Salir();
            return resultado;
        }
        private void ObtenerPagoParaLiquidar(EstadoCuentaCreditoDto estadoCuenta)
        {
            try
            {
                var CoreBankAPI = configuration.GetSection("CoreBank");
                var url = CoreBankAPI["Url"] + CoreBankAPI["SaldoLiquidacionProyectado"];
                var body = new Dictionary<string, object>()
                {
                    { "fecha", DateTime.Today.ToString("yyyy-MM-dd") },
                    { "creditoId", estadoCuenta.NumeroCredito },
                    { "coreId", CoreId + "MBU" }
                };

                var result = HTTPClientApi.PostRequest(url, string.Empty, string.Empty, null, null, body).Result;

                if (result.IsSuccessStatusCode)
                {
                    Task<string> content = result.Content.ReadAsStringAsync();
                    IList<SaldoLiquidacion> pagoLiquidar = JsonConvert.DeserializeObject<IList<SaldoLiquidacion>>(content.Result);
                    if (pagoLiquidar != null && pagoLiquidar.FirstOrDefault().SaldoTotal > 0)
                        estadoCuenta.PagoLiquidar = Convert.ToDecimal(pagoLiquidar.FirstOrDefault().SaldoTotal);
                }
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
            }
        }
        /// <summary>
        /// Obtener transacciones de una cuenta
        /// </summary>
        /// <param name="encodedKey">Encoded key de la cuenta</param>
        /// <param name="fecha">Fecha para obtener las transacciones </param>
        /// <returns>Coleccion de transacciones</returns>
        private IEnumerable<DetalleTransaccion> ObtenerTransaccionesAhorro(string encodedKey, DateTime fecha)
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

            var idProductosPermitidos = new string[] { "Patrimonial", "Ahorrale", "DEPOSITO_TECHREO", "DepositoTechreo", "CLUB_CAME_D", "CRECE_MAS_D", "Patrimonial_D", "AHO_PLAZO_GPAL", "EMPRESARIAL_PM", "EMPRESARIAL_PFAE"
            , "CRECE_MAS_PFAE", "CRECE_MAS_PM","KJA_MAS","AHORRO_META","TECHREO_DEPOSITO" };
            foreach (var idProducto in idProductosPermitidos)
            {
                var producto = _unitOfWorkApi.RepositorioProductosAhorro.GetById(idProducto).Result;
                if (producto != null)
                    _productos.Add(producto);
            };
        }
        /// <summary>
        /// Cambia el Id de un estatus de cuenta por un nombre amigable
        /// </summary>
        /// <param name="estatus">Id del estatus</param>
        /// <returns>Nombre del estatus amigable</returns>
        private string ObtenerEstatusDeCuentaAmigable(string estatus)
        {
            return estatus switch
            {
                "PARTIAL_APPLICATION" => "APLICACION PARCIAL",
                "PENDING_APPROVAL" => "APROBACION PENDIENTE",
                "APPROVED" => "APROBADO",
                "ACTIVE" => "ACTIVO",
                "ACTIVE_IN_ARREARS" => "ACTIVO EN ATRASOS",
                "CLOSED" => "CERRADO",
                "CLOSED_WRITTEN_OFF" => "CERRADO POR ESCRITO",
                "CLOSED_REJECTED" => "CERRADO RECHAZADO",
                _ => estatus,
            };
        }
        /// <summary>
        /// Mapear transaccion de credito
        /// </summary>
        /// <param name="transacciones">Lista de transacciones</param>
        /// <returns>Coleccion de <see cref="DetalleTransaccionCredito"/></returns>
        private IEnumerable<DetalleTransaccionCredito> MapearTransaccionesCredito(IEnumerable<TransaccionCreditoDto> transacciones)
            => transacciones
                .Where(t => string.IsNullOrEmpty(t.AdjustmentTransactionKey) && t.Type.HasValue && !t.Type.Value.ToString().Contains("ADJUST"))
                .OrderBy(t => t.ValueDate)
                .Select(t => new DetalleTransaccionCredito
                {
                    FechaMovimiento = t.ValueDate.ToString("dd/MM/yyyy"),
                    Concepto = t.TypeDescription,
                    Pagos = t.Type == TipoTransaccionCredito.REPAYMENT ? t.Amount : 0,
                    Cargos = t.Type == TipoTransaccionCredito.REPAYMENT ? 0 : t.Type == TipoTransaccionCredito.FEE ?
                        t.Amount - t.Taxes.TaxOnFeesAmount : t.Type == TipoTransaccionCredito.PENALTY_APPLIED ? t.Amount - t.Taxes.TaxOnPenaltyAmount : t.Amount,
                    Capital = t.Type == TipoTransaccionCredito.REPAYMENT ? t.AffectedAmounts.PrincipalAmount : 0,
                    ComisionMora = t.Type == TipoTransaccionCredito.REPAYMENT ? t.AffectedAmounts.FeesAmount - t.Taxes.TaxOnFeesAmount : 0,
                    InteresMora = t.Type == TipoTransaccionCredito.REPAYMENT ? t.AffectedAmounts.PenaltyAmount - t.Taxes.TaxOnPenaltyAmount : 0,
                    Interes = t.Type == TipoTransaccionCredito.REPAYMENT ? t.AffectedAmounts.InterestAmount - t.Taxes.TaxOnInterestAmount : 0,
                    IVA = new List<TipoTransaccionCredito?> { TipoTransaccionCredito.REPAYMENT, TipoTransaccionCredito.FEE, TipoTransaccionCredito.PENALTY_APPLIED }
                        .Contains(t.Type) ? t.Taxes.TaxOnFeesAmount + t.Taxes.TaxOnPenaltyAmount + t.Taxes.TaxOnInterestAmount : 0,
                    Seguro = t.Type == TipoTransaccionCredito.FEE_CHARGED ? t.Amount : 0,
                    GastoSupervision = "NO APLICA",
                    Saldo = t.AccountBalances.TotalBalance
                });
        /// <summary>
        /// Obtener las transacciones de una cuenta de credito
        /// </summary>
        /// <param name="encodedKeyCredito">encodedKey  de la cuenta de credito</param>
        /// <returns>Coleccion de transacciones</returns>
        private IEnumerable<TransaccionCreditoDto> ObtenerTransaccionesCredito(string encodedKeyCredito)
        {
            var searchCriteria = new SearchCriteria();
            searchCriteria.AgregarFiltro("parentAccountKey", "EQUALS", encodedKeyCredito);
            searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");
            searchCriteria.AgregarOrdenamiento("valueDate", "ASC");
            searchCriteria.AgregarLimite(1000);
            searchCriteria.Parameters.Add("detailsLevel", "FULL");
            return _unitOfWorkApi.RepositorioTransaccionesCredito.Search(searchCriteria).Result;
        }

        public string ObtenerDatosEstadoCuentaDescripcion(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            ObtenerCanales();
            ObtenerProductos();
            string ProductoDesc = string.Empty;
            string resultado;
            Cliente cliente = new Cliente();
            if (documSolicitud.Grupal)
            {
                var grupo = _unitOfWorkApi.RepositorioGrupos.GetById(documSolicitud.NumeroCliente, DetailsLevel.FULL).Result;
                cliente = new Cliente
                {
                    id = grupo.Id,
                    firstName = grupo.GroupName,
                    middleName = "",
                    lastName = "",
                    mobilePhone = string.IsNullOrEmpty(grupo.HomePhone) ? grupo.MobilePhone : grupo.HomePhone,
                    emailAddress = string.IsNullOrEmpty(grupo.EmailAddress) ? "" : grupo.EmailAddress
                };
                cliente._Datos_Personales_Clientes = new DatosPersonalesClientes
                {
                    RFC_Datos_Personales_Cliente = grupo.Datos_Moral.RFC
                };
                cliente._Direccion_Clientes = new DireccionClientes
                {
                    Calle_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Calle_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Calle_Direccion_Grupo,
                    Colonia_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Colonia_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Colonia_Direccion_Grupo,
                    Numero_Ext_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Numero_Ext_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Numero_Ext_Direccion_Grupo,
                    Numero_Int_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Numero_Int_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Numero_Int_Direccion_Grupo,
                    Municipio_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Municipio_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Municipio_Direccion_Grupo,
                    CP_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.CP_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.CP_Direccion_Grupo,
                    Estado_Direccion_Cliente = string.IsNullOrEmpty(grupo.Direccion_Grupo?.Estado_Direccion_Grupo) ? "" : grupo.Direccion_Grupo.Estado_Direccion_Grupo
                };

            }
            else
            {
                cliente = _unitOfWorkApi.RepositorioClientes.GetById(documSolicitud.NumeroCliente, DetailsLevel.FULL).Result;
                if (cliente == null)
                    throw new BusinessException($"El número de cliente: {documSolicitud.NumeroCliente} no existe.");
            }
            var cuenta = _unitOfWorkApi.RepositorioAhorros.GetById(documSolicitud.NumeroCuenta, DetailsLevel.FULL).Result;
            if (cuenta == null)
                throw new BusinessException($"El número de cuenta: {documSolicitud.NumeroCuenta} no existe.");

            if (!_productos.Any(p => p.EncodedKey == cuenta.ProductTypeKey))
                throw new BusinessException($"El numero de cuenta: {cuenta.Id} no corresponde a un producto valido");

            ProductoDesc = (from productos in _productos
                            where productos.EncodedKey == cuenta.ProductTypeKey.ToString()
                            orderby productos.Name ascending
                            select productos.Name).FirstOrDefault();

            var inicioMes = new DateTime(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month, 02, 00, 00, 01);
            var finalMes = new DateTime(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month, DateTime.DaysInMonth(documSolicitud.Fecha.Year, documSolicitud.Fecha.Month)).AddDays(1);

            var encodedKeyTutor = cuenta._Relaciones_Ahorro.FirstOrDefault(c => c._Tipo_Relacion.ToUpper() == "PADRE O TUTOR")?._Encoded_Key_Relacionado;
            var direccionCliente = cliente._Direccion_Clientes;
            if (!string.IsNullOrEmpty(encodedKeyTutor))
            {
                var tutor = _unitOfWorkApi.RepositorioClientes.GetById(encodedKeyTutor, DetailsLevel.FULL).Result;
                direccionCliente = tutor._Direccion_Clientes;
            }


            var estadoCuenta = new EstadoCuentaDescripcion()
            {
                NumeroCuenta = cuenta.Id,
                CuentaCLABE = cuenta._Datos_Adicionales_Cuenta?._Cuenta_Clabe ?? string.Empty,
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
                GATNominal = cuenta._Datos_Adicionales_Cuenta?.Gat_Nominal.GetValueOrDefault(0) ?? 0.00M,
                GATReal = cuenta._Datos_Adicionales_Cuenta?.Gat_Real.GetValueOrDefault(0) ?? 0.00M,
                Correo = cliente.emailAddress,
                SaldoMinimo = 0,
                Comisiones = 0,
                OtrosCargos = 0,
                Direccion1 = $"{direccionCliente?.Calle_Direccion_Cliente} {direccionCliente?.Numero_Ext_Direccion_Cliente}, {direccionCliente?.Numero_Int_Direccion_Cliente}".Replace("  ", " ").Trim(new char[] { ' ', ',' }),
                Direccion2 = $"{direccionCliente?.Colonia_Direccion_Cliente}, {direccionCliente?.CP_Direccion_Cliente}".Trim(new char[] { ' ', ',' }),
                Direccion3 = direccionCliente?.Municipio_Direccion_Cliente,
                Direccion4 = direccionCliente?.Estado_Direccion_Cliente
            };
            if (documSolicitud.SubProceso == "EstadosCuentaCAME_SPEI")
            {
                estadoCuenta.Transacciones = ObtenerTransaccionesAhorroDescripcion(cuenta.EncodedKey, documSolicitud.Fecha).ToList();
            }
            else
            {
                estadoCuenta.Transacciones = ObtenerTransaccionesAhorroDescripcionInversiones(cuenta.EncodedKey, documSolicitud.Fecha, ProductoDesc).ToList();
            }


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
                searchCriteria.AgregarFiltro("valueDate", "BEFORE", inicioMes.ToString("yyyy-MM-dd"));
                searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");

                searchCriteria.AgregarOrdenamiento("valueDate", "DESC");
                searchCriteria.AgregarLimite(LimiteTransacciones);
                var ultimasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result;
                var transaccion = ultimasTransacciones.FirstOrDefault(t => t.Type.HasValue && !t.Type.Value.ToString().Contains("ADJUST"));
                if (transaccion != null)
                    estadoCuenta.SaldoInicial = transaccion.AccountBalances?.TotalBalance ?? 0.00m;
                else
                {
                    while (true)
                    {
                        searchCriteria.AgregarOffset(ultimasTransacciones.Count());
                        ultimasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result;
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
        private IEnumerable<DetalleTransaccionDescripcion> ObtenerTransaccionesAhorroDescripcion(string encodedKey, DateTime fecha)
        {
            var inicioMes = new DateTime(fecha.Year, fecha.Month, 02);
            var finalMes = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)).AddDays(1);
            var searchCriteria = new SearchCriteria();
            searchCriteria.AgregarFiltro("parentAccountKey", "EQUALS", encodedKey);
            searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");
            searchCriteria.AgregarFiltroBetween("valueDate", inicioMes.ToString("yyyy-MM-dd"), finalMes.ToString("yyyy-MM-dd"));
            searchCriteria.AgregarOrdenamiento("valueDate", "ASC");
            //searchCriteria.AgregarLimite(LimiteTransacciones);
            searchCriteria.Parameters.Add("detailsLevel", "FULL");
            var transacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result.ToList();

            if (transacciones.Count() == LimiteTransacciones)
            {
                while (true)
                {
                    try
                    {
                        searchCriteria.AgregarOffset(transacciones.Count());
                        var nuevasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result;
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

            //Regresa el listado de los bancos            
            var banco = UnitOfWork.RepositorioPlantillas.ObtieneBancos();

            List<DetalleTransaccionDescripcion> detalleTransaccionDescripcion = new List<DetalleTransaccionDescripcion>();
            foreach (var tr in transacciones)
            {
                var canalId = tr.TransactionDetails?.transactionChannelId;
                var canalDesc = string.Empty;
                if (canalId != null)
                {
                    canalDesc = (from cust in _canlesTrasanccion
                                 where cust.Id == canalId.ToString()
                                 orderby cust.Name ascending
                                 select cust.Name).FirstOrDefault();
                }

                string descripcion = string.Empty;
                string concepto = string.Empty;
                switch (canalDesc)
                {
                    case "SPEI_DEPOSITO":
                        descripcion = $"{canalDesc} {" Hora " + tr.ValueDate.ToString("MM/dd/yyyy hh:mm:ss")} {tr._Transferencia_SPEI?.S_clabeOrigen} {" Al Cliente " + tr._Transferencia_SPEI?.S_nombreDestino} {" Clave de rastreo " + tr._Transferencia_SPEI?.S_cveRastreo}{" Referencia " + tr._Transferencia_SPEI?.S_refNum} " +
                            $"{"Concepto " + tr._Transferencia_SPEI?.S_conceptoPago} {" De " + tr._Transferencia_SPEI?.S_nombOrigen} {"https://www.banxico.org.mx/cep/"}";
                        break;
                    case "SPEI_RETIRO":
                        var Banco = (from bn in banco
                                     where bn.Clave == tr._Transferencia_SPEI.S_bancoDestino
                                     orderby bn.Banco ascending
                                     select bn.Banco).FirstOrDefault();
                        descripcion = $"{canalDesc}{" a " + Banco} {" Hora " + tr.ValueDate.ToString("MM/dd/yyyy hh:mm:ss")} {tr._Transferencia_SPEI?.S_clabeOrigen}" +
                            $"{"Al cliente " + tr._Transferencia_SPEI?.S_nombreDestino + "\" Dato no verificado por la institución\""} {"Clave de rastreo " + tr._Transferencia_SPEI?.S_cveRastreo}" +
                            $"{"Referencia " + tr._Transferencia_SPEI?.S_refNum} {"Concepto " + tr._Transferencia_SPEI?.S_conceptoPago} {"RFC " + tr._Transferencia_SPEI?.S_rfcOrigen} " +
                            $"{"https://www.banxico.org.mx/cep/"}";
                        break;
                    case "TRANSF_RET":
                    case "TRANSF_RETIRO":
                    case "TRANSF_DEP":
                    case "TRANSFERENCIA_DEPOSITO":
                        concepto = tr._Datos_Adicionales_Transaccion?.Concepto_Transaccion == "null" ? "" : tr._Datos_Adicionales_Transaccion?.Concepto_Transaccion;
                        descripcion = $"{canalDesc} {"Transferencia entre cuentas Techreo "} {" Concepto " + concepto}";
                        break;
                    case "TRANSF_RET_RV":
                        concepto = tr._Datos_Adicionales_Transaccion?.Concepto_Transaccion == "null" ? "" : tr._Datos_Adicionales_Transaccion?.Concepto_Transaccion;
                        descripcion = $"{canalDesc} {"Transferencia entre cuentas Techreo "} {" Concepto " + concepto} {" Motivo de devolución " + tr._Reversas_PTS?.P_MotivoReverso}";
                        break;
                    case "RECARGA TAE":
                    case "PAGO SERVICIOS":
                    case "COMPRA PIN":
                        descripcion = $"{"Nombre del establecimiento " + tr._Datos_Adicionales_Transaccion?.S_Producto_TCH} {tr._Datos_Adicionales_Transaccion?.Concepto_Transaccion} {tr._Datos_Adicionales_Transaccion?.S_Producto_TCH} {" Referencia " + tr._Datos_Adicionales_Transaccion?.S_Referencia_TCH}";
                        break;
                    case "COMPRA_CARD":
                        descripcion = $"{canalDesc} {"Origen " + tr._Compras_Card?._mcNombreComercio} {tr._Compras_Card?._mcIdentificador}";
                        break;
                    case "COMPRA_CARD_RV":
                        descripcion = $"{canalDesc} {"Origen " + tr._Compras_Card?._mcNombreComercio} {tr._Compras_Card?._mcIdentificador} {"Devolución"}";
                        break;
                    case "CAJA_CAME":
                    case "OXXO_C":
                    case "BBVA_6252_C":
                    case "BNMX_1515_C":
                    case "BNBJO_6456_C":
                    case "SCOTIA_3415_C":
                    case "TELECOMM_C":
                    case "OPENPAY_C":
                        descripcion = $"{canalDesc} {"Origen caja " + tr._Caja?.Caja_Origen} {"Id transacción " + tr._Caja?.Caja_Id_Transaccion} {"Sucursal " + tr._Caja?.Caja_Num_Sucursal}  ";
                        break;
                    case "ATM_RETIRO":
                        descripcion = $"{tr._Compras_Card?._mcNombreComercio}{tr._Compras_Card?._mcIdentificador} ";
                        break;
                    case "ATM_RETIRO_RV":
                        descripcion = $"{tr._Compras_Card?._mcNombreComercio}{tr._Compras_Card?._mcIdentificador} {"Devolución"} ";
                        break;
                    case "SPEI_RETIRO_RV":
                        descripcion = $"{"Depósito por Reversa "} {"ID Transacción Original " + tr._Reversas_PTS?.P_IdTranOriginal} {" Motivo de Reverso " + tr._Reversas_PTS?.P_MotivoReverso}{" Fecha Transacción Original " + tr._Reversas_PTS?.P_FechaTranOriginal}";
                        break;
                    case "RECOMP_BNVN":
                        descripcion = $"{canalDesc}";
                        break;
                    default:
                        descripcion = $"{canalDesc}";
                        break;
                }
                DetalleTransaccionDescripcion detalleTransaccionDescripcion1 = new DetalleTransaccionDescripcion()
                {
                    Fecha = tr.ValueDate.ToString("MM/dd/yyyy"),
                    Referencia = tr.Id,
                    Descripcion = descripcion,
                    Deposito = tr.Amount > 0.0M ? tr.Amount : 0.0M,
                    Retiro = tr.Amount < 0.0M ? tr.Amount : 0.0M,
                    Saldo = tr.AccountBalances.TotalBalance,
                    Cantidad = tr.Amount,
                    Transaccion = tr.Amount < 0.0M ? "Retiro" : "Depósito",
                };


                detalleTransaccionDescripcion.Add(detalleTransaccionDescripcion1);

            }


            return detalleTransaccionDescripcion;

        }

        private void ObtenerValorUdiPesos(EstadoCuentaDescripcion estadoCuenta, DateTime fecha)
        {
            try
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
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
            }
        }

        public string ObtenerDatosEstadoCuentaGrupal(EstadoCuentaMensualSol documSolicitud)
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

            var sucursal = _unitOfWorkApi.RepositorioSucursales.GetById(cliente.assignedBranchKey, DetailsLevel.FULL).GetAwaiter().GetResult();
            var direccion = sucursal.addresses.FirstOrDefault();
            var direccionSucursal = "Sin dirección";
            if (direccion != null)
                direccionSucursal = $"{direccion.line1}, {direccion.city} C.P. {direccion.postcode}, {direccion.region} {direccion.country}";
            var estadoCuenta = new EstadoCuenta()
            {
                NumeroCuenta = cuenta.Id,
                CuentaCLABE = cuenta._Datos_Adicionales_Cuenta?._Cuenta_Clabe ?? string.Empty,
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
                GATNominal = cuenta._Datos_Adicionales_Cuenta?.Gat_Nominal.GetValueOrDefault(0) ?? 0.00M,
                GATReal = cuenta._Datos_Adicionales_Cuenta?.Gat_Real.GetValueOrDefault(0) ?? 0.00M,
                Correo = cliente.emailAddress,
                SaldoMinimo = 0,
                Comisiones = 0,
                OtrosCargos = 0,
                Direccion1 = $"{direccionCliente?.Calle_Direccion_Cliente} {direccionCliente?.Numero_Ext_Direccion_Cliente}, {direccionCliente?.Numero_Int_Direccion_Cliente}".Replace("  ", " ").Trim(new char[] { ' ', ',' }),
                Direccion2 = $"{direccionCliente?.Colonia_Direccion_Cliente}, {direccionCliente?.CP_Direccion_Cliente}".Trim(new char[] { ' ', ',' }),
                Direccion3 = direccionCliente?.Municipio_Direccion_Cliente,
                Direccion4 = direccionCliente?.Estado_Direccion_Cliente,
                Sucursal = direccionSucursal,
                TelefonoSucursal = sucursal.phoneNumber ?? "Sin Teléfono"
            };


            estadoCuenta.Transacciones = ObtenerTransaccionesAhorro(cuenta.EncodedKey, documSolicitud.Fecha).ToList();

            var cuentaActivaEnPeriodo = Convert.ToDateTime(cuenta.ActivationDate) <= finalMes && Convert.ToDateTime(cuenta.ActivationDate) >= inicioMes;

            if (cuentaActivaEnPeriodo)
            {
                estadoCuenta.SaldoInicial = 0;
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
            }

            estadoCuenta.Rendimientos = estadoCuenta.Transacciones.Where(t => t.Descripcion.Contains("Interés Aplicado")).Sum(t => t.Cantidad);
            estadoCuenta.ImpuestosRetenidos = estadoCuenta.Transacciones.Where(t => t.Descripcion.Contains("ISR")).Sum(t => t.Cantidad);
            estadoCuenta.DepositosD = estadoCuenta.Transacciones.Where(t => t.Transaccion.Contains("Depósito")).Sum(t => t.Cantidad);
            estadoCuenta.RetirosD = estadoCuenta.Transacciones.Where(t => t.Transaccion.Contains("Retiro")).Sum(t => t.Cantidad) * -1;
            estadoCuenta.Depositos = estadoCuenta.Transacciones.Count(t => t.Transaccion.Contains("Depósito"));
            estadoCuenta.Retiros = estadoCuenta.Transacciones.Count(t => t.Transaccion.Contains("Retiro"));
            estadoCuenta.SaldoFinal = (estadoCuenta.SaldoInicial + estadoCuenta.DepositosD) - estadoCuenta.RetirosD;
            estadoCuenta.SaldoMinimo = estadoCuenta.Transacciones.Min(t => t.Saldo);

            estadoCuenta.SaldoDiario = estadoCuenta.Transacciones.Average(t => t.Saldo);
            ObtenerValorUdiPesos(estadoCuenta, documSolicitud.Fecha);
            resultado = JsonConvert.SerializeObject(estadoCuenta);
            gestorLog.Salir();
            return resultado;
        }
        private IEnumerable<DetalleTransaccionDescripcion> ObtenerTransaccionesAhorroDescripcionInversiones(string encodedKey, DateTime fecha, string producto)
        {
            var inicioMes = new DateTime(fecha.Year, fecha.Month, 02);
            var finalMes = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)).AddDays(1);
            var searchCriteria = new SearchCriteria();
            searchCriteria.AgregarFiltro("parentAccountKey", "EQUALS", encodedKey);
            searchCriteria.AgregarFiltro("adjustmentTransactionKey", "EMPTY", "");
            searchCriteria.AgregarFiltroBetween("valueDate", inicioMes.ToString("yyyy-MM-dd"), finalMes.ToString("yyyy-MM-dd"));
            searchCriteria.AgregarOrdenamiento("valueDate", "ASC");
            //searchCriteria.AgregarLimite(LimiteTransacciones);
            searchCriteria.Parameters.Add("detailsLevel", "FULL");
            var transacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result.ToList();




            if (transacciones.Count() == LimiteTransacciones)
            {
                while (true)
                {
                    try
                    {
                        searchCriteria.AgregarOffset(transacciones.Count());
                        var nuevasTransacciones = _unitOfWorkApi.RepositorioTransaccionesAhorro.SearchTransacctions(searchCriteria).Result;
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



            List<DetalleTransaccionDescripcion> detalleTransaccionDescripcion = new List<DetalleTransaccionDescripcion>();

            string PrimeraTransaccion = (from Transacciones in transacciones
                                         where Transacciones.TypeDescription == "Depósito"
                                         orderby Transacciones.Id ascending
                                         select Transacciones.Id).FirstOrDefault();
            foreach (var tr in transacciones)
            {
                var canalId = tr.TransactionDetails?.transactionChannelId;
                var canalDesc = string.Empty;
                if (canalId != null)
                {
                    canalDesc = (from cust in _canlesTrasanccion
                                 where cust.Id == canalId.ToString()
                                 orderby cust.Name ascending
                                 select cust.Name).FirstOrDefault();
                }

                string descripcion = string.Empty;
                string concepto = string.Empty;
                switch (canalDesc)
                {

                    case "TRANSFERENCIA_DEPOSITO":
                    case "TRANSFERENCIA_RETIRO":
                        // valida si es la primera transaccion 
                        string tipo = PrimeraTransaccion == tr.Id ? "Inversión" : canalDesc == "TRANSFERENCIA_DEPOSITO" ? tr.TypeDescription == "Interés Aplicado" ? "Interés" : "Inversión Incremento" : tr._Datos_Adicionales_Transaccion != null ? tr._Datos_Adicionales_Transaccion.Tipo_transaccion == "Retiro Interés" ? "Interés" : "Inversión" : "";

                        descripcion = $"{canalDesc} {tipo} {producto} {"|" + tr.Id} {tr.ValueDate.ToString("yyyy-MM-dd hh:mm:ss")}";
                        break;
                    case "ISR CARGO ISR":
                        descripcion = $"{canalDesc} {producto} {"|" + tr.Id} {tr.ValueDate.ToString("yyyy-MM-dd hh:mm:ss")}";
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(tr.TypeDescription))
                        {
                            descripcion = tr.TypeDescription;
                        }
                        else
                        {
                            descripcion = $"{tr.Notes}";
                        }
                        break;
                }
                DetalleTransaccionDescripcion detalleTransaccionDescripcion1 = new DetalleTransaccionDescripcion()
                {
                    Fecha = tr.ValueDate.ToString("MM/dd/yyyy"),
                    Referencia = tr.Id,
                    Descripcion = descripcion,
                    Deposito = tr.Amount > 0.0M ? tr.Amount : 0.0M,
                    Retiro = tr.Amount < 0.0M ? tr.Amount : 0.0M,
                    Saldo = tr.AccountBalances.TotalBalance,
                    Cantidad = tr.Amount,
                    Transaccion = tr.Amount < 0.0M ? "Retiro" : "Depósito",
                };


                detalleTransaccionDescripcion.Add(detalleTransaccionDescripcion1);

            }


            return detalleTransaccionDescripcion;

        }
    }
}
