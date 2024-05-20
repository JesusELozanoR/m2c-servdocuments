using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Entidades.Comun;
using System;
using System.Collections.Generic;
using System.Data;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioDatosJson : RepositorioBase, IRepositorioDatosJson
    {
        public RepositorioDatosJson(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public void Eliminar(DatosJson datosJson)
        {
            gestorLog.Entrar();
            try
            {
                connection.Execute(
                      "docp_DatosJson_del"
                    , new
                    {
                        datosJson.Credito,
                        @Usuario = datosJson.UsuarioModificacion
                    }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public void Insertar(DatosJson datosJson)
        {
            gestorLog.Entrar();
            try
            {
                connection.Execute(
                      "docp_DatosJson_ins"
                    , new
                    {
                        datosJson.Credito,
                        datosJson.AlfrescoId,
                        datosJson.Url,
                        datosJson.Hash,
                        @Usuario = datosJson.UsuarioCreacion,
                        datosJson.Json
                    }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        public DatosJson Obtener(string credito)
        {
            gestorLog.Entrar();
            DatosJson datosJson = null;
            try
            {
                datosJson = connection.QueryFirstOrDefault<DatosJson>(
                      "docp_DatosJson_get"
                    , new { @Credito = credito }
                    , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return datosJson;
        }
        /// <summary>
        /// Obtiene el listado de los AlfrescoId donde su campo Base64 se encuentra nulo
        /// </summary>
        /// <returns>Instancia de <see cref="IEnumerable<string>"/></returns>
        public IEnumerable<string> ObtenerListadoIdSinBase64()
        {
            gestorLog.Entrar();
            try
            {
                return connection.Query<string>(
                       "docp_DatosJson_ObtenerListadoIdSinBase64_get"
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        /// <summary>
        /// Actualiza el campo Base64 basado en el campo AlfrescoId
        /// </summary>
        /// <param name="base64">Texto en base64 con el contenido de los datosjson</param>
        /// <param name="idAlfresco">Id referencia de alfresco</param>
        /// <returns>Entero que contiene la cantidad de registros modificados</returns>
        public int ActualizarMigracionBase64(string base64, string idAlfresco)
        {
            gestorLog.Entrar();
            try
            {
                return connection.Execute("docp_DatosJson_ActualizarMigracionBase64_upd", new
                {
                    @i_AlfrescoBase64 = base64,
                    @i_AlfrescoId = idAlfresco
                }, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }
    }
}
