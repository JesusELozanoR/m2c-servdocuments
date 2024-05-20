using cmn.std.Log;
using ServDocumentos.Core.Excepciones;
using System;
using Dapper;
using System.Data;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using System.Data.SqlClient;
using System.Collections.Generic;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Documento;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioSubProcesos : RepositorioBase, IRepositorioSubProcesos
    {
        public RepositorioSubProcesos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(SubProcesoDto subproceso)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_SubProceso_ins"
                     , new
                     {
                         @i_Nombre = subproceso.SubProcesoNombre,
                         @i_UsuarioCreacion = subproceso.Usuario,
                         @i_ProcesoId = subproceso.ProcesoId,
                         @i_Descripcion = subproceso.Descripcion,
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

        public bool EliminaxNombre(SubProcesoNombreDto subproceso)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_SubProceso_del_nom"
                     , new
                     {
                         @i_Nombre = subproceso.SubProcesoNombre,
                         @i_UsuarioModificacion = subproceso.Usuario,
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

        public bool EliminaxId(SubProcesoIdDto subproceso)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_SubProceso_del"
                     , new
                     {
                         @i_SubProcesoId = subproceso.SubProcesoId,
                         @i_UsuarioModificacion = subproceso.Usuario,
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

        public IEnumerable<SubProcesoc> Obtiene(SubProcesoGetDto subproceso)
        {
            gestorLog.Entrar();
            IEnumerable<SubProcesoc> listadoSubProcesos = null;
            try
            {
                listadoSubProcesos = connection.Query<SubProcesoc>(
                       "docp_SubProceso_get"
                     , new
                     {
                         @i_SubProcesoId = subproceso.SubProcesoId,
                         @i_Nombre = subproceso.SubProcesoNombre,
                         @i_ProcesoId = subproceso.ProcesoId,
                         @i_ProcesoNombre = subproceso.ProcesoNombre,
                         @i_ProcesoDescripcion = subproceso.ProcesoDescripcion,
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
            return listadoSubProcesos;
        }

        public bool Modifica(SubProcesoUpdDto subproceso)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_SubProceso_upd"
                     , new
                     {
                         @i_SubProcesoId = subproceso.SubProcesoId,
                         @i_Nombre = subproceso.SubProcesoNombre,
                         @i_UsuarioModificacion = subproceso.Usuario,
                         @i_ProcesoId = subproceso.ProcesoId,
                         @i_Descripcion = subproceso.Descripcion,
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

        public IEnumerable<SubProcesoc> ObtienexClasificacion(SubProcesoClasDto subproceso)
        {
            gestorLog.Entrar();
            IEnumerable<SubProcesoc> listadoSubProcesos = null;
            try
            {
                listadoSubProcesos = connection.Query<SubProcesoc>(
                       "docp_SubProceso_Clas_get"
                     , new
                     {
                         @i_Empresa = (subproceso.Empresa == Core.Enumeradores.EmpresaSel.NINGUNA ? "" :  subproceso.Empresa.ToString()),
                         @i_ProcesoId = subproceso.ProcesoId,
                         @i_SubProcesoId = subproceso.SubProcesoId,
                         @i_ClasificacionId = subproceso.ClasificacionId,
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
            return listadoSubProcesos;
        }

        
    }
}
