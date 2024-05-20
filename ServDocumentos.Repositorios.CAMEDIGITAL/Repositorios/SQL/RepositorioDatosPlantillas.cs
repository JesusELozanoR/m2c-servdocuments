using cmn.std.Log;
using ServDocumentos.Core.Contratos.Repositorios.CAME;
using ServDocumentos.Core.Dtos.DatosCore;
using System;
using System.Data;
using Dapper;
using System.Collections.Generic;
using sybase = ServDocumentos.Core.Dtos.DatosSybase;

namespace ServDocumentos.Repositorios.CAMEDIGITAL.Repositorios.SQL
{
    public class RepositorioDatosPlantillas : RepositorioBase, IRepositorioDatosPlantillasCore
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
                          "cbkp_ObtenerDatosCredito"
                        , new { numeroCredito }
                        , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return credito;
        }

        public List<sybase.Dividendo> ObtenerTablaAmortizacion (string numeroCredito)
        {
            gestorLog.Entrar();

            List<sybase.Dividendo> dividendos = null;

            try
            {
                var result = connection.Query<sybase.Dividendo>(
                          "cbkp_ObtenerTablaAmortizacionGrupal"
                        , new { numeroCredito }
                        , commandType: CommandType.StoredProcedure);



                dividendos = result.AsList();
           

            }
            finally
            {
                gestorLog.Salir();
            }
            return dividendos;

        }

        public string ObtenerGrupoId (string numeroCredito)
        {
            gestorLog.Entrar();

            string grupoId = "";

            try
            {
                grupoId = connection.QueryFirstOrDefault<string>(
                          "cbkp_ObtenerGrupoId"
                        , new { numeroCredito }
                        , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return grupoId;

        }
    }
}
