using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using ServDocumentos.Core.Excepciones;
using System.Data;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using cmn.std.Log;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioClasificaciones : RepositorioBase, IRepositorioClasificaciones
    {
        public RepositorioClasificaciones(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(ClasificacionInsDto clasificaciones)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_Clasificaciones_ins"
                     , new
                     {
                         @i_Clasificacion = clasificaciones.Clasificacion,
                         @i_UsuarioCreacion = clasificaciones.Usuario,
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

        public bool EliminaxNombre(ClasificacionNombreDto clasificaciones)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Clasificaciones_del_nom"
                     , new
                     {
                         @i_Clasificacion = clasificaciones.Clasificacion,
                         @i_UsuarioCreacion = clasificaciones.Usuario,
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

        public bool Elimina(ClasificacionIdDto clasificaciones)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Clasificaciones_del"
                     , new
                     {
                         @i_ClasificacionId = clasificaciones.ClasificacionId,
                         @i_UsuarioCreacion = clasificaciones.Usuario,
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

        public IEnumerable<ResultadoClasificacion> Obtiene(ClasificacionGetDto clasificaciones)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoClasificacion> listadoClasificacioness = null;
            try
            {
                listadoClasificacioness = connection.Query<ResultadoClasificacion>(
                       "docp_Clasificaciones_get"
                     , new
                     {
                         @i_ClasificacionId = clasificaciones.ClasificacionId,
                         @i_Clasificacion = clasificaciones.Clasificacion,
                         @i_Empresa = (clasificaciones.Empresa == Core.Enumeradores.EmpresaSel.NINGUNA ? "" : clasificaciones.Empresa.ToString() )
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
            return listadoClasificacioness;
        }

        public bool Modifica(ClasificacionUpdDto clasificaciones)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Clasificaciones_upd"
                     , new
                     {
                         @i_ClasificacionId = clasificaciones.ClasificacionId ,
                         @i_Clasificacion  = clasificaciones.Clasificacion ,
                         @i_Usuario = clasificaciones.Usuario ,
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

        //public string Obtiene(int procesoId)
        //{
        //    gestorLog.Entrar();
        //    string ResultJson = "";
        //    try
        //    {
        //        var datos = connection.Query("docp_Clasificaciones_get", new
        //        {
        //            @i_GeneralId = procesoId,
        //            @i_ProcesoId = 0,
        //            @i_CampoNombre = "",
        //        }, commandType: CommandType.StoredProcedure);
        //        ResultJson = JsonConvert.SerializeObject(datos, Formatting.Indented);
        //    }
        //    catch (SqlException ex)
        //    {
        //        if (ex.Number == 50000 && ex.Class == 16)
        //            throw new BusinessException("No se pudo obtener la información . " + ex.Message);
        //        else
        //            throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        gestorLog.Salir();
        //    }
        //    return ResultJson;
        //}
    }
}
