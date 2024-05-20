using cmn.std.Log;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using Dapper;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioCamposTipos : RepositorioBase, IRepositorioCamposTipos
    {
        public RepositorioCamposTipos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog) {}

        public ResultadoCampoTipo Agrega(CampoTipoInsDto campotipo)
        {
            gestorLog.Entrar();
            ResultadoCampoTipo resultado = null;
            try
            {
                resultado = connection.QuerySingleOrDefault<ResultadoCampoTipo>(
                       "docp_CampoTipo_ins"
                     , new
                     {
                         @i_CampoTipo = campotipo.CampoTipo,
                         @i_UsuarioCreacion = campotipo.Usuario,
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 && ex.Class == 16)
                    throw new BusinessException("No se pudo agregar . " + ex.Message);
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return resultado;
        }

        public IEnumerable<ResultadoCampoTipo> Obtiene(CampoTipoGetDto campotipo)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoCampoTipo> listadoCampos = null;
            try
            {

                listadoCampos = connection.Query<ResultadoCampoTipo>(
                    "docp_CampoTipo_get"
                     , new
                     {
                         @i_CampoTipoId = campotipo.CampoTipoId,
                         @i_CampoTipo = campotipo.CampoTipo,
                         @i_Estado = (Byte)campotipo.Estado,
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 && ex.Class == 16)
                    throw new BusinessException("No se pudo obtener la información . " + ex.Message);
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return listadoCampos;
        }

        public ResultadoCampoTipo Modifica(CampoTipoUpdDto campotipo)
        {
            gestorLog.Entrar();
            ResultadoCampoTipo respuesta = null;
            try
            {
                respuesta = connection.QuerySingleOrDefault<ResultadoCampoTipo>(
                       "docp_CampoTipo_upd"
                     , new
                     {
                         @i_CampoTipoId = campotipo.CampoTipoId,
                         @i_CampoTipo = campotipo.CampoTipo,
                         @i_UsuarioModificacion = campotipo.Usuario,
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 && ex.Class == 16)
                    throw new BusinessException("No se pudo modificar . " + ex.Message);
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }
    }
}
