using cmn.std.Log;
using Dapper;
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
   public class RepositorioPlantillaTipoSubProceso : RepositorioBase, IRepositorioPlantillaTipoSubProceso
    {

        public RepositorioPlantillaTipoSubProceso(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog) { }

        public int Agrega(PlantillaTipoSubProcesoInsDto campotipo)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_PlantillaTipoSubProceso_ins"
                     , new
                     {
                         @Tipo = campotipo.Tipo,
                         @Descripcion = campotipo.Descripcion,
                         @DescripcionCorta = campotipo.DescripcionCorta,
                         @UsuarioCreacion = campotipo.Usuario
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

        public IEnumerable<ResultadoPlantillaTipoSubProceso> Obtener(PlantillaTipoSubProcesoGetDto campotipo)
        {
            gestorLog.Entrar();
            IEnumerable<ResultadoPlantillaTipoSubProceso> listadoCampos = null;
            try
            {

                listadoCampos = connection.Query<ResultadoPlantillaTipoSubProceso>(
                    "docp_PlantillaTipoSubProceso_get"
                    , new
                    {
                        @Id = campotipo.Id,
                        @Tipo = campotipo.Tipo,
                        @DescripcionCorta = campotipo.DescripcionCorta
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

        public bool  Modificar(PlantillaTipoSubProcesoUpdDto campotipo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_PlantillaTipoSubProceso_upd"
                     , new
                     {
                         @Id = campotipo.Id,
                         @Tipo = campotipo.Tipo,
                         @Descripcion = campotipo.Descripcion,
                         @DescripcionCorta = campotipo.DescripcionCorta,
                         @UsuarioModificacion = campotipo.Usuario
                     }
                     , commandType: CommandType.StoredProcedure);

                if (Id > 0) { respuesta = true; }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 && ex.Class == 14)
                    throw new BusinessException("No se pudo modificar .Valor duplicado en Tipo o DescripcionCorta");
                else if (ex.Number == 50000 && ex.Class == 16)
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

        public bool Eliminar(PlantillaTipoSubProcesoDelDto campotipo)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            try
            {
                int Id = connection.Execute(
                       "docp_PlantillaTipoSubProceso_del"
                     , new
                     {
                         @Id = campotipo.Id,
                         @UsuarioModificacion = campotipo.Usuario
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
    }
}
