using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using cmn.std.Log;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using System.Data.SqlClient;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioSubProcesosPlantillas : RepositorioBase, IRepositorioSubProcesosPlantillas
    {
        public RepositorioSubProcesosPlantillas(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public Dictionary<int, string> Agrega(SubProcesoPlantillaInsDto SubProcesoPlantillas)
        {
            gestorLog.Entrar();
            Dictionary<int, string> listaErrId = new Dictionary<int, string>();
            try
            {
                foreach (SubProcesoPlantilla plantilla in SubProcesoPlantillas.listadoPlantillas)
                {
                    int Id = 0;
                    try
                    {
                        Id = connection.QuerySingle<int>(
                        "docp_Sub_Plan_ins"
                        , new
                        {
                            @i_SubProcesoId = SubProcesoPlantillas.SubProcesoId,
                            @i_PlantillaId = plantilla.PlantillaId,
                            @i_UsuarioCreacion = SubProcesoPlantillas.Usuario,
                            @i_RECA = plantilla.RECA,
                            @i_Tipo = plantilla.Tipo
                        }
                        , commandType: CommandType.StoredProcedure);
                    }
                    catch (SqlException ex)
                    {
                        listaErrId.Add(plantilla.PlantillaId, ex.Message);                     
                    }
                    catch (Exception)
                    {
                        listaErrId.Add(plantilla.PlantillaId, "No se pudo guardar el dato.");
                    }
                }
            }           
            finally
            {
                gestorLog.Salir();
            }
            return listaErrId;
        }

        public bool EliminaxSubprocesoId(SubProcesoIdDto subproceso)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Sub_Plan_del_subpid"
                     , new
                     {
                         @i_SubProcesoId = subproceso.SubProcesoId,
                         @i_UsuarioModificacion = subproceso.Usuario
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

        public bool Elimina(SubprocesoPlantillaDelDto subprocesoplantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Sub_Plan_del"
                     , new
                     {
                         @i_SubProcesoId = subprocesoplantilla.SubProcesoId,
                         @i_PlantillaId = subprocesoplantilla.PlantillaId,
                         @i_UsuarioModificacion = subprocesoplantilla.Usuario
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

        public IEnumerable<ResultadoSubProcesoPlantilla> Obtiene(SubprocesoPlantillaGetDto subprocesoplantilla)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoSubProcesoPlantilla> listadoSubProcesos = null;
            try
            {
                listadoSubProcesos = connection.Query<ResultadoSubProcesoPlantilla>(
                       "docp_Sub_Plan_get"
                     , new
                     {
                         @i_Sub_PlanId = subprocesoplantilla.SubpPantId
                         ,@i_SubProcesoId = subprocesoplantilla.SubProcesoId
                         ,@i_PlantillaId = subprocesoplantilla.PlantillaId
                         ,@i_Reca = subprocesoplantilla.Reca
                         ,@i_Tipo = subprocesoplantilla.Tipo
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

        public bool Modifica(SubProcesoPlantillaUpdDto subprocesoplantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Sub_Plan_upd"
                     , new
                     {
                         @i_SubpPlantId = subprocesoplantilla.SubProcesoPlantillaId,
                         @i_SubProcesoId = subprocesoplantilla.SubProcesoId,
                         @i_PlantillaId = subprocesoplantilla.PlantillaId,
                         @i_UsuarioModificacion = subprocesoplantilla.Usuario,
                         @i_Reca = subprocesoplantilla.RECA,
                         @i_Tipo = subprocesoplantilla.Tipo
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
