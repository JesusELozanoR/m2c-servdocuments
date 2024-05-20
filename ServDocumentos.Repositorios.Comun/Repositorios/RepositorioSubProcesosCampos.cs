using System;
using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using ServDocumentos.Core.Excepciones;
using System.Data;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using cmn.std.Log;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioSubProcesosCampos : RepositorioBase, IRepositorioSubProcesosCampos
    {
        public RepositorioSubProcesosCampos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }
        public int Agrega(SubProcesoCampoInsDto subprocesoscampos)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_SubProcesosCampos_ins"
                     , new
                     {
                         @i_SubProcesoId = subprocesoscampos.SubProcesoId,
                         @i_CampoId = subprocesoscampos.CampoId,
                         @i_UsuarioCreacion = subprocesoscampos.Usuario,
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
            return Id;
        }

        public bool EliminaxIds(SubProcesoCampoDelDto subprocesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_SubProcesosCampos_del_ids"
                     , new
                     {
                         @i_SubProcesoId = subprocesoscampos.SubProcesoId,
                         @i_CampoId = subprocesoscampos.CampoId,
                         @i_UsuarioModificacion = subprocesoscampos.Usuario
                     }
                     , commandType: CommandType.StoredProcedure);
                if (Id > 0) { respuesta = true; }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 && ex.Class == 16)
                    throw new BusinessException("No se pudo eliminar . " + ex.Message);
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public bool Elimina(SubProcesoCampoIdDto subprocesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_SubProcesosCampos_del"
                     , new
                     {
                         @i_SubProcesoCampoId = subprocesoscampos.SubProcesoCampoId,
                         @i_UsuarioModificacion = subprocesoscampos.Usuario
                     }
                     , commandType: CommandType.StoredProcedure);
                if (Id > 0) { respuesta = true; }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 && ex.Class == 16)
                    throw new BusinessException("No se pudo eliminar . " + ex.Message);
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public IEnumerable<ResultadoSubProcesoCampo> Obtiene(SubProcesoCampoGetDto subprocesoscampos)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoSubProcesoCampo> listadoSubProcesosCamposs = null;
            try
            {
                listadoSubProcesosCamposs = connection.Query<ResultadoSubProcesoCampo>(
                       "docp_SubProcesosCampos_get"
                     , new
                     {
                         @i_SubProcesoCampoId = subprocesoscampos.SubProcesoCampoId,
                         @i_SubProcesoId = subprocesoscampos.SubProcesoId,
                         @i_CampoId = subprocesoscampos.CampoId,
                         @i_CampoNombre = subprocesoscampos.CampoNombre,
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
            return listadoSubProcesosCamposs;
        }

        public bool Modifica(SubProcesoCampoUpdDto subprocesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_SubProcesosCampos_upd"
                     , new
                     {
                         @i_SubProcesoCampoId = subprocesoscampos.SubProcesoCampoId,
                         @i_SubProcesoId = subprocesoscampos.SubProcesoId,
                         @i_CampoId = subprocesoscampos.CampoId,
                         @i_UsuarioModificacion = subprocesoscampos.Usuario,
                     }
                     , commandType: CommandType.StoredProcedure);
                if (Id > 0) { respuesta = true; }
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
