using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
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
    public class RepositorioProcesos : RepositorioBase, IRepositorioProcesos
    {
        public RepositorioProcesos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public int Agrega(ProcesoDto proceso)
        {
            gestorLog.Entrar();
            int procesoId = 0;
            try
            {
                procesoId = connection.QuerySingle<int>(
                       "docp_Proceso_ins"
                     , new
                     {
                         @i_Nombre = proceso.ProcesoNombre,
                         @i_UsuarioCreacion = proceso.Usuario,
                         @i_Descripcion = proceso.Descripcion,
                         @i_Empresa = proceso.Empresa.ToString(),
                         @i_ClasificacionId = proceso.ClasificacionId,
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
            return procesoId;
        }

        public bool EliminaxNombre(ProcesoNombreDto proceso) {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int procesoId = connection.Execute(
                       "docp_Proceso_del_nom"
                     , new
                     {
                         @i_Empresa = proceso.Empresa.ToString(),
                         @i_Nombre = proceso.ProcesoNombre,
                         @i_UsuarioModificacion = proceso.Usuario
                     }
                     , commandType: CommandType.StoredProcedure);
                if (procesoId > 0) { respuesta = true; }
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

        public bool EliminaxId(ProcesoIdDto proceso) {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int procesoId = connection.Execute(
                       "docp_Proceso_del"
                     , new
                     {
                         @i_Empresa = proceso.Empresa.ToString(),
                         @i_ProcesoId = proceso.ProcesoId,
                         @i_UsuarioModificacion = proceso.Usuario
                     }
                     , commandType: CommandType.StoredProcedure);
                if (procesoId > 0) { respuesta = true; }
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

        public IEnumerable<Procesoc> Obtiene()
        {
            gestorLog.Entrar();
            IEnumerable<Procesoc> listadoProcesos = null;
            try
            {
                listadoProcesos = connection.Query<Procesoc>(
                       "docp_Proceso_get"
                       //, new
                       //{
                       //    @i_ProcesoId = proceso.ProcesoId,
                       //    @i_Nombre = proceso.Nombre.
                       //}
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
            return listadoProcesos;
        }

        public bool Modifica(ProcesoUpdDto proceso) {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int procesoId = connection.Execute(
                       "docp_Proceso_upd"
                     , new
                     {                         
                         @i_ProcesoId = proceso.ProcesoId,
                         @i_Nombre = proceso.ProcesoNombre,
                         @i_UsuarioModificacion = proceso.Usuario,
                         @i_Descripcion = proceso.Descripcion,
                         @i_Empresa = ( proceso.Empresa == Core.Enumeradores.EmpresaSel.NINGUNA  ? "" : proceso.Empresa.ToString() ) ,
                         @i_ClasificacionId = proceso.ClasificacionId,
                     }
                     , commandType: CommandType.StoredProcedure);
                if (procesoId > 0) { respuesta = true; }
            }
            catch (SqlException ex) {
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

        public IEnumerable<Procesoc> Obtener(ProcesoGetDto proceso)
        {
            gestorLog.Entrar();
            IEnumerable<Procesoc> listadoProcesos = null;
            try
            {
                listadoProcesos = connection.Query<Procesoc>(
                       "docp_Proceso_gets"
                     , new
                     {
                         @i_empresa = proceso.Empresa.ToString(),
                         @i_ProcesoId = proceso.ProcesoId,
                         @i_Nombre = proceso.ProcesoNombre,
                         @i_ClasificacionId = proceso.ClasificacionId,
                         @i_Clasificacion = proceso.ClasificacionNombre,
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
            return listadoProcesos;
        }
    }
    
}
