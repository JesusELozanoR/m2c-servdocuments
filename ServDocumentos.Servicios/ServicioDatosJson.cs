using cmn.std.Binarios;
using cmn.std.Log;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using ServDocumentos.Core.Entidades.Comun;
using ServDocumentos.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioDatosJson : ServicioBase, IServicioDatosJson
    {
        public ServicioDatosJson(GestorLog gestorLog, IUnitOfWork unitOfWork, GestorBinarios gestorBinarios) : base(gestorLog, unitOfWork, gestorBinarios)
        {
        }

        public void Eliminar(DatosJson datosJson)
        {
            gestorLog.Entrar();
            UnitOfWork.RepositorioDatosJson.Eliminar(datosJson);
            gestorLog.Salir();
        }

        public void Insertar(DatosJson datosJson)
        {
            gestorLog.Entrar();
            UnitOfWork.RepositorioDatosJson.Insertar(datosJson);
            gestorLog.Salir();
        }

        public DatosJson Obtener(string credito)
        {
            gestorLog.Entrar();
            var datosJson = UnitOfWork.RepositorioDatosJson.Obtener(credito);
            gestorLog.Salir();
            return datosJson;
        }
        /// <summary>
        /// Migra todos los documentos con un id del alfresco a la DB con base64
        /// </summary>
        /// <returns>Entero con la cantidad total de registros modificados</returns>
        public int MigrarDatosJsonBase64()
        {
            gestorLog.Entrar();
            IEnumerable<string> listadoIds = UnitOfWork.RepositorioDatosJson.ObtenerListadoIdSinBase64();
            int count = 0;
            foreach (string id in listadoIds)
                if (gestorBinarios.ArchivoObtiene(id).Objeto is BufferedStream m)
                    count += UnitOfWork.RepositorioDatosJson.ActualizarMigracionBase64(Convert.ToBase64String(ArchivosHelper.LeerArchivoBytes(m)), id);
            gestorLog.Salir();
            return count;
        }
    }
}
