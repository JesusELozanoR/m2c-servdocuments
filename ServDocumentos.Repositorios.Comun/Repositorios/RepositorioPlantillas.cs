using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Entidades.Comun;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioPlantillas : RepositorioBase, IRepositorioPlantillas
    {
        public RepositorioPlantillas(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public IEnumerable<Plantillas> ObtenerPorSubProceso(ObtenerPlantillasProcesoDto plantillasProceso)
        {
            gestorLog.Entrar();
            IEnumerable<Plantillas> plantillas = null;
            try
            {
                plantillas = connection.Query<Plantillas>(
                       "docp_PlantillasPorSubProceso_get"
                     , new
                     {
                         //@ProcesoId = (int)plantillasProceso.Proceso,
                         @SubProcesoNombre = plantillasProceso.SubProcesoNombre
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return plantillas;
        }

        public IEnumerable<PlantillaDto> ObtenerListadoPorSubProceso(ProcesoSubDto ProcesoSub)
        {
            gestorLog.Entrar();
            IEnumerable<PlantillaDto> plantillas = null;
            try
            {
                plantillas = connection.Query<PlantillaDto>(
                       "docp_PlantillasListadoxSubProceso_get"
                     , new
                     {
                         @Proceso = ProcesoSub.ProcesoNombre.ToString(),
                         @SubProcesoNombre = ProcesoSub.SubProcesoNombre
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return plantillas;
        }

        public IEnumerable<string> SubProcesoPlantillaFiltro(SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();
            IEnumerable<string> plantillas = null;
            string grupo;
            string filtro;
            try
            {
                switch (solicitud.SubProcesoNombre)
                {
                    case "TeChreoPalnegocio":
                        grupo = "ConCuentaLigada";
                        filtro = solicitud.ConCuentaAhorroLigada;
                        break;
                    case "ClubCAMECompInv":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    case "CreceMasCompInv":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    case "ClubCAMETicketInv":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    case "CreceMasTicketInv":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    case "CreceMasEmpresas":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    case "XAdela":
                        grupo = "TipoComprobante";
                        filtro = solicitud.TipoComprobante;
                        break;
                    default:
                        grupo = "TipoCliente";
                        filtro = solicitud.TipoPersona;
                        break;
                }


                plantillas = connection.Query<string>(
                       "docp_SubProcesoPlantillaFiltro_get"
                     , new
                     {
                         @SubProcesoNombre = solicitud.SubProcesoNombre,
                         @Filtro = filtro,
                         @Grupo = grupo
                     }
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return plantillas;
        }

        private IEnumerable<Plantilla> AplicarFiltro(SolictudDocumentoDto solicitud, IEnumerable<Plantilla> plantillas)
        {
            gestorLog.Entrar();
            IEnumerable<Plantilla> _plantillas = null;

            try
            {
                List<Plantilla> Plantillas = new List<Plantilla>();
                List<int> index = new List<int>();

                var filtro = SubProcesoPlantillaFiltro(solicitud).AsList();
                Plantillas = plantillas.AsList().OrderBy(x => x.PlantillaId).ToList();

                foreach (var item in filtro)
                {
                    foreach (var plantilla in Plantillas)
                    {
                        if (plantilla.PlantillaId == Int32.Parse(item))
                        {
                            index.Add(Plantillas.IndexOf(plantilla));
                        }
                    }
                }

                index = index.OrderByDescending(x => x).ToList();
                foreach (var item in index)
                {
                    Plantillas.RemoveAt(item);
                }

                _plantillas = Plantillas;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }

            return _plantillas;
        }

        public DocData DocumentoDatosPorSubProceso(SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();
            DocData datos = null;
            try
            {
                var result = connection.QueryMultiple(
                       "docp_DocumDatosPorSubProceso_get"
                     , new
                     {
                         @ProcesoNombre = solicitud.ProcesoNombre,
                         @SubProcesoNombre = solicitud.SubProcesoNombre
                     }
                     , commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    datos = new DocData();
                    datos = result.ReadFirstOrDefault<DocData>();
                    datos.Plantillas = result.Read<Plantilla>();
                    datos.Campos = result.Read<Campo>();

                    if (!string.IsNullOrEmpty(solicitud.TipoPersona) || !string.IsNullOrEmpty(solicitud.ConCuentaAhorroLigada) || !string.IsNullOrEmpty(solicitud.TipoComprobante))
                        datos.Plantillas = AplicarFiltro(solicitud, datos.Plantillas);

                }
            }
            finally
            {
                gestorLog.Salir();
            }
            return datos;
        }

        public int Agrega(PlantillaInsDto plantilla, PlantillaArchivo archivo)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_Plantilla_Sub_ins"
                     , new
                     {
                         @i_Nombre = archivo.PlantillaNombre,
                         @i_Descripcion = plantilla.Descripcion,
                         @i_Version = plantilla.Version,
                         @i_DescripcionDocumentos = plantilla.DescripcionDocumentos,
                         @i_AlfrescoId = archivo.AlfrescoId,
                         @i_AlfrescoURL = archivo.AlfrescoUrl,
                         @i_UsuarioCreacion = plantilla.Usuario,
                         @i_SubProcesoId = plantilla.SubProcesoId,
                         @i_Reca = plantilla.RECA,
                         @i_Tipo = plantilla.Tipo,
                         @i_Base64 = plantilla.Archivo64
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return Id;
        }

        public bool EliminaxNombre(PlantillaNombreDto plantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Plantilla_del_nom"
                     , new
                     {
                         @i_Nombre = plantilla.PlantillaNombre,
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public bool Elimina(PlantillaIdDto plantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Plantilla_del"
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public IEnumerable<PlantillaDto> Obtiene(PlantillaGetDto plantilla)
        {
            gestorLog.Entrar();
            IEnumerable<PlantillaDto> listadoPlantillas = null;
            try
            {
                listadoPlantillas = connection.Query<PlantillaDto>(
                       "docp_Plantilla_get"
                     , new
                     {
                         @i_PlantillaId = plantilla.PlantillaId,
                         @i_Nombre = plantilla.PlantillaNombre,
                         @i_Descripcion = plantilla.Descripcion,
                         @i_AlfrescoId = plantilla.AlfrescoId,
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return listadoPlantillas;
        }

        public bool Modifica(PlantillaUpdDto plantilla)
        {
            gestorLog.Entrar();
            bool respuesta = false;
            int Id = 0;
            try
            {
                Id = connection.Execute(
                       "docp_Plantilla_upd"
                     , new
                     {
                         @i_PlantillaId = plantilla.PlantillaId,
                         @i_Nombre = plantilla.PlantillaNombre,
                         @i_Descripcion = plantilla.Descripcion,
                         @i_Version = plantilla.Version,
                         @i_AlfrescoId = plantilla.AlfrescoId,
                         @i_AlfrescoUrl = plantilla.AlfrescoUrl,
                         @i_UsuarioModificacion = plantilla.usuario,
                         @i_DescripcionDocumentos = plantilla.DescripcionDocumentos,
                         @i_Base64 = string.IsNullOrEmpty(plantilla.Archivo64) ? null : plantilla.Archivo64
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public int Registra(PlantillaRegDto plantilla, PlantillaArchivo archivo)
        {
            gestorLog.Entrar();
            int Id = 0;
            try
            {
                Id = connection.QuerySingle<int>(
                       "docp_Plantilla_ins"
                     , new
                     {
                         @i_Nombre = archivo.PlantillaNombre,
                         @i_Descripcion = plantilla.Descripcion,
                         @i_Version = plantilla.Version,
                         @i_DescripcionDocumentos = plantilla.DescripcionDocumentos,
                         @i_AlfrescoId = archivo.AlfrescoId,
                         @i_AlfrescoURL = archivo.AlfrescoUrl,
                         @i_UsuarioCreacion = plantilla.Usuario,
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
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return Id;
        }


        //consultar procedure
        public Plantillas ObtenerPorId(int plantillaId)
        {
            gestorLog.Entrar();
            Plantillas plantillas = null;
            try
            {
                plantillas = connection.QuerySingle<Plantillas>(
              "docp_PlantillasPorId_get"
                      , new
                      {
                          @PlantillaId = plantillaId
                      }
                       , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return plantillas;
        }

        public Plantillas ObtenerPorNombre(string plantillaNombre)
        {
            gestorLog.Entrar();
            Plantillas plantillas = null;
            try
            {
                plantillas = connection.QuerySingle<Plantillas>(
              "docp_PlantillasPorNombre_get"
                      , new
                      {
                          @PlantillaNombre = plantillaNombre
                      }
                       , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
            return plantillas;
        }
        /// <summary>
        /// Obtiene el listado de los AlfrescoId donde su campo Base64 se encuentra nulo
        /// </summary>
        /// <returns>Instancia de <see cref="IEnumerable<string>"/></returns>
        public IEnumerable<string> ObtenerListadoIdSinBase64()
        {
            gestorLog.Entrar();
            try
            {
                return connection.Query<string>(
                       "docp_Plantilla_ObtenerListadoIdSinBase64_get"
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        /// <summary>
        /// Actualiza el campo Base64 basado en el campo AlfrescoId
        /// </summary>
        /// <param name="base64">Texto en base64 con el contenido de la plantilla</param>
        /// <param name="idAlfresco">Id referencia de alfresco</param>
        /// <returns>Entero que contiene la cantidad de registros modificados</returns>
        public int ActualizarMigracionBase64(string base64, string idAlfresco)
        {
            gestorLog.Entrar();
            try
            {
                return connection.Execute("docp_Plantilla_ActualizarMigracionBase64_upd", new
                {
                    @i_AlfrescoBase64 = base64,
                    @i_AlfrescoId = idAlfresco
                }, commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }
        }

        public IEnumerable<Bancos> ObtieneBancos()
        {
            gestorLog.Entrar();
            try
            {
                return connection.Query<Bancos>(
                       "docp_Bancos_get"
                     , commandType: CommandType.StoredProcedure);
            }
            finally
            {
                gestorLog.Salir();
            }


        }
    }
}
