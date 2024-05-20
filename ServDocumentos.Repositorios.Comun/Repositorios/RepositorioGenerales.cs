using cmn.std.Log;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioGenerales : RepositorioBase, IRepositorioGenerales
    {
        public RepositorioGenerales(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(GeneralesInsDto generales)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_Generales_ins"
                     , new
                     {
                         @i_ProcesoId = generales.ProcesoId,
                         @i_CampoNombre = generales.CampoNombre,
                         @i_CampoValor = generales.Valor,
                         @i_UsuarioCreacion = generales.Usuario
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

        public bool EliminaxNombre(GeneralesNombreDto generales)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Generales_del_nom"
                     , new
                     {
                         @i_ProcesoId = generales.ProcesoId,
                         @i_CampoNombre = generales.CampoNombre,
                         @i_UsuarioModificacion = generales.Usuario
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

        public bool Elimina(GeneralesIdDto generales)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Generales_del"
                     , new
                     {
                         @i_GeneralId = generales.GeneralId,
                         @i_UsuarioModificacion = generales.Usuario
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

        public IEnumerable<ResultadoGeneral> Obtiene(GeneralesGetDto generales)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoGeneral> listadoGeneraless = null;
            try
            {
                listadoGeneraless = connection.Query<ResultadoGeneral>(
                       "docp_Generales_get"
                     , new
                     {
                         @i_GeneralId = generales.GeneralId,
                         @i_ProcesoId = generales.ProcesoId,
                         @i_CampoNombre = generales.CampoNombre,
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
            return listadoGeneraless;
        }

        public bool Modifica(GeneralesUpdDto generales)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Generales_upd"
                     , new
                     {
                         @i_GeneralId = generales.GeneralId,
                         @i_ProcesoId = generales.ProcesoId,
                         @i_CampoNombre = generales.CampoNombre,
                         @i_CampoValor = generales.Valor,
                         @i_UsuarioModificacion = generales.Usuario,
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

        public string Obtiene(int procesoId) {
            gestorLog.Entrar();
            string ResultJson = "";
            try
            {
                var datos = connection.Query("docp_Generales_get", new
                {
                    @i_GeneralId = 0,
                    @i_ProcesoId = procesoId,
                    @i_CampoNombre = "",
                }, commandType: CommandType.StoredProcedure);
                ResultJson = JsonConvert.SerializeObject(datos, Formatting.Indented);
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
            return ResultJson;
        }
    }
}
