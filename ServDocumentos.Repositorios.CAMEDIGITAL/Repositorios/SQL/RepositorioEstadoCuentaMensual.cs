using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.CAME;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosEdoCuenta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.SQL
{
    public class RepositorioEstadoCuentaMensual : RepositorioBase, IRepositorioEstadoCuentaMensual
    {
        public RepositorioEstadoCuentaMensual(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog) { }

        public int ActualizaDatosEstadoCuenta(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            try
            {
                return connection.QueryFirstOrDefault<int>(
                    "des.sp_stag_came_EdoCtaMesActualiza"
                    , new { documSolicitud.FechaEnvio, documSolicitud.Fecha, documSolicitud.Estatus, NCliente = documSolicitud.NumeroCliente, NCuenta = documSolicitud.NumeroCuenta, documSolicitud.Observaciones }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public async Task<int> ActualizaDatosEstadoCuentaAsync(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            try
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    "des.sp_stag_came_EdoCtaMesActualiza"
                    , new { documSolicitud.FechaEnvio, documSolicitud.Fecha, documSolicitud.Estatus, NCliente = documSolicitud.NumeroCliente, NCuenta = documSolicitud.NumeroCuenta, documSolicitud.Observaciones }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public int EstadoCuentaMensualProcesa(DateTime fecha)
        {
            gestorLog.Entrar();
            try
            {
                return connection.QueryFirstOrDefault<int>(
                    "des.sp_stag_came_EdoCtaMesProcesa"
                    , new { fecha }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public async Task<int> EstadoCuentaMensualProcesaAsync(DateTime fecha)
        {
            gestorLog.Entrar();
            try
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    "des.sp_stag_came_EdoCtaMesProcesa"
                    , new { fecha }
                    , commandType: CommandType.StoredProcedure, commandTimeout: 120);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            try
            {
                return connection.Query<EstadoCuentaMensualProcResp>(
                    "des.sp_stag_came_EdoCtaMesxProc"
                    , new { solicitud.Fecha, solicitud.Elementos }
                    , commandType: CommandType.StoredProcedure).AsList();
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public async Task<List<EstadoCuentaMensualProcResp>> EstadosCuentaMensualObtieneAsync(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            try
            {
                return await connection.QueryAsync<EstadoCuentaMensualProcResp>(
                    "des.sp_stag_came_EdoCtaMesxProc"
                    , new { solicitud.Fecha, solicitud.Elementos }
                    , commandType: CommandType.StoredProcedure) as List<EstadoCuentaMensualProcResp>;
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public EstadoCuenta ObtenerDatosEstadoCuenta(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            try
            {
                GridReader result = connection.QueryMultiple(
                    "des.sp_stag_came_EdoCtaMesObtiene"
                    , new { documSolicitud.NumeroCuenta, documSolicitud.NumeroCliente, FechaEstado = documSolicitud.Fecha }
                    , commandType: CommandType.StoredProcedure);
                EstadoCuenta edocta = result.ReadFirstOrDefault<EstadoCuenta>();
                edocta.Transacciones = result.Read<DetalleTransaccion>() as IList<DetalleTransaccion>;
                return edocta;
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public async Task<EstadoCuenta> ObtenerDatosEstadoCuentaAsync(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            try
            {
                GridReader result = await connection.QueryMultipleAsync(
                    "des.sp_stag_came_EdoCtaMesObtiene"
                    , new { documSolicitud.NumeroCuenta, documSolicitud.NumeroCliente, FechaEstado = documSolicitud.Fecha }
                    , commandType: CommandType.StoredProcedure);
                EstadoCuenta edocta = await result.ReadFirstOrDefaultAsync<EstadoCuenta>();
                edocta.Transacciones = await result.ReadAsync<DetalleTransaccion>() as IList<DetalleTransaccion>;
                return edocta;
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public EstadoCuentaCreditoDto EstadoCuentaCredito(EstadoCuentaCreditoSolDto documSolicitud)
        {
            gestorLog.Entrar();
            try
            {
                GridReader result = connection.QueryMultiple(
                   "des.sp_stag_came_EdoCtaMesProDigCol"
                   , new { documSolicitud.NumeroCredito, documSolicitud.NumeroCliente }
                   , commandType: CommandType.StoredProcedure);
                EstadoCuentaCreditoDto edocta = result.ReadFirstOrDefault<EstadoCuentaCreditoDto>();
                edocta.Transacciones = result.Read<DetalleTransaccionCredito>() as IList<DetalleTransaccionCredito>;
                TotalDetalleTransaccionCredito Totales = result.ReadFirstOrDefault<TotalDetalleTransaccionCredito>();
                edocta.TotPagos = Totales.TotPagos;
                edocta.TotCargos = Totales.TotCargos;
                edocta.TotCapital = Totales.TotCapital;
                edocta.TotComisionMora = Totales.TotComisionMora;
                edocta.TotInteresMora = Totales.TotInteresMora;
                edocta.TotInteres = Totales.TotInteres;
                edocta.TotIVA = Totales.TotIVA;
                edocta.TotSeguros = Totales.TotSeguros;
                edocta.TotGastoSupervision = Totales.TotGastoSupervision;
                return edocta;
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(DateTime fecha)
        {
            gestorLog.Entrar();
            try
            {
                return connection.Query<BitacoraEstadoCuentaRespuesta>(
                    "des.sp_stag_came_ObtenerBitacorasPorMes"
                    , new { fecha }
                    , commandType: CommandType.StoredProcedure).AsList();
            }
            catch(Exception ex)
            {
                gestorLog.RegistrarError(ex);
                return new List<BitacoraEstadoCuentaRespuesta>();
            }
            finally
            {
                gestorLog.Salir();
            }
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
            try
            {
                connection.Execute(
                    "des.sp_stag_came_InsEstadoCuenta"
                    , new { numeroCliente, numeroCuenta, fecha, estado, correo }
                    , commandType: CommandType.StoredProcedure);
                return true;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                return false;
            }
            finally
            {
                gestorLog.Salir();
            }
        }
    }
}
