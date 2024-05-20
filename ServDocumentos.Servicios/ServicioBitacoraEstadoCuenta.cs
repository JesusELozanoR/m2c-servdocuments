using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using System;
using System.Collections.Generic;

namespace ServDocumentos.Servicios.Comun
{
    /// <summary>
    /// Servicio bitacora estado cuenta
    /// </summary>
    public class ServicioBitacoraEstadoCuenta : IServicioBitacoraEstadoCuenta
    {
        /// <summary>
        /// Gestor Log
        /// </summary>
        private readonly GestorLog _gestorLog;
        /// <summary>
        /// Unidad de trabajo comun
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog"></param>
        /// <param name="unitOfWork"></param>
        public ServicioBitacoraEstadoCuenta(
            GestorLog gestorLog,
            IUnitOfWork unitOfWork)
        {
            this._gestorLog = gestorLog;
            this._unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Actualiza la informacion de un estado de cuenta
        /// </summary>
        /// <param name="estadoCuenta">Informacion del estado de cuenta</param>
        /// <returns>Cantidad de registros afectados</returns>
        public int ActualizaEstadoCuenta(ActualizaEstadoCuentaDto estadoCuenta) =>
            _unitOfWork.RepositorioBitacoraEstadoCuenta.ActualizaDatosEstadoCuenta(estadoCuenta);
        /// <summary>
        /// Inserta un estado de cuenta
        /// </summary>
        /// <param name=empresa">Empresa</param>
        /// <param name="numeroCliente">Numero de cliente</param>
        /// <param name="numeroCuenta">Numero de cuenta</param>
        /// <param name="fecha">Fecha de generacion</param>
        /// <param name="estado">Estado</param>
        /// <returns>Verdadera si la insercion fue exitosa, falso en caso constrario</returns>
        public bool InsertarEstadoDeCuenta(string empresa, string numeroCliente, string numeroCuenta, DateTime fecha, string estado) =>
            _unitOfWork.RepositorioBitacoraEstadoCuenta.InsertarEstadoDeCuenta(empresa, numeroCliente, numeroCuenta, fecha, estado);
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <param name="empresa">Empresa relacionada a los estados de cuenta</param>
        /// <param name="elementos">Numero de elementos a obtener</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha, int elementos)
            => _unitOfWork.RepositorioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuenta(empresa, fecha, elementos);
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <param name="empresa">Empresa relacionada a los estados de cuenta</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha)
            => _unitOfWork.RepositorioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuenta(empresa, fecha);
        /// <summary>
        /// Obtiene las bitacoras de los estados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        public IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuentaTodos(string empresa, DateTime fecha)
            => _unitOfWork.RepositorioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuentaTodos(empresa, fecha);
    }
}
