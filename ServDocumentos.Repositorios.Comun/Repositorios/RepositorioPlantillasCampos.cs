using System;
using System.Collections.Generic;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using System.Data;
using System.Data.SqlClient;
using ServDocumentos.Core.Excepciones;
using cmn.std.Log;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioPlantillasCampos : RepositorioBase, IRepositorioPlantillasCampos
    {
        public RepositorioPlantillasCampos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public List<int> Agrega(PlantillaCampoInsDto plantillacampo)
        {
            gestorLog.Entrar();
            List<int> listaErrId = new List<int>()  ;
            try
            {
                foreach (int campoid in plantillacampo.ListaCampoIds) {
                    int Id = 0;
                    try
                    {
                        Id = connection.QuerySingle<int>(
                        "docp_PlantillaCampo_ins"
                        , new
                        {
                            @i_PlantillaId = plantillacampo.PlantillaId,
                            @i_CampoId = campoid,
                            @i_UsuarioCreacion = plantillacampo.Usuario,
                        }
                        , commandType: CommandType.StoredProcedure);
                    }
                    catch (SqlException ex)
                    {
                        listaErrId.Add(campoid);
                        //if (ex.Number == 50000 && ex.Class == 16)
                        //    throw new BusinessException("No se pudo agregar . " + ex.Message);
                        //else
                        //    throw new Exception(ex.Message);
                    }
                    catch (Exception)
                    {
                        listaErrId.Add(campoid);
                    }
                }
            }
            //catch (SqlException ex)
            //{
            //    if (ex.Number == 50000 && ex.Class == 16)
            //        throw new BusinessException("No se pudo agregar . " + ex.Message);
            //    else
            //        throw new Exception(ex.Message);
            //}
            finally
            {
                gestorLog.Salir();
            }
            return listaErrId;
        }
        public bool Elimina(PlantillaCampoIdDto plantillacampo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_PlantillaCampo_del"
                    , new
                    {
                        @i_PlantillaCamposId = plantillacampo.PlantillaCampoId,
                        @i_UsuarioModificacion = plantillacampo.Usuario
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
        public bool EliminaxPlantilla(PlantillaIdDto plantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_PlantillaCampo_del_pla"
                     , new
                     {
                         @i_PlantillaId = plantilla.PlantillaId,
                         @i_UsuarioModificacion = plantilla.Usuario
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
        public bool EliminaxCampo(PlantillaCampoIdcDto plantillacampo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_PlantillaCampo_del_campoid"
                     , new
                     {
                         @i_PlantillaId = plantillacampo.PlantillaId,
                         @i_CampoId = plantillacampo.CampoId,
                         @i_UsuarioModificacion = plantillacampo.Usuario
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

        public IEnumerable<ResultadoPlantillaCampo> Obtiene(PlantillaCampoGetDto plantillacampo)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoPlantillaCampo> listadoPlantillas = null;
            try
            {
                listadoPlantillas = connection.Query<ResultadoPlantillaCampo>(
                       "docp_PlantillaCampo_get"
                     , new
                     {
                         @i_PlantillaId = plantillacampo.PlantillaId,
                         @i_CampoId = plantillacampo.CampoId
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
            return listadoPlantillas;
        }

        public bool Modifica(PlantillaCampoUpdDto Plantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                //int Id = connection.Execute(
                //       "docp_PlantillaCampo_upd"
                //     //, new
                //     //{
                //     //    @i_PlantillaId = Plantilla.PlantillaId,
                //     //    @i_Nombre = Plantilla.PlantillaNombre,
                //     //    @i_UsuarioModificacion = Plantilla.Usuario,
                //     //}
                //     , commandType: CommandType.StoredProcedure);
                //if (Id > 0) { respuesta = true; }
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
        public IEnumerable<ResultadoPlantillaCampo> ObtienexProceso(PlantillaCampoPSGet plantillacampo)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoPlantillaCampo> listadoPlantillas = null;
            try
            {
                listadoPlantillas = connection.Query<ResultadoPlantillaCampo>(
                       "docp_PlantillaCampo_get_proc"
                     , new
                     {
                         @i_ProcesoId = plantillacampo.ProcesoId,
                         @i_SubprocesoId = plantillacampo.SubprocesoId,
                         @i_CampoNombre = plantillacampo.CampoNombre,
                         @i_Descripcion = plantillacampo.CampoDescripcion
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
            return listadoPlantillas;
        }

    }
}
