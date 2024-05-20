using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.DatosEdoCuenta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.TCR
{
    public interface IRepositorioEstadoCuentaMensual
    {
        int EstadoCuentaMensualProcesa(DateTime fecha);
        Task<int> EstadoCuentaMensualProcesaAsync(DateTime fecha);
        List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud);
        Task<List<EstadoCuentaMensualProcResp>> EstadosCuentaMensualObtieneAsync(EstadosCuentaMensualProcSol solicitud);
        EstadoCuenta ObtenerDatosEstadoCuenta(EstadoCuentaMensualSol documSolicitud);
        Task<EstadoCuenta> ObtenerDatosEstadoCuentaAsync(EstadoCuentaMensualSol documSolicitud);

        int ActualizaDatosEstadoCuenta(ActualizaEstadoCuentaDto documSolicitud);
        Task<int> ActualizaDatosEstadoCuentaAsync(ActualizaEstadoCuentaDto documSolicitud);
        /// <summary>
        /// Obtiene las bitacoras de los estados de cuenta generados en base a una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha para obtener las bitacoras</param>
        /// <returns>Coleccion de bitacoras obtenidas</returns>
        IEnumerable<BitacoraEstadoCuentaRespuesta> ObtenerBitacorasEstadoCuenta(DateTime fecha);
        /// <summary>
        /// Inserta un estado de cuenta
        /// </summary>
        /// <param name="numeroCliente">Numero de cliente</param>
        /// <param name="numeroCuenta">Numero de cuenta</param>
        /// <param name="fecha">Fecha de generacion</param>
        /// <param name="estado">Estado</param>
        /// <param name="correo">Correo al que se enviara el email</param>
        bool InsertarEstadoDeCuenta(string numeroCliente, string numeroCuenta, DateTime fecha, string estado, string correo);
    }
}
