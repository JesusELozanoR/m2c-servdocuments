using System;
using System.Collections.Generic;
using Dapper;
using ServDocumentos.Core.Excepciones;
using System.Data.SqlClient;
using System.Data;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using cmn.std.Log;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioProcesosCampos : RepositorioBase, IRepositorioProcesosCampos
    {
        public RepositorioProcesosCampos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(ProcesoCampoInsDto procesoscampos)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_ProcesosCampos_ins"
                     , new
                     {
                         @i_ProcesoId = procesoscampos.ProcesoId,
                         @i_CampoId = procesoscampos.CampoId,
                         @i_UsuarioCreacion = procesoscampos.Usuario,
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

        public bool EliminaxIds(ProcesoCampoDelDto procesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_ProcesosCampos_del_ids"
                     , new
                     {
                         @i_ProcesoId = procesoscampos.ProcesoId,
                         @i_CampoId = procesoscampos.CampoId,
                         @i_UsuarioModificacion = procesoscampos.Usuario
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

        public bool Elimina(ProcesoCampoIdDto procesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_ProcesosCampos_del"
                     , new
                     {
                         @i_ProcesoCampoId = procesoscampos.ProcesoCampoId,
                         @i_UsuarioModificacion = procesoscampos.Usuario
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

        public IEnumerable<ResultadoProcesoCampo> Obtiene(ProcesoCampoGetDto procesoscampos)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoProcesoCampo> listadoProcesosCamposs = null;
            try
            {
                listadoProcesosCamposs = connection.Query<ResultadoProcesoCampo>(
                       "docp_ProcesosCampos_get"
                     , new
                     {
                         @i_ProcesoCampoId = procesoscampos.ProcesoCampoId,
                         @i_ProcesoId = procesoscampos.ProcesoId,
                         @i_CampoId = procesoscampos.CampoId,
                         @i_CampoNombre = procesoscampos.CampoNombre,
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
            return listadoProcesosCamposs;
        }

        public bool Modifica(ProcesoCampoUpdDto procesoscampos)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_ProcesosCampos_upd"
                     , new
                     {
                         @i_ProcesoCampoId = procesoscampos.ProcesoCampoId,
                         @i_ProcesoId = procesoscampos.ProcesoId,
                         @i_CampoId = procesoscampos.CampoId,
                         @i_UsuarioModificacion = procesoscampos.Usuario,
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
