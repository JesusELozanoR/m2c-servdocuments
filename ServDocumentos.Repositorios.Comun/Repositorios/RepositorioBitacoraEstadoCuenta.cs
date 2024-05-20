using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using System;
using System.Collections.Generic;
using System.Data;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    /// <summary>
    /// Repositorio bitacora estado cuenta
    /// </summary>
    public class RepositorioBitacoraEstadoCuenta : RepositorioBase, IRepositorioBitacoraEstadoCuenta
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="connection">Conexion a la bd</param>
        /// <param name="transaction">Transaccion</param>
        /// <param name="gestorLog">Gestor log</param>
        public RepositorioBitacoraEstadoCuenta(
            IDbConnection connection,
            Func<IDbTransaction> transaction,
            GestorLog gestorLog)
            : base(connection, transaction, gestorLog)
        {
        }
        /// <summary>
        /// Actualiza la informacion de un estado de cuenta
        /// </summary>
        /// <param name="estadoCuenta">Informacion del estado de cuenta</param>
        /// <returns>Cantidad de registros afectados</returns>
        public int ActualizaDatosEstadoCuenta(ActualizaEstadoCuentaDto estadoCuenta)
        {
            try
            {
                gestorLog.Entrar();
                return connection.QueryFirstOrDefault<int>(
                    "dbo.cbkp_UpdBitacoraEstadoCuenta",
                    new
                    {
                        estadoCuenta.Empresa,
                        estadoCuenta.Fecha,
                        estadoCuenta.FechaEnvio,
                        NCliente = estadoCuenta.NumeroCliente,
                        NCuenta = estadoCuenta.NumeroCuenta,
                        estadoCuenta.Estatus,
                        estadoCuenta.Observaciones
                    },
                    commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        /// <summary>
        /// Inserta un estado de cuenta
        /// </summary>
        /// <param name=empresa">Empresa</param>
        /// <param name="numeroCliente">Numero de cliente</param>
        /// <param name="numeroCuenta">Numero de cuenta</param>
        /// <param name="fecha">Fecha de generacion</param>
        /// <param name="estado">Estado</param>
        /// <returns>Verdadera si la insercion fue exitosa, falso en caso constrario</returns>
        public bool InsertarEstadoDeCuenta(string empresa, string numeroCliente, string numeroCuenta, DateTime fecha, string estado)
        {
            try
            {
                gestorLog.Entrar();
                connection.Execute(
                    "dbo.cbkp_InsBitacoraEstadoCuenta",
                    new { numeroCliente, numeroCuenta, empresa, fecha, estado },
                    commandType: CommandType.StoredProcedure);
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
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <param name="elementos">Numero de elementos a obtener</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha, int elementos)
        {
            try
            {
                gestorLog.Entrar();
                return connection.Query<BitacoraEstadoCuentaRespuesta>(
                    "dbo.cbkp_ObtenerBitacorasPorProcesar",
                    new { empresa, fecha, elementos },
                    commandType: CommandType.StoredProcedure).AsList();
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
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha)
        {
            try
            {
                gestorLog.Entrar();
                return connection.Query<BitacoraEstadoCuentaRespuesta>(
                    "dbo.cbkp_ObtenerBitacorasPorProcesar",
                    new { empresa, fecha },
                    commandType: CommandType.StoredProcedure).AsList();
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
        /// Obtiene las bitacoras de los estados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuentaTodos(string empresa, DateTime fecha)
        {
            try
            {
                gestorLog.Entrar();
                return connection.Query<BitacoraEstadoCuentaRespuesta>(
                    "dbo.cbkp_ObtenerBitacorasEstadoCuenta",
                    new { empresa, fecha },
                    commandType: CommandType.StoredProcedure).AsList();
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
        }
    }
}
