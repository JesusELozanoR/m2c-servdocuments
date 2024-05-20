using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosEdoCuenta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.TCR.Repositorios.SQL
{
    public class RepositorioEstadoCuentaMensual : RepositorioBase, IRepositorioEstadoCuentaMensual
    {
        public RepositorioEstadoCuentaMensual(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int ActualizaDatosEstadoCuenta(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            int resultado = 1;

            resultado = connection.QueryFirstOrDefault<int>(
                       "des.sp_stag_tcr_EdoCtaMesActualiza"
                     , new { FechaEnvio = documSolicitud.FechaEnvio, Fecha = documSolicitud.Fecha, Estatus = documSolicitud.Estatus, NCliente = documSolicitud.NumeroCliente, NCuenta = documSolicitud.NumeroCuenta, Observaciones = documSolicitud.Observaciones }
                     , commandType: CommandType.StoredProcedure);

            gestorLog.Salir();

            return resultado;
        }

        public async Task<int> ActualizaDatosEstadoCuentaAsync(ActualizaEstadoCuentaDto documSolicitud)
        {
            gestorLog.Entrar();
            int resultado = 1;


            resultado = await connection.QueryFirstOrDefaultAsync<int>(
                "des.sp_stag_tcr_EdoCtaMesActualiza"
              , new
              {
                  FechaEnvio = documSolicitud.FechaEnvio,
                  Fecha = documSolicitud.Fecha,
                  Estatus = documSolicitud.Estatus,
                  NCliente = documSolicitud.NumeroCliente,
                  NCuenta = documSolicitud.NumeroCuenta,
                  Observaciones = documSolicitud.Observaciones
              }
              , commandType: CommandType.StoredProcedure);



            gestorLog.Salir();

            return resultado;
        }

        public int EstadoCuentaMensualProcesa(DateTime fecha)
        {
            gestorLog.Entrar();
            int resultado = 1;

            resultado = connection.QueryFirstOrDefault<int>(
                       "des.sp_stag_tcr_EdoCtaMesProcesa"
                     , new { Fecha = fecha }
                     , commandType: CommandType.StoredProcedure);

            gestorLog.Salir();

            return resultado;
        }
        public async Task<int> EstadoCuentaMensualProcesaAsync(DateTime fecha)
        {
            gestorLog.Entrar();
            int resultado = 1;

            resultado = connection.QueryFirstOrDefault<int>(
               "des.sp_stag_tcr_EdoCtaMesProcesa"
             , new { Fecha = fecha }
             , commandType: CommandType.StoredProcedure);
            gestorLog.Salir();
            return resultado;
        }

        public List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            List<EstadoCuentaMensualProcResp> resultado = null;

            resultado = connection.Query<EstadoCuentaMensualProcResp>(
                      "des.sp_stag_tcr_EdoCtaMesxProc"
                    , new { Fecha = solicitud.Fecha, Elementos = solicitud.Elementos }
                    , commandType: CommandType.StoredProcedure).AsList();
            gestorLog.Salir();
            return resultado;
        }

        public async Task<List<EstadoCuentaMensualProcResp>> EstadosCuentaMensualObtieneAsync(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            List<EstadoCuentaMensualProcResp> resultado = null;

            resultado = connection.Query<EstadoCuentaMensualProcResp>(
                      "des.sp_stag_tcr_EdoCtaMesxProc"
                    , new { Fecha = solicitud.Fecha, Elementos = solicitud.Elementos }
                    , commandType: CommandType.StoredProcedure).AsList();

            gestorLog.Salir();

            return resultado;
        }

        public EstadoCuenta ObtenerDatosEstadoCuenta(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            EstadoCuenta edocta = null;

            var result = connection.QueryMultiple(
                      "des.sp_stag_tcr_EdoCtaMesObtiene"
                    , new
                    {

                        @NumeroCliente = documSolicitud.NumeroCliente,
                        @NumeroCuenta = documSolicitud.NumeroCuenta,
                        @FechaEstado = documSolicitud.Fecha
                    }
                    , commandType: CommandType.StoredProcedure);
            edocta = result.ReadFirstOrDefault<EstadoCuenta>();
            edocta.Transacciones = (IList<DetalleTransaccion>)result.Read<DetalleTransaccion>();

            gestorLog.Salir();

            return edocta;
        }

        public async Task<EstadoCuenta> ObtenerDatosEstadoCuentaAsync(EstadoCuentaMensualSol documSolicitud)
        {
            gestorLog.Entrar();
            EstadoCuenta edocta = null;

            var result = connection.QueryMultiple(
                      "des.sp_stag_tcr_EdoCtaMesObtiene"
                    , new
                    {

                        @NumeroCliente = documSolicitud.NumeroCliente,
                        @NumeroCuenta = documSolicitud.NumeroCuenta,
                        @FechaEstado = documSolicitud.Fecha
                    }
                    , commandType: CommandType.StoredProcedure);
            edocta = result.ReadFirstOrDefault<EstadoCuenta>();
            edocta.Transacciones = (IList<DetalleTransaccion>)result.Read<DetalleTransaccion>();

            gestorLog.Salir();

            return edocta;
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
                    "des.sp_stag_tcr_ObtenerBitacorasPorMes"
                    , new { fecha }
                    , commandType: CommandType.StoredProcedure).AsList();
            }
            catch (Exception ex)
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
                    "des.sp_stag_tcr_InsEstadoCuenta"
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
