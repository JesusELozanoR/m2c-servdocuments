using cmn.std.Binarios;
using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Excepciones;
using ServDocumentos.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioPlantillas : ServicioBase, IServicioPlantillas
    {
        public ServicioPlantillas(GestorLog gestorLog, IUnitOfWork unitOfWork, GestorBinarios gestorBinarios) : base(gestorLog, unitOfWork, gestorBinarios)
        {
        }

        public List<ArchivoPlantillaDto> ObtenerPorSubProceso(ObtenerPlantillasProcesoDto plantillasProceso)
        {

            var listaPlantillas = UnitOfWork.RepositorioPlantillas.ObtenerPorSubProceso(plantillasProceso);
            List<ArchivoPlantillaDto> archivoPlantillas = new List<ArchivoPlantillaDto>();
            foreach (var plantilla in listaPlantillas)
            {
                ArchivoPlantillaDto archivoPlantilla = new ArchivoPlantillaDto
                {
                    Nombre = $"{plantilla.Descripcion}.doc",
                    Base64 = plantilla.Base64
                };
                archivoPlantillas.Add(archivoPlantilla);
            }

            return archivoPlantillas;
        }

        public List<PlantillaDto> ObtenerListadoPorSubProceso(ProcesoSubDto ProcesoSub)
        {
            List<PlantillaDto> listaPlantillas = new List<PlantillaDto>();

            var datoslistaPlantillas = UnitOfWork.RepositorioPlantillas.ObtenerListadoPorSubProceso(ProcesoSub);
            foreach (var plantilla in datoslistaPlantillas)
            {
                listaPlantillas.Add(plantilla);
            }

            return listaPlantillas;
        }

        public int Agrega(PlantillaInsDto plantilla)
        {
            PlantillaArchivo archivo = new PlantillaArchivo();
            int Id = 0;
            try
            {
                Guid g = Guid.NewGuid();
                archivo.PlantillaNombre = string.Format("{0}.{1}", g.ToString(), plantilla.Extension);
                Id = UnitOfWork.RepositorioPlantillas.Agrega(plantilla, archivo);
                return Id;

            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool EliminaxNombre(PlantillaNombreDto plantilla)
        {
            return UnitOfWork.RepositorioPlantillas.EliminaxNombre(plantilla);
        }
        public bool Elimina(PlantillaIdDto plantilla)
        {
            return UnitOfWork.RepositorioPlantillas.Elimina(plantilla);
        }
        public IEnumerable<PlantillaDto> Obtiene(PlantillaGetDto plantilla)
        {
            return UnitOfWork.RepositorioPlantillas.Obtiene(plantilla);
        }
        public bool Modifica(PlantillaUpdDto plantilla)
        {
            try
            {
                PlantillaArchivo archivo = new PlantillaArchivo();
                return UnitOfWork.RepositorioPlantillas.Modifica(plantilla);
            }
            catch (BusinessException exb)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int Registra(PlantillaRegDto plantilla)
        {
            PlantillaArchivo archivo = new PlantillaArchivo();
            int Id = 0;
            try
            {
                Guid g = Guid.NewGuid();
                archivo.PlantillaNombre = string.Format("{0}.{1}", g.ToString(), plantilla.Extension);

                Id = UnitOfWork.RepositorioPlantillas.Registra(plantilla, archivo);
                return Id;

            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //funcion ObtenerporId
        public ArchivoPlantillaDto ObtenerPorId(int plantillaId)
        {

            var plantilla = UnitOfWork.RepositorioPlantillas.ObtenerPorId(plantillaId);
            ArchivoPlantillaDto archivoPlantilla = new ArchivoPlantillaDto
            {
                Nombre = $"{plantilla.Descripcion}.doc",
                Base64 = plantilla.Base64
            };
            return archivoPlantilla;
        }


        //funcion ObtenerporId
        public ArchivoPlantillaDto ObtenerPorNombre(string plantillaNombre)
        {
            var plantilla = UnitOfWork.RepositorioPlantillas.ObtenerPorNombre(plantillaNombre);
            ArchivoPlantillaDto archivoPlantilla = new ArchivoPlantillaDto
            {
                Nombre = $"{plantilla.Descripcion}.doc",
                Base64 = plantilla.Base64
            };
            return archivoPlantilla;
        }
        /// <summary>
        /// Migra todos los documentos con un id del alfresco a la DB con base64
        /// </summary>
        /// <returns>Entero con la cantidad total de registros modificados</returns>
        public int MigrarPlantillasBase64()
        {
            gestorLog.Entrar();
            IEnumerable<string> listadoIds = UnitOfWork.RepositorioPlantillas.ObtenerListadoIdSinBase64();
            int count = 0;
            foreach (string id in listadoIds)
                if (gestorBinarios.ArchivoObtiene(id).Objeto is BufferedStream m)
                    count += UnitOfWork.RepositorioPlantillas.ActualizarMigracionBase64(Convert.ToBase64String(ArchivosHelper.LeerArchivoBytes(m)), id);
            gestorLog.Salir();
            return count;
        }
    }
}
