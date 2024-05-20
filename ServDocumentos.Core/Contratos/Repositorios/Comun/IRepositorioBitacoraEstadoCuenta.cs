using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
    /// <summary>
    /// Define los metodos para el repositorio de bitacora estado cuenta
    /// </summary>
    public interface IRepositorioBitacoraEstadoCuenta
    {
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <param name="elementos">Numero de elementos a obtener</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha, int elementos);
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(string empresa, DateTime fecha);
        /// <summary>
        /// Obtiene las bitacoras de los estados en base a una fecha especifica
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuentaTodos(string empresa, DateTime fecha);
        /// <summary>
        /// Inserta un estado de cuenta
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="numeroCliente">Numero de cliente</param>
        /// <param name="numeroCuenta">Numero de cuenta</param>
        /// <param name="fecha">Fecha de generacion</param>
        /// <param name="estado">Estado</param>
        /// <returns>Verdadera si la insercion fue exitosa, falso en caso constrario</returns>
        bool InsertarEstadoDeCuenta(string empresa, string numeroCliente, string numeroCuenta, DateTime fecha, string estado);
        /// <summary>
        /// Actualiza la informacion de un estado de cuenta
        /// </summary>
        /// <param name="estadoCuenta">Informacion del estado de cuenta</param>
        /// <returns>Cantidad de registros afectados</returns>
        int ActualizaDatosEstadoCuenta(ActualizaEstadoCuentaDto estadoCuenta);
    }
}
