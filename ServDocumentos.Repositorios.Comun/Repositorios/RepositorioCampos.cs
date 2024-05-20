using System;
using System.Collections.Generic;
using Dapper;
using cmn.std.Log;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using System.Data;
using System.Data.SqlClient;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Dtos.Comun.Respuestas;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioCampos : RepositorioBase, IRepositorioCampos
    {
        public RepositorioCampos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(CampoInsDto campo)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_Campo_ProSub_ins"
                     , new
                     {
                         @i_Nombre  = campo.CampoNombre,
                         @i_Descripcion  = campo.Descripcion,
                         @i_Tipo  = campo.Tipo.ToString(),
                         @i_DatoConjunto = campo.DatoConjunto,
                         @i_DatoCampo = campo.DatoCampo,
                         @i_DatoConjuntoGrupal = campo.DatoConjuntoGrupal,
                         @i_UsuarioCreacion  = campo.Usuario,
                         @i_Ejemplo = campo.Ejemplo,
                         @i_ProcesoId = campo.ProcesoId,
                         @i_SubProcesoId = campo.SubProcesoId,
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

        public bool EliminaxNombre(CampoNombreDto campo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Campo_del_nom"
                     , new
                     {
                         @i_CampoNombre = campo.CampoNombre,
                         @i_UsuarioModificacion = campo.Usuario,
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

        public bool EliminaxId(CampoIdDto campo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Campo_del"
                     , new
                     {
                         @i_CampoId = campo.CampoId,
                         @i_UsuarioModificacion = campo.Usuario,
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

        public IEnumerable<ResultadoCampo> Obtiene(CampoGetDto campo)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoCampo> listadoCampos = null;
            try
            {

                listadoCampos = connection.Query<ResultadoCampo>(
                    "docp_Campo_get"
                     , new
                     {
                         @i_CampoId = campo.CampoId,                         
                         @i_Nombre = campo.CampoNombre,
                         @i_Tipo = ( string.IsNullOrEmpty(campo.Tipo) ?  "" : campo.Tipo.ToString() ),
                         @i_DatoCampo = campo.DatoCampo,
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

        public bool Modifica(CampoUpdDto campo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_Campo_upd"
                     , new
                     {
                         @i_CampoId = campo.CampoId,
                         @i_Nombre = campo.CampoNombre,
                         @i_Descripcion  = campo.Descripcion,
                         @i_Tipo   = ( string.IsNullOrEmpty( campo.Tipo )  ? null : campo.Tipo.ToString() ),
                         @i_DatoConjunto  = campo.DatoConjunto,
                         @i_DatoCampo  = campo.DatoCampo,
                         @i_DatoConjuntoGrupal = campo.DatoConjuntoGrupal,
                         @i_UsuarioModificacion = campo.Usuario,
                         @i_Ejemplo = campo.Ejemplo,
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
