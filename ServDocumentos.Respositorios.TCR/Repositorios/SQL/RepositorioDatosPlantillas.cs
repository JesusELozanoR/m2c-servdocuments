using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Core.Dtos.DatosExpress;
using System;
using System.Data;

namespace ServDocumentos.Repositorios.TCR.Repositorios.SQL
{
    public class RepositorioDatosPlantillas : RepositorioBase, IRepositorioDatosPlantillasExpress
    {
        public RepositorioDatosPlantillas(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public DatosSolicitudCredito ObtenerDatosPlantilla(string numeroCredito)
        {
            gestorLog.Entrar();
            DatosSolicitudCredito credito = null;
            try
            {
                credito = connection.QueryFirstOrDefault<DatosSolicitudCredito>(
                          "tcrp_SolicitudesPorCredito_get"
                        , new { numeroCredito }
                        , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return credito;
        }

       
    }
}
