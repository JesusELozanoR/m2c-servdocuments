using System;
using QRCoder;
using System.IO;
using AutoMapper;
using cmn.std.Log;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using Syncfusion.Pdf;
using Newtonsoft.Json;
using System.Threading;
using Syncfusion.DocIO;
using cmn.std.Parametros;
using Newtonsoft.Json.Linq;
using Syncfusion.DocIO.DLS;
using System.Globalization;
using System.IO.Compression;
using Syncfusion.OfficeChart;
using Syncfusion.Pdf.Parsing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Syncfusion.DocIORenderer;
using System.Collections.Generic;
using ServDocumentos.Core.Mensajes;
using ServDocumentos.Core.Excepciones;
using Microsoft.Extensions.Configuration;
using ServDocumentos.Core.Entidades.Comun;
using ServDocumentos.Core.Dtos.DatosEngine;
using ServDocumentos.Core.Dtos.DatosSybase;
using ServDocumentos.Core.Dtos.Comun.Correo;
using ServDocumentos.Core.Dtos.Comun.Documento;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using Dcame = ServDocumentos.Core.Dtos.DatosCame;
using mambu = ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Contratos.Factories.Comun;
using ServDocumentos.Core.Contratos.Servicios.Comun;
using static ServDocumentos.Core.Helpers.Conversiones;
using cmn.core.GestorRepositorioDocumentos.Dtos.Common.Request;
using cmn.core.GestorRepositorioDocumentos;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu.Solicitudes;

namespace ServDocumentos.Servicios.Comun
{
    public class ServicioDocumentos : ServicioBase, IServicioDocumentos
    {
        /// <summary>
        /// Se usa para guardar los documentos generados
        /// </summary>
        private readonly GestorDocumentos _gestorDocumentos;
        /// <summary>
        /// Factory de repositorio del API de MAMBU
        /// </summary>
        private readonly IMambuRepositoryFactory _mambuFactory;
        /// <summary>
        /// Limite de cuentas a obtener por peticion
        /// </summary>
        private const int LimiteCuentas = 200;

        private SolictudDocumentoDto documSolicitud = null;
        private SolictudDocumentoJsonDto documSolicitudJson = null;
        private SolictudDocumentoJsonDatosDto documSolicitudJsonDatos = null;
        private SolicitudDocumentoReciboJson documentoReciboJson = null;
        //private string mensaje = "";
        private DocData datosDocumento = null;
        private string datosJson = null;
        private string datosQR = null;
        private string datosCD = null;
        private string datosGenerales = null;
        private Cliente cliente = null;
        //private DatosSolicitud datosSolicitud  = null;
        private mambu.ClienteJson clienteJson = null;
        private Dcame.Recibo clienteRecibo = null;
        private EstadoCuenta estadoCuenta = null;
        private Int64 lastId = 0;
        private DocGuarda docGuarda;
        private readonly Dictionary<string, MemoryStream> ArchivoPDF = new Dictionary<string, MemoryStream>();
        private readonly IMapper _mapper;
        /// <summary>
        /// Porducto digipro
        /// </summary>
        private string _productoDigipro = "";
        /// <summary>
        /// Empresa
        /// </summary>
        private string _empresaDigpro = "";

        public ServicioDocumentos(
            GestorLog gestorLog,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            Func<string, IServiceFactoryComun> serviceProvider,
            IServiceFactory factory,
            GestorParametros gestorParametros,
            IMapper mapper,
            GestorDocumentos gestorDocumentos,
            IMambuRepositoryFactory mambuFactory) : base(gestorLog, unitOfWork, configuration, serviceProvider, factory, gestorParametros)
        {
            _mapper = mapper;
            _gestorDocumentos = gestorDocumentos;
            _mambuFactory = mambuFactory;
        }

        public ResultadoDocumentoDto ObtieneDocumento(SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();
            //ResultadoDocumentoDto resultado = new ResultadoDocumentoDto();

            documSolicitud = solicitud;
            ObtenerDatos();
            MemoryStream documento = GeneraDocumento();
            DocumentoGuarda();
            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            resultado = GuardaResultado(resultado);
            gestorLog.Salir();
            return resultado;
        }
        private ResultadoDocumentoDto PreparaResultado(MemoryStream documento)
        {
            gestorLog.Entrar();
            ResultadoDocumentoDto resultado = new ResultadoDocumentoDto();
            try
            {
                if (documSolicitudJsonDatos != null)
                {
                    documSolicitud.NumeroCredito = Guid.NewGuid().ToString();
                }

                //resultado.Estado = "OK";
                List<byte[]> ListaArchivos = new List<byte[]>();

                if (documSolicitud.Separado)
                {
                    string nombres = "";
                    List<string> ListaBase64 = new List<string>();
                    foreach (var item in ArchivoPDF)
                    {
                        nombres += (nombres == "" ? "" : "|") + item.Key.ToString();

                        if (documSolicitud.Base64 == true)
                            ListaBase64.Add(Convert.ToBase64String(item.Value.ToArray()));

                        if (documSolicitud.Base64 == false)
                            ListaArchivos.Add(item.Value.ToArray());

#if DEBUG
                        string docfin = @"C:\ServDocsPruebas\" + documSolicitud.NumeroCredito + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + item.Key + ".pdf";
                        try
                        {
                            using (FileStream file = new FileStream(docfin, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                item.Value.CopyTo(file);
                            }
                        }
                        catch { }
#endif

                    }
                    resultado.Mensaje = nombres;

                    if (documSolicitud.Comprimido == true)
                    {
                        MemoryStream tmpdocumento = ArchivosToZiP(ArchivoPDF);

                        if (documSolicitud.Base64 == true)
                            resultado.Dato = Convert.ToBase64String(tmpdocumento.ToArray());

                        if (documSolicitud.Base64 == false)
                        {
                            ListaArchivos = new List<byte[]>();
                            ListaArchivos.Add(tmpdocumento.ToArray());
                            resultado.ListaArchivos = ListaArchivos;
                        }
                    }
                    else
                    {
                        if (documSolicitud.Base64 == true)
                            resultado.ListaDatos = ListaBase64;

                        if (documSolicitud.Base64 == false)
                            resultado.ListaArchivos = ListaArchivos;
                    }

                }
                else
                {
                    resultado.Mensaje = string.Format("Documentos_{0}{1}", documSolicitud.NumeroCredito, documSolicitud.Comprimido ? ".zip" : ".pdf");
                    if (documSolicitud.Comprimido == true)
                    {
                        documento = ArchivoToZiP(documento);
                    }
                    else
                    {
                        //// Confiracion de Creacion de PDF OK
                        //string docfin = @"C:\FGGP\DOCUMS\" + documSolicitud.numeroCredito + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".pdf";
                        //using (FileStream file = new FileStream(docfin, FileMode.Create, System.IO.FileAccess.Write))
                        //{
                        //    documento.CopyTo(file);
                        //}

                        // resultado.Dato = Convert.ToBase64String(documento.ToArray());
                    }

#if DEBUG
                    string docfin = @"C:\ServDocsPruebas\" + documSolicitud.NumeroCredito + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".pdf";
                    try
                    {
                        using (FileStream file = new FileStream(docfin, FileMode.Create, System.IO.FileAccess.Write))
                        {
                            documento.CopyTo(file);
                        }
                    }
                    catch { }
#endif

                    if (documSolicitud.Base64 == true)
                        resultado.Dato = Convert.ToBase64String(documento.ToArray());

                    if (documSolicitud.Base64 == false)
                    {
                        ListaArchivos.Add(documento.ToArray());
                        resultado.ListaArchivos = ListaArchivos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException("No se pudo crear el archivo de respuesta.");
            }

            gestorLog.Salir();
            return resultado;
        }
        /// <summary>
        /// Guardar el resultado en un gestor de documentos
        /// </summary>
        /// <param name="archivo">Archivo</param>
        /// <returns>Resultado despues de guardar</returns>
        private ResultadoDocumentoDto GuardaResultado(ResultadoDocumentoDto archivo)
        {
            ResultadoDocumentoDto resultado = archivo;

            if (string.IsNullOrEmpty(documSolicitud.GuardarDocumento))
                return resultado;

            if (!Enum.TryParse(documSolicitud.GuardarDocumento, true, out Core.Enumeradores.TipoGestorDocumentos tipoGestorDocumentos))
            {
                gestorLog.RegistrarError(new Exception($"No existe el gestor de documentos: {documSolicitud.GuardarDocumento}"));
                return resultado;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(archivo.Dato))
                    archivo.ListaArchivos.Add(Convert.FromBase64String(archivo.Dato));

                foreach (string itemBase64 in archivo.ListaDatos)
                {
                    archivo.ListaArchivos.Add(Convert.FromBase64String(itemBase64));
                }

                string[] Nombres = archivo.Mensaje.Split('|');
                if (Nombres.Length == 0)
                    Nombres = new string[] { archivo.Mensaje };

                int contArchivo = 0;
                foreach (byte[] itemArchivo in archivo.ListaArchivos)
                {
                    string archivoNombre = (Nombres[contArchivo].StartsWith("Documentos_") ? "" : documSolicitud.NumeroCredito + "_") + Nombres[contArchivo];
                    if (tipoGestorDocumentos == Core.Enumeradores.TipoGestorDocumentos.Digipro)
                    {
                        var base64 = Convert.ToBase64String(itemArchivo);
                        var extension = "PDF";

                        if (archivoNombre.Contains("."))
                            archivoNombre = archivoNombre.Remove(archivoNombre.IndexOf('.') - 1, archivoNombre.Length - archivoNombre.IndexOf('.'));

                        if (!base64.Contains("JVBER"))
                            extension = "ZIP";

                        var archivoDigipro = GuardarDocumentoEnDigipro(base64, $"{archivoNombre}.{extension}", extension);
                        if (archivoDigipro != null)
                        {
                            resultado.ArchivosGuardados.Add(archivoDigipro);
                            resultado.ListaArchivosAlfresco.Add(new ArchivosAlfresco
                            {
                                Archivo = archivoDigipro.Nombre,
                                AlfrescoId = archivoDigipro.Id,
                                AlfrescoUrl = archivoDigipro.Url,
                                Mensaje = archivoDigipro.Mensaje
                            });
                        }
                    }

                    contArchivo++;
                }
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
            }
            return resultado;
        }

        /// <summary>
        /// Guardar documento en digipro
        /// </summary>
        /// <param name="documentoBase64">Archivo en base 64</param>
        /// <param name="nombre">Nombre de archivo</param>
        /// <param name="mimeType">Tipo de archivo</param>
        /// <returns>Informacion del archivo guardo en digipro</returns>
        private ArchivoGuardado GuardarDocumentoEnDigipro(string documentoBase64, string nombre, string mimeType)
        {
            try
            {
                gestorLog.Entrar();
                var productCode = ObtenerProductoDigipro();
                if (string.IsNullOrEmpty(productCode))
                {
                    gestorLog.RegistrarError(new ArgumentException("No existe una configuración de producto para DIGIPRO"));
                    return new ArchivoGuardado() { Mensaje = "No existe una configuración de producto para DIGIPRO" };
                }

                var empresa = ObtenerEmpresaDigipro();
                var documento = new PublishDocumentDto
                {
                    Document = documentoBase64,
                    MimeType = mimeType,
                    NameDocument = nombre,
                    ProductCode = productCode,
                    Empresa = empresa,
                    Metadatos = new Dictionary<string, object> {
                        { "IdCliente", documSolicitud.NumeroCliente },
                        { "IdCredito", documSolicitud.NumeroCredito },
                        { "Origen", "ServicioDocumentos" },
                    }
                };

                var respuesta = _gestorDocumentos.PublishDocument(documento);

                if (respuesta.IsError)
                {
                    gestorLog.RegistrarError(new Exception(respuesta.Error));
                    return new ArchivoGuardado { Mensaje = $"No fue posible guardar el documento {nombre}." };
                }

                return new ArchivoGuardado
                {
                    Nombre = nombre,
                    Url = respuesta.UrlDigiPro,
                    Id = respuesta.IdSolicitud,
                    Mensaje = "OK"
                };
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                return new ArchivoGuardado { Mensaje = $"No fue posible guardar el documento {nombre}." };
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        private void DocumentoGuarda()
        {
            gestorLog.Entrar();

            docGuarda = new DocGuarda
            {
                ClienteNumero = int.Parse(documSolicitud.NumeroCliente),
                NumeroCredito = documSolicitud.NumeroCredito,
                Usuario = documSolicitud.Usuario
            };

            lastId = UnitOfWork.RepositorioDocumentos.DocumentosRegistroGuarda(docGuarda);
            docGuarda.DocumentoId = lastId;

            gestorLog.Salir();
        }
        private MemoryStream ArchivoToZiP(MemoryStream archivo)
        {
            gestorLog.Entrar();
            MemoryStream resultado = null;
            try
            {
                using (MemoryStream zipStream = new MemoryStream())
                {
                    using (var zipFile = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        ZipArchiveEntry zipPdf = zipFile.CreateEntry("Archi.pdf");
                        using (Stream entryStream = zipPdf.Open())
                        {
                            archivo.CopyTo(entryStream);
                        }
                    }
                    zipStream.Position = 0;

#if DEBUG
                    // Confirmacion de Archivo = OK
                    using (var fileStream = new FileStream(@"C:\ServDocsPruebas\test.zip", FileMode.Create))
                    {
                        zipStream.Seek(0, SeekOrigin.Begin);
                        zipStream.CopyTo(fileStream);
                    }
#endif
                    resultado = zipStream;

                }

            }
            catch (Exception)
            {
                throw new BusinessException("No se pudo crear el comprimido.");
            }
            gestorLog.Salir();
            return resultado;
        }
        private MemoryStream ArchivosToZiP(Dictionary<string, MemoryStream> archivos)
        {
            gestorLog.Entrar();
            MemoryStream resultado = new MemoryStream();
            try
            {
                //using (MemoryStream zipStream = new MemoryStream())
                //{
                using (var zipFile = new ZipArchive(resultado, ZipArchiveMode.Create, true))
                {
                    foreach (var item in archivos)
                    {
                        ZipArchiveEntry zipPdf = zipFile.CreateEntry(item.Key.ToString() + ".pdf");
                        using (Stream entryStream = zipPdf.Open())
                        {
                            item.Value.CopyTo(entryStream);
                        }
                    }
                }
                resultado.Position = 0;

                // Confirmacion de Archivo = OK
                //using (var fileStream = new FileStream(@"C:\FGGP\DOCUMS\test.zip", FileMode.Create))
                //{
                //    zipStream.Seek(0, SeekOrigin.Begin);
                //    zipStream.CopyTo(fileStream);
                //}

                // resultado = resultado;
                //}

            }
            catch (Exception)
            {
                throw new BusinessException("No se pudo crear el comprimido multiple.");
            }
            gestorLog.Salir();
            return resultado;
        }
        private void ObtenerDatos()
        {
            gestorLog.Entrar();

            datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            if (datosDocumento != null)
            {
                ObtenerGeneralesJson();
                ObtenerDatosJson();
                SeleccionaPlantillas();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }

            gestorLog.Salir();
        }
        private void AjustaPlantillasCampos()
        {
            gestorLog.Entrar();
            try
            {
                //mensaje = "";
                foreach (Plantilla item in datosDocumento.Plantillas)
                {
                    var listaCampos = from Campo c in datosDocumento.Campos
                                      where c.PlantillaId == item.PlantillaId
                                      select c;
                    item.Campos = new List<Campo>();
                    foreach (Campo item2 in listaCampos)
                    {
                        item.Campos.Add(item2);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new BusinessException("No se pudieron asignar los campos a las plantillas.");
            }
            gestorLog.Salir();
        }
        private void SeleccionaPlantillas()
        {
            gestorLog.Entrar();

            if (datosDocumento.Empresa.Equals("TCR") && datosDocumento.ProcesoNombre.Equals("CreditosTeCreemos") && !datosDocumento.SubProcesoNombre.Equals("TeCreemosPalNegocio"))
            {
                // Se Eliminan las Plantillas No Necesarias
                AjustaCondicionesCredito();
            }

            if (documSolicitud.ProcesoNombre.Equals("SegurosCame") && documSolicitud.SubProcesoNombre.Equals("TechreoSeguroVida"))
            {
                documSolicitud.ListaPlantillasIds = AjustaTechreoSeguroVida();
            }

            if (documSolicitud.ProcesoNombre.ToLower() == "creditocamedigital" && documSolicitud.SubProcesoNombre.ToLower() == "techreopalnegocio")
            {
                var json = JObject.Parse(datosJson);
                var credito = json.SelectToken("Credito");
                if (credito["ConCuentaAhorroLigada"] != null)
                {
                    string conCuentaAhorroLigada = credito["ConCuentaAhorroLigada"].ToString();
                    if (!string.IsNullOrWhiteSpace(conCuentaAhorroLigada))
                    {
                        documSolicitud.ConCuentaAhorroLigada = conCuentaAhorroLigada;
                        datosDocumento = null;
                        datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
                    }
                }
            }
            if (documSolicitud.ProcesoNombre.ToLower() == "ahorrotcr" && documSolicitud.SubProcesoNombre.ToLower() == "tcrpatrimonial")
            {
                var json = JObject.Parse(datosJson);
                var ahorro = json.SelectToken("Ahorro");

                string tipo = "";
                tipo = ahorro["TipoPersona"].ToString();
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    documSolicitud.TipoPersona = tipo;
                    datosDocumento = null;
                    datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
                }



            }

            //if (documSolicitud.ProcesoNombre.ToLower() == "ahorrocame" && documSolicitud.SubProcesoNombre.ToLower() == "bancadigitalempresarial")
            //{
            //    var json = JObject.Parse(datosJson);
            //    var ahorro = json.SelectToken("Ahorro");

            //    string tipo = "";
            //    tipo = ahorro["TipoPersona"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tipo))
            //    {
            //        documSolicitud.TipoPersona = tipo;
            //        datosDocumento = null;
            //        datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            //    }
            //}
            gestorLog.Salir();
        }
        private MemoryStream GeneraDocumento()
        {
            gestorLog.Entrar();
            bool filtrarPlantillas = false;
            bool Exist = false;
            bool existePlantilla = false;
            bool filtrarPlantillasimpresiones = false;
            MemoryStream resultado = new MemoryStream();

            try
            {
                int numeroCopias = 1;
                filtrarPlantillas = (documSolicitud.ListaPlantillasIds == null ? false : true);
                filtrarPlantillasimpresiones = (documSolicitud.ImpresionesPlantillas == null ? false : true);
                PlantillasImpresiones plantillasImpresiones;

                //Se crea el objeto que contendrá todos los documentos
                PdfDocument documentos = new PdfDocument
                {
                    EnableMemoryOptimization = true,
                    Compression = PdfCompressionLevel.Best
                };

                //Se crea el ciclo que genera los documentos
                foreach (Plantilla item in datosDocumento.Plantillas)
                {
                    if (filtrarPlantillas || filtrarPlantillasimpresiones)
                    {
                        if (filtrarPlantillas)
                        {
                            Exist = false;
                            existePlantilla = documSolicitud.ListaPlantillasIds.IndexOf(item.PlantillaId) != -1;
                            if (existePlantilla == true)
                            {
                                documSolicitud.ImpresionesPlantillas = new List<PlantillasImpresiones>();
                                plantillasImpresiones = new PlantillasImpresiones()
                                {
                                    NumeroImpresion = 1,
                                    Id = item.PlantillaId
                                };
                                documSolicitud.ImpresionesPlantillas.Add(plantillasImpresiones);
                                Exist = true;
                            }
                        }
                        else
                        {
                            Exist = documSolicitud.ImpresionesPlantillas.FindIndex(p => p.Id == item.PlantillaId) != -1;
                            if (Exist == true)
                                numeroCopias = documSolicitud.ImpresionesPlantillas.First(p => p.Id == item.PlantillaId).NumeroImpresion;
                        }
                    }
                    else
                        Exist = true;

                    if (Exist == true)
                    {
                        if (!string.IsNullOrEmpty(item.Base64))
                        {
                            int numeroIntegrantes = 1;
                            if (cliente != null)
                            {
                                if (cliente.Credito?.TipoCredito == "G")
                                {
                                    if (cliente.Credito?.TipoCredito == "G")
                                    {
                                        if (item.Tipo == "G")
                                            numeroIntegrantes = cliente.AhorroIndividual.Count();
                                        else if (item.Tipo == "V")
                                            numeroIntegrantes = cliente.AhorroIndividual.Where(d => d.SeguroVoluntario != null).Count();
                                        else if (item.Tipo == "P")
                                            numeroIntegrantes = cliente.Credito.PagoPorDividendo.Count();
                                    }
                                }
                            }
                            else
                                numeroIntegrantes = ObtenerContadorJson(item.Tipo); ///Json para came movil

                            for (int numInt = 1; numInt <= numeroIntegrantes; numInt++)
                            {
                                WordDocument documentoWord = CrearDocumentWordFromBase64(item.Base64);

                                if (documentoWord == null)
                                {
                                    gestorLog.RegistrarError(new BusinessException($"No se puedo generar el documento para la platilla: {item.PlantillaNombre}, con el Id: {item.PlantillaId}"));
                                    break;
                                }

                                if (item.Tipo == "I")
                                    MapeaDatosDocumentoWord(documentoWord, item, 0);
                                else
                                    MapeaDatosDocumentoWord(documentoWord, item, numInt);

                                MemoryStream streamPDF = WordToPDF(documentoWord);
                                documentoWord.Close();
                                documentoWord.Dispose();
                                documentoWord = null;

                                if (documSolicitud.Separado)
                                {
                                    if (numeroCopias > 1)
                                    {
                                        PdfLoadedDocument cargaDocumentoPdf = new PdfLoadedDocument(streamPDF);
                                        for (int i = 1; i <= numeroCopias; i++)
                                        {
                                            //Se agrega el documento al documento final
                                            PdfDocumentBase.Merge(documentos, cargaDocumentoPdf);
                                        }

                                        //Se cierra el stream
                                        cargaDocumentoPdf.Close();
                                        cargaDocumentoPdf.Dispose();
                                        cargaDocumentoPdf = null;
                                        streamPDF.Close();
                                        streamPDF.Dispose();

                                        documentos.Save(resultado);


                                        if (item.Tipo == "I")
                                            PDFStreamToArray(resultado, item.Descripcion);
                                        else
                                            PDFStreamToArray(resultado, item.Descripcion + "_" + numInt);
                                        resultado = new MemoryStream();
                                        documentos = new PdfDocument
                                        {
                                            EnableMemoryOptimization = true,
                                            Compression = PdfCompressionLevel.Best
                                        };
                                    }
                                    else
                                    {
                                        if (item.Tipo == "I")
                                            PDFStreamToArray(streamPDF, item.Descripcion);
                                        else
                                            PDFStreamToArray(streamPDF, item.Descripcion + "_" + numInt);
                                    }
                                }
                                else
                                {
                                    //Se agrega el número de copias de cada documento
                                    PdfLoadedDocument cargaDocumentoPdf = new PdfLoadedDocument(streamPDF);
                                    for (int i = 1; i <= numeroCopias; i++)
                                    {
                                        //Se agrega el documento al documento final
                                        PdfDocumentBase.Merge(documentos, cargaDocumentoPdf);
                                    }

                                    //Se cierra el stream
                                    cargaDocumentoPdf.Close();
                                    cargaDocumentoPdf.Dispose();
                                    cargaDocumentoPdf = null;
                                    streamPDF.Close();
                                    streamPDF.Dispose();

                                }
                            }

                        }
                    }
                }

                documentos.Save(resultado);

                resultado.Position = 0;
                //se elimina las firmas dependiendo si es representante fisico, moral o menor de edad ahooro vista
                //if (datosDocumento.Empresa == "TCR")
                //{
                //    if (documSolicitud.SubProcesoNombre.ToLower().Contains("ahorrovista") || documSolicitud.SubProcesoNombre.ToLower().Contains("depositoahorrale"))
                //    {
                //        resultado = AjustePDF(resultado);
                //    }
                //}
                //if (datosDocumento.Empresa == "CAME")
                //{
                //    if (documSolicitud.SubProcesoNombre.ToUpper().Contains("CAME_GRUPAL") 
                //        )
                //    {
                //        resultado = AjustePDF(resultado);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                gestorLog.Registrar(Nivel.Debug, ex.Message);
                // GestorErrores.Manejar(ex, CapaExcepcion.LogicaNegocios);
            }

            gestorLog.Salir();
            return resultado;
        }

        private WordDocument AjusteWord(WordDocument doc)
        {
            var sol = JsonConvert.DeserializeObject<DatosSolicitud>(datosJson);
            try
            {
                if (sol.Representantes.Count != 0)
                {
                    doc.Sections.RemoveAt(10);
                    doc.Sections.RemoveAt(8);

                }
                else
                {
                    if (sol.Tutores.Count != 0)
                    {
                        doc.Sections.RemoveAt(8);
                        doc.Sections.RemoveAt(8);
                    }
                    else
                    {

                        doc.Sections.RemoveAt(9);
                        doc.Sections.RemoveAt(9);

                    }
                }

            }
            catch (Exception)
            {

            }


            return doc;
        }
        private MemoryStream AjustePDF(MemoryStream res)
        {
            var sol = JsonConvert.DeserializeObject<DatosSolicitud>(datosJson);
            PdfLoadedDocument ldDoc = new PdfLoadedDocument(res);

            try
            {
                if (sol.Representantes != null && sol.Representantes.Count != 0)
                {
                    ldDoc.Pages.RemoveAt(10);
                    ldDoc.Pages.RemoveAt(8);

                }
                else
                {
                    if (sol.Tutores != null && sol.Tutores.Count != 0)
                    {
                        ldDoc.Pages.RemoveAt(8);
                        ldDoc.Pages.RemoveAt(8);
                    }
                    else
                    {
                        ldDoc.Pages.RemoveAt(9);
                        ldDoc.Pages.RemoveAt(9);

                    }
                }
            }
            catch (Exception ex)
            {
                gestorLog.Registrar(Nivel.Debug, ex.Message);
                throw ex;
            }


            MemoryStream resultado = new MemoryStream();
            ldDoc.Save(resultado);

            resultado.Position = 0;

            return resultado;
        }
        private void PDFStreamToArray(MemoryStream archivo, string ArchivoNombre)
        {
            gestorLog.Entrar();

            //MemoryStream streamDoc = new MemoryStream();
            MemoryStream streamDoc = archivo;
            streamDoc.Position = 0;

            ArchivoPDF.Add(ArchivoNombre, streamDoc);

            // Comprobacion de Archivo = OK
            //string docfin = @"C:\FGGP\DOCUMS\" + ArchivoNombre + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".pdf";
            //using (FileStream file = new FileStream(docfin, FileMode.Create, System.IO.FileAccess.Write))
            //{
            //    streamDoc.CopyTo(file);
            //}

            gestorLog.Salir();
        }
        private void WordtoArray(WordDocument wordDoc)
        {
            gestorLog.Entrar();

            MemoryStream streamDoc = new MemoryStream();
            //Saves the document to stream
            wordDoc.Save(streamDoc, FormatType.Docx);
            //Closes the document
            wordDoc.Close();
            streamDoc.Position = 0;

            // Comprobacion de Archivo = OK
            //string docfin = @"C:\FGGP\DOCUMS\" + ArchivoNombre + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".doc";
            //using (FileStream file = new FileStream(docfin, FileMode.Create, System.IO.FileAccess.Write))
            //{
            //    streamDoc.CopyTo(file);
            //}

            gestorLog.Salir();
        }
        /// <summary>
        /// Crear un documento word desde base64
        /// </summary>
        /// <param name="base64File">Base64 del archivo</param>
        /// <returns>Documento word</returns>
        private WordDocument CrearDocumentWordFromBase64(string base64File)
        {
            try
            {
                gestorLog.Entrar();
                var bytes = Convert.FromBase64String(base64File);
                var ms = new MemoryStream(bytes);
                var documento = CreaWordfromStream(ms);
                return documento;
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                return null;
            }
            finally
            {
                gestorLog.Salir();
            }
        }
        private MemoryStream WordToPDF(WordDocument documentoWord)
        {
            gestorLog.Entrar();
            //Se inicializa de nuevo el stream para ocuparlo
            MemoryStream streamDocumento;
            try
            {
                DocIORenderer render = new DocIORenderer();
                render.Settings.EnableAlternateChunks = false;
                render.Settings.OptimizeIdenticalImages = false;
                render.Settings.PdfConformanceLevel = PdfConformanceLevel.None;
                render.Settings.ChartRenderingOptions.ImageFormat = ExportImageFormat.Jpeg;

                //using (FileStream stream = File.Create(@"C:\ServDocsPruebas\Wordpdf" + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".docx"))
                //{
                //    documentoWord.Save(stream, FormatType.Docx);
                //}

                PdfDocument documentoPdf = render.ConvertToPDF(documentoWord);
                render.Dispose();
                documentoWord.Dispose();

                //Se inicializa el stream
                streamDocumento = new MemoryStream();
                //Se almacena el documento pdf en un stream
                documentoPdf.Save(streamDocumento);
                ////Se destruye el objeto
                documentoPdf.Close(true);
                documentoPdf = null;

                // Comprobacion = OK
                //using (FileStream stream = File.Create(@"C:\ServDocsPruebas\Wordpdf" + DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "").Replace(".", "") + ".pdf"))
                //{
                //    streamDocumento.Position = 0;
                //    streamDocumento.CopyTo(stream);
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new BusinessException("No se pudo convertir el archivo word a pdf.");
            }

            gestorLog.Salir();
            return streamDocumento;
        }
        private void AjustaCondicionesCredito()
        {
            gestorLog.Entrar();

            try
            {
                if (cliente == null)
                {
                    var listc = datosDocumento.Plantillas.Where(p => p.Descripcion.Contains("EstadoCuenta"));
                    datosDocumento.Plantillas = listc.ToList();
                }
                else
                {

                    //Si el origen de la solicitud es por renovación, se agrega el documento de solicitud de crédito
                    if (!(cliente.Credito.DatoSolicitudCredito != null && cliente.Credito.DatoSolicitudCredito.OrigenSolicitud == 3))
                    {
                        //Se Elimina la plantilla
                        var listc = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("SOLCRED"));
                        datosDocumento.Plantillas = listc.ToList();
                    }

                    var conyuge = (from a in cliente.Avales
                                   select new
                                   {
                                       a.NumeroCliente,
                                       a.NombreCompleto,
                                       a.Nombre,
                                       a.ApellidoPaterno,
                                       a.ApellidoMaterno,
                                       a.DireccionCompleta,
                                       a.Telefono,
                                       a.CURP,
                                       a.RFC,
                                       a.FechaNacimiento,
                                       a.Nacionalidad,
                                       a.TipoAval
                                   }).Where(a => a.TipoAval == "CONYUGE").Distinct().FirstOrDefault();

                    bool banderaCartaInf = false;

                    if (conyuge != null)
                    {
                        //RevisionServicio
                        DateTime fecha;
                        if (ValidarFecha(conyuge.FechaNacimiento, out fecha))
                        {
                            DateTime fechaEdad = Convert.ToDateTime(fecha).AddYears(70);

                            if (fechaEdad <= cliente.Credito.FechaDesembolso)
                            {
                                banderaCartaInf = true;
                            }

                        }

                    }

                    if (!banderaCartaInf)
                    {
                        var list1 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("CartaInformativaS"));
                        datosDocumento.Plantillas = list1.ToList();
                    }

                    if (cliente.Credito.TipoCredito != "G")
                    {
                        //Se quitan plantillas si no tiene seguros basicos o voluntarios
                        if (cliente.SeguroBasico == null && cliente.SeguroVoluntario == null)
                        {
                            var list11 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("CONSEN_PAQUETE")
                                                                            && !p.Descripcion.Contains("_PAQ_BASICO")
                                                                            && !p.Descripcion.Contains("_PAQ_PREMIUM")
                                                                            && !p.Descripcion.Contains("_PAQ_PLATINO")
                                                                            && !p.Descripcion.Contains("NEGULTRA_CERTC_PCON_CENCEL"));
                            datosDocumento.Plantillas = list11.ToList();
                        }
                        else
                        {
                            if (cliente.SeguroVoluntario != null)
                            {
                                var list1 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_BASICO"));
                                datosDocumento.Plantillas = list1.ToList();


                                if (cliente.SeguroVoluntario.Tipo == 1)
                                {
                                    var list2 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_PLATINO"));
                                    datosDocumento.Plantillas = list2.ToList();
                                }

                                if (cliente.SeguroVoluntario.Tipo == 2)
                                {
                                    var list3 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_PREMIUM"));
                                    datosDocumento.Plantillas = list3.ToList();
                                }

                                if (cliente.PlanConexion == null)
                                {
                                    var list4 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("NEGULTRA_CERTC_PCON_CENCEL"));
                                    datosDocumento.Plantillas = list4.ToList();
                                }
                            }
                            else
                            {
                                var list5 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_PREMIUM")
                                                                                && !p.Descripcion.Contains("_PAQ_PLATINO")
                                                                                && !p.Descripcion.Contains("NEGULTRA_CERTC_PCON_CENCEL"));
                                datosDocumento.Plantillas = list5.ToList();

                            }
                        }

                    }
                    else
                    {
                        //SE QUITA LAS SI NO TIENE
                        int coutSegBasicos = 0;
                        int counSegPremium = 0;
                        int counSegPlatino = 0;
                        int counPaquetes = 0;
                        foreach (var item in cliente.AhorroIndividual)
                        {
                            if (item.SeguroBasico != null)
                            {
                                coutSegBasicos = coutSegBasicos + 1;
                            }

                            if (item.SeguroVoluntario != null)
                            {
                                if (item.SeguroVoluntario.Tipo == 2)
                                    counSegPlatino = counSegPlatino + 1;
                                if (item.SeguroVoluntario.Tipo == 1)
                                    counSegPremium = counSegPremium + 1;

                            }
                            if (item.PlanConexion != null)
                            {
                                counPaquetes = counPaquetes + 1;
                            }
                        }

                        if (coutSegBasicos == 0 && counSegPlatino == 0 && counSegPremium == 0)
                        {
                            var list11 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("CONSEN_PAQUETE")
                                                                            && !p.Descripcion.Contains("_PAQ_BASICO")
                                                                            && !p.Descripcion.Contains("_PAQ_PREMIUM")
                                                                            && !p.Descripcion.Contains("_PAQ_PLATINO")
                                                                            && !p.Descripcion.Contains("NEGULTRA_CERTC_PCON_CENCEL"));
                            datosDocumento.Plantillas = list11.ToList();
                        }
                        else
                        {
                            if (coutSegBasicos == 0)
                            {
                                var list1 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_BASICO"));
                                datosDocumento.Plantillas = list1.ToList();

                            }
                            if (counSegPlatino == 0)
                            {
                                var list2 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_PLATINO"));
                                datosDocumento.Plantillas = list2.ToList();
                            }
                            if (counSegPremium == 0)
                            {
                                var list3 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_PAQ_PREMIUM"));
                                datosDocumento.Plantillas = list3.ToList();
                            }
                            if (counPaquetes == 0)
                            {
                                var list4 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("BANCA_CERTC_PCON_CENCEL"));
                                datosDocumento.Plantillas = list4.ToList();
                            }

                        }
                    }


                    //Incentivos
                    if (cliente.Credito.MontoIncentivo == 0)
                    {
                        var list1 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("VALE_FIDELIDAD"));
                        datosDocumento.Plantillas = list1.ToList();

                    }

                    //Se agrega Monto Seguda Dispersion 
                    if (cliente.Credito.MontoSegundaDispersion == 0)
                    {
                        var list1 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("TICKET_SEGUNDA_DISPOSICION"));
                        datosDocumento.Plantillas = list1.ToList();

                    }

                    // Descomentar para ActaFundacion
                    //if (cliente.Credito.DatoSolicitudCredito.OrigenSolicitud != 3) 
                    //{
                    //    var list2 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("B20_Actafundacion"));
                    //    datosDocumento.Plantillas = list2.ToList();
                    //}

                    var lista = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("EstadoCuenta"));
                    datosDocumento.Plantillas = lista.ToList();

                    if (cliente.BanderaCreditosSubsecuentes == 0)
                    {
                        var list2 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_SolicitudCreditosSubsecuentes"));
                        datosDocumento.Plantillas = list2.ToList();

                    }
                    if (cliente.BanderaOrdenPago == 0)
                    {
                        var list2 = datosDocumento.Plantillas.Where(p => !p.Descripcion.Contains("_OrdenPago"));
                        datosDocumento.Plantillas = list2.ToList();

                    }




                }
            }
            catch (Exception error)
            {
                throw new BusinessException("No se pudieron Ajustar las Plantillas." + error);
            }
            gestorLog.Salir();
        }
        private WordDocument CreaGrafica(WordDocument documentoWord, Campo dataCampo)
        {
            try
            {

                var jDatosTabla = JObject.Parse(dataCampo.DatoCampo);
                string graficaTitulo = jDatosTabla.SelectToken("Titulo").ToString();

                JArray datos = new JArray();

                string token = string.Empty;
                var grafDatos = new List<GraficaDatosDto>();


                JArray jCampos = (JArray)jDatosTabla.SelectToken("Datos");

                var jDatosJson = JObject.Parse(datosJson);

                foreach (var item in jCampos.Children())
                {
                    token = item["Valor"].ToString();
                    double valorCampo = Convert.ToDouble(jDatosJson[token].ToString());
                    grafDatos.Add(new GraficaDatosDto { Leyenda = item["Leyenda"].ToString(), Valor = valorCampo });
                }
                var grafica = new GraficaDto
                {
                    Titulo = graficaTitulo,
                    Datos = grafDatos
                };
                return CrearGrafica(documentoWord, grafica);
            }
            catch (Exception ex)
            {

                gestorLog.RegistrarError(ex);
            }
            return documentoWord;

        }
        public WordDocument CrearGrafica(WordDocument document, GraficaDto grafica)
        {
            var JSONString = JsonConvert.SerializeObject(grafica);
            document.UpdateDocumentFields();
            WChart chart = document.FindChartByTitle(grafica.Titulo);
            if (chart == null) return document;
            int count = 2;
            //Data source for the charts
            foreach (var item in grafica.Datos)
            {
                chart.ChartData.SetValue(count, 1, item.Leyenda);
                chart.ChartData.SetValue(count, 2, item.Valor);
                count++;
            }

            chart.ChartTitleArea.FontName = "Calibri";
            chart.ChartTitleArea.Size = 10;
            //Sets data label
            chart.Series[0].DataPoints.DefaultDataPoint.DataLabels.IsValue = true;

            chart.Refresh();
            return document;
        }

        private void MapeaDatosDocumentoWord(WordDocument documentoWord, Plantilla plantilla, int numInt)
        {
            gestorLog.Entrar();

            bool isGrupal = false;
            long numVol = 0;
            try
            {

                if (numInt > 0)
                {
                    isGrupal = true;
                    if (plantilla.Tipo == "V")
                    {
                        numVol = ObtenerClienteVoluntario(numInt);
                    }
                }

                //Ciclo que recorre la lista de campos que se van a mapear en el documento
                foreach (Campo item in plantilla.Campos)
                {
                    if (CamposFijos(item, documentoWord, plantilla) == false)
                    {
                        if (CamposEspeciales(item, documentoWord, numInt, numVol) == false)
                        {

                            string valor = "";
                            if (item.Tipo == "Forma")
                            {
                                DataTable dtTable = CreaForma(item, numInt);
                                documentoWord.MailMerge.ClearFields = true;
                                documentoWord.MailMerge.ExecuteGroup(dtTable);
                            }
                            else if (item.Tipo == "Tabla")
                            {
                                DataTable dtTable = CreaTabla(item, numInt);
                                documentoWord.MailMerge.ClearFields = true;
                                documentoWord.MailMerge.ExecuteGroup(dtTable);
                            }
                            else if (item.Tipo == "Grafica")
                            {
                                documentoWord = CreaGrafica(documentoWord, item);


                            }
                            else if (item.Tipo == "TablaMultiple")
                            {
                                DataSet dsTablas = CreaTablas(item, numInt);
                                foreach (DataTable tabla in dsTablas.Tables)
                                {
                                    documentoWord.MailMerge.ClearFields = true;
                                    documentoWord.MailMerge.ExecuteGroup(tabla);
                                }
                            }
                            else if (item.Tipo == "RegistroDeTabla")
                            {
                                valor = ObtenerRegistroDeTabla(item, numInt);
                            }

                            else
                            {
                                if (isGrupal == true)
                                {
                                    valor = ObtenerValorPropiedad(item, numInt);
                                }
                                else if (item.DatoConjunto == "Calculado")
                                {
                                    valor = ObtenerValorPropiedades(item);
                                }
                                else
                                {

                                    valor = ObtenerValorPropiedad(item, 0);
                                }
                            }

                            if (item.DatoCampo == "TotCapital")
                                valor = CampoTipo(valor, item.Tipo);


                            valor = CampoTipo(valor, item.Tipo);



                            //Si el campo es imagen
                            if (item.Tipo == "Imagen")
                            {
                                //debe venir de item.CampoNombre

                                //Datos prueba para tipo 1:
                                //valor = "iVBORw0KGgoAAAANSUhEUgAAA9QAAAPUCAQAAADm3bkPAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QA/4ePzL8AAAAJcEhZcwAAAGAAAABgAPBrQs8AAAAHdElNRQflAxoLHx++XgEwAACAAElEQVR42u3dd5gUVdrG4V8NIFmSqKhgDiQFQTEAJoyYFcwY15x2112z4rfqrm4wu8ZVwYCiooiBYAQDIIISzQooEiRHYeZ8fxCcgemZ7p6qfk9VPfde114mqLeHqnr6nDpvncAhIn5Z7uYzn3nMZz6LWM4yFrOS + axiYam / W33tlrBg3a9bzMo1f1WDeuv + aQOKAAhoyEbUpQ412ZjqNKQG9ahNLerTkEY0pCENqRVYf3oRKStQUIvYWOhmMJtZzGA2s9fE8ur / X25YVa1Sod2IpjSlGZvSlGZsrAgXMaGgFimAmW460 / mRmfwezpZxnI9apUJ7c1rQnK3YVOEtEjkFtUjoFrqp / Mh0pjOVaUxnWuxCOVu1aM5WNGdrtmIrtqYF9RXdIiFTUIuEYJ77jt//9z3pva4asV3p/ym2RapMQS2Sl4VuCpP4hm/4lm+Yb12OpxqxPTuwAzvQil002hbJi4JaJGsL3ddMZBITmcQPlFiXEzvNaM12tKI1u9FUoS2SJQW1SIWWu/GMYxITmcx062ISpDktaU0r2tFGLWEiFVJQi5RjofuCMUxiIp+ywrqYhKvOTrSmFR3Ym00U2SIbUFCLrPOLG8tYxvJZqpeD2QnYjt1pR3vas7kiW2QNBbWk3kr3BSMYwxgmWZci6zSjAx3ozL7UVmRLyimoJbV+dh8ygjGa3PZadXZaE9itCBTZkkoKakmZpe4ThjOSkcy1LkVy0oRO7EUXOmmMLSmjoJaUWOI+ZgQfMiKxbwlLi+rsRjf2pQsNFdiSCgpqSbhFbiTDGMFofrMuRUJVjXbsS2cOpIkCWxJNQS0Jtdi9yzDeY4JeTJJwRezKfhzMftRTYEsiKaglYYrdOIYxjOFaIpYy1WhHN7qxHzUU2JIoCmpJjO/cMIbxthaJpVw99qIbR9FKcS0JoaCW2FvihjCYoXxnXYh4ZXsO5jC6UVeBLTGnoJYY+9EN5jWGapJbMqpFZ7pxHDspriW2FNQSQyVuLK8xiM/0ok/J0nYcyVHsT3UFtsSOglpiZYl7h0G8xgzrQiSWmnAgR3K0OrAlVhTUEhNz3SD6M0Td0FJl1diLHpykjT8kJhTU4r3Z7k36M5iV1oVIoqyO6xPZUnEtnlNQi8emugH052O9skQiU0R7juQ0dlRci7cU1OKlb1x/XmKMdRmSEgEdOYEebKe4Fg8pqMUzv7qX6MNHWs8tBlrRizP17Fo8o6AWbyxwr9Kft1hlXYikWhF704uT2VhxLZ5QUIsHVrgh9OdlllgXIrJGLbrRgxOpo7gWcwpqMVXi3uNpXmaBdSEi5WjICZzGfhQprsWQglrMTHfP8Ije0C3e24rTOF8LzcSMgloMLHev8Qhva8GYxMbqJ9enaYsPMaCglgIb4/rwtLailFjamGPoxUEEimspIAW1FMwM9wL/4wvrMkSqaGdO5hxaKKylQBTUUgDOvc0jvKKXgEpiVOMALudIja2lABTUErH57gXuYZJ1GSIR2IHzOJdNFNYSKQW1RGiMe4SnWWpdhkiEanI059NNYS2RUVBLJBa55/gv46zLECmQllzAudRTXEsEFNQSusnuPp5mkXUZIgW2MadzGbsorCVkCmoJkXNvcw+vqz9aUmxfrtYiMwmVglpCsty9wJ1MtC5DxAM7cgnn6eUoEhIFtYRghnuY+/nVugwRjzTgTP7E1gprqTIFtVTRGHcP/dQhLVKOIo7gCq0IlypSUEveStzL/JNR1mWIeK4Tf+VY7cAleVNQS15+c/34O1OsyxCJie25jAuopbCWPCioJWeL3eP8i+nWZYjEzGZcyJU0VFhLjhTUkpPZ7gHuZZ51GSIxtTFncTVbKKwlBwpqydr37m4eZZl1GSIxV5Oe3MBOCmvJkoJasjLR3Up/iq3LEEmI6vTkBloqrCULCmqp1ER3B88qpEVCVsQR/B/tFdZSCQW1VEghLRIlhbVUTkEtGU1wdyqkRSIX0J1b2F1hLRkoqKVc490/eYYS6zJEUiKgO73poLCWciioZQOfu5sZqB2wRAos4Dh601ZhLetRUEsZP7rbeVzT3SJGijiB29lBYS2lKKhlndnu39zNCusyRFKuBmdzs16KIusoqAWAue5O7tXLTEQ8UYfLuJpGCmtBQS3AEnc//2C+dRkiUkZ9LuZ66iusU09BnXIr3RP0ZoZ1GSJSrqb8mSupqbBONQV1ijn3AtfyvXUZIlKh7fkHJxAorFNLQZ1ao9xVDLcuQkSysif/prOiOqUU1Kn0nbuGF9UpHQs1aUh9GlCDetSiNnXZiAZUoyHVqb/uv6pDzXJ+7QqWrvvrRaxiPqtYyG8sYRnLWcxKFrCQBVrpHwsBJ/F3tlFYp5CCOnXmudu4X7dmr9RlMzajKU3ZjAY0pBENaEBDGtCQWgW4MS9zC5jPAhYwf83/ZjKb2cxkFkusfzhSSi0u5zoaKKxTRkGdKivdf/k/frUuI9U2Y0u2ojlb0IymNGVzmlLH4xvvUjeLX5jNbGbwM9OYzk/Msi4q1ZpyMxdQ3eNzRsKmoE6RAe5qvrYuImVqsz3bsg1bsiUt2JKtErF+d7mbzk9MZfX/f8t3LLcuKWV24U6OSsCZJNlRUKfEV+4K3rIuIhUasV2p/21DUSpup/Pcd/z+v++1/qEADuReWqfi7BIFdQosdL25n5XWZSRWDXakJbvQml3YkXq6dbLQfc0UJjGFSXyrMy8yG3EFN+qFKCmgoE445/pwDb9Yl5E4tWlFS1qxM63Zjhq6VWa00n3NZKYwkSlM1hR56LbgTk5Vj3XCKagTbay7jA+ti0iQRrSiAx1oTVs20q0xZ6vcVCYyhjGMZqZ1MQmyB/ezp87HBFNQJ9av7noepcS6jNgL2JF2tGc32tFMN8PQTHfj+JxxjOVb61ISoBoX8n801vmZUArqRCp2j3ADc63LiLXN2ZM92ZM9aKjbX6TmulGs/t9s61JibRNu47yULF5MGwV1An3uLmCkdRExVZd2dFg9va0bXsH97MYwhg/5qNQb1SQXu/MwHXXmJo6COmEWuxu5j2LrMmKnGV3oTFfaUE23OXOr3DiGM5wRGmPnrDpX0pu6OosTRUGdKK+5y/jRuohYaUZnurEvrbRu1kvfuRF8yAgmWRcSK1tyDyfofE4QBXViTHeX8Yp1EbHRim50oTOb63YWC9PdBwznbb1ZL2sncg9b6OxOCAV1IpS4x/gLC63LiIHN6Eo3Dqe5bmGx9IsbzjDeYLp1ITFQlxu5So9yEkFBnQDj3AWMsi7Cc/XYi250Y3dNcSfCd24YwxjCAutCPNeeh9lDZ3zsKahjbpm7kbu1eCyjIjpyBIfTQSOLBFrpPuEt3mCcdSEeq86f6V2QzVIlOgrqWPvIncsU6yI8VZcDOIoj9ZwuBWa5txik8XVG2/MY++s6iDEFdWwtc7fwT715rBzbcSRH0VUv+UyZVe4TBjGMMdaFeCjgD/xbG8bEloI6pt5x5/G9dRGeqU5XjuNoWuh2lGrfuFd5hY/0JXY92/M4++naiCUFdQwtdDdyv25DpdSiM0dyMpvpNiRrzHFv0J+hrLAuxCMBf+Bf2hYzhhTUsTPYnc9U6yK8UYcD6cGxbKybj5RjqXub/rzCIutCvLEFD3GUrpaYUVDHygL3R56wLsITjTieHhygJ9FSqWVuCP0ZqLgGIOAc/k0DXTcxoqCOkQ/dmdoSEKjNQfTiGEW05GS5G0p/XmaJdSEeaMGTHKDrJzYU1DGx3PXWGm9qcjA9OF6rVyVvS93r9GEIv1kXYizgD9xFHV1JsaCgjoXPXC8mWhdhqhrdOJnjNGEnoZjjXqQfw1P+1bcNT7ObrqgYUFB7r9j9i5tS/f2/JT05R01XErqf3NM8nuqNPmpwHTfqrX3eU1B77nt3Fh9YF2GmET04g866jUiExrg+PMOv1mWY2Ys+7KhrzGsKaq/1cZew2LoIE9U4gPO1YEwKZLl7jT68xSrrQkzU4Xau0JXmMQW1t2a7s3nduggTrTmPU9lUNw4psOmuL4+ntLPiaB5nE11znlJQe+pddzo/WxdRcDU5mvM5SFtRihnn3uYRXmGldSEFtxlPcaiuPC8pqD20yt3K31K3HnUnzuFcfacXL8x0T/II31mXUWABl/Evauga9I6C2jtT3al8aF1EQWkcLT4qce+kcGy9J8+xna5EzyioPfOCOz9Vu+pux8WcRRPdGMRTM9xjPJSqx1CNeJQTdEV6RUHtkWXuGu61LqKA9uUKjqO6bgniuWL3BvcyzLqMAjqD/1JXV6Y3FNTemOROZrx1EQVSix78hba6EUiMfOYepi/LrMsokJY8p7eWeUNB7YmH3B9Zbl1EQWzLxZxLI90CJIZmuod5iBnWZRREbe7hD7pOvaCg9sASdyFPWxdREPtxJUfphYUSa7+5F/kPY6zLKIhe/Fcbd3hAQW3uK3diCqa8iziC69hbl7wkxAh3B6+T/PvnLrxEK123xhTUxl5xZyV+lXdNenIdu+hil4QZ7/5Jv8S3b9XnUU7S1WtKQW1olbuBO6yLiNjGnMXVbKHLXBLqF/cQ9zDfuoyInc99eu++IQW1mamuJyOti4jU1vyJc9XkIYk3z/2Xe5lpXUak9uYFttK1bERBbeQdd2qiL+xtuYILqakLW1JihXuKvzHduowIbcLTehe4kSLrAtKoxPV2Byc4plvSl6+5IlBMS3rUDM4PvuFBtrYuJDJzOIK/OaexnQGNqAtukTuTAdZFRKY1f+U0NWBJaq10z3EbX1mXEZnuPEMDXd8FpqAusK/dsUyyLiIiu/JnhbQIJe4lbmKKdRkR2YlXaKmrvKAU1AX1ujstoc1Yu3ML3bX/lcgaxe4FbuFL6zIi0YhnOFzXegHpGXXBOHeHOzqRMb0LTzGaIwPFtMha1YJTgkm8wPbWhURgHt25Rk+rC0gj6gJZ7M7mResiItCC6zlHO2CJZLDSPcEtidwmsyf/U/NlgSioC+Jbd1wCXxO6KX/iSrVgiVRiqXuU25llXUbodmUA2+n6LwAFdQEMdScz17qIkG3C1VxCbV2kIllZ4P7DXSyyLiNkTXieg3QXiJyCOnIPuCtZZV1EqOryJ/5CfV2eIjmZ427lv/xmXUaoqnMfF+peEDEFdaSK3fUJe5t3ESfwT7bWhSmSl6nuBp5O2K5b5/OA1qlESkEdocXuFAZZFxGqbvyL3XRBilTJaHcVH1gXEapDeV6vQYmQgjoy091RjLMuIkStuJPuuhRFQjHMXclE6yJC1JbXNNMWGfVRR+QT1zFBMb0VD/OFYlokNN2CsTzMZtZlhGY8HRmhcV9ENKKOxLPuXJZbFxGSulzDn7W+WyQCC9yt3JuY5WW1eZKeulNEQEEdOufu5NrELBY5kvs1oSUSoa/dnxKzliXgJm7Wq4RDp6AO2Qp3Fv2siwhJO+6liy45kcgNcn/kG+siQnIGj7GR7huhUlCHar47lvetiwhFY27iUu2EJVIgK92D3MRC6zJCcQADtAY8VArqEP3sjuBz6yJCUMRp/JumutBECmqG681jlFiXEYI2vEFz3UFCo6AOzXh3BNOtiwhBV+6nrS4xERMfuUsS0S/SgjdppftISNSeFZJ3XdcExHQj7uZdxbSImX2C0dzNxtZlVNlUOvO+xoEh0Yg6FC+502PfjhVwuia8Rbzws7uGvtZFVFlNnuRk3VFCoBF1CO5xPWMf0zsxlD6BYlrEB1sEfYJBbGNdRhWt4FTu1FgwBArqKnLuandlzJd/1OZmvtBmdSJe6R5M4mY2si6jShxXc4UrUVhXkaa+q+Q3dwYvWBdRRd25j20V0iJemuAu5EPrIqroFJ6ihu4xVaCgroKl7gTesi6iShrxD87XBSTiMece5SoWWZdRJUfQnzq60+RNQZ23+e7ImH/T7cEDWjwmEgM/u4sYaF1ElXThNb0EJW8K6jzNdIfFuttxcx7geF02IrHR313Er9ZFVEEbBrOF7jl5UVDn5Ud3MF9bF1EFPfgvTXTJiMTKTHcpL1oXUQXbMZTtdN/Jg4I6D5PdITF+uck2PMLBulhEYuk1dyE/WxeRt2YMoY3uPjlTe1bORrsusY3pgEsYr5gWia2jgi841bqIvM3gQMZodJgzjahz9L47OrY73GzOY3RXSIvE3ovuIuZYF5Gnegygm+5DOdGIOieD3GGxjenTmayYFkmEE4PxdLcuIk+LOZo3NULMiYI6B4PciTF9VWgjnqZv0FAxLZIQmweDgqeob11GXpZxLAMU1TnQ1HfW+rvTWGldRF4O43G1RYgk0A/ubN6zLiIvNXiOE3RXypJG1Fl63p0ay5jemId5M1BMiyTRNsE73E1N6zLysJKT6KtxYpY0os7KU+5ciq2LyMO+PM02CmmRRBvvTmWCdRF5qMYTnKH7UxY0os7C4+6cGMZ0Na7mXcW0SOK1DUZzOfG71Is5kwc1VsyCRtSVesRdFMNtLJvzNF3jd+WKSJ5edefG8BWjAfdwme5UldCIuhIPuQtjGNPHMU4xLZIqxwQTOdS6iJw5ruBujRcroaCu0L/dRcTtHKrDQ7wcNFZMi6TMZsHr3Ep16zJy5PgTd8XtNltgmvquwL3uCusSctaK59hVIS2SWqPcqXxrXUTO7uQvum9lpBF1Rv91V1qXkLOLGKOYFkm1PYMxnGhdRM6u5iGNGjPSiDqDJ9x5MXs2XY9HOEUhLSLAI+4yfrMuIicB/+UC3cHKpaAu11PunJjF9C701/ZxIrLOp64n31sXkZMinlRfdbk09V2O/rEbTZ/Op4ppESmlYzCaI6yLyEkJZ/Osxo7lUFBv4CV3Kqusi8hBTe6mb1BXMS0iZTQJBnE3NazLyEExveinqN6Apr7X84rrGat3em/NC+ypkBaRDN53pzDDuogc1OBFjtY9rQyNqMsY5E6KVUwfy+eKaRGpwH7BGLpYF5GDlfTUftXrUVCX8q7rEaN1kgFX8xINFNMiUqFmwTtcbV1EDlZwAu8rqkvR1Pc6o91BLLIuImv1eYrjFNIikqVn3B9YZl1E1jbmHTroDreGgnqNr1wXZlkXkbUdeYVWOolFJAdj3XH8aF1E1jbhA1rqLgdo6nuNae7gGMX0EYxSTItIjtoHn3KAdRFZm8PB/KCRJKCgBmC2O5ip1kVkKeBqXqOhYlpEcrZJMCRGT6t/4mB+UVSjqW9gvtufz62LyFJ9+nCsQlpEquBxdwkrrIvIUnve1ZJZBfVSdxjDrYvI0lYMpH3qT1kRqaqP3XHMtC4iS3szlLS/0CnlU98r3Ymxiem9+FQxLSIh2Dv4lHbWRWTpY45jRcpHlKkO6hJ3Om9aF5Gl03iPzRTTIhKKrYL3Ody6iCwN5RxKUh3VqQ7qP/OCdQlZCbiZvtRUTItIaDYOXovNwrJn+at1CaZS/Iz6X+4v1iVkpRaPc6pCWkQi8Ki7JCavTb6LK1N7H0xtUPdzp8ViK8steJWOqT09o3S7G2ddguSkHdfpSojAUNeT+dZFZKGI5zkxpWdASoP6XXd4LNoT2vEaW6X01IzaIHeUdQmSk9c4UtdCJCa7I/nOuogs1GIwXVN5DqQyqCe6LsyzLiILB/KyOggjtKcbbV2CZG13PiXQ1RCRX91RfGxdRBYa8AG7pvAsSOFisp/cEbGI6TN4UzEdqRutC5Ac3KKYjlCTYChHWheRhQUcwbQUji5TN6Je6LrwhXURWbicu3VjilwnN8q6BMmKxtPRK3aX8pB1EVlow/DUvUQ5ZSPq39zxMYjpajzAPYFuS9HTmDoueiumI1ct+G/wD/z/MU9I4QtQUjWidu5U+lkXUak6PMsx/l8tieBcJ/Sc2n8dGK2gLpD/uQtYZV1Epc7gqVSdEakaUfeOQUw3ZohiumCC4CbrEiQLN6fqpmzrnOBNNrYuolJ9uc26hIJK0Yj6WXc6vn/abRjMTrolFZTWfvtO4+lC+9QdwWzrIioR0I+eqTkrUhPUo93+LLUuohK7MITmqTn1fKF+at8N5ChdFQU2xR3CNOsiKlGLd9krJWdGSoL6B7eX95u6deBNmqbktPOL1n77TOu9bUx1h/CldRGV2JyRtEjFuZGKZ9SL3NHex/R+vKOYNqK13z7Tem8bLYKP6GRdRCV+4QgWpGKsmYKgLnanMt66iEocxZtsrNuRkSODPa1LkAzax+I1HMnUOBhGN+siKjGRUyhOQVSnIKivZJB1CZU4nZeorZg2pLXfvtJ42lK9YBDHWRdRiTeJxy6IVZP4Z9SPuT9Yl1CJS7iXIt2MjOk5tY/aM0ZBbazYnceT1kVU4kEuSvhZkvAR9VB3kXUJlbiJ+wPFtL2brQuQcmg8ba9a8DgXWhdRiSt4J+EjzkSPqH9wezDHuogK3cJNuhF5QmNq32g87Qvn/sTd1kVUqDGj2S7B50qCR9SL3dGex/StimmP6Dm1bzSe9kUQ3BVcb11EheZyPEsSPOpMbFA7d67Xa70D7uJ63YY80l1rv73SHr2Ixie3ev663c/phUtsVCc2qG/jBesSKhBwD1cqpj3j940obTSe9s0twT+sS6jQy/zTuoTIJPQZ9WDXnWLrIjIKuJ+LdRPykJ5T+0LPp/10p7vauoQKFPEaRyTyrElkUH/lOjHfuoiMqvEYZyXyZIq/N1x36xIEgFc5WteIl/7jrvJ4c6NGjGKHBJ45CQzqha4TU6yLyKgaT3J6Ak+kpNCY2gcaT/vsHneldQkVaM0n1EvcuZO4Z9TOneNxTAc8qJj2mvqpfaD9p312RXCXdQkVmMjpCVxUlrigvoWXrEvIKOBhztcNyGtHBL5vRJB87TnaugSp0JXB7dYlVOBVfK4uPwmb+n7bHertIrKA+7hEMe09Pae29grH6DrxXm93i3UJGRXxJock6hxKVFBPc7t7/IqTO/hrok6d5NrLjbQuIcX0fDoubnC3WZeQ0aaMYasEnUUJmvpe6U72OKZvV0zHhvqpLen5dFzcGlxjXUJGs+jBbwkahSYoqP/ER9YlZPQ3rtXNJzb0nNqOnk/Hyd+Dq6xLyOgT/P0akbvEBPXz7n7rEjK6iRsU07Gitd9WbtJ4Olbu5ALrEjK6m5cSM6ZOyDPqr9weLLQuIoNLuU+3ntjRc2oL7fhMQR0zzp3j7X7V9RjNLok4nxIxol7ijvc2pk/nHusSJA8aU1vQ8+n4CYJH8LVPYjE9WZqIsWgigvpiJlqXkMHRPEGRbj0xdLieUxdcO46xLkHyUCPoT1frIjIYzx+sSwhFAoL6IdfHuoQMDuR5qiumY6q3dQGpo/F0XNUOBtLeuogMnuWxBIypY/+MeoLbk2XWRZRrD96mvm48MdbZfWhdQoro+XS8zXZdPX15cy1GsmvMz6yYj6iXup6exvRODFJMx9z11gWkisbT8dY0GMrW1kWUazmnxv5JdcyD+nImW5dQruYMZVPddmLu8GBf6xJSQ8+n42+rYCibWRdRron42++dnVgH9QvucesSyrUJw2ihmE6AG6wLSA31TyfBjsFr1LUuolz/5eVYj6lj/Ix6mmvHXOsiylGbYeyjm05CdHEjrEtIAT2fTo433dGssi6iHA0ZyzaxPcdiO6Je5U72Mqar8YxiOkH0nLoQNJ5OjsODh6xLKNd8zmBVbMelsQ3qGz19s/fdHKdbToIcFnS2LiHx2uj5dKKcG/i5rc0I/mZdQt5iOvX9nuvm5b7T13GbYjph3nKHW5eQcC/ry23COHce/7MuohxFDOXAWJ5rsQzq2W43ZlgXUY5TeEZTeAmk59RRasPnentf4hS74xloXUQ5tmQcm8TwbIvh1LdzZ3sZ0wfzlGI6kbT2O0r/p5hOoGrBM+xuXUQ5forpK0VjOKJ+2F1oXUI52vE+G+uGk1D7uI+tS0io3Rirr7cJ9Yvbh++tiyjH45wTuzMudkH9nWvHIusiNtCMkTSP3R++ZEvPqaOi59NJNtntw3zrIjZQl3HsELOzLmZT3yXuLA9jujYDFNOJprXf0dB672RrGTxPdesiNrCEsyiO2Qg1ZkF9O8OtS9hAwON0UkwnnJ5TR+EWPZ9OuEOC/1qXUI4P+Zd1CTmK1dT3WLcXv1kXsYFbuV43mxTQ2u+wab13OlzqHrAuYQMbMYrdYnTuxSioV7g9GG9dxAZ60k+LYVJhsDvMuoSEeYnjdeWkQLE7lkHWRWygFWOoFZvzL0ZT39d4GNP7qCUrNQ7Vc+pQteFY6xKkIKoFz9LWuogNTOJG6xJyEJsR9XC3PyXWRaxna0aymWI6NTSmDpPG02nyg+vELOsi1lPEMA6IyTkYk6Be4HZlqnUR66nPR7SJyR+zhGNf5+cb5uNnV8ZpLipVRrhurLAuYj3b8HlM3n4Rk6nvP3oX0wF9FNOpc7N1AYlxs2I6ZToH91mXsIEf+LN1CVmKxYh6mDsE3+q8md660aRQV+dfg2D8tOYLrfdOoYucb1tgBrzJoTE4E2MQ1AtdW+/G00czQDeaVBriDrUuIQFe5ARdPSm00nXjA+si1rM146nv/dkYg6nvv3gX0zvTRzGdUocEXaxLiL3WHGddgpioEbzAVtZFrOdHrrUuIQveB/W77lHrEtZTn5dpoJhOLb2jrKr0PrL02ix4kZrWRaznQYZ5P7Hs+dT3Urcr31oXUUYRr3KkbjOppufUVaHn02nXx51pXcJ6tuUL6nl9Tno+or7as5iG3orp1NOYuio0nk67XsFF1iWs53vv+zm8HlH795KTY3lZbSVCZ/ehdQkxpf5pgZXuQPx6c341hrO3x+elx0G9zO3G19ZFlLEDn+rptKC13/nTem8BmOnaM8O6iDJ2ZpzH7/72eOr7Rs9iuhYvKKYFgEOCfa1LiKW2Wu8tAGwW9KWadRFlfMkt1iVUwNug/tzdY13Ceu6jvWJa1vD9mZafbtbzaVnjoMC3a+hfjPV2gtnTqe8Stw8jrYso4xSe1S1GStHa71xpvbeUVuKO5E3rIsrYg4+p5uUZ6umI+l7PYro1vnVzi7U4bZLnh96KaSmlKHiabayLKGM0D1uXkIGXI+qfXSsWWBdRSj1G0kq3GFmPxtS50HhaNjTKdeE36yJK2ZhJbOnhWerliPoSr2IaHlRMSzk0ps6FxtOyoT2DO61LKGMhf7IuoVwejqhfdcdal1DGRTyoG4yUS/3U2WrLOAW1lMO5HrxkXUQZr3OEd2eqd0G9xLXhB+siSmnDKGp798cmfhjqDrEuISb6c6KuIinXAtee762LKKUFE717oah3U983eBXTtXhWMS0ZHay9tLLSmuOtSxBvNQj6UcO6iFKm8jfrEjbgWVB/4e63LqGMe2irmJYK3GRdQCzo+bRUZM/Ar/Ue/2GcZ1PNXk19F7u9+NS6iFKO5yXdXqQSXZxfby32Txs+V1BLhYrdQbxvXUQpe/GhV+esVyPqh72K6a14xLoEiQHf3q/kn5u8uuWJj6oFz9DEuohSPuF/1iWU4dGIeq7bmTnWRaxTxDAO0O1FsrCf+8C6BI+pf1qyM8D5tJKhCV/SxJvz1qMR9XUexTTcqJiWLPn1fM03er+3ZOe44ELrEkr51atNOrwZUY91e1BsXcQ6+/Ie1XV7kSxpTJ2JxtOSveWuE19YF7FONcawmyfnricjaucu8SimN+ZpxbTkQGu/M9F4WrJXK+hLTesi1inmCpwnI1lPgroPH1uXUMrdbKObi+TgoKCrdQleas0J1iVIrOwa+DTh/D7PW5ewhhdT34vczsywLmKdoxiomJYcve26WZfgoRfooWtJclLiDvSoUWtLpnjxljIvRtQ3exTTm6gpS/KgMfWGWmk8LTkrCp6gvnUR6/zE7dYlAF4E9SSv3kb2IJt78P1J4kfPqden59OSj22Df1qXUMq/+dKDaWcPpr4Pc4OtS1inF0/p1iJ50trv0loxXkEteTrSvW5dwjqHMNj8PDYPap+a3JvzBQ3N/0gkrt5xB1mX4JHn6alrSfI0w7XlV+si1rHf+NI4qFe6Nnxl+xP4/UfBGxymW4tUgcbUa2k8LVXj0xBuF8YbN+waP6P+rzcxDRcrpqWK9N7vtfR8WqrmuOAU6xLWmcJjxhWYjqjnux28md7YmgleLMOXeNOYGjSeljD86loz07qINZryNQ0Mz2jTEfXfvInpgEcU0xICrf0GjaclDE2Ce61LWGc2d5ge33BE/b1ryQrTD/+7c3lMNxYJxf7On9c12NB4WsJygnvZuoQ1ajGFrc3OasMR9V+9ielm+NS3J/GmMbX2n5awPEAj6xLWWM51hkc3C+pP3EuGH7usB2mkG4uE5MBgP+sSTLWih3UJkhibB/+2LmGd5/jIbALaKKiduwrrDu61TuFYxbSEKN1jao2nJUxnB4dal7CG4yqz3bSMnlH3c74svd+EiWyqG4uEKr3PqfV8WsL2o2vLIusi1niJ403ObpMR9W/Ocra/rHsV0xK69PZTazwtYds6uM26hHWuZqXJ2NYkqB/me4vDlqM7p+i2IqE7IKV7aen5tEThEva2LmGNb4xefWIw9b3E7cAvJh92fbWZwHYKaolAOt/73Y+TdD1JBMa7Dqy0LgKAZnxDnYKf5QYj6rs8iWm4STEtEUnj2u+WGk9LRNoGV1iXsMYMLLZlLviIep7bnnkGH3RDrRlLDQW1RORdd6B1CQX2HCfrepKILHWt+cG6CAAa8i2NC3ymF3xE/XdPYjrgPsW0ROiAlI2pW9LTugRJsDrBXdYlrDGfwvd2F3hE/bPbkaUF/5Dl0UtDJWrpGlNrPC1RO8YNtC4BgLp8w+YFPdsLPKK+xZOYbsLfrUuQxEvTmFrjaYnefdSzLgGAJRS6YaygQf21e6LAHy+Tf9JU3/4lcunpp1b/tESvReDLW/8e5ruCTkYXdOr7FNevkJ8to858QKDbihRAOt5R1pIJCmopgFVuD8ZZFwFAL54q4BlfwKD+3O1OSeE+WUbV+Yy2uqlIQbznDrAuoQD0fFoKZbjbz4t9IqrxOa0LdtYXcOr7Zi9iGi5STEvB7J+C59R6Pi2F0yU42boEAIq5pYBHK9iIepzb3YvvQY35iiYKaimYEa6LdQkR03haCukntwuLrYsAAsaxa4HO/IKNqG/xIqbhdsW0FFTnYH/rEiKl8bQU1pbBNdYlAOAKuPa7QCPqCW43Lya+2/Ep1RTUUlDJHlM/q41tpMBWuLZ8bV0EUMS4Aj1ILdCI2pfn03crpqXgkjymbslJ1iVI6tQM7rQuAYASbi/QkQoyov7CtfNi4vsUnlVMi4GZbrp1CRHZjK10TYmBQ90Q6xKAIsbTqgBXQEGCuod7sQBHqUwdJrG1bioiIrE32e3mxcaXhRn+FWDqe4J7OfqDZOFaxbSISCK0DC61LgGAF5hcgNFuAUbUPV3/6D9HpbbkK4PtvkVEJArz3Q78al0EcBpPR54skY+oJ7mXoj5EVv6umBYRSYyGwQ3WJQDQjymRj3cjD+q/ebHeuz2nWZcgIiIhupjtrUsAigvQTx3x1Pd3bmdWRf4hKjeEgzWeFhFJlP7Oh9ftVONLto80YSIeUd/pRUwfoZgWEUmcHsG+1iUAxdwd8REiHVHPdNuyLOIPULlqjKONglpEJHE+cft48JaOOvxA0whTJtIR9d0exDScrZgWEUmkvYLjrUsAlnJfpL9/hCPqhW5r5kdafDZq85XenSQiklDfuVassC6CRvxI/ciSJsIR9UMexDRcpZgWEUms7YILrEsA5vFohL97ZCPqFW47fo6w8OxswrdsrKAWEUmsOW47FlkXwZZ8x0YRpU1kI+qnPIhpuEYxLSKSaJsEV1iXAPzEM5H93hGNqEvcLh7sF9qMb/Q+MhGRhFvgtmOudRHszCSKIkmciEbUL3kQ03CjYlpEJPEaBFdZlwB8ycCIfueIRtSd3KjofhpZ2pYpkT0xEBERfyxxO/CLdRHsw4fxGVF/6EFMQ2/FtIhIKtQNrrUuAfiIkZGMfSMJ6nui/VlkZWdOtS5BREQK5EK2tS4BuDeS3zWCoJ7qBkT8o8jGbVTXeFpEJCU2Cq63LgHoz/QIxtQRBPX9HmzE0QEfXisnIiKFcha7WJfASv4bwe8a+mKyJa458wrx86jQaxyp8bSISKo84063LoEmTA293yj0EfVTHsR0O7pblyAiIgV2MjtZl8CvPB367xlyUDt3f4F+GBW5iUDjaRGRlKkWXGNdAnAPLuSp6pCnvt9w9mPZ1nwR0dthRETEZyvdznxvXQSDOSTUDAp5RO1DY9ZNimkRkVSq4cmYOlyhjqgnuTZEt791dloyQUEtIpJSK92O/GhcQ8BEWoaYQ9XDLO5+85iG6xTTkhDfuxcKeLSebKsrRxKgRvBXd4lxDY4HCHO9Vogj6sVuSxYW/idSxvZM0YtOJCFudTcW8mhcrytHEmGF256fjGuoz0/UD+2KCvEZ9dPmMQ3XK6YlMd5M8NFEolPTg720FtEvxN8txBF1O/d54X8aZWzDV9RQUEsizHWbUlzA41VjFo119UgiLHXbMsu4ht0Z49+I+iPzmIY/KqYlMYYUNKahmKHWH1kkJHWCS61L4DNGhTYODi2oH7L5WZTSmHOsSxAJTeGnojX5LclxCfWsSwgxFUMK6l9df6Mfxe8upZ7G05IQzg0u+DHfpMS+bUMkFI0D+4FbP+aGdEWFFNRPsNzupwFALS4yrkAkPGOYWfBjzmKs9ccWCc1V1DCuYBl9Q/qdQglq5x41/GGsdjabazwtiWEzDa3Jb0mO5kEP6xJ4KKS3focS1EP5yvanQTX+aFyBSJgU1CJVdTXWo7cpvB/K7xNKUNsvJDuOHa3/RERCM8+NMjnuSH7VU2pJjF2DbtYlhJSOIQT1DPea8Y8C7NvbRcIzuMCtWWupRUuS5S/WBTCAWSF8+Q0hqPuwyvhHsR+dNJ6WBLGbgtbktyTJwUF74wp+4+kQfpcQgvop4x8Eej4tiVJi0Jq1llq0JFns0+GJEH6PKgf1R26y8Y9ha440rkAkTJ8ZtGatNVstWpIoJ7G5cQUT+LTKX36rHNRhfFuomsuopolvSRDb6WdNfkuSbBRcYF1CCClZxU05lrktmG/6I6jDNG0lIImyj/vY8uh8qOtJEmSG24bfTCtowAxqV+mqquKI+kXjmIYzFdOSKHONWrPWUouWJEsz8xefLODVKv4OVQxq+4lvvThUksWqNWsttWhJ0sR/QVmVgvoHF85bV/LXjbYaT0ui2D8jtq9AJEwdgr2MKxjG1CrNU1UpqJ+gxPjjX2Z8fJFwlbgh1iWoRUsSxzopSuhTpV9fhcVkzu3Ad6Yffhu+0YpvSZTRbk/rEoBP6aDrShJkpduWn0wr2JZvCfK+qqowon7HOKbhUsW0JMxb1gUA8IZ1ASKhqhGcb1zB93xQhV9dhaB+xviD16SXcQUiYfMjIvWUWpLmD+a7Uz9bhV+b99T3ctfMuDXrNJ7WeFoSZa7b1HjN92rVmEkTXV2SKMe6qjZJVU0jZlAzz6sq7xH16+Yd1PbvmxEJ11texLRatCSJrBNjHvm/wz/voH7O+EPvQmfjCkTC5s+Usz+ViITjULYxriD/1MwzqBc662dpF1ZhBZ2Ij3xozVpLLVqSNEXBucYVDGRxnldVnkH9IstMP3AtzjA9vkj4xjDLuoR1ZvOZdQkiITvPeEHZUl7J81fmGdTWE9899IZvSRzrWaqyNPktSbN5YL0lcr7JmVdQ/+LeNf641ssCRMLnVzT6VY1IGKy7qYcwM6/J77yCup/x2tSW7GN6fJHwzXWfWpdQxijtoiWJcyjbmx5/FS/m9evyCmrrie/ztZBMEseX1qy1ivFnaZtIOILgLOMK8kvPPIL6G+P9cmtwqunxRaLg31SzfxWJVFWvqu7tXEUf8UMeM1V51PyC6ceEI9hU42lJGJ9as9Z6Sy1akjgtgv1Nj+/ymvzOI6hfMv2YcKbx8UXC96lHrVlrqUVLksg6QfJJ0JyD+gc31vRDNuEI0+OLRMHPaWY/qxKpihOob3r8kUzNeaYq56B+EdvZsFPzfq25iL/8jEQ/qxKpirrBiabHd3m89iTnoNbEt0jY5njWmrWWWrQkiaxTJPcUzTGof3IjTT9gazpoPC2JM9iz1qy11KIlSdTVuJt6BDNy/AKcY1C/bDzxfZbp0UWi4e8Us7+VieQrCE43PX4JA3OtOLfg3d+9b/jxqjOVZhpRS8KUuGYervlerSm/UKRrThLmB7ed6aDzYIbkdFXlNKKe7UYYfjQ4SDEtCeRja9ZasxljXYJI6LYJ9jU9/ns5rv7IKagHGD9JO8X06CLR8GvXrPVp8luS6CTTo6/McfI7p6C2XfFdk2NNjy8SDb+j0O/qRPJzEtVNj59bmubwjHqBa8pKww92LAM08S2JM8dtRol1ERWoxi9soitPEudgN8zw6DWZTf2sr6scRtRvmca0Jr4lmQZ7HdNQzFDrEkQicLLp0VfkdF3lENSDTD9WXbqbHl8kGv5PLftfoUjuTqCm6fFfz+G/zTqoi91bph/qWOpq+k0Sp8T5P159U7toSQI1DA41Pf6gHK6rrIP6Y+aYfijbNXoi0RjtcWvWWnPUoiWJZJsqs8j+xcFZB3Uuw/TwNeIQ0+OLRCMe08rxqFIkN8dQ1/T42T9OzjqoXzP9QCdozyxJpHhEYDyqFMlN3eBI0+OHHtQ/uImmH+gE06OLRGO2p7tmrW80c/SUWhLIdsPLcfyU5XWVZVDbjqcbcKDp8UWi4Xtr1lraRUuS6QjqGB7dZT2mzjKobVuzjmQjTXxLAsVnSjk+lYpkr05wsOnxs137lVVQLzbdMwuOMz26SDRKTN+MlJu31KIliWSbLsNYmtV1lVVQD2OF4UepjW23m0g04tCatdacHFpJROLjKNN3fi/j3az+u6yC2nba6xDqaeJbEsjvXbPWp8lvSaLGwf6mxx+c1X+V5Yjakia+JZniFX3xqlYkW7YJk92bCbPYPetbt4Phx9DePZJMvu+atb4iZrCprkRJnJ9dc9Mr8UdaVHpdZTGitn0X8f6KaUmkt2IV01BiPLMmEo0tgj1Nj5/NdeV9UGviW5IpflPJ8atYJBv+T35XOvVd7DZhvuGHyGZaQCRuit3mxtvc5K4pv1Ckq1ES50u3i+HRN2FmpddVpSPq0aYx3VYxLYk0OnYxDbPVoiWJtHNguQ5rDuMq/W8qDWrbie8jTI8uEpV4TiPHs2qRyhxuevTKU1ZBLWIgnpEXz6pFKmObNJWnbCXPqBe5Jqw0K78Bs6mhqW9JnNlu85it+V5NLVqSTCvcJiw2O3pNfqVuhddVJSPq9wxjGg5TTEsixa01a60S4xk2kWjUDPY3PPoKRlTyX1QS1Ladk7bPDUSi8pZ1AXnT5LckU3fTo1f2BbiSqe/d3BdmpRfxM5tpRC2JU+w241frIvKUTSuJSPxMcy0Mj96R0flPfc9zE0xLV0xLEo2KbUxrFy1JquZBG8Ojj2VBhWPmCoN6uOmTNK34lmSK9/RxvKsXycQycYr5uMJ/X2FQf2BYuJ5QS1LF9wk1xG1zTpFs2SbO+xX+2wqfUe/h7Ka5GjKHapr6lsSJa2vWWmrRkmT6zTVmidnR9+HDCq6qCkbUi904s6LhAMW0JNKbsY5ptWhJUm0UdDY8+miWVjBqriCoR7DKsOyDDI8tEp34P+ON/ycQKY9l6qzkkwr+bQVBbfuEupvp0UWiUeziPx4dTEllm+6JxJBt6lSUuBUEdcUPt6O1JTtr4lsSKM6tWWvNYbR1CSIR2I2mhkevKHEzBvUyw4VkcLDhsUWik4xp42R8CpGyioIDDI/+CcszzlRlDOqP+c2wZD2hlmRKRsQl41OIrM8yeZZX8DKhjEFd2UvCoxRwoOHRRaIyy31mXUIoPmW2nlJLAvn6lLqCEbWdVmyhJ9SSQHHdNWt9JQyxLkEkAtsF2xoefWTGf5MhqJ0bZViuxtOSTMmZMtb7ySSZLMfUmRu0MgT1V8w1LHc/w2OLRCUJrVlrDaFYk9+SQF0Mjz2L7zJcVRmCOvMQvBD2MT26SDRGJqA1ay3toiXJZBnUmcfUHgb1TjTTE2pJoORMfCfv04istk3Q3PDomZI3Q1BX9DKzqFm+b1UkOsmKtmR9GpG1LBMop6Be5sYblmo79SASjdlurHUJofqUWXpKLQlkmUBjM7z0pNygHsNKw1I1opYkmmxdQMjUoiXJZBnUv1H+1/lyg9py4ntzdtATakmgrsEwtrQuIlSa/JYkak0Tw6OXn77lBrXlUrKuhscWidIBwTiOtC4iRG+pRUsSKAgs+47KT1/vRtR6Qi3JtUkwkIepbV1GSOaqRUsSyTKFsg7qX9x0wzL1hFqSLAjODz5kZ+syQqL3k0kSWQb1D+Uu0iwnqC23DahHW8OjixRC+2Asl1sXEQo9pZYk6mA661XecjLPgroj1bSUTBKvdnBP8CKNrMuosjFq0ZIEqhHsZnj0LIPastuzk+GxRQrphGAc+1oXUUVq0ZJk2tPw2DEI6j0Mjy1SWC2C97iZatZlVIkmvyWJfAvqYP2Zq/muMXazWVNprqlvSZV33Rn8ZF1E3hozS4+rJHG+cTuaHTtgPhuvd01tMKIeZxjTzRTTkjoHBOM4yrqIvM1ltHUJIqHb3vClJ44vNvhnGwS15VIyPaGWNNokeDXG3dWa/JbkCQLLx7AbTn4XVf6fFI6eUEs6BcH5wWjaWJeRFwW1JJHlsNHzoLZ8gC9iq3UwKpbd1WrRkiTyaznZekG9zH1pVlxAR7Nji9iLZ3d1CYOtSxAJXSfsFkxNYsV6X37XC+rxrDIrbicaaimZpNwJwRj2si4iR5r8luRpEmxrduzfmLjeP1kvqD83Kw3aGR5bxBfbBsO5tvzdcjw1RLtoSQK1Mzz2+km83v1gkmFp7QyPLeKP6sHtsdq7+le1aEkCWb5GdP0kXi+oJ2b9G4WvneGxRfwSr+5qTX5L8rQzPHYlQa0RtYgf4tRdraCW5GlneOz1k7jMK0QXuoZm7yXblJlaSiaynonuZCZYF1GpIn5mM12/kjCbuF+NjhywgPqlrqgyI+qJhq8PbW92ZBF/xaO7WrtoSRLtanZkx5Qyf79eUNtpZ3hsEX/VDu4JXvK+u1qT35I8/iwnKxPUkw3LsvyRiETBuQvd3FAmqY73vrtaLVqSPO0Mj1122KwRtUhEPuNh2vFBKAG2bTDc672rf2WUdQkiIfN0RG235rs2O5kdWyQabwLTOJBr3MoQwrp60DsY6nF3tSa/JWlasZHZsTMG9UI33ayoVtp8XhJndXQVcwfdmBbKuPqAYBxHWn+sCj+tSHJsFOxsduwfWVLqnlEqqCcbrvluZXZkkWjMc79PBn9AW54P5fLaJBjIw9Sx/nDl+IyZekotCbOL2ZFLyqz7LhPUdux+HCLRGFJmg5sFnMzFblkIURYE5wcfenjFaBctSR7LIWTpye9SQf2NYUkaUUvSvLHBP/kvHfgilFFnu+AzD7ur37IuQCRkLQ2P/W2pv/YkqC1/HCLhc25oOf90Mp24J5So9rG7+i21aEnCWCZT6UT2IqhrsJ3dT0MkAp8xo9x/vpwrOS607upxdLb+oKXMU4uWJMxOhi2RGYL625x/o7DsSA2t+ZZEqWgN9CuhdVe3CN71qrtaK78lWWoF25odu9yp7zluvllBekItSVNxZCW1u1pBLUljN/k9h3nr7hDrglpPqEXCUro1q3zF3EFXvg+tu9qPvavVoiVJ48dyMgW1SOjKtmZl8gntw+uuDp7yoLtaLVqSNH4sJ1sX1HZPqNVFLUmT7RRweN3V0MuL7mpNfkuyWD6Y9WxEvYPhsUXC5lwuuzMnq7t6sFq0JFEs08mroN6M+lrzLQmSqTUrkyR1V6tFS5KlcWB3PZUz9W0X1OqhlmTJffo3Sd3VmvyWZLFLqA2CeqGbY1bM9mZHFolCfq/SfIX2DA+tu/q6sjvYFtAbVf8tRDxil1C/rNtBa83V/GMqfwwi4ZvnRub5K6dyQGjd1bcFbxt1V6tFS5LFLqEcU9f81Zqgnmb4Y9DUtyRJdq1Z5Quzu3p/o+5qpxYtSRTLhFqbzGuCemrev1HVaUQtSVLVZ7Tx767WU2pJEsuEWi+oLUfUCmpJjtxas8q3gJPp5ZaG1F09irYF/hkMZpUmvyUxPBpR2wV1HTYz/DGIhGtsjq1ZmfSlY0jd1a2DkQXurlaLliRJc2qaHduboN6OQF3UkhjhTfvGubtak9+SHEXB1mbHXm8x2XSzQrSUTJIkzIhazpWcEFp39WfsHcufgog1u8ezZUbUztkFtd13FZGw5d+alcnLoe1dvU3wQcH2rv6MGXpKLYnRwuzIZYJ6FsvNCvFlJ12RqqtKa1Ym4e5dPawgV5yj6kvqRHyxldmRl/KrgzVBbbnmu7nhsUXCFc2Ubxy7qzX5LclhmVKr09k8qO2+q4iEK4zWrEzi1l09RC1akhiWKbV6OVkRWC4l09S3JEdYrVnlC3fv6o9oGenPYh5hP60XseLJiPpnsyICjaglMaKf7g1v7+rdgjERd1fntzWJiH8sU2r1l/8igJlmRWxKTXVRS0IU4rls2N3VjWP90xAphDpBdNdJZVans3FQazwtSRF+a1b5wt27emxke1erRUuSw27yexawrj3LioJakmJoBK1ZmbwSWnd1i+DdiLqr1aIlyWGXVF6MqNWcJUlR2KneOHRXa/JbksIuqUoF9WyzIrYwO7JImJwr9C7MxdxBN6Z53F2tFi1JCrvupHVT3/Od3XvJNjc7skiYom3NyuQD2nrcXa0WLUkKuz0el7LYQZHlxDdsanhskfBYTfOGu3f16JD3rtbktySDZVLNBIosl5IpqCUpLCMpvL2rW4W8d7WCWpLBg6DWiFqkagrVmpWJr93VY9WiJYlgmVSzUFCLhKCQrVnl87O7Wi1akgwejKjtpr7rU1vvJZME8GOK18fuaj9+MiJVUz+obXbsNUFt15zV1OzIIuEpfGtWJtM4kFtccUjd1UNDaEpRi5Ykg11azQaKYJ5ZAXZL3kXCM86kNat8xfSmc0h7Vx8QTKBHFX8PtWhJMthNfs9ndR91Cj+6SHh8m94Nb+/qhsELVe6u9u2nI5IPBbVIrPkXRT51V/v30xHJnV1azcM4qDcxO7JIWOa7T6xLKFdfOjExpO7qTzgv71+tFi1JArtn1PMxfkZtt8enSFiGmLdmZTKBjiF1V9cJHs27u9rhy1I7kfw1NDvyfIxH1A3MjiwSFp+ndsPuru6S16/0+Sckkp2GZkeeBxQtcytS+NFFwuGc76/0CLO7+p28uquHqkVLYq+h2ZGXssIVzTf86BpRS9yN42frEioV9t7VW+X4q9SiJfHX0PDYCyiye0KtEbXEXzymdYu5g64hdVfvH4ynZ46/Jh4/JZHMGhoeez6mI2rLjy4ShvhEUJjd1c/n2F39hvWHF6kiy/nfebZBralvibcFxrtm5VitWXf1OH7WU2qJtYaGx55P0QLDwyuoJd6GsNK6hByF2139hyz/W7VoSdw1NDz2fIoWmx28JrW0d5bEWnwmvn8XZnf1I8HLWXZXv2X9sUWqpG6wkdmxl1C01OzgjcyOLBIGf3bNyk2Y3dXHZdldrRYtibuNzY68lKJlKfzYImH4PAatWZm8QnuGh9ZdfUOl3dXz8PNFqyLZamh25KWWI+q6ZkcWCUO81zJP5YDQuqv/lkV3dRwfE4j8rmq7yFXFMssRdW2zI4uEIe7RU9ju6rj/tCTt7BJrmeWI2u77iUjVxas1K5PCdVerRUvizS6oTae+NaKWOItfa1b5CtVdrRYtiTe7oaWCWiRPSZrK7UtHvgipu3oUl2f4d0n6iUn6mI6o7Z5Ra+pb4iuurVmZTKZTSN3VtYJ7MnRXDyGMpWsiNkwXk2lELZK7OLdmlS/c7upx5XRXL9AuWhJjKX1GrRG1xFcyp3HD27u6efBuOXtXJ/OnJumQ0qlvjaglvpIaOeHtXV0t6B28vV53dVJ/apIGplPfy80OrqCWuPrNjbMuITLF3EE3poUyrt4vGMcxpf5eLVoSX6ZBvcrs4ApqiauNgo9oY11EhD6gHQNCidQmwQDup9aav3PanENiyy6xVlkGdQ2zI4tUVZtgNJeT3O3f5nJ8SN3VQXBJMIZd1/ydglriqrrZkYspKjY7eGWv8Rfx2eo2pCbWZUQozO7qkWu6q9WiJXFll1gKapEqODYYS1frIiIUfnf1Au2iJTFlN6JepaAWqYrmwTv8I8GPccLvrtbKb4knjahFYqtacHXwNs2ty4hQeHtXNw/eob31xxHJi+mI2m4xmYJakqJLMJ6TrYuIUJh7V/dI7go8SbQisyNrRC0SigbBc8FT1LUuIzJh7l0tEkea+hZJgF7BaHazLiJC4e1dLRI/as8SSYSWwSeJ7q4Oc+9qkXixSyyt+hYJlbqrRZJJU98iCaLuapHkMQ1q6w8vkjzqrhaR8BRZfksQSapqwdXBcLa1LiNC4e1dLRIHlrPPCmqRiHQKxia6uzq8vatF/KegFkkkdVeLJIVdYlVXUItES93VIklg+RZPBbVIxFZ3VyeXuqslDUynvi3ftiKSDrWCe4IBNLYuI0Lqrpak09S3SOIdG4xTd7VIbGkxmUgKqLtaJL4U1CKpUC24OvhA3dUiMWS3mKy6nlGLZOcuNzOUANor+JTjrD9MhKZxILe4YoW1JEyJ2ZFNR9QrzY4skqvf3M3sxuBQ4qdx8HLwILWtP1JkiunNgUxTVEui2CWW6arvZWZHFsnVCBYxk8P5i/stlAC6KBhFa+sPFaEPaMcARbUkiF1iVaeoVgo/tkiu3gTA8S/24etQAqhN8Gmiu6vncry6qyVBlpoduTZFdhNwdh9bJFdvrvurMXSgbyjxo+5qkfgwDeo6ZgfXiFriYrqbWOrvFtGLnm5+KAGk7mqReLBLrDoKapHKvbHBP+lPez4OJYDUXS0SBykNak19S1y8Wc4/+4Gu9HYlIQSQuqtF/GeXWHUsn1FrRC3xsNK9W+4/X8UtHMKMkLqrx3KS9QeNkPaulrizSyzTZ9QaUUs8jGBBxn/3NrvxRijx0yDoFzyF3fUYNe1dLfFmOqLWM2qRir1Z4b+dzZFcEVJ3da/gU3a1/rgR0t7VEl+mz6g19S1SsTcr+feOe0Prrm4ZjEx0d7X2rpa4SunU9xKzI4tkr2xrViZj6MDToXVXv0Qj648dob50YqKiWmImpS88mW92ZJHsvUl2mbKIM0Lrrj4++DzR3dUT6KjuaomZeWZHNn1GPd/syCLZq2ziuzR1V2dL3dUSNwvNjlyHonpmB/+NZbpMxXMr3ds5/fc/sD//cS6k7uphNLf+AUToFdozXPcAiYUlzm73rLoUNTT86Auq/luIROqjnL9H/8afQ+uu7hqMT3R39VQOUHe1xILdxDc0sg3q+YbHFslGLhPfvxtGO3VXZ0Xd1RIP8w2P3Ygiy9WlGlGL7/ILapjFUVwd0lixVzAy0XtXf8IJ1iWIVMLuCTU00IhaJLMZbnzev7aEO9mHb7R3dRbG8ZPG1OI146lvjahFMnkjy9asTD5l9xC7q5O7d7VjsHUJIhWab3jsBhTVDGql8qOLVC7fie/fLeIMernF2ru6ElX/SYtEab7ZkeuxUVAEDVP40UUqtyrH1qxM+tKWT9RdXaEhaO23+Gy+2ZEbAkUYvqzQ7qOLVO6j0M7QH9hP3dUVWsjH1iWIVMDuQW0DwHREPdvsyCKVC3M6Vt3VldHkt/hsltmRG2Mc1HYfXaRyYUfHMNrxprqrM1BQi8/s0qohCmqRDGa4L0L/PWfRXXtXZ/CFWrTEY3bzvw0xfkatoBZ/ZbtrVm4c99KFb0Pau/oTLijwTyU6jresSxDJyHxE3TSFH12kMtFNxY6ifUjd1bWDhxLUXa3Jb/GX3Yh6U6AINjMrYDFLNdklXlrlhkX4u6u7ujxD1aIlnlrolpkde01Qb2r48TWmFj99HHnzYF92VXd1GWrREl9ZJtVmGI+oFdTiq0JMw35PV3q7kpC6qz9g2wLUHC1NfoufUh7UMw2PLZJZYSJjJbdwaEjd1XsFY2PfXf2GdQEi5bIMak19i5TrF/d5wY6l7urffcE0PaUWD3kwom5guC2Hglp8FE1rViazOJJrtHc1gHbREi/ZrfmuQ90AisByTD3d7MgimRX6WWkJd2jvagD1UouXppkdeXNgTVDbPaVWUIt/ikPaNSs32rsa1KIlfrJLqtXDaAW1yHo+Zq7JcdVdDQv5yLoEkQ3YjahXp7Px1LfdxxfJxLJJqC8d+CzV3dVq0RL/eBHUdiPqOSzXRJd4xjYqvmKvVHdXK6jFN0vdPLNjl5r6bmZWhOMns2OLlOcXN864gpXcwlHMDqm7+lOONf48uRnPdH15F69YPqIttZisRUp/BCIbequgrVmZvEGbkLqrGwcDYtVdrV20xDdTDY+9NbAmqJsblqGn1OIXX6Ze07t3tS9/AiKrWQ4nV6ezeVBrRC0+KY5016zcOO6lc0jd1S2DkbHprh5GOF9PRMJhOZwsFdSbUNusDAW1+OQTo9asTEbTgWdS1l2tFi3xi11K1aVxAGuCOgi2MivEcvZfZH3+PR9dyOmc45aE1F09ls7WHygLmvwWn9iNqNfOdheV/dvC+97syCIb8nP/pifowNhQorpF8B43U836A1VCQS0++c7syGsXepsH9Xc4PY8ST8wyb83K5Es6hdZd3TsY4Xl39Xim6q4gnihxP5gd25sR9VJ+MTu2SFlvUmJdQkYruYXD+CUle1drFy3xxXRWmB3bm6CGbw2PLVKaf0+oyxrKbryVir2rNfktvrBMKAW1yHqK3VDrEio1iyNS0V2tFi3xhWVCefOM2vJBvUhpo/jVuoQspKO7ehEfWpcgAtgm1HojasuXiGpELX6Iz3TraDrSL7Tu6hdpZP2ByhGfPw1JNrugDljbOL0mqDcOmqbwxyBSWpyiYQGnhLZ39QnB5x7uXR2nPw1JMruhZDPqBqv/qmjtP9ohhT8Gkd/Ndp9Zl5CjvnQMqbu6efCOd93VE9SiJV6wS6jfU9mDoJ7FQl2QYu4tj1uzMvmSvbkjsd3Vvq/BlzSYb7gX9fbr/sqDoIZvDI8tslo8p1pXcA3HJHTv6nj+iUiyfGV47HKCevu8fqNwTDE8tghAsRtiXULeBtEmpO5qv/auHsYKzbWJscmGx/Zq6tv2RyECMDoWrVmZJLO7erF20RJzngW15YhaQS3W4j7Nurq7+tuEdVfH/U9F4s8yncqZ+t4ksOulVFCLtSREwmh2T9je1X7uZSZpMsnsyJvQMFj710W//2O7MfXXrNSzKDE0x42xLiEUCzk9tO7qY4Nx5t3VE/lRdwYxtMJw56zSj6OLyv/HhbVSvdRiKo6tWZkkq7tau2iJpa9YZXZs74Jak99iKwkT379LUnd1sv5kJG7sJr7LznF7EtSWPw5Ju5IYt2aVb3V39ZwEdFdrFy2xZNk6nGFEvYthSeqkFjujmWNdQgSS0V29mBEmxxUB2yFkq1J/XVT6Hwc5/1Zh0Yha7CT1VZUz6c71blVI3dWf0NrkU2jlt9ixeyhbxM5l/m6d+oHdrtSTCed2IpK75C5YKuF2uvJDKNdW22A0Fxh8Bj2lFiu/uS/Njr3Nup2zoExQlx1qF9Yy0zeqSrrdy47WJUToY3bj2VCiunbwkEF39SS1aImRSfxmduyyaexJUMM4w2NLunUMxnCGdRERWshpse6uTuqjCfHdOMNjl33Q5E1Qf254bEm7+kGf4AUaWpcRoTh3V2vyW2xYppJG1CLl6BGMZW/rIiL0JXtzj3Mx7K7WLlpiw9Ogbm247nus2ZFFVtsm+ICby14SibKCKzmMX0Lqrh7LSQWqe4latMTEF2ZHDtZrly5zV9o42NKssNn8rG/NYqx60DsYTDPrMiI0hHYMDuVKaxD0K1h3tSa/pfCmOrutb7ehXplR83rDBz2llrTrFozjCOsiIjSTw7nChbMNTqH2rlZQS+GNMzz2+km8XlDbvNJgtXGGxxb53abBIO5mI+syIhO/vasnhdQLLpK9cYbHriSoNaIWgSC4IhhhuPFr9EbRkRdC27v6RaLezV4tWlJodk+oKw3q3QxL03Iy8ckewWecZl1EhOZzEn9wS0MJ6xOCcXSOtFpNfkuhjTM89vpJHJS9Tle4+qw0Ki3gVxrZLTsXKUd/dz7zrYuI0C48R7tQrrpi9zdupTiiOuvyKzV1d5CC+dU1xep5y0YsYqOKFpPVDOz20HJ8anZskfIlvbt6CnvFort6CcML+nORtBtpFtPQZr2YZsOm0fZmxcFIw2OLlE/d1dmLsrtak99SSKMMj71hCnsV1KMNjy2Sibqrsxddd7WCWgpJQZ3RJ4bHFqmIuquzF0139WS+U4uWFIhzlsPGrILabsXGLKbqUhRPqbs6e9F0Vyd333DxzbfMMTt2UTlfczcI6o2D7cwK1FNq8VkQXBF8mOi9q0exO8+F1l0d9t7VmvyWQrGc+N5xvdeHAuWtkdFTapFMkr939an0cku83Lv6He2iJQViOWQsL4E9C2rL7zEi2UjH3tXjPNy7Wi1aUih+LSXzLqg/ZZW+M4v31F2drXC7qzX5LYWw0lm+0DoGQb2E8YZHF8nWNsF7/Mlw4WXUVnAlx/JrSN3Vozk2lKresPyRSGp8xjLDo5f3Iu9ygnrzoIVhkdoiXuJho+DfwZBEd1cPpHVI3dVNggGhdFdPUYuWFIDlI5Zt2bSc7//lvnCpk2GZegol0Rvh5oVyw+8WfMbB1h8mQjPpzo0unMdRvYJPQtidT7toSfQsh4vlp2+5Qb2XYZkKaonebezGB6HEz+bB4ER3VxdzK/uGNI5tG4ypcne1glqi5txHhkcvP329G1H/wjea3JJILXMfMI0D6e2KQzjX0tBd3T7E7uqXq9Rd/Q7LdX+QSE1ituHRcwjqDqYjBI2pJVrvsRQo5hY6830ot/2OwRhOt/5YEQqzu/q4YBxd8v7VatGSqFmeYTVpV+4/LzeoawXhv6k3e1pOJtH6vcnnE9rzfCjxUz/oG7xAA+uPFqEwu6vfrUJ3tVq0JFqWCbR7hl3XM+zep6fUklylb/ULOJlebmkoAdQjGGd65URtCnt70F2toJZoWSZQpvtHhqC2fEr9NTP0FEoi87X7Zr1/0peOfBHKObdNMDzRe1cvD7W7Or+9q6eEtHGISHmmu6mGR8+UvB4GNXxoenRJtvJGZJPpxD2h3P6Tv3f1QFozxHTvau2iJdF53/ToOQb1DjQxLNb2RyXJVv7U6XKu5AR1V2dlJkdwk2F3td5PJtH5wPDYm7FNhpcdZgjqILAcU79jeGxJtmUu84X4srqrs1TM3wy7q99Ti5ZE5m3DY2fePyDj4zTLRTGT+FkXokRidWtWJtM4kFtC664ezvbWHzdCo+jIi6F1V79Io6z/+yWmox5Jsh/ct4ZHzzw8zhjUnQ3Ltf1WI0lW2bRpMb3ZN6Tu6j2DsYnurp5Hj9C6q08IPs+hu1orvyUaw0yPnvkKyBjUe1PLsGAFtUQjm1dQjqQ9/dRdnRWb7moFtUTDMnlq0zHjv8sY1LWCzL8oekMNjy3JtWFrVvkWcEpoY0V1V2cr++7qL9WiJRFw7l3Do++d4WUnQEUtn10NS/6ZKboQJXS5rBfuyx58ru7qLCznSo4rcHe1NueQ8H3OTMOjV5S4nga19bMCSabcpkwns1dIY8Xkd1e/SjveL2B3tSa/JXy2j1zzDOp9qWFYtJ5SS9gqas0q33Ku5PiQxordgnEcbv0jiNB0DuAKtzKk7urRVLzfwDss05ybhMwydTaq8DVjFQR1vaC9YdnvEUaTjMjv3mVZHr/qFdqF1F29afB6orurHffSJaTu6lbByAq7q5epRUtC9puzfMv3ntQJMv/bCh+cWU5+z+dTw6NLEuU7XTo91L2rk91dPZKOvFSQ7mpNfku4PmGx4dErbk70Nqh1IUrY8j+jirkltDdxJb+7+sSCdFfr/iDhsj2j9qvw31YS1PnuGRsGvdFXwvRVFd85NJLdQ+yufop61j+QCPWlY0gr5jN3V3+lFi0JlWXiVGefCv99hUHdIGhjWPoYZupClNBU/ftymN3VvYLxCe+uDmvFfObuao2pJTzT3XjDo7enflDRv6+kuXN/w9JLdCFKiMI5m/rSiQkhdVe/z58Iqv4beWr1ivm5IXVXj+aYDf6p7g8SnjewHBfuV8m/rySouxmWrgtRwpN7a1YmE9kjpLHiRsG/gyGJ7q5+hd1C6q5uEryyQXf1u2rRktDYPmqtbFvcoOIzfYlrwgqz4hswmxrJHXJIAb3ujgz19zuGx2kSyrk5y52V6K+kRVzKv0K6jie5U/ii1N+/xaG6P0gIVrimLDI7ei3mUrsqU991TfelXsDHhkeXJAk7CsN7E1fSu6tLIuyuTvIXHCmk9w1jGjpXEtOVBnXlQ/JoaeW3hCP8d0NP58CQ3sQVBFcEH7KDwU+lUMJbMV8ruCd4mcZr/k73BwmH7Ve+ylPW86B+3fTokhRfRrIdfJhjxY7BZ4nurg5zxfxxwbg13dVf842eUksIbJPmkEr/i0qDumMF7waK3gR+1IUoVRbdXkvhvYmrftA3eDLh3dVhrZhvHrzDjVTDeiQkyfCN+9rw6JtW8l57yCKoqwUHGn4EGGh6dEmGKKdIw3wT15kJ764Ob8V89eD/ghFsq+0uJQSvmB69G0WVLonMYpNc28nvAaZHlyRYFvnL9vvSkXHauzoLYe9d3ZSlmnOTKrJNmWwSNqj8LP/OWW4iUJ1fQmqDkbQKuzWrfLX4NxeHdKa+6c5iVgFqttKcZ+gS0s9qpVMLp1TFL25LSgyPP42twhhRbxdYBvUqTX5LFRXmOeZyLuFYF85Y8fBgfKL3rp7GAVwT0t7VimmpmgGmMd0qi5jOKqg1+S3xVrjnmK/SmiHqrs5CMXfQhe81bS3m/J/4zjKoDzP9IENZrMtZ8hZNa1YmMzmCm9wq7V2dhZF05GVd22JqvnvP9PiHZvVfZRXU3ahl+EGWqwVDqqDQZ08xf9Pe1VmaywmhrZgXycdAVhoevS4HZPXfZRXUdYPsfrOoaPJb8mfxNW8U7XlOe1dnpS97hLR3tUjurCe+a2W1xiLLPpDuph9mECt0IUteloS2a1ZuFnKq9q7O0uTQ9q4Wyc1SN9T0+Nkma5ZBfZTpvrmLeMfw6BJn77Lc7Njqrs7W6r2rw1kxL5K9N1liePQg696OLK/9FkFbw48DL5oeXeLLdn3DFPYO7U1cvYPBCd+7uj0fKKqloGyTpQNbZjkCzvpLeiFeGZHZy5r8lrwMNj7+cq7k2JDGit2CcQnvrj4wtO5qkcotdYNMj5/9I+Wsg9r2KfV88xuuxNGUgrZmZTKQ1gxWd3UWirmDruqulgIZyGLT42c//M06qPdiM9OP1M/06BJPvjT2zeRw7V2dpU9oz/OKaikA21RpRoes/9usg7oosH3tyauo21Jy5UtQg+NeOvOt9q7OwgJOppfTZhsSrYXOdp62O0HWa7RzWEhqO/m9FNunCRI/SyLfNSs3o9hd3dVZ6ktHvlBUS4ReMuwIgdwSNYegPtT42ZgmvyU3lq1Z5VvIqZwf0lixVzAmh6mz+JnMXjysqJbIPG969Jp0y+G/ziGoNw4ONP1gbzJfl63k4A3rAsr1KB1C6q7eKfgk0d3Vy7iQ49xcXfUSgdnubdPjH0q9HF5OktNVfoLpB1vBK6bHl7jxtVNgSmhv4kpDd3U7dVdLBPqzyvT4uaVpTkF9LNVNP5omvyV7U9x31iVktIIrOZY5IXVXf2a8EW20pnEg/+eKFdYSqhdMj14jxzeT5BTUmwRdTT/c28zQ5SpZ8nPi+3cDaRNSd/XmweCEd1ffTGd1V0uIfjReaHogjXN6K3eOD7hsJ79X8Yzp8SVO/GnNykTd1dlTd7WEqQ8lpsfPNUmD3M79X9yWph+wNRMsdweR2FjimrDCuois7MmzbB/KWb3IXczT1h8nUmfwEHV0B5Aq28V9aXj0avzMplGOqDcP9jb8eDCRMfpWLVl4JyYxvbq7+ll1V2dF3dUShhGmMQ1dc4zpnIPaevIbnjI+vsSD/xPfv1vIafRyi7V3dRYm04l7FNVSJdYpknuK5jj1DdPc1lheJ034iZqa/JJKbO/xmu/y7cxztA/lzF7lbuVvxs/gonUsj+e4GEdkrWVuC+YbHr+IaWwR9Yi6edDR8CPCr96v5hV7k2MX0/Ale3OHK1F3dRZeoR3DNa6WvAwwjWnYK+eYziOoNfkt/ovTxPfvVnANh/OL9q7OwjQOoLe6qyUP1gmST4LmPPUN37odTSe/azA950fxki6HuKHWJeRtC/pyYCjnd4n7Fzew0voDRWg/nmYr3QskB9PdNhQbHj/gB1oUYkS9fdDJ8GPCSp41Pb74bon7wLqEKviZg7nerQrhu3BR8NdgBNtbf6AIvU87XtGoWnLwtGlMQ+c8YjqvoIZTTD8oPEIY70mWpHo7Nq1Z5SvhdvYJae/qPYOxid67+leO097VkjXn/mdcQX7pmVdQn2z8zu/JfGR6fPFbPJ9QlzWa3XlG3dVZ6cse6q6WrLzN16bHr57nGq+8gnpT4w0v4WHj44vPhlgXEIqFnK7u6ixNUne1ZOUR4+Mfmuf6qjw3s7We/O6PdqmV8sWxNSuTvnRkbChn+jbB8ETvXb2cKzlee1dLhWa7gcYVnJrnr8vzyj2e2qYfdzl9TY8v/kpWn726q7M3QN3VUqHHjVev1OHoPH9lnkG9cdDd9APDQ1pQJuVKwhPq0lZwDYepuzor6q6WzOwXkh1LvTybCfOeC8t3CB+WKdjuJyp+WuxGWJcQgaF04J1Q4mfTYBB3UMP6A0WmmFs4iN8U1bIB64VkcHLevzLvoO5OY+MP/ajx8cVH8dk1Kzc/040rXBgBVBT8Nfgo0XtXv6+uECmH9RLkxhya96/NO6g3Co4z/tj9ma3vzbKepE18/85xL535JpRzvmPwWaK7q5N7Fki+ZrpXjSvoyUZ5v0WvCstATzP+2Cu0oEw2kOxb9Gg60i+07uonE9tdneyzQPLxhPnLdKvyuDiPd32v5dwO2DbCbMM3VNObfmWdSa61dQkFcAYP5r0opawf3Cl8Yv1xIjGV5rozyDqr3HZMM61gW74lsBhRB8EZph8cfuB14wrEL+kYSam7unLJeOmNhGWAcUzDuVWI6SoFNZxlfonfZ3x88Us6ghq+pBO91V1dgbScCZKde42PX0SvKv36Kkx9Axzk3jH+AXxBW01xCQBLXJOErvku38H0YfNQzv5Z7qyERdvGzKGG7gwCwFi3u3EFh/JWlc7GKg6Jzzb++PCgdQHijWGpimkYym68FVJ39evczUbWHyhEC/nYugTxxj3WBVQ5KasY1CfQ0PgH0Edv/ZY1kjUmzMYsjgipuzoIrgg+TFR39VvWBYgnZrvnjStozDFV/B2qGNS1g57GP4KlWL8WTnyRxluzuqszSd/XNinfQyw3ruBUalXxMUwVn1HDJ25v4x/C1nyrJi1homtjXYKZjXmA00O6Bvq4S1hs/YFCEDCdLXRfSL2VbjumG9cwht2reCZWedn2XkFL4x/Cj7xmXIH4IM0jqIWcob2r1+PUoiVAf/OYblPlmA4hqH1YUHa3dQHigTROfJcWZnf1+/yJ+A9G0/zVTdayX0h2bgi/R5WnvuEX18L85Wyf0Cn+9xWpgsWuCb9ZF2GuJv/giiq9WOF3b7qzmGX9gaqkEbOorvtCqr3nDjCuYCOm09SHEfXmwVHGPwr4p3UBYuxtxTSwgj9yCDNCGVcfHoyP+d7V8xhpXYIYu9O6AI4PIaZDCWq4yPpnwQC+VpNWqmmac61htONNdVcDehySdhOc/RlwYSi/SyhBfRA7mf4ooIT/GFcgtuwvSH/Moru6qwF9fUu7f2I9fmtJ11B+n1CCOgjON/1hADzBL9Z/JmJmovvRugSvOO5lX3VX8xkzdVdIrZ9cP+sSuDCkFSMh7apxDnUsfxrACr1MNMU0ctrQp+zO06HtXf1ULPeuVotWmt1lvm6lNmHtMBlSUDcKTjT7Yaz1AOH0kUr8KKjLs0jd1TozUmuhe8y6BE6hUUhdB6HtUxnOI/OqmKuXiabUYvehdQne6suufJLivasHU6yv76n0IAusSwgxFUO77vYO2ln8JMq4i5W6KFMobbtm5eZ79ucu50Lau3oQm1p/oJzMZbR1CWJghbvPugQ6sEdoXfwhfkG2H1P/wDPWJYgBTW9WbAV/SnF3tfoB0uhxfrYuIdREDOHNZGstdluysPA/jTK2Z4reRZQ622jNdxY25UkOD+XacO5e/mq+UCdbezJSd4SUWel24gfjGhrwE3V9HFHXC+ybOL7FeudRKbSZzvql+/EwiyO52oXxcCgIrgiGs731B8rSBMLoKJc4edI8puGMEGM61KCGKzxYanIbJbosU2Wz4A7rEmKihDvZJ6Tu6j2DsbHorg54hI00ok6VYmf/SumAS0L9/UJN1p2CQwr6wyjPZF60LkEK7M8evHAnLtLWXX0dpymmU6YvX1uXQHd2CfW8C/EZNcBbzn6hSWu+oEgXZ6qsdIfyrnURMXIGD1IvlGvkB3cKn1h/nIyO40XdC1Km2LXmS+siGEq3UM+7kOeqD6VlIX8a5ZrIQOsSpMBqBP1j/EbqwutLWz5OfHd1O/oqplOnnwcx3ZqDQv4dQ77CguDygv0wMvs/wugalThpEgykgXURMfIDXentwljPUT3oHQymmfUH2sDmDAx1OY/EQYn7h3UJwJUhveH7dyFPfcNS15y5BfuBZPIaR+oSTZ033NEUWxcRK4fxFJuGcqX84nox1PrjlFKL9+ike0DqvOBOsi6BTZhK7ZDPvdDnrOp4sbCnt8bUKXREYL9NfLy8RVveCOVK2TwY7NHe1QGPKaZTqNjdYl0CcGHoMR1BUMOl1CjET6NCY3jZugQx8KfgAusSYmYWRyZw7+rrtdY7lZ5hknUJ1CCKe1DoU98AJzv7147szAS9oyyFtP47Hx15lh1DuVoWuYt52vjTHE9/LSJLoZWuJd9aF8Hp9I3g3ItkueYVUf8ssvAlz1mXIAa0/jsfn9KBvgnprm5HH8V0Kj3uQUzDZZH8rpGMqGFvZ99buR1TqKELNoUmub3N3zofR724n/qhXDFfuVP4zOQzNGMUW+mqT6Hlbgd+si6CfRkRydkXUQPkX6L8WWTpOx63LkFMtApeoLp1ETHUh11D6q7eKRhp0l1di5cV0yn1oAcxDVdH9PtGNKJ2ro0Hj/Wb8Q11dNmm0l3uT9YlxFJ1ruemkKaOh7lezChg7QF9tYgspRa77ZllXQQtmRDRY5eIvvQGgQ+3yRk8ZF2CGPmj1n/nZRW3hLZ3dbdgXEH3rr5BMZ1ad3kQ03BNZKsjIhpRw0q3PdMi+4FkaxO+ZWNdvKmk9d/5a8qTHBGzvau11ju95rvtmGddBFvxbWQ7tUX2GKmGFy8TncO/rUsQIzWC/rHZM9k3szmSv4TWXV2Ivat313u9U+x2D2Ia/hzhhqqRjahhiduaXyP73bNVm6+0vCS1pri9mW9dRGx14LmYdFdvziia6ypPqR9cS5ZbF0FjfgxpR7ryRLgws25wYXS/edaW4cNL5cTGLkE/rf/O2xg68EgMuqtrMUAxnWLXeRDTcFmEMR3piBpmuW1YFuHvn51qjKWtLuPUutv90bqEWOvBIzQM5fr53p0awd7VAU9zqq7v1BrrOlJiXQR1+JFNIjwLI2113DQ4O8rfPkvFkfW2SRxc6cXMTnz1p31I3dXbBh9E0F19o2I61a7yIKbhD5HGdMQjavje7cSqSI+QnSEcrIs5tVa6w3jHuohY87e7Wmu90+1Vd6x1CUANvmbrOAc1nOaejfgI2WjPp7qcU+xX18mL9wDH2aE8xWae7V29O8P1SqMUW+XaMsW6CKAXT0V8Fkb+lr8bDF4kuKGx5jv6iKUmwSAaWhcRc4Npy+te7V29Oa8oplPtES9iuhrXRX6MyFO0ZdAz8g+RjetYGvXkgXhsl+B5rf+uotkc5dHe1bV4RWu9U22R+5t1CQCcys6Rn4cFGO7e6MWY+if+Y12CmDok0MtvqspxL/vydShfeTsGYzg1718d8ASdFNOpdhu/WJcAVOP6AhylABnaKjixAB+kcrfzo8bUqXa51n+H4NPQuqs3Dp7Ju7v6Rk5WTKfat+5u6xIAOLkA4+kCLCYDmOTaerGE/hSe1cWdaivd4bxtXUQi2HZXa623HOUGWZcAVGMCuxTgTCzIrHSr4IRCHKZSz/G+xtSpViN4gR2ti0iE/rTnI6Pu6vb0UUyn3FAvYhpOKkhMF2hEDRPdrl6MqdvxKdV0iafal24vvf87FDbd1c0YqUVkKfeb25UvrYsAivicNgU5Fwu0zqt1cFxhDlSJcTxmXYIY2zl4Qeu/Q7GKWziYnwu6d7Xe6y1wrxcxDScVKKYLNqL2Z0zdmK9oogs95e51V1iXkBhNeYLuBdq7Wu/1FpjpdmaBdREUcjxdsBE1tA6OLdShKjRXu2kJlwcXWZeQGKu7q1cUpLv6JsW0cI0XMQ09CxbTBRxRw3jXzosxdXXGFvAHLH7S+7/DFd7e1QvdRZT/2uEePE+gKzflRrm9vciRaoynZcHOxgK+i6RtkP/rDcK0iotxWv2dcjWC/lr/HaIx7MY9kXZXt+cJxXTqlbgrvIhpOLOAMV3QETX84Hau8AlU4TzBWbrkU+9LtzfzrItIlBN5NKLu6maMYitds6l3n7vcugQANmIK2xbwfCzo2z23Cc4r5OEqcBVzNKZOvZ31/u+QvUi7SLqrazFAMS384m6yLmGNiwsa0wUOariBuoU9YAa/cq11CeKBgwO9AT5cP7IfvV1JCGFdI+gdDKYZEPA/vddbgCs9eQNCPa4p8BELHNTNgssK/AEzeZz3NKYWLtP675CF3119E6copoUh7nnrEtb4U0g7s2evoM+oAea57T15LtiGz6ihG0Dq6f3fUdiUJzk8lKurxAVaRCYsc7vyjXURADThWxoU+Iws+A6UjYK/FvqQGUzQxpfC6vXfO1kXkTiz6B5Sd3VRoJgWuN2TmIZrCx7TBiNqWOZ25KeCH7U8dZhQ4CUB4iet/47G7jzHTrrCpMq+cruywroIALbga+oU/Jwu+IgaagfXFf6g5VqKH0v9xZrWf0fjMzrwlNaCSBU5d6EnMQ03G8S0yYgaVrqWfGtw3PI8pw3oBYAH3KXWJSTUqfyXjXWVSd4ed7409u7ERKqnJajhRdfD5Lgb2oSJbKqbiACXuAetS0iorXmGfXWVSV5muNbePJh6hWNMzmODqW+AE4PONgfewBz+aF2CeOIeulmXkFA/sn9I3dWSPhd7E9P7GcW02YgaRrm98OW6HcCx+rYvwDy3F19ZF5FYB9KXLXSlSU6ec37sEQFFjKRj2oIaTnH9zI5dVjMm0kg3EAG+c52YY11EYm3CExypK02y9qtrxSzrItboxVNm567R1DfA36lld/AyZnC1dQniie2Cl9jIuojEmsPRIXVXSzpc5k1M1+Zvhkc3DOptAn+aox5jqG4eAkDXQEvKouO4lw5M0NUmWXjDPWddwjpX0cJwLshw6hsWuR2ZaXj80rZhPPU0KScAXOoesC4h0Wrzd67Q1SYVWujaMM26iDU25WvTFkPDETXUD260PHwZP3CDdQnijbs52LqERFvGlZzqFmpcLRW4ypuYhluN3wRgOqKGVa4dE00rKPWj4E0O1bd8AWCh28ebMzOp1F0tmQ12h3vTF9SKz01ec/I70xE1VA/+bltAKY4LWODLmSHGNg5eppF1EQn3IwfwD3VXSzl+dWd7E9Nwl3FMmwc1HBUcbl3COj9ypXUJ4o2dgle0/jtiK7mWbvzkzx1ZPHExM6xLWOdoDjGf9zGe+gb42rX15oXr0J8Tzf9QxBf+vGM4ydRdLWU97c6wLmGdmoxnR/Oz03xEDTt61KYFFzHT/ruLeOLcQBt1RG8On1uXIB75yfmUCH/xIKa9GFHDItfSkx2qAY7mVQ/+YMQPxe5o3rAuIuF68DyBrjkBwLkjeMu6iHWaM5m6HpybHoyooX5wh3UJpQzkSR++vYgXqgXP0dq6iETrwJOKaVnnfo9iGv7jRUx7MqIG5/ZjuHUR6zTgc7b24o9HfPC124u51kUkVHNGsbmuNVnjS7c7S62LWOdA3vbk3PRiRA1B8CDVrYtYZwGnU+zHNxjxwI7BAK3/jkRtXlJMyzor3ZkexXR17rYuYR1PghraBOdbl1DKCG6zLkE80jX4r3UJCRTwBHsopmWdaxlpXUIpl9HWm7PTk6lvgHluZ2ZbF7FOEW+zvzd/TGLvcnefdQkJcyvX6wqTdYa4wymxLmKdTfmSht6cn96MqKFRcKt1CaWUcAZz/fkWI+buort1CYnSg+usSxCPzHZneRTTcIdHMe3ViBpK3D5eTX304AWP/qjE2ny3N1Osi0iITrxHLV1dsoZz3XnTuohS9mW4V70IHo2ooSh4lBrWRZTSn8d9+h4jxhoGb7CJdRGJsAUvKaallLu9iunqPOBVTHsW1NA2uMK6hDKuYLKiWtbZNnhZ67+rrDavsKVXt0GxNd759Rjkr+zm2fnp1dQ3wFLXhu+tiyilDaP13V9K+Z8717qEWAt4jpN0Rck6S1xHrx4pbcMET15z8jvPRtRQJ/Brbe0E/mJdgnjlHK/eTR8/f1NMSxmXeBXT8LB3Me3hiBqgh3vRuoQy+nK6d39wYqfYHcPr1kXElN7rLWU95v5gXUIZp/KMh+enl0H9i2vJfOsiSqnHKFp6+IcnVha6fZlgXUQMdeAD6uhKknW+cHt79C4yaMAktvDwDPVu6htg8+Bv1iWUsZieLPXxG40Y2TgYSFPrImJnC15VTEspi1xPr2Ia7vQypj0NariYva1LKGMCfk3PiLVtg5e0/jsnWustZTl3Nl9aF1FGJ86zLiEDT4O6KHjIq45qeJb/aUwtpXQJHrIuIUb0Xm9Z3928ZF1CGdV5mCJPz1FPgxp2Df5kXcJ6LuULRbWUcrZnXf8++z+t9ZYyPnZXW5ewnmu8657+nZeLyVZb4XZnknURZezApzTw9o9SCq/YHcsg6yJiQGu9pay5bnd+tC6ijJZ85vEbM7wdUUPN4FHPyvuGc3H+frORgqsWPE0r6yK814k+imkppdid4llMV+NJj2Pa66CGfbx7tcRL/N26BPFKg2CQ1n9XSO/1lvVdyxDrEtbzZ/b0+hz1eOobYKnbjW+siyijiIF09/qPVApthDuI36yL8FRt3tciMinjZXcifuXOToyjttdnqdcjaqgTPIZfP78STtVGHVJGZ63/zkBrvWV9X7hensV0EY95HtPeBzXsF1xkXcJ6FnI8C/w608TY2cGV1iV4Se/1lrLmuuNZYl3Eei6ji/dnqedT3wBL3K58Z13Eeo5hgJbHSCkl7hit/16P1npLWSXuSK/2nQbYhvHU8/4s9X5EDXWDRz2b/oZXuc26BPFKUfAsbayL8EoHnlRMSxnXeBfTAY/EIKZjMaIGOM89bl3CerSoTNb3nevEHOsiPLEVo2im60NKed6d4tnTaTifh2NxlsYkqBe43Tzru4MGfKw9taSMD9zBWv8N1OUDdte1IaWMdV28ezq9LZ9TPxbnaQymvgEaBE9TzbqI9SygO7Pj8T1HCqRr8LB1CR4IeFwxLWXMcMd4F9NFPBGTmI5NUENnD9fVfs/xrFBUSylnBX+0LsGc3ustZS1zxzHNuogN/JX9YnOexmTqG2CF25MvrIvYwJk8GZs/bCmEEncsr1kXYUhrvaUs506ln3URG2jNpzF6Y15sRtRQM3iWWtZFbOAp7ozPdx0pgKLgGdpaF2FGa71lfTd4GNM1eTZGMR2roIbWwc3WJZTjWl5RVEsp9YOBbGpdhIkteJU6Mbr9SfSedz7uj3Abu8bqPI3R1DdAiTuQ962L2IDeZyzrG+G6scK6iALTdSDr+9Ad5OF10IV3qRarMzVmQQ3fu3YstC5iA1swkq1i9QcvUXvSnW1dQkEFPKdFZFLGD64Ts6yL2EADPmfrmJ2psZr6Btg2+I91CeX4mWNZHLfvPBKps4I/W5dQUFrrLWXNdUd4GNNwT+xiOoYjaoBj3EDrEspxOAOpHrsTQKJT7I7hdesiCuQUntEiMilluTuE4dZFlOMEXozheRrLoJ7n2jHVuohynEZf3ayklEVuX8ZbF1EAHfhAi8iklBJ3Ei9aF1GOrRhHkxieqbGb+gZoFPT17j1lAM9wi3UJ4pV0rP/WWm9Z35+9jOki+sQypmMa1NA1uNa6hHLdwoNxnKKQyGwTvExN6yIiVZtX2DKWNz+Jyr/d3dYllOtmDojpmRrLqW+AVW5/PrQuohzVeIljYnoySDSecmdZlxAZrfWW9b3gTqHEuohyxK8p63exDWqY5tox17qIctThbfaK6ekg0bjK/du6hIj8jRt0rkspH7hDWW5dRDkaMY4WsT1XYxzU8JI70bqEcm3GR2wX21NCwlfsjmWQdRER0FpvKWuS68w86yLKETAg1jOdMX1GvdoJwQXWJZRrJgczI87fgCRk1YJnE/j+7w48ppiWUqa5w72Mabgk1jEd8xE1LHedPNxRC6AN79M41qeGhMvPtzTlbwtGaRGZlDLbdWWKdRHlasMoasf6XI31iBpqBc9Sx7qIck3gCL2rTEpJ1vpvrfWWsha6wzyN6Vo8G/OYjn1QQ+vgbusSMhjJifymqJZ19g0esi4hJAF9tP2GlLLUdecz6yIyeJC2sT9XYx/U8IfgLOsSMhjMSaxSVMs6ZwV/sS4hFP/HibG/9Ul4VrqejLAuIoPzODsB52rMn1Gvttztw1jrIjI4kye04EbWKXHH4eOb6nNxIi/onJZ1Stxp9LMuIoNd+TgRb81LRFDD124PFlgXkcHV/CMBJ4qEZbHb19MFkNnZneGJuPVJWK5091iXkEFDPmX7RJyrCZj6BtgxeMy6hIzu4O/J+DYkoagXvBrj939vxSDFtJRyrbcxHfBEQmI6MUENJwZXWpeQ0XXcoaiWdbYJBsR0/XdtXqZZQm59Eobe7h/WJWR0Fccm5lxNyNQ3wEp3oLcLGgLu5dLEnDRSdX3cmdYl5CzgWU7WWSzr/Mf92bqEjPbmfWok5mxNzIgaagTP0dS6iAwcl/Pf5HwnkirrFfzVuoSc3aKYllLu9jimN6N/gmI6USNqgHfcIRRbF5FBwEOcn6BTR6qmxB3Pq9ZF5EBrvaW0R90F+JoeRbzFwYk6VxM0ogY4MLjZuoSMHBfztK9nthRcUfA0u1oXkbU96aOYlnUe8Tim4baExXTiRtTg3En0ty4io2r05ZSEnUKSvx9dJ2ZaF5GFLRjJVjpvZY2n3Dle7ji92nG8lLgvlYkLaljk9mKSdREZ1aAfxyfsJJL8feQOZIV1EZWow3B21zkrazzjzvT2ASPsykfUTdzZmrCpb4D6wSCaWBeR0Up6agJc1tkneNS6hEoEPK6YlnWedWd5HNONeTmBMZ3IoIZtg+eoZl1ERsWcxVOKalnjjOBq6xIqpLXe8rvH3Bmssi4io2o8nZhXnJSVyKCGg4NbrUuoQDHn8ISiWta4nWOsS8joRG6wLkG88bC7wONn03AHhycyphP5jHo150719kXxAAH3cUlCTyrJ1WLXmc+tiyiH3ustv3vQXerxSm84lWcSe64mNqhhmevCGOsiKhBwN5cn9sSS3Pi4/rsZo7TWW9b4p/P7FT3t+DDBXyoTOvUNUDt4yds3lQE4ruSu5H5PkpxsHbzs2fu/a/OKYlrWuMPzmG7CywmO6UQHNWwdPEd16yIq4PgTdyqqBfBt/XdAH/ZM8I1PcnGDu8a6hArV4EW2TfTZmuighoOCh6xLqMTVXOOcwlqAMwJ/boe3cGKib3ySLef+7G6zLqIS97J/ws/WBD+jXsvfbc3XupAHKEr4iSbZ8OX933qvt6xW7P7AE9ZFVOIq/pn4czUFQV3ijmOgdRGVOJk+idrrRfK11O3Hp8Y1aK23rLbCncrL1kVU4ggGUi3xZ2sKghoWuS5eNr+UdhCvUC/xp5tU7me3Bz8bHl9rvWW1xe44hlkXUYl2DE/FfTMVQQ0/uU78ZF1EJfbkDZqk4JSTynzqurLM6Ni1eU+LyASY645gpHURlWjGSJqn4mxN+GKytbYMXqWOdRGVGMV+/JyO701SoY7Bk9jcfQL+p5gWYIbbz/uYrs0rKYnp1AQ1dAieMLr5ZW8i+/ODolroabT+u7fe6y3Al24vJlgXUYkink7Rl8rUBDX0DG6xLqFSX7M3YxTVwm30LPgxT+RG648tHhjpujDVuohK3Z6q7YJT8ox6NefOpK91EZWqT38OTdEpKOVb4jozroDH24P3qa3zLvUGuNPMVkhk72z+l6pzNVVBDSvdkQyxLqJS1XmA81N1Gkp5fnZ7FmwJpNZ6C8B97o8e7za91oG8yUapOltTNPUNUCN4kXbWRVRqFRdwTbq+QUk5tijYEki911vAud7u8hjE9O68mrKYTt2IGuBntw8/WheRhbN4RC9BSb3+7qTItxYMeFaLyFLvN3c2z1oXkYVt+YjNU3e2pmxEDbBF8AaNrYvIwpN0Z1H6vkdJGT2CayM/htZ6yyJ3VCxiuglvpDCmUzmiBvjAHcpy6yKysAcDU3layu9KXI9IX+N4Es/pvd4pN9V1974dC6A2w9gnledqSoMaBrrjY/A0BrZkILun8tSUtZa5/Rgd0e+t93rLSHcsv1gXkYUi+qeqJavsZ0+po4P7rEvIyk/sx6tp/TYlANQOXmHLSH7nZryqmE65F92BsYhpuCu1MZ3ioIaLgj9bl5CVxRxHb0V1qkWz/ltrvdPOuTvcSSy1LiMr13B5is/V1E59A5S403nOuogs/YEHtAY81Z51p4e6/jvgeXrojEqx5e6c2Nz/zuCpVK+kSPGIGoqCp+huXUSWHuVAZqf5W1XqnRpcF+rv11sxnWpz3CGxiemj+F+qYzrlI2qAZe4wPrAuIks78Bq7pPp0TTfnTuH5kH6vE3kh5be+dJvgjuIH6yKytD9vUivl52rqgxoWuAMYa11ElhrzrN4DnmJLXFc+C+H30Xu9022AO5NF1kVkqSNvs3Hqz9VUT32v1iAYTEvrIrI0l8Pp7Zy+XaVU3eC1ENZ/N+NlxXRqOXeHOzE2Mb0jgxTTaES9xnTXORavFV2tJ/+jrk7elBrjulZpnW5t3kvRPr5S1iLXi1esi8hac4aztc5VNKJeY6tgKJtZF5G1F9iH7/QNK6U6BE+S/70r4H+K6dT6ynWKUUw3ZYhieg0F9Ro7BoNpZF1E1r5gD4YoqlOqR3B93r9W7/VOr0FuTyZbF5G1BrylpbPraOq7lI/dwSyxLiJr1biNv2rlbio5dyr98vh1J9BfZ0wqOXcn11FiXUbWajOYLjpT11FQl/GmO44V1kXk4GQepZ5O5xRa6rrkvP5ba73Tap7rxSDrInJQi1c5RGdqKQrq9bzljo1VVO9Mf9rqlE6hn10npufw3zdjlF4YmkpjXQ++tS4iBxvxIkfpTC1Dz6jXc1jwHDWsi8jBl3TicX3bSqHc3v+t93qnVR+3b6xiuhp9FdMbUFBv4LjgWapbF5GDZZxHL7dMYZ06uwdPZbn+O+BxrfVOoWXuXHcmy6zLyEE1+tJTZ+oGFNTlODF4hmrWReSkL/vyraI6dU4Mbsjqv+vNKbr5pc6Xbk/+Z11ETop4UmdqufSMOoMn3bkxWiMJsDGPc6JO8pTJZv231nqn0TPuQhZbF5GTgIc4X+dpuRTUGT3kLg51W8HoBfyZ27UZZsosdV0ZU8G/78gHWuudMsvdlTxsXUSOFNMVUVBX4D53uXUJOevIc+yg0z1VZrg9M67/1lrv9JnsTuFz6yJyFHAvl+o8zUjPqCtwWfCfKrys0candOBpfftKlWbBy9Qu99/UYaBiOmUech1iGNP/UUxXSEFdoT8GD8YuqhdyBj3dfIV1iuwR9C3nPA14jI5xO32lCha4k91FsVrlDRBwF1fqPK2QgroSFwYPx/CH1J/2fKSoTpETgps2+Gc3awVtqrzn2vC8dRE5C7iPK3SeVkLPqLPwrOtFsXUROavO9dxEkS6BlHDuNJ4r9fda650mq9yt3BrDu1Q1HuVsnaWVUlBnpa87O4YXAXTjKbbQZZASS91+fLrmr7XWO02+d6fzkXUReajOk5ymszQL8ZvVNXFG8GysXiy61jBa01ffxVKiTjCQrQBoxgDFdGr0d7vHMqarKaazphF11ga5E2O1XcfvevBfmuiCSIXPXFdKeE8vDE2JWe4CXrEuIi8b8RzH6yzNkoI6BwNdz5hG9ZY8zqG6KFLhBYfelpwSA9wFzLYuIi+1eJHuOkuzpqDOyZvuhNg1P6wWcCH/pK4uDZFEWOAup491EXmqwwDtN50TBXWOPnBHs8C6iDxty5N01eUhEnsfujNjtXllaQ15jc66D+VEi8ly1DV4h6bWReTpew7kOrdC381EYmypu9J1iW1Mb8a7iumcaUSdhy/dwUyzLiJvO/AY++lCEYml4e48vrIuIm8tGMpOuvvkTCPqPOwcjGBn6yLy9g0HcIFbrG9oIjGz0F3h9o9xTO/CCMV0XjSiztMsdxhjrYuogm15hG66ZERi4w13YYxn8qADb9JU95y8aESdp02DDzjIuogq+J5D6OXm6nuaSAzMcxe47rGO6a68o5jOm0bUVbDCnRzTlw2s1YwHOE4Xj4jX+rtLYtovvdaRvKB35VWBRtRVUDN4gVOti6iSGRzPKW6Gvq2JeOpHd7TrGfOYPkOvtK0iBXWV1Aie5mbrIqqoHy25xxUrrEU8s8rd49rwmnUZVXQ5T1JdMV0lmvoOwb3uj5RYF1FF7XlI74cW8ciH7iLGWxdRRdW4h0t0X6kyBXUoBrjTYvpq0d8VcR7/ZGNdVCLm5rne3B/7r/816aP3zodCQR2ST9xRzLEuosqa8Q966cISMdXfXcos6yKqrDGv0EV3k1AoqEMzyR3OVOsiQnAE97K9Li8RExPdpbxnXUQItuVNdtZ9JCRaTBaaVsEntLcuIgRv0JIr3EJ9gxMpsPnuGrd7ImK6LcMV0yHSiDpUi10P3rIuIhRb8HfOINClJlIQJe5p/pKACW+Abryk1S6h0og6VPWCVznDuohQ/MyZdOEzfY8TKYAP3R6cmZCYPps3FNMhU1CHbKOgT3B3Qn6sH7IHvdxMhbVIhH52vVwXPrMuIxQBN/O/oIZiOmSa+o7EC+6s2LdrrdWQ3lyMLj2R8C13d3E7i63LCEkd+nK87hQRUFBHZJw7iunWRYRmJ27lRD2xFgmRcy9yDd9ZlxGaZrzKHrpHREJBHZmf3NEJmc5arRP/orMuQ5FQfOL+zEfWRYRoV16jhe4PEUnGw1QvbRm8z9HWRYRoJF04yn2rb3YiVfSl6+n2TlRMH85wxXSEFNQRqhe8zNXWRYRqEC25wM1WWIvk6Vd3jduV/tZlhOpyBmmdd6Q09R25h92lrLIuIlSNuZ5LqKkLUyQnS91d3MEi6zJCVYMHOU/3gogpqAvgHXdSAt4DXlYLbuQsbV4nkqXf3CPcxi/WZYSsKS+wv+4CkVNQF8R0dxyfWhcRum24lnOppstUpEIl7iWu5VvrMkLXjgFso+u/APSMuiC2CobTy7qI0P3ABexKf+f0bU8kA+f6u1b0TGBMn8yHiukCUez/JXcAAA4lSURBVFAXSK3gqeBhqluXEbpJ9GQvXlNUi5RjmOtIT760LiN01fgHzwV1FNMFoqnvghrsTmGedRGR2J//096zIqUMdTfxiXURkWhMPw7W1V5ACuoC+9YdywTrIiKyL1dzlC5fEYa5GxMa0rArA9hO13lBaeq7wLYPPuZE6yIi8iFHs697Tc+sJdWGuU7u4MTG9NEMV0wXnIK64OoFL3Ar1azLiMhHHM1eDFJYSwqVuBddO3cwo6wLiUg1/s4rerWJAU19G3nPnZK4nsrSduXPnKbWLUmNEvc6NzPWuowINeUZPZk2oqA285M7iQ+ti4hUG/7CKdogUxJvuevDv/nKuoxIdaEfW+haNqKgNrTK3cCdJPtPYHMu4Eoa6gKXhFronuCf/GRdRqQCLuNf+sptSEFtbKA7k/nWRUSsPmfzF7bSZS4J84t7iHsSf/1uzOOcqKvXlILa3NfuRL6wLiJyG3ES19BKl7skxHj3T/qx0rqMyLWjPzvoujWmoPbAUncJT1oXUQBFHMUf2U8XvcSac8P4D4MT/tBqtXO5j9q6Ys0pqD3xmLucZdZFFMRuXM6p1NLFLzG0xPXlXiZbl1EQdbifs3WdekFB7Y3J7hQ+ty6iQBpwJn+mhW4CEiM/u0e4n1+tyyiQVvSjra5QTyioPbLcXc291kUUTBFHcAXddCuQGBjh7mUAq6zLKJgzeAhtueEPBbVnXnbnJXTbjvJ14FJO0lMw8dYS9yz3Md66jAJqwuMcoyvSKwpq70xzpzHcuoiCasBJXMxuujWIZya7p3iUudZlFNRePMu2uhY9o6D2ULH7G7dSbF1GgXXgfE7XdJt4Ybl7jUcYZl1GgVXjBm7Ui389pKD21PvudKZbF1FwDTiJS9hVNwoxlMZxNEBznqarrj0vKai99as7j1esizDRhfM4gbq6ZUiBLXQv8BgjrcswcQKP0FjXnKcU1F7r785P/AsKy1ebIzmfgwh065ACKHEf0ZdnWWxdiImN+Sfn60rzmILacz+6M3nfuggzW3Ea5+kFhhKpL91z9OF76zLM7EMfttc15jUFtfecu5erWWFdhqEOnM/J2q5eQrfAvUpf3k7Fy0DLV5NbuErLx7ynoI6Fz90Zqerk3FAdjuFkDqWmbikSgmXuDfoxiOXWhZjajb56+1gsKKhjYqW7LYUtW+tryFH04DDtjCt5K3Yf05d+LLQuxFjAZdypL74xoaCOkY9dL76xLsIDjelOLw6kSDcZyUGJ+4j+9GOWdSEe2JqntI9djCioY2Wh+zOPp/iJWmnNOYke7KF14VKp1RHdnxnWhXgh4A/8i/q6bmJEQR07w925fG1dhDeacpgmwyWjYvcx/XmRn60L8cY2PKqtcGJHQR1DS93/8U9KrMvwSGO6cxRH6CUpss4yN4xBDGC2dSEeCfgD/6aerpLYUVDH1AfuXD2vXk89Dud4DqeBbkSpNte9zgAGs9S6EM/sxON01rURSwrq2FrmbuFfqV8HvqFqtONIjmJ3Pb1One/cawzifVZaF+KdIs7jP5pxii0Fdax94s5hsnURntqaQ+nGYVo0kwJL3Ue8xgCmWRfiqdb8jz11HcSYgjrmlrtb+BerrMvwVm32pzuH6RWJCfWle5M3+CDVb+6rWA2u5gb1S8ecgjoBvnAX8rF1EZ5rRme60Z0tdcNKhFnufYYxmB+tC/HcPjxMG53zsaegTgTn+vLHFO6gm7vt6EY3DtGCs5ha4j5mGMP4TG8TqFRDenOZXguUCArqxJjhrqC/dRExsRF7cRBd6URt3cZiYYn7hA94m5F6zJOlU7iLzXR2J4SCOlHecJfwg3URMVKd3diXzhxIE93SvLTQjWIYI/hUT6FzsB0PcJjO6ARRUCfMMncHf+c36zJiZzv2pTMHs61ub1742X3ICD5krF7sk6MaXMTtasRKGAV1Ao13F2hxWZ6a0YEOdKAzjXSrK7jFbhxjGMMIvrcuJab25SEtHksgBXUilbj/cZ1enlgF1WjNnnRiT1pTTTe+SK1yExjJSEYxWePnKtiMv3OWXvOTSArqxJrnbuIhLb2psrrsTnt2YzfaqBs1RMvdBMYxjnGM1cs+q6w6l9JbvQyJpaBOtCnuCoZYF5EY1dmJ1rSiA3uwuW6JeZnnJjKGMUxigpaHhWZ/7qWtzsgEU1AnnHP9+As/WZeROFvShtbsQkta0Vi3yArNcZOYzGQmM147QoeuBf+ih87AhFNQp8Bidxv/0UrwyGxKK3ahFS3Zha10ywRgqpuyJpwnMce6mMSqxVVcSx2dc4mnoE6Jb9yVvG5dRArUZEu2W/e/lqm5ia500/hu3f++ZLF1QSnQjfvYJSXnV9opqFPkDfcXJlkXkSrV2Irt2Y7mtGBLtqJFYvpbF7upTOcnpjKV7/iO6VqvXVBt+ReHJORcksopqFNllXuMm5llXUaKNWJLWrAVW9KCpmzKZjT1/jWmy9xsfmE2s5jGdKYzlekssC4qxTbn/zhHTYOpoqBOnYXuH9zFcusyZJ16bMam62K7IQ1ouOb/C/nSlXluPvNZwOr/n80M5jCbmcxkifUPSNapw5+4mnoK6ZRRUKfSVHctz2n/oRhYHdgNqElDqlOfmtShDjWpT3UaARDQcN1/XY8aa/5qZamnxPNxgGM+q1jECpaylBUsYhXz+G1NOGuE7L8iTuN2LVdMJQV1an3qruJ96yJEJCt78R/2VkinVJF1AWKlY/Be8CI7WJchIpXYiQF8HCim00sj6pRb6Z7gFn62LkNEyrUlf+FCvbw25RTUwlL3KLdrLbiIZxrzVy73vitAoqegFgAWuwe4nYXWZYgIAHW5lGtoqJAWFNRSyhz3L+5R45aIsY04i1u08Yuso6CWMqa623icYusyRFKqiBP4B9sppKUUBbVsYKLrzct6JaRIgRXRg5tpqZCW9SiopVwT3R08q5G1SIEUcQR/o51CWsqhoJaMJrl/KKxFIlfECdyikbRkpKCWCimsRaJUxAn8n7arlAopqKVS37k7+B+rrMsQSRiFtGRHQS1ZmeJuo5/CWiQkNTiF69lJIS1ZUFBL1n50/+ExllqXIRJzNenJjeyokJYsKaglJ3Pc/dzPr9ZliMTUxpzFtXqZieREQS05W+we599Msy5DJGY25wL+SAOFtORIQS15Weme4w4mWZchEhM7cKl2wZI8KaglbyVuIP/kI+syRDzXmb9wFIFCWvKkoJYq+szdTT9WWpch4qEaHMuV7KOIlipRUEsIfnEP8QBzrMsQ8UgDzuQqmiukpcoU1BKSFe55/skE6zJEPLATF/MH6iikJRQKagnVCHcvL+uVo5JaAQdxOUfqibSESEEtofvS3UdfFlqXIVJgDTiDS9lZES0hU1BLJJa7F7iHz6zLECmQDpzPqdRTSEsEFNQSoTHuEZ5hiXUZIhGqxVGcTzdFtERGQS0RW+Ce537GW5chEoGdOIfzaKKQlkgpqKUgxrhH6MNy6zJEQrIRx3A+B2nRmBSAgloKZp7rz0OMtS5DpIpa0Yuz2VQRLQWioJYCm+j68rhejiKx1ICTOIPOimgpKAW1GFjhBtKHt1hlXYhIloo4kDM4US8xEQMKajHzk3uax/jGugyRSjTnVC5gW0W0GFFQiynnhvMMLzLXuhCRcjThRE6js5aMiSkFtXig2L1LHwaw2LoQkTVq0Y1eHMNGimgxp6AWbyxzg+jDYG2ZKaaqcQBncBz1FdHiCQW1eGauG0Rf3kZnphReB87gZDZTRItXFNTipR9cf15mpOJaCqKIvTieHrRQRIuHFNTisenuDV5TG5dEqIi96cGJbKmIFm8pqMV7v7rX6c8QfrMuRBKlGnvRg540U0SL5xTUEhNz3au8xDBWWBcisVeLgzmeo2msiJZYUFBLrCx1bzOI1/nJuhCJpaYcxlEcysaKaIkRBbXE0kQ3iNf4SIvNJEutOIoj2YciRbTEjoJaYmyWe4tBvMUi60LEW7XZlyM5ga0U0BJbCmqJvWXuXd5iKFOsCxGvtORgDuMAaimiJeYU1JIYv7jhDGMQP1sXIqY24QC6cShbK6AlIRTUkjjfudcYxAiWWxciBVWdThxFN9rrObQkjIJaEmqp+4BhvM9Yiq1LkUhVpz37cTBdqK2AlkRSUEvCLXEfM4IPGa4O7ISpzm7sS2cOpqECWhJNQS0psdR9xocM40OWWZciVVKb3enMvuynbmhJCQW1pMwKN5oP+ISRzLIuRXKyGXuxF13YQ3tES8ooqCW1fnZj+JARjNGyM49VZyc6sy8daEWggJZUUlBL6q1yX64J7Ml605k3mtFhTUBriZiknYJaZJ1Zbiyr//eNIttAETvQnva0px2bKp5F1lBQi5RjkfuKiYxhjCbGI1ednehAB1qzu/azEimHglqkQr+5CXzOJCYymR81zg5JwNa0ojWt2I3WWhwmUiEFtUjWVrhvmMREJjGRKZRYlxM7zWi9Lp7rK5xFsqSgFsnLEjeZKXzNN3zLN/xqXY6nNmF7dmAHdmQXWlJH4SySBwW1SAjmu2/5bt3/fkjxaLsR2635Xyva6J1hIiFQUIuEbqn7gelMYxpTmcZ0fkzs29Dq0ILmbEULWrAVzdlGzVQioVNQixTAHDedafzITGYwi9n8wiyWWheVozpsyuZsyqZszua0oDnNaaJYFomcglrEyGL3y5rQnsls5jOfecxnPvNZYlhVXRrSkEY0pCENacpmbL7m/+sqlEVMKKhFvPObm78muBcyj5UsZhnLWcQq5rGKRSxnGYtZuea//v2vlq7bIawmddb8VQ3qlfqr2tSiPtVpRHXqU4vaa/6uwZpgbkQNxbGIZ/4fdLRH/G+mOUcAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjEtMDMtMjVUMjA6MDY6NTIrMDA6MDAliGLtAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE2LTA0LTE2VDA3OjU0OjA4KzAwOjAwcqpWsQAAAABJRU5ErkJggg==";
                                //valor="http://www.google.com/images/logos/ps_logo2.png";
                                //valor="http://appdt286-001-site1.btempurl.com/api/imagen";
                                //llama el servicio de imagen
                                ServicioCampoImagen campoimagen = new ServicioCampoImagen();
                                campoimagen.SetImagen(item.DatoConjunto, item.DatoCampo);
                                WParagraph paragraph = new WParagraph(documentoWord);
                                //FileStream imageStream = campoimagen.Imagen;
                                WPicture picture = paragraph.AppendPicture(campoimagen.Imagenbyte) as WPicture;
                                TextSelection newSelection = new TextSelection(paragraph, 0, 1);
                                TextBodyPart bodyPart = new TextBodyPart(documentoWord);
                                bodyPart.BodyItems.Add(paragraph);
                                documentoWord.Replace(item.CampoNombre, bodyPart, true, true);
                            }
                            else
                            {
                                gestorLog.Registrar(Nivel.Debug, $"remplasaetiqueta{valor}:valor=[{valor}],camponombre=[{item.CampoNombre}]");
                                documentoWord.Replace(item.CampoNombre, valor, true, true);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // GestorErrores.Manejar(ex, CapaExcepcion.LogicaNegocios);
            }
            gestorLog.Salir();
        }

        private string ObtenerValorPropiedades(Campo campoOriginal)
        {
            string resultado = "";
            string valor = "";
            Dictionary<string, string> listaCampos = new Dictionary<string, string>();
            try
            {
                // campoOriginal.DatoCampo = "( { Credito.MontoCredito } / 12 ) * { TablaAmortizacion[0].SaldoInicial } * { Credito.Plazo }  / 12";

                string[] arrCampos = ObtenerCampos(campoOriginal.DatoCampo);
                foreach (string campo in arrCampos)
                {
                    if (campo.Trim() != "")
                    {
                        Campo c = new Campo() { DatoConjunto = "", DatoCampo = campo.Replace("{", "").Replace("}", "").Trim() };
                        valor = ObtenerValorPropiedad(c, 0);
                        listaCampos.Add(campo, valor);
                    }
                }

                foreach (var item in listaCampos)
                {
                    campoOriginal.DatoCampo = campoOriginal.DatoCampo.Replace(item.Key, item.Value);
                }

                var dt = new DataTable();
                dt.Columns.Add("Calculo", typeof(decimal));
                dt.Rows.Add(1);
                object dtResult = dt.Compute(campoOriginal.DatoCampo.ToString(), "");
                // Console.WriteLine(doublePrresultadoice);

                resultado = dtResult.ToString();
            }
            catch (Exception)
            {
                resultado = "";
            }
            return resultado;
        }

        private string[] ObtenerCampos(string campos)
        {
            List<string> listaCampos = new List<string>();
            try
            {
                string[] listaCamposII = campos.Split('{');
                for (int i = 1; i < listaCamposII.Count(); i++)
                {
                    if (listaCamposII[i].Trim() != "")
                        listaCampos.Add("{" + listaCamposII[i].Substring(0, listaCamposII[i].IndexOf('}')) + "}");
                }

            }
            catch (Exception)
            {
                throw;
            }
            return listaCampos.ToArray();
        }

        public WordDocument CrearGraficaMano(WordDocument document)
        {
            IWSection section = document.AddSection();
            section.PageSetup.Orientation = PageOrientation.Portrait;
            section.PageSetup.Margins.Top = 0;
            section.PageSetup.Margins.Left = 30;
            section.AddParagraph().AppendText("");
            IWTable table = section.AddTable();
            table.ResetCells(3, 1);
            var p1 = table[0, 0].AddParagraph();
            p1.ParagraphFormat.AfterSpacing = 1f;
            p1.ParagraphFormat.BeforeSpacing = 1f;
            var p1Text = p1.AppendText("CARGOS OBJETADOS");
            p1Text.CharacterFormat.FontSize = 9;
            p1Text.CharacterFormat.Bold = true;
            var p2 = table[1, 0].AddParagraph();
            p2.ParagraphFormat.AfterSpacing = 0f;
            p2.ParagraphFormat.BeforeSpacing = 0f;

            var p2Text = p2.AppendText("*GAT NOMINAL: Ganancia Anual Total Nominal\n*GAT REAL: Ganancia Anual Total Real");
            p2Text.CharacterFormat.FontSize = 9;

            IWParagraph paragraph = table[2, 0].AddParagraph();
            //Creates and Appends chart to the paragraph
            WChart chart = paragraph.AppendChart(450, 180);
            //Sets chart type
            chart.ChartType = OfficeChartType.Pie;
            //Sets chart title
            chart.ChartTitle = "GRÁFICOS DE SALDOS Y MOVIMIENTOS A TU CUENTA";
            chart.ChartTitleArea.FontName = "Calibri";
            chart.ChartTitleArea.Size = 14;
            //Sets data for chart
            chart.ChartData.SetValue(1, 1, "");
            chart.ChartData.SetValue(1, 2, "Sales");
            chart.ChartData.SetValue(2, 1, "Phyllis Lapin");
            chart.ChartData.SetValue(2, 2, 141.396);
            chart.ChartData.SetValue(3, 1, "Stanley Hudson");
            chart.ChartData.SetValue(3, 2, 80.368);
            chart.ChartData.SetValue(4, 1, "Bernard Shah");
            chart.ChartData.SetValue(4, 2, 71.155);
            chart.ChartData.SetValue(5, 1, "Patricia Lincoln");
            chart.ChartData.SetValue(5, 2, 47.234);
            chart.ChartData.SetValue(6, 1, "Camembert Pierrot");
            chart.ChartData.SetValue(6, 2, 46.825);
            chart.ChartData.SetValue(7, 1, "Thomas Hardy");
            chart.ChartData.SetValue(7, 2, 42.593);
            chart.ChartData.SetValue(8, 1, "Hanna Moos");
            chart.ChartData.SetValue(8, 2, 41.819);
            chart.ChartData.SetValue(9, 1, "Alice Mutton");
            chart.ChartData.SetValue(9, 2, 32.698);
            chart.ChartData.SetValue(10, 1, "Christina Berglund");
            chart.ChartData.SetValue(10, 2, 29.171);
            chart.ChartData.SetValue(11, 1, "Elizabeth Lincoln");
            chart.ChartData.SetValue(11, 2, 25.696);
            //Creates a new chart series with the name “Sales”
            IOfficeChartSerie pieSeries = chart.Series.Add("Sales");
            pieSeries.Values = chart.ChartData[2, 2, 11, 2];
            //Sets data label
            pieSeries.DataPoints.DefaultDataPoint.DataLabels.IsValue = true;
            pieSeries.DataPoints.DefaultDataPoint.DataLabels.Position = OfficeDataLabelPosition.Outside;
            //Sets background color
            chart.ChartArea.Border.LinePattern = OfficeChartLinePattern.None;
            //Sets category labels
            chart.PrimaryCategoryAxis.CategoryLabels = chart.ChartData[2, 1, 11, 1];

            return document;
        }
        public WordDocument RemplazarImagen(WordDocument document, string palabra)
        {
            document.MailMerge.MergeImageField += new MergeImageFieldEventHandler(MergeField_ProductImage);
            //Specifies the field names and field values
            string[] fieldNames = new string[] { palabra
            };
            string[] fieldValues = new string[] { "Logo.jpg" };
            //Executes the mail merge
            document.MailMerge.Execute(fieldNames, fieldValues);
            //Unhooks mail merge events handler
            document.MailMerge.MergeImageField -= new MergeImageFieldEventHandler(MergeField_ProductImage);
            //Saves the WordDocument instance
            return document;
        }

        public void MergeField_ProductImage(object sender, MergeImageFieldEventArgs args)
        {
            //Binds image from file system during mail merge
            if (args.FieldName == "Photo")
            {
                string ProductFileName = args.FieldValue.ToString();
                //Gets the image from file system
                using (MemoryStream ms = new MemoryStream())
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"img/", ProductFileName);
                    var img = Image.FromFile(path);
                    img.Save(ms, ImageFormat.Jpeg);

                    args.ImageStream = ms;
                }

                //Gets the picture, to be merged for image merge field
                WPicture picture = args.Picture;
                //Gets the text box format
                WTextBoxFormat textBoxFormat = (args.CurrentMergeField.OwnerParagraph.OwnerTextBody.Owner as WTextBox).TextBoxFormat;
                //Resizes the picture to fit within text box
                float scalePercentage = 100;
                if (picture.Width != textBoxFormat.Width)
                {
                    //Calculates value for width scale factor
                    scalePercentage = textBoxFormat.Width / picture.Width * 100;
                    //This will resize the width
                    picture.WidthScale *= scalePercentage / 100;
                }
                scalePercentage = 100;
                if (picture.Height != textBoxFormat.Height)
                {
                    //Calculates value for height scale factor
                    scalePercentage = textBoxFormat.Height / picture.Height * 100;
                    //This will resize the height
                    picture.HeightScale *= scalePercentage / 100;
                }
            }
        }
        private DataTable CreaTabla(Campo dataCampo, int numInt)
        {
            DataTable Tabla = new DataTable();
            try
            {

                var jDatosTabla = JObject.Parse(dataCampo.DatoCampo);
                string valor = jDatosTabla.SelectToken("Tabla").ToString();
                Tabla.TableName = valor;
                string token = string.Empty;
                JArray datos = new JArray();

                valor = jDatosTabla.SelectToken("Encabezado").ToString();
                string[] Columns = valor.Split(',');
                foreach (string Titulo in Columns)
                {
                    Tabla.Columns.Add(Titulo, typeof(String));
                }

                JArray jCampos = (JArray)jDatosTabla.SelectToken("Datos");

                var jDatosJson = JObject.Parse(datosJson);
                if (string.IsNullOrEmpty(dataCampo.DatoConjuntoGrupal))
                {
                    token = dataCampo.DatoConjunto;
                    if (jDatosJson.SelectToken(token) is JArray)
                        datos = (JArray)jDatosJson.SelectToken(token);
                }
                else
                {
                    token = dataCampo.DatoConjuntoGrupal;
                    if (numInt > 0)
                    {
                        numInt = numInt - 1;
                        token = token.Replace("[]", "[" + numInt.ToString() + "]");
                        datos = (JArray)jDatosJson.SelectToken(token);
                    }

                }

                foreach (JToken dato in datos.Children())
                {
                    DataRow dtRow = Tabla.NewRow();
                    int Cols = 0;
                    string valorCampo = "";
                    foreach (string Titulo in Columns)
                    {
                        token = jCampos[Cols]["Campo"].ToString();
                        string formato = jCampos[Cols]["Tipo"].ToString();
                        valorCampo = "";

                        if (token.Contains("SUMA"))
                        {
                            token = token.Replace("SUMA(", "").Replace(")", "");
                            string[] listaCampos = token.Split('+');
                            decimal suma = 0;
                            foreach (string campo in listaCampos)
                            {
                                valorCampo = (string)dato[campo.Trim()];
                                decimal.TryParse(valorCampo, out decimal valordecimal);
                                suma += valordecimal;
                            }
                            valorCampo = suma.ToString();
                        }
                        else
                        {
                            if (token.Contains("[") && token.Contains("]"))
                            {
                                string tipo = string.Empty;
                                if (token.Contains("Beneficiarios"))
                                {
                                    tipo = dato.SelectToken(token.Substring(0, 17) + "TipoBeneficiario").ToString();

                                    if (tipo == "A")
                                    {
                                        valorCampo = dato.SelectToken(token).ToString();
                                    }
                                    else
                                    {
                                        valorCampo = " ";
                                    }
                                }
                                else
                                {
                                    valorCampo = dato.SelectToken(token).ToString();
                                }
                            }
                            else
                            {
                                var jvalor2 = dato[token] ?? "";
                                // valorCampo = (string)dato[token]; 
                                valorCampo = jvalor2.ToString();
                            }
                        }
                        valorCampo = CampoTipo(valorCampo, formato.Trim());
                        dtRow[Titulo] = valorCampo;
                        Cols++;
                    }
                    Tabla.Rows.Add(dtRow);
                }

                if (Tabla.Rows.Count == 0)
                {
                    DataRow dtRow = Tabla.NewRow();
                    foreach (string Titulo in Columns)
                    {
                        dtRow[Titulo] = " ";
                    }
                    Tabla.Rows.Add(dtRow);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }
            return Tabla;
        }

        private DataSet CreaTablas(Campo dataCampo, int numInt)
        {
            DataSet dsTablas = new DataSet();
            List<string> Columns = new List<string>();
            string token = string.Empty;

            try
            {
                var jDatosTabla = JObject.Parse(dataCampo.DatoCampo);
                List<string> tablas = jDatosTabla.SelectToken("Tablas").ToString().Split(",").ToList();
                JArray datos = new JArray();
                bool duplicarColumnas = false;
                int numeroDuplicacionColumnas = 0;

                if (tablas.Count() == 1)
                {
                    string numeroColumasDuplicadas = jDatosTabla.SelectToken("DuplicarColumnas")?.ToString();
                    duplicarColumnas = int.TryParse(numeroColumasDuplicadas, out numeroDuplicacionColumnas);
                }

                // Se determina si se duplicaran las columnas en una misma DataTable
                if (duplicarColumnas)
                {
                    DataTable Tabla = new DataTable();
                    string valor = string.Empty;

                    Tabla.TableName = tablas.First();
                    valor = jDatosTabla.SelectToken("Encabezado").ToString();
                    Columns = valor.Split(',').ToList();
                    for (int i = 1; i <= numeroDuplicacionColumnas; i++)
                    {
                        foreach (string Titulo in Columns)
                            Tabla.Columns.Add($"{Titulo}{i}", typeof(String));
                    }
                    dsTablas.Tables.Add(Tabla);
                }
                else
                {
                    // De lo contratrio se crean múltiples DataTable con el mismo número de columnas
                    // Ciclo para crear los DataTable
                    foreach (string nombreTabla in tablas)
                    {
                        DataTable Tabla = new DataTable();
                        string valor = string.Empty;

                        Tabla.TableName = nombreTabla;
                        valor = jDatosTabla.SelectToken("Encabezado").ToString();
                        Columns = valor.Split(',').ToList();
                        foreach (string Titulo in Columns)
                        {
                            Tabla.Columns.Add(Titulo, typeof(String));
                        }
                        dsTablas.Tables.Add(Tabla);
                    }
                }

                // Se obtienen los datos para llenar los DataTable
                JArray jCampos = (JArray)jDatosTabla.SelectToken("Datos");
                var jDatosJson = JObject.Parse(datosJson);
                if (string.IsNullOrEmpty(dataCampo.DatoConjuntoGrupal))
                {
                    token = dataCampo.DatoConjunto;
                    if (jDatosJson.SelectToken(token) is JArray)
                        datos = (JArray)jDatosJson.SelectToken(token);
                }
                else
                {
                    token = dataCampo.DatoConjuntoGrupal;
                    if (numInt > 0)
                    {
                        numInt = numInt - 1;
                        token = token.Replace("[]", "[" + numInt.ToString() + "]");
                        datos = (JArray)jDatosJson.SelectToken(token);
                    }
                }

                int numeroRegistrosTotales = datos.Children().Count();
                int numeroRegistrosPorTabla = 0;
                if (duplicarColumnas)
                {
                    //numeroRegistrosPorTabla = numeroRegistrosTotales / numeroDuplicacionColumnas;
                    numeroRegistrosPorTabla = (int)Math.Ceiling(Convert.ToDouble(numeroRegistrosTotales) / Convert.ToDouble(numeroDuplicacionColumnas));
                    if ((numeroRegistrosTotales * numeroDuplicacionColumnas) < numeroRegistrosTotales)
                        numeroRegistrosPorTabla++;
                }
                else
                {
                    numeroRegistrosPorTabla = numeroRegistrosTotales / dsTablas.Tables.Count;
                    if ((numeroRegistrosTotales * dsTablas.Tables.Count) < numeroRegistrosTotales)
                        numeroRegistrosPorTabla++;
                }

                if (duplicarColumnas)
                {
                    for (int i = 0; i < numeroRegistrosPorTabla; i++)
                    {
                        DataRow dtRow = dsTablas.Tables[0].NewRow();
                        foreach (DataColumn col in dsTablas.Tables[0].Columns)
                            dtRow[col.ColumnName] = string.Empty;
                        dsTablas.Tables[0].Rows.Add(dtRow);
                    }
                }

                int indiceTabla = 0;
                int indiceRegistro = 0;
                int indiceColumnasDuplicadas = 1;
                foreach (JToken dato in datos.Children())
                {
                    DataRow dtRow = dsTablas.Tables[indiceTabla].NewRow();
                    int Cols = 0;
                    string valorCampo = "";
                    foreach (string Titulo in Columns)
                    {
                        token = jCampos[Cols]["Campo"].ToString();
                        string formato = jCampos[Cols]["Tipo"].ToString();
                        valorCampo = "";

                        if (token.Contains("SUMA"))
                        {
                            token = token.Replace("SUMA(", "").Replace(")", "");
                            string[] listaCampos = token.Split('+');
                            decimal suma = 0;
                            foreach (string campo in listaCampos)
                            {
                                valorCampo = (string)dato[campo.Trim()];
                                decimal.TryParse(valorCampo, out decimal valordecimal);
                                suma += valordecimal;
                            }
                            valorCampo = suma.ToString();
                        }
                        else
                        {
                            if (token.Contains("[") && token.Contains("]"))
                            {
                                string tipo = string.Empty;
                                if (token.Contains("Beneficiarios"))
                                {
                                    tipo = dato.SelectToken(token.Substring(0, 17) + "TipoBeneficiario").ToString();

                                    if (tipo == "A")
                                    {
                                        valorCampo = dato.SelectToken(token).ToString();
                                    }
                                    else
                                    {
                                        valorCampo = " ";
                                    }
                                }
                                else
                                {
                                    valorCampo = dato.SelectToken(token).ToString();
                                }
                            }
                            else
                            {
                                var jvalor2 = dato[token];
                                // valorCampo = (string)dato[token]; 
                                valorCampo = jvalor2?.ToString();
                            }
                        }
                        valorCampo = CampoTipo(valorCampo, formato.Trim());
                        if (duplicarColumnas)
                            dsTablas.Tables[0].Rows[indiceRegistro][$"{Titulo}{indiceColumnasDuplicadas}"] = valorCampo;
                        else
                            dtRow[Titulo] = valorCampo;
                        Cols++;
                    }
                    if (duplicarColumnas == false)
                        dsTablas.Tables[indiceTabla].Rows.Add(dtRow);
                    indiceRegistro++;
                    if (indiceRegistro == numeroRegistrosPorTabla)
                    {
                        indiceRegistro = 0;
                        indiceColumnasDuplicadas++;
                        if (dsTablas.Tables.Count > 1)
                            indiceTabla++;
                    }
                }

                if (indiceRegistro < numeroRegistrosPorTabla && indiceTabla < tablas.Count() && duplicarColumnas == false)
                {
                    for (int i = indiceRegistro; i <= numeroRegistrosPorTabla; i++)
                    {
                        DataRow dtRow = dsTablas.Tables[indiceTabla].NewRow();
                        foreach (string Titulo in Columns)
                            dtRow[Titulo] = " ";
                        dsTablas.Tables[indiceTabla].Rows.Add(dtRow);
                    }
                }

                foreach (DataTable tabla in dsTablas.Tables)
                {
                    if (tabla.Rows.Count == 0)
                    {
                        DataRow dtRow = tabla.NewRow();
                        foreach (string Titulo in Columns)
                        {
                            dtRow[Titulo] = " ";
                        }
                        tabla.Rows.Add(dtRow);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }
            return dsTablas;
        }

        private string ObtenerRegistroDeTabla(Campo dataCampo, int numInt)
        {
            string valor = string.Empty;
            try
            {
                DataTable tabla = CreaTabla(dataCampo, numInt);
                if (tabla != null)
                {
                    var jDatosTabla = JObject.Parse(dataCampo.DatoCampo);
                    string indiceTexto = jDatosTabla.SelectToken("IndiceRegistro").ToString();
                    int indiceRegistro = 0;
                    string[] columnas = jDatosTabla.SelectToken("Encabezado").ToString().Split(',');
                    if (columnas.Length > 0 && int.TryParse(indiceTexto, out indiceRegistro))
                    {
                        if (tabla.Rows.Count >= indiceRegistro)
                        {
                            string columna = columnas[0];
                            valor = tabla.Rows[indiceRegistro - 1][columna].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return valor;
        }

        private string CampoTipo(string valorOriginal, string tipo)
        {
            gestorLog.Entrar();

            List<string> tipos = new List<string>();
            string valor = valorOriginal.TrimStart().TrimEnd();


            if (tipo.Contains("[") && tipo.Contains("]"))
            {
                tipo = tipo.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                tipos = tipo.Split(',').Select(x => x.Trim()).ToList();
            }
            else
            {
                tipos.Add(tipo);
            }

            CultureInfo culture = new CultureInfo("es-Mx");
            string[] arrayFormats = { "dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy/MM/dd", "yyyy-MM-dd" };

            string resultado = "";
            foreach (string fto in tipos)
            {
                try
                {
                    decimal monto;
                    switch (fto)
                    {
                        //Agregartipo
                        case "Decimal1":
                            decimal.TryParse(valor, out monto);
                            resultado = string.Format("{0:0.0}", monto);
                            break;
                        case "Decimal":
                            decimal.TryParse(valor, out monto);
                            resultado = string.Format("{0,15:N2}", monto);
                            break;
                        case "Decimales":
                            decimal.TryParse(valor, out monto);
                            resultado = ObtieneDecimales(monto, 2);
                            break;
                        case "DecimalTruncado":
                            decimal.TryParse(valor, out monto);
                            resultado = TruncarDecimales(monto, 2);
                            break;
                        case "Monto":
                            decimal.TryParse(valor, out monto);
                            resultado = string.Format("{0:C}", monto);
                            break;
                        case "Fecha":
                            resultado = DateTime.ParseExact(valor.Substring(0, 10), arrayFormats, culture).ToString("dd/MM/yyyy");
                            break;
                        case "FechaGuion":
                            resultado = DateTime.ParseExact(valor.Substring(0, 10), arrayFormats, culture).ToString("dd-MM-yyyy");
                            break;
                        case "FechaFinal":
                            resultado = valor.Substring(0, 10);
                            break;
                        case "FechaIngles":
                            resultado = FechaAjusta(valor.Substring(0, 10));
                            break;
                        case "FechaLarga":
                            resultado = DateTime.ParseExact(ValidarFecha(valor), arrayFormats, culture).ToString("dddd, d  'de' MMMM 'de' yyyy", new CultureInfo("es-MX"));
                            break;
                        case "Dia":
                            resultado = DateTime.ParseExact(ValidarFecha(valor), arrayFormats, culture).Day.ToString("D2");
                            break;
                        case "Mes":
                            resultado = DateTime.ParseExact(ValidarFecha(valor), arrayFormats, culture).Month.ToString("D2");
                            break;
                        case "MesLetra":

                            int mesNumero = 0;
                            int.TryParse(Convert.ToDateTime(valor.Substring(0, 10)).Month.ToString(), out mesNumero);
                            if (mesNumero > 0)
                            {
                                DateTimeFormatInfo mesNombre = CultureInfo.CurrentCulture.DateTimeFormat;
                                string mesNom = mesNombre.GetMonthName(mesNumero);
                                resultado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mesNom);
                            }
                            break;
                        case "DiaLetra":

                            string diaLetra = "";
                            try
                            {
                                DateTime FechaValida = DateTime.Now;
                                FechaValida = DateTime.ParseExact(valor.Substring(0, 10), "dd/MM/yyyy", culture);
                                diaLetra = culture.DateTimeFormat.GetDayName(FechaValida.DayOfWeek);
                            }
                            catch (Exception ex)
                            {
                                diaLetra = $" Error : {valor} : {ex.Message}";
                            }
                            if (string.IsNullOrEmpty(diaLetra)) { diaLetra = $" vacio x  : {valor} "; }
                            resultado = diaLetra;
                            break;
                        case "Anio":

                            resultado = DateTime.ParseExact(valor.Substring(0, 10), arrayFormats, culture).Year.ToString();
                            break;
                        case "MontoLetra":
                            NumberToText numerToText = new NumberToText();
                            monto = Convert.ToDecimal(valor);
                            resultado = numerToText.Convert(monto);
                            break;
                        case "NumeroLetra":
                            NumberToText numerToText2 = new NumberToText();
                            monto = Convert.ToDecimal(valor);
                            resultado = numerToText2.Convert((long)monto).Trim();
                            break;
                        case "Plazo":
                            string Plazo = "";
                            switch (valor)
                            {
                                case "MONTHS":
                                case "30":
                                case "1":
                                    Plazo = "MENSUAL";
                                    break;
                                case "YEARS":
                                case "360":
                                    Plazo = "ANUAL";
                                    break;
                                case "WEEKS":
                                case "7":
                                    Plazo = "SEMANAL";
                                    break;
                                case "DAYS":
                                    Plazo = "DIARIO";
                                    break;
                                case "15":
                                    Plazo = "QUINCENAL";
                                    break;
                                case "14":
                                    Plazo = "CATORCENAL";
                                    break;
                                default:
                                    Plazo = valor;
                                    break;
                            }
                            resultado = Plazo;
                            break;
                        case "PlazoTiempo":
                            string PlazoTiempo = "";
                            switch (valor)
                            {
                                case "MONTHS":
                                case "Mensual":
                                case "30":
                                case "1":
                                    PlazoTiempo = "MESES";
                                    break;
                                case "YEARS":
                                case "Anual":
                                case "360":
                                    PlazoTiempo = "AÑOS";
                                    break;
                                case "Quincenal":
                                case "15":
                                    PlazoTiempo = "QUINCENAS";
                                    break;
                                case "Catorcenal":
                                case "14":
                                    PlazoTiempo = "CATORCENAS";
                                    break;
                                case "WEEKS":
                                case "Semanal":
                                case "7":
                                    PlazoTiempo = "SEMANAS";
                                    break;
                                case "DAYS":
                                    PlazoTiempo = "DIAS";
                                    break;
                                default:
                                    PlazoTiempo = valor;
                                    break;
                            }
                            resultado = PlazoTiempo;
                            break;
                        case "Periodo":
                            string Periodo = "";
                            switch (valor)
                            {
                                case "MONTHS":
                                    Periodo = "MESES";
                                    break;
                                case "YEARS":
                                    Periodo = "AÑOS";
                                    break;
                                case "WEEKS":
                                    Periodo = "SEMANAS";
                                    break;
                                case "DAYS":
                                    Periodo = "DIAS";
                                    break;
                                default:
                                    Periodo = valor;
                                    break;
                            }
                            resultado = Periodo;
                            break;
                        case "FechaOffset":
                            if (valor.Length >= 19)
                            {
                                string sfecha;
                                if (valor.Contains("T"))
                                {
                                    sfecha = valor.Substring(0, 19);
                                }
                                else
                                {
                                    sfecha = valor;
                                }

                                DateTime fechaHoraFinal = System.DateTime.Parse(sfecha);
                                bool isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(fechaHoraFinal);
                                gestorLog.Registrar(Nivel.Debug, $"bevilad:isDaylight_1_:{isDaylight}");
                                //Codigo para eliminar horario de verano desde la fecha dada
                                DateTime FinHorarioVerano = new DateTime(2023, 3, 13);
                                int EsIgualMayorHorarioVerano = DateTime.Compare(fechaHoraFinal, FinHorarioVerano);

                                if ((EsIgualMayorHorarioVerano >= 0))
                                {
                                    isDaylight = false;
                                }
                                gestorLog.Registrar(Nivel.Debug, $"bevilad:isDaylight_2_:{isDaylight}");
                                gestorLog.Registrar(Nivel.Debug, $"bevilad:Compare({fechaHoraFinal},{FinHorarioVerano})={EsIgualMayorHorarioVerano}:{JsonConvert.SerializeObject(TimeZoneInfo.Local)}");
                                //fin

                                if (isDaylight)
                                {
                                    fechaHoraFinal = fechaHoraFinal.AddHours(5);
                                }
                                else { fechaHoraFinal = fechaHoraFinal.AddHours(6); }
                                resultado = fechaHoraFinal.ToString("yyyy-MM-dd");
                                gestorLog.Registrar(Nivel.Debug, $"bevilad:Resultado:{resultado}");
                            }
                            else { resultado = valor; }
                            break;
                        case "FechaCortaDia":
                            string fechacorta = valor;
                            string[] formatos = new string[] { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" };
                            foreach (string f in formatos)
                            {
                                try
                                {
                                    fechacorta = DateTime.ParseExact(valor.Substring(0, 10), f, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                                    break;
                                }
                                catch (Exception)
                                {
                                    fechacorta = valor; ;
                                }
                            }
                            resultado = fechacorta;
                            break;
                        default:
                            resultado = valor;
                            break;
                    }

                    valor = resultado;
                }
                catch (Exception ex)
                {
                    gestorLog.Registrar(Nivel.Information, $"{fto}:[resultado={resultado},valor={valor}]{ex.Message}");
                    gestorLog.RegistrarError(ex);
                    resultado = "";
                }
            }
            gestorLog.Salir();
            return resultado;
        }
        private string FechaAjusta(string valor)
        {
            string resultado = "";
            try
            {
                string[] arrFecha = valor.Split('/');
                if (arrFecha.Length > 0)
                {
                    if (arrFecha[2].Length == 4)
                    {
                        resultado = string.Format("{0}/{1}/{2}", arrFecha[1], arrFecha[0], arrFecha[2]);
                    }
                }
            }
            catch (Exception)
            {
                resultado = valor;
                // throw;
            }
            return resultado;
        }
        private string ObtenerclienteJson(int numInt)
        {
            gestorLog.Entrar();
            string valor = string.Empty;
            var datosCampo = "";
            try
            {
                var jDatos = JObject.Parse(datosJson);
                numInt = numInt - 1;
                if (cliente.Credito.TipoCredito == "G")
                {
                    datosCampo = "AhorroIndividual[" + numInt + "].NumeroCliente";
                }
                else
                {
                    datosCampo = "NumeroCliente";
                }

                valor = jDatos.SelectToken(datosCampo).ToString();

            }
            catch (Exception)
            {

                valor = "";
            }

            gestorLog.Salir();
            return valor;
        }
        private int ObtenerContadorJson(string tipoPlantilla)
        {
            gestorLog.Entrar();
            int contador = 1;

            try
            {

                var jDatos = JObject.Parse(datosJson);

                switch (tipoPlantilla)
                {
                    case "A":
                        contador = jDatos.SelectToken("Adeudos").Count();
                        break;
                    case "O":
                        contador = jDatos.SelectToken("Odp").Count();
                        break;
                    case "D":
                        contador = jDatos.SelectToken("OdpInterciclo").Count();
                        break;
                    case "S":
                        contador = jDatos.SelectToken("SolicitudPrestamo").Count();
                        break;
                    case "T":
                        contador = jDatos.SelectToken("PagoTerceros").Count();
                        break;
                    case "N":
                        contador = jDatos.SelectToken("Interciclo").Count();
                        break;
                    case "L":
                        contador = jDatos.SelectToken("IntercicloLideres").Count();
                        break;
                    case "C":
                        contador = jDatos.SelectToken("IntercicloCaja").Count();
                        break;

                    default:
                        contador = 1;
                        break;
                }


            }
            catch (Exception)
            {

                contador = 1;
            }
            gestorLog.Salir();
            return contador;

        }
        private string ObtenerValorPropiedad(Campo Campo, int numInt)
        {
            gestorLog.Entrar();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");

            if (documSolicitudJsonDatos != null && Campo.DatoConjunto != "Generales")
            {
                Campo.DatoConjunto = Campo.DatoConjunto.ToLower();
                Campo.DatoCampo = Campo.DatoCampo.ToLower();
            }
            string valor = "";
            string conjuntoJson = string.Empty;
            string token = string.Empty;
            string tokenGen = string.Empty;

            JToken General = null;
            try
            {
                string campo = Campo.DatoCampo.Replace("<", "").Replace(">", "");

                var jDatos = JObject.Parse(datosJson);

                if (Campo.Tipo == "Seleccion")
                {
                    var jDatosSolicitud = jDatos.SelectToken("solicitudJSON");

                    if (documSolicitudJson == null)
                    {
                        jDatosSolicitud = jDatos;
                    }

                    var jCampo = JObject.Parse(Campo.DatoCampo);
                    token = Campo.DatoConjunto + (Campo.DatoConjunto == null ? "" : ".") + jCampo.SelectToken("DatoCampo").ToString();
                    string datoselecccion = jDatosSolicitud.SelectToken(token)?.ToString() ?? "";
                    string datosOpciones = jCampo.SelectToken("Opciones").ToString();

                    if ((!string.IsNullOrEmpty(datosOpciones)) && (!string.IsNullOrEmpty(jCampo.SelectToken("TipoOpciones")?.ToString())))
                    {
                        AgregaSoporteTipoOpciones(ref datosOpciones, jCampo.SelectToken("TipoOpciones")?.ToString(), ref datoselecccion);
                    }

                    valor = CampoTipo(datoselecccion, Campo.Tipo, datosOpciones);
                }
                else if (Campo.Tipo == "Predeterminado")
                {
                    var jCampo = JObject.Parse(Campo.DatoCampo);
                    var jDatoConjunto = jDatos.SelectToken(jCampo.SelectToken("DatoConjunto").ToString());
                    string datoselecccion = jCampo.SelectToken("Predeterminado").ToString();
                    string datosOpciones = jCampo.SelectToken("Opciones").ToString();

                    if (string.IsNullOrEmpty(jDatoConjunto?.SelectToken(jCampo.SelectToken("DatoCampo").ToString())?.ToString()))
                    {
                        datoselecccion = string.Empty;
                    }

                    valor = CampoTipo(datoselecccion, Campo.Tipo, datosOpciones);
                }
                else if (Campo.DatoConjunto == "Generales")
                {
                    if (!string.IsNullOrEmpty(datosGenerales))
                    {
                        try
                        {
                            JArray jdata = JArray.Parse(datosGenerales);
                            tokenGen = string.Format("$[?(@.CampoNombre == '{0}')]", Campo.DatoCampo);
                            General = (JToken)jdata.SelectToken(tokenGen);
                        }
                        catch (Exception)
                        {
                            General = null;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(Campo.DatoConjuntoGrupal))
                {
                    if (Campo.DatoConjunto.StartsWith("solicitudJSON."))
                    {
                        if (Campo.DatoConjunto.Contains("[]"))
                        {
                            Campo.DatoConjunto = Campo.DatoConjunto.Replace("[]", $"[{numInt}]");
                        }
                    }
                    token = (Campo.DatoConjunto == "" ? "" : Campo.DatoConjunto + ".") + campo;
                }
                else
                {
                    if (numInt > 0)
                    {
                        numInt = numInt - 1;
                        conjuntoJson = Campo.DatoConjuntoGrupal; // AhorroIndividual[2]
                        conjuntoJson = conjuntoJson.Replace("[]", "[" + numInt.ToString() + "]");
                        token = (conjuntoJson == "" ? "" : conjuntoJson + ".") + campo;
                    }
                    else
                    {
                        token = (Campo.DatoConjunto == "" ? "" : Campo.DatoConjunto + ".") + campo;
                    }
                }
                if (Campo.DatoConjunto == "Generales")
                {
                    if (General != null)
                        valor = (((JValue)General["Valor"]).Value).ToString();
                }
                else
                {
                    gestorLog.Registrar(Nivel.Debug, $"token=[{token}]");
                    if (valor == "")
                    {
                        if (jDatos.SelectToken(token)?.Type == JTokenType.Array)
                        {
                            valor = jDatos.SelectToken(token)?.ToString() ?? "";
                        }
                        else
                        {
                            //valor = (string)jDatos.SelectToken(token) ?? "";
                            valor = jDatos.SelectToken(token)?.ToString() ?? "";
                        }
                    }

                    gestorLog.Registrar(Nivel.Debug, $"valor=[{valor}]");
                }

            }
            catch (Exception ex)
            {
                gestorLog.Registrar(Nivel.Information, $"ObtenerValorPropiedad{valor}:valor=[{valor}]{ex.Message}");
                valor = "";
            }

            gestorLog.Salir();
            return valor;
        }
        private void ObtenerDatosJson()
        {
            gestorLog.Entrar();

            //LAM
            ObtenerDatosDto solicitud = new ObtenerDatosDto
            {
                Empresa = datosDocumento.Empresa,
                Proceso = datosDocumento.ProcesoNombre,
                NumeroCredito = documSolicitud.NumeroCredito,
                NumeroCliente = documSolicitud.NumeroCliente,
                Usuario = documSolicitud.Usuario,
                Subproceso = documSolicitud.SubProcesoNombre
            };


            if (documSolicitud.NumerosDividendos != null || documSolicitud.NumerosClientes != null)
            {

                solicitud = new ObtenerDatosDto
                {
                    Empresa = datosDocumento.Empresa,
                    Proceso = datosDocumento.ProcesoNombre,
                    NumeroCredito = documSolicitud.NumeroCredito,
                    NumeroCliente = documSolicitud.NumeroCliente,
                    Usuario = documSolicitud.Usuario,
                    Subproceso = documSolicitud.SubProcesoNombre,
                    NumerosClientes = documSolicitud.NumerosClientes,
                    NumeroDividendos = documSolicitud.NumerosDividendos

                };
            }

            if (solicitud.Subproceso.ToLower() == "clubcame" || solicitud.Subproceso.ToLower() == "crecemas" || solicitud.Subproceso.ToLower() == "creditogarantizado")
            {
                try
                {
                    factory.ServicioDatosPlantillas.ResetearDatos(solicitud);
                }
                catch (BusinessException buex)
                {
                    Console.WriteLine(buex.Message);
                }

            }


            datosJson = factory.ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            //Se deserializa el objeto para mapear la documentación
            cliente = JsonConvert.DeserializeObject<Cliente>(datosJson, settings);



            gestorLog.Salir();
        }
        private bool CamposFijos(Campo item, WordDocument documentoWord, Plantilla plantilla)
        {
            bool resultado = true;
            string valor = "";
            try
            {
                switch (item.CampoNombre)
                {
                    case "<<RECA>>":
                        valor = plantilla.Reca;
                        break;
                    case "<<FechaActual>>":
                        //valor = DateTime.Now.ToShortDateString();
                        valor = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("es-MX"));
                        break;
                    case "<<HoraActual>>":
                        valor = DateTime.Now.ToLongTimeString();
                        break;
                    case "<<FechaActualLetra>>":
                        DateTimeFormatInfo y = CultureInfo.CurrentCulture.DateTimeFormat;
                        string m = y.GetMonthName(DateTime.Now.Month);
                        valor = DateTime.Now.Day + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m) + " de " + DateTime.Now.Year;
                        break;
                    case "<<FechaActualAnio>>":
                        valor = DateTime.Now.Year.ToString();
                        break;
                    case "<<FechaActualDia>>":
                        valor = DateTime.Now.Day.ToString();
                        break;
                    case "<<FechaActualDiaLetra>>":
                        DateTimeFormatInfo letra = CultureInfo.CurrentCulture.DateTimeFormat;
                        string dia = letra.GetDayName(DateTime.Now.DayOfWeek);
                        valor = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dia);
                        break;
                    case "<<FechaActualMes>>":
                        valor = DateTime.Now.Month.ToString();
                        break;
                    case "<<FechaActualMesLetra>>":
                        DateTimeFormatInfo letraMes = CultureInfo.CurrentCulture.DateTimeFormat;
                        string mes = letraMes.GetMonthName(DateTime.Now.Month);
                        valor = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mes);
                        break;
                    case "<<FechaActualLarga>>":
                        valor = DateTime.Now.ToLongDateString();
                        //valor = DateTime.Now.ToLongDateString();
                        valor = DateTime.Now.ToString("dddd, d  'de' MMMM 'de' yyyy", new CultureInfo("es-MX"));
                        break;
                    case "<<FechaHoyDia>>":
                        valor = DateTime.Now.Day.ToString("D2");
                        break;
                    case "<<FechaHoyMes>>":
                        valor = DateTime.Now.Month.ToString("D2");
                        break;
                    case "<<FechaHoyAnio>>":
                        valor = DateTime.Now.Year.ToString();
                        break;

                    default:
                        resultado = false;
                        break;
                }
                if (valor != "")
                {
                    documentoWord.Replace(item.CampoNombre, valor, true, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                valor = "";
            }
            return resultado;
        }
        private bool CamposEspecialesGrupal(Campo item, WordDocument documentoWord, int numInt, long numVol)
        {
            bool resultado = true;
            string valorClienteBen = string.Empty;
            try
            {
                valorClienteBen = ObtenerclienteJson(numInt);
                int contador = 1;
                string nombreCampoBeneficiario = "";
                string porcentajeCampoBeneficiario = "";
                string parentescoBenef = "";
                string domicilioBenef = "";
                string telefonoBenef = "";
                string fechaNacBenef = "";
                NumberToText numerToText = new NumberToText();

                switch (item.CampoNombre)
                {
                    case "<<BeneficiariosAhorro>>":
                        #region BeneficiariosAhorro
                        nombreCampoBeneficiario = "<<BeneficiarioAhorro";
                        porcentajeCampoBeneficiario = "<<BAPorc";
                        parentescoBenef = "<<BeneficiarioRel";
                        domicilioBenef = "<<BeneficiarioDom";
                        telefonoBenef = "<<BeneficiarioTel";
                        fechaNacBenef = "<<BeneficiarioFechaNac";

                        foreach (var ahorroIndvidual in cliente.AhorroIndividual)
                        {
                            if (ahorroIndvidual.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroIndvidual.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)).Count() == 0)
                                {
                                    documentoWord.Replace("<<BeneficiarioAhorro1>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro2>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BAPorc1>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc2>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc3>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc4>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel1>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel2>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel3>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioDom1>>", "-----------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioDom2>>", "-----------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioTel1>>", "-----", true, true);
                                    documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                                    documentoWord.Replace("<<BeneficiarioFechaNac1>>", "-----------", true, true);
                                    documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------", true, true);

                                }
                                else if (ahorroIndvidual.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)).Count() == 1)
                                {
                                    foreach (
                                    Beneficiario beneficiario in
                                        ahorroIndvidual.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)))
                                    {
                                        documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                        documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                        documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                        if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                            documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                        else
                                            documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                        _ = documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                        if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                            documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                        else
                                            documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                    }

                                    documentoWord.Replace("<<BeneficiarioAhorro2>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BAPorc2>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc3>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc4>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel2>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel3>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioDom2>>", "--------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                                    documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------------", true, true);

                                }
                                else if (ahorroIndvidual.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)).Count() == 2)
                                {
                                    foreach (
                                     Beneficiario beneficiario in
                                         ahorroIndvidual.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)))
                                    {
                                        documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                        documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                        documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                        if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                            documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                        else
                                            documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                        documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                        if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                            documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                        else
                                            documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                        contador++;
                                    }

                                    documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                                    documentoWord.Replace("<<BAPorc3>>", "", true, true);
                                    documentoWord.Replace("<<BAPorc4>>", "", true, true);
                                    documentoWord.Replace("<<BeneficiarioRel3>>", "", true, true);


                                }
                            }

                        }
                        #endregion BeneficiariosAhorro

                        break;
                    case "<<FechaIniBasico>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroBasico != null)
                                {
                                    documentoWord.Replace("<<FechaIniBasico>>", ahorroInd.SeguroBasico.FechaInicio, true, true);
                                }
                                else { documentoWord.Replace("<<FechaIniBasico>>", "", true, true); }
                            }
                        }
                        break;
                    case "<<FechaFinBasico>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroBasico != null)
                                {
                                    documentoWord.Replace("<<FechaFinBasico>>", ahorroInd.SeguroBasico.FechaFin, true, true);
                                }
                                else { documentoWord.Replace("<<FechaFinBasico>>", "", true, true); }
                            }
                        }

                        break;
                    case "<<FechaIniVoluntario>>":

                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.SeguroVoluntario != null)
                            {
                                if (ahorroInd.NumeroCliente == numVol)
                                {
                                    documentoWord.Replace("<<FechaIniVoluntario>>", ahorroInd.SeguroVoluntario.FechaInicio, true, true);
                                    return true;
                                }

                            }
                        }
                        break;

                    case "<<FechaFinVoluntario>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.SeguroVoluntario != null)
                            {
                                if (ahorroInd.NumeroCliente == numVol)
                                {
                                    documentoWord.Replace("<<FechaFinVoluntario>>", ahorroInd.SeguroVoluntario.FechaFin, true, true);
                                    return true;
                                }

                            }
                        }
                        break;
                    case "<<PaqBasico>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroBasico != null)
                                {
                                    documentoWord.Replace("<<PaqBasico>>", "X", true, true);
                                }
                                else { documentoWord.Replace("<<PaqBasico>>", "", true, true); }
                            }
                        }
                        break;
                    case "<<PaqPremium>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroVoluntario != null && ahorroInd.SeguroVoluntario.Tipo == 1)
                                {
                                    documentoWord.Replace("<<PaqPremium>>", "X", true, true);
                                }
                                else { documentoWord.Replace("<<PaqPremium>>", "", true, true); }
                            }
                        }
                        break;
                    case "<<PaqPlatino>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroVoluntario != null && ahorroInd.SeguroVoluntario.Tipo == 2)
                                {
                                    documentoWord.Replace("<<PaqPlatino>>", "X", true, true);
                                }
                                else { documentoWord.Replace("<<PaqPlatino>>", "", true, true); }
                            }
                        }
                        break;
                    case "<<NoCertBasico>>":
                        #region NoCertBasico
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroBasico != null)
                                {
                                    documentoWord.Replace("<<NoCertBasico>>", cliente.SeguroBasico.Certificado, true, true);
                                    if (ahorroInd.Beneficiarios != null && ahorroInd.Beneficiarios.Count() > 0)
                                    {
                                        List<Beneficiario> beneficiarios = ahorroInd.Beneficiarios.Where(b => b.TipoBeneficiario == "S").ToList();
                                        if (beneficiarios != null && beneficiarios.Count > 0)
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].ApellidoPaterno + " " + beneficiarios[0].ApellidoMaterno + " " + beneficiarios[0].Nombre, true, true);
                                            documentoWord.Replace("<<Parentesco1>>", beneficiarios[0].Relacion, true, true);
                                            documentoWord.Replace("<<Porcentaje1>>", beneficiarios[0].Porcentaje.ToString(), true, true);

                                            if (beneficiarios.Count > 1)
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].ApellidoPaterno + " " + beneficiarios[1].ApellidoMaterno + " " + beneficiarios[1].Nombre, true, true);
                                                documentoWord.Replace("<<Parentesco2>>", beneficiarios[1].Relacion, true, true);
                                                documentoWord.Replace("<<Porcentaje2>>", beneficiarios[1].Porcentaje.ToString(), true, true);
                                            }
                                            else
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                                documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                                documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                                            }
                                        }
                                        else
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                            documentoWord.Replace("<<Parentesco1>>", "", true, true);
                                            documentoWord.Replace("<<Porcentaje1>>", "", true, true);
                                            documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                            documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                            documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                                        }

                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                        documentoWord.Replace("<<Parentesco1>>", "", true, true);
                                        documentoWord.Replace("<<Porcentaje1>>", "", true, true);
                                        documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                        documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                        documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                                    }


                                }
                                else { documentoWord.Replace("<<NoCertBasico>>", "", true, true); }
                            }

                        }

                        #endregion NoCertBasico
                        break;
                    case "<<NoCertVoluntario>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == numVol)
                            {
                                if (ahorroInd.SeguroVoluntario != null)
                                {
                                    documentoWord.Replace("<<NoCertVoluntario>>", cliente.SeguroVoluntario.Certificado, true, true);
                                }
                                else { documentoWord.Replace("<<NoCertVoluntario>>", "", true, true); }
                            }
                        }
                        break;

                    case "<<NombreBeneficiarios>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                if (ahorroInd.SeguroBasico != null)
                                {
                                    if (ahorroInd.Beneficiarios != null && ahorroInd.Beneficiarios.Count() > 0)
                                    {
                                        List<Beneficiario> beneficiarios = ahorroInd.Beneficiarios.Where(b => b.TipoBeneficiario == "S").ToList();
                                        if (beneficiarios != null && beneficiarios.Count > 0)
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].NombreCompleto, true, true);

                                            if (beneficiarios.Count > 1)
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].NombreCompleto, true, true);
                                            }
                                            else
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                            }
                                        }
                                        else
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                            documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                        }
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                        documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                    }
                                }
                                else { documentoWord.Replace("<<NoCertBasico>>", "", true, true); }

                            }
                        }
                        break;
                    case "<<NombreCompletoVoluntarios>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.SeguroVoluntario != null)
                            {
                                if (ahorroInd.NumeroCliente == numVol)
                                {
                                    documentoWord.Replace("<<GrupalNombreCompleto>>", ahorroInd.NombreCompleto, true, true);
                                    return true;
                                }

                            }
                        }

                        break;
                    case "<<NombreBeneficiariosVoluntarios>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.SeguroVoluntario != null)
                            {
                                if (ahorroInd.NumeroCliente == numVol)
                                {
                                    if (ahorroInd.Beneficiarios != null && ahorroInd.Beneficiarios.Count() > 0)
                                    {
                                        List<Beneficiario> beneficiarios = ahorroInd.Beneficiarios.Where(b => b.TipoBeneficiario == "S" && b.NumeroCliente == ahorroInd.SeguroVoluntario.ClienteId).ToList();
                                        if (beneficiarios != null && beneficiarios.Count > 0)
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].NombreCompleto, true, true);

                                            if (beneficiarios.Count > 1)
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].NombreCompleto, true, true);
                                            }
                                            else
                                            {
                                                documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                            }
                                            return true;
                                        }
                                        else
                                        {
                                            documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                            documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                        }
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                        documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                    }
                                }

                            }
                        }

                        break;
                    case "<<NombreCompletoContratoBeneficiarios>>":
                        foreach (var ahorroInd in cliente.AhorroIndividual)
                        {
                            if (ahorroInd.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                documentoWord.Replace("<<NombreCompleto>>", ahorroInd.NombreCompleto, true, true);
                                return true;
                            }


                        }
                        break;
                    case "<<ConcentimientoCredito>>":
                        foreach (var ahorro in cliente.AhorroIndividual)
                        {
                            if (ahorro.NumeroCliente == Convert.ToInt64(valorClienteBen))
                            {
                                documentoWord.Replace("<<NumeroCredito>>", cliente.Credito.NumeroCredito, true, true);
                                documentoWord.Replace("<<rol>>", ahorro.Rol, true, true);
                                documentoWord.Replace("<<NumeroCliente>>", Convert.ToString(ahorro.NumeroCliente), true, true);
                                documentoWord.Replace("<<NombreCompleto>>", ahorro.NombreCompleto, true, true);
                                documentoWord.Replace("<<MontoCredito>>", Convert.ToString(ahorro.Monto), true, true);
                                documentoWord.Replace("<<ahorro>>", Convert.ToString(ahorro.Ahorro), true, true);
                                documentoWord.Replace("<<montoLiq>>", Convert.ToString(ahorro.MontoLiq), true, true);
                                documentoWord.Replace("<<basico>>", Convert.ToString(ahorro.Basico), true, true);
                                documentoWord.Replace("<<voluntario>>", Convert.ToString(ahorro.Voluntario), true, true);
                                documentoWord.Replace("<<paquete>>", Convert.ToString(ahorro.Paquete), true, true);
                                documentoWord.Replace("<<montoIni>>", Convert.ToString(ahorro.MontoInicial), true, true);
                                documentoWord.Replace("<<montoEn>>", Convert.ToString(ahorro.MontoRecivir), true, true);
                                documentoWord.Replace("<<plan>>", ahorro.Descripcion, true, true);
                                documentoWord.Replace("<<Vigencia>>", ahorro.Vigencia, true, true);

                                if (ahorro.SeguroBasico.ClienteId == Convert.ToInt64(valorClienteBen))
                                {
                                    documentoWord.Replace("<<paqueteB>>", ahorro.SeguroBasico.Paquete, true, true);
                                    documentoWord.Replace("<<certifB>>", ahorro.SeguroBasico.Certificado, true, true);
                                }
                                if (ahorro.SeguroVoluntario != null)
                                {
                                    documentoWord.Replace("<<paqueteV>>", ahorro.SeguroVoluntario.Paquete, true, true);
                                    documentoWord.Replace("<<certifV>>", ahorro.SeguroVoluntario.Certificado, true, true);

                                }
                                else
                                {
                                    documentoWord.Replace("<<paqueteV>>", "N/A", true, true);
                                    documentoWord.Replace("<<certifV>>", "N/A", true, true);
                                }

                                if (ahorro.Beneficiarios != null && ahorro.Beneficiarios.Count() > 0)
                                {
                                    List<Beneficiario> beneficiarios = ahorro.Beneficiarios.Where(b => b.TipoBeneficiario == "S" && b.NumeroCliente == Convert.ToInt64(valorClienteBen)).ToList();
                                    if (beneficiarios != null && beneficiarios.Count > 0)
                                    {
                                        documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].NombreCompleto, true, true);
                                        documentoWord.Replace("<<Parentesco1>>", beneficiarios[0].Relacion, true, true);
                                        documentoWord.Replace("<<Porcentaje1>>", Convert.ToString(beneficiarios[0].Porcentaje), true, true);

                                        if (beneficiarios.Count > 1)
                                        {
                                            documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].NombreCompleto, true, true);
                                            documentoWord.Replace("<<Parentesco2>>", beneficiarios[1].Relacion, true, true);
                                            documentoWord.Replace("<<Porcentaje2>>", Convert.ToString(beneficiarios[1].Porcentaje), true, true);
                                        }
                                        else
                                        {
                                            documentoWord.Replace("<<Beneficiario2>>", "****************", true, true);
                                            documentoWord.Replace("<<Parentesco2>>", "********", true, true);
                                            documentoWord.Replace("<<Porcentaje2>>", "*******", true, true);
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario1>>", "**************", true, true);
                                        documentoWord.Replace("<<Parentesco1>>", "*********", true, true);
                                        documentoWord.Replace("<<Porcentaje1>>", "********", true, true);
                                        documentoWord.Replace("<<Beneficiario2>>", "************", true, true);
                                        documentoWord.Replace("<<Parentesco2>>", "*****", true, true);
                                        documentoWord.Replace("<<Porcentaje2>>", "*********", true, true);
                                    }
                                }
                                else
                                {
                                    documentoWord.Replace("<<Beneficiario1>>", "***************", true, true);
                                    documentoWord.Replace("<<Parentesco1>>", "**********", true, true);
                                    documentoWord.Replace("<<Porcentaje1>>", "*********", true, true);
                                    documentoWord.Replace("<<Beneficiario2>>", "********", true, true);
                                    documentoWord.Replace("<<Parentesco2>>", "*********", true, true);
                                    documentoWord.Replace("<<Porcentaje2>>", "*******", true, true);
                                }


                            }

                        }
                        break;
                    default:
                        resultado = false;
                        break;
                }

            }
            catch (Exception)
            {


            }
            return resultado;
        }
        private Int64 ObtenerClienteVoluntario(int numInt)
        {
            Int64 respuesta = 0;
            int contador = 1;
            try
            {
                List<Seguro> seguros = new List<Seguro>();
                foreach (var item in cliente.AhorroIndividual)
                {
                    if (item.SeguroVoluntario != null)
                    {
                        Seguro segIndividual = new Seguro();
                        segIndividual.ClienteId = item.NumeroCliente;
                        segIndividual.Tipo = contador;
                        seguros.Add(segIndividual);
                        contador = contador + 1;
                    }
                }
                respuesta = seguros[numInt - 1].ClienteId;

            }
            catch (Exception)
            {

                respuesta = 0;
            }
            return respuesta;

        }
        private bool CamposEspeciales(Campo item, WordDocument documentoWord, int numInt, long numVol)
        {

            bool resultado = true;
            string valorClienteBen = string.Empty;
            try
            {
                if (datosDocumento.Empresa != "TCR")
                {
                    if (datosDocumento.SubProcesoNombre != "CAME_GRUPAL" && datosDocumento.SubProcesoNombre != "CAME_INTERCICLO" && datosDocumento.SubProcesoNombre != "Recibo" && datosDocumento.SubProcesoNombre != "SEMANA_0")
                    {
                        return false;
                    }
                }

                int contador = 1;
                string nombreCampoBeneficiario = "";
                string porcentajeCampoBeneficiario = "";
                string parentescoBenef = "";
                string domicilioBenef = "";
                string telefonoBenef = "";
                string fechaNacBenef = "";
                string valor = "";
                NumberToText numerToText = new NumberToText();
                string montoLetraT = "";
                string[] temporalT;
                string centavosT = "";
                string[] tempCentavosT;
                string valortemp = "";
                string montoLetraI;
                string[] temporalI;

                switch (item.CampoNombre)
                {
                    case "<<BeneficiariosAhorro>>":
                        #region BeneficiariosAhorro
                        //Se valida si es integrante grupal a individual
                        if (numInt > 0)
                        {
                            if (CamposEspecialesGrupal(item, documentoWord, numInt, numVol) == false)
                            {
                                return false;
                            }

                            return true;
                        }

                        nombreCampoBeneficiario = "<<BeneficiarioAhorro";
                        porcentajeCampoBeneficiario = "<<BAPorc";
                        parentescoBenef = "<<BeneficiarioRel";
                        domicilioBenef = "<<BeneficiarioDom";
                        telefonoBenef = "<<BeneficiarioTel";
                        fechaNacBenef = "<<BeneficiarioFechaNac";

                        if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A").Count() == 0)
                        {
                            documentoWord.Replace("<<BeneficiarioAhorro1>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro2>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BAPorc1>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc2>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel1>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel2>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom1>>", "-----------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom2>>", "-----------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioTel1>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac1>>", "-----------", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A").Count() == 1)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                _ = documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                            }

                            documentoWord.Replace("<<BeneficiarioAhorro2>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BAPorc2>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel2>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom2>>", "--------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A").Count() == 2)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }

                            documentoWord.Replace("<<BeneficiarioAhorro3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BAPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BAPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A").Count() == 3)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A" && d.NumeroCliente == Convert.ToInt64(valorClienteBen)))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }

                            documentoWord.Replace("<<BeneficiarioAhorro4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BAPorc4>>", "-----", true, true);
                        }
                        else
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "A"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }
                        }


                        #endregion BeneficiariosAhorro
                        break;
                    case "<<BeneficiariosCredito>>":
                        #region BeneficiariosCredito
                        nombreCampoBeneficiario = "<<BeneficiarioCredito";
                        porcentajeCampoBeneficiario = "<<BCPorc";
                        parentescoBenef = "<<BeneficiarioRel";
                        domicilioBenef = "<<BeneficiarioDom";
                        telefonoBenef = "<<BeneficiarioTel";
                        fechaNacBenef = "<<BeneficiarioFechaNac";

                        if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S").Count() == 0)
                        {
                            documentoWord.Replace("<<BeneficiarioCredito1>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito2>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BCPorc1>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc2>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel1>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel2>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom1>>", "-----------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom2>>", "-----------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioTel1>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac1>>", "-----------", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S").Count() == 1)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                            }

                            documentoWord.Replace("<<BeneficiarioCredito2>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BCPorc2>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel2>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                            documentoWord.Replace("<<BeneficiarioDom2>>", "--------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioTel2>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioFechaNac2>>", "-----------------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S").Count() == 2)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }

                            documentoWord.Replace("<<BeneficiarioCredito3>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BeneficiarioCredito4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BCPorc3>>", "-----", true, true);
                            documentoWord.Replace("<<BCPorc4>>", "-----", true, true);
                            documentoWord.Replace("<<BeneficiarioRel3>>", "---------", true, true);
                        }
                        else if (cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S").Count() == 3)
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }

                            documentoWord.Replace("<<BeneficiarioCredito4>>", "----------------------------", true, true);
                            documentoWord.Replace("<<BCPorc4>>", "-----", true, true);
                        }
                        else
                        {
                            foreach (
                                Beneficiario beneficiario in
                                    cliente.Beneficiarios.Where(d => d.TipoBeneficiario == "S"))
                            {
                                documentoWord.Replace(nombreCampoBeneficiario + contador + ">>", beneficiario.NombreCompleto, true, true);
                                documentoWord.Replace(porcentajeCampoBeneficiario + contador + ">>", beneficiario.Porcentaje.ToString() + " %", true, true);
                                documentoWord.Replace(parentescoBenef + contador + ">>", beneficiario.Relacion, true, true);

                                if (string.IsNullOrEmpty(beneficiario.DireccionCompleta) || beneficiario.DireccionCompleta.Contains("NO DEFINIDA"))
                                    documentoWord.Replace(domicilioBenef + contador + ">>", "", true, true);
                                else
                                    _ = documentoWord.Replace(domicilioBenef + contador + ">>", beneficiario.DireccionCompleta, true, true);

                                _ = documentoWord.Replace(telefonoBenef + contador + ">>", beneficiario.Telefono ?? "", true, true);

                                if (beneficiario.FechaNacimiento == "01/01/1900" || beneficiario.FechaNacimiento == "01/01/0001")
                                    _ = documentoWord.Replace(fechaNacBenef + contador + ">>", "", true, true);
                                else
                                    _ = documentoWord.Replace(fechaNacBenef + contador + ">>", beneficiario.FechaNacimiento, true, true);
                                contador++;
                            }
                        }
                        #endregion BeneficiariosCredito
                        break;
                    case "<<Obligados>>":
                        #region Obligados
                        var listaAvales1 = (from a in cliente.Avales
                                            select new
                                            {
                                                a.NumeroCliente,
                                                a.NombreCompleto,
                                                a.Nombre,
                                                a.ApellidoPaterno,
                                                a.ApellidoMaterno,
                                                a.DireccionCompleta,
                                                a.Telefono,
                                                a.CURP,
                                                a.RFC,
                                                a.FechaNacimiento,
                                                a.Nacionalidad,
                                                a.TipoAval
                                            }).Where(a => a.TipoAval == "OBLIGADO SOLIDARIO").Distinct().ToList();
                        var conyuge1 = (from a in cliente.Avales
                                        select new
                                        {
                                            a.NumeroCliente,
                                            a.NombreCompleto,
                                            a.Nombre,
                                            a.ApellidoPaterno,
                                            a.ApellidoMaterno,
                                            a.DireccionCompleta,
                                            a.Telefono,
                                            a.CURP,
                                            a.RFC,
                                            a.FechaNacimiento,
                                            a.Nacionalidad,
                                            a.TipoAval
                                        }).Where(a => a.TipoAval == "CONYUGE").Distinct().FirstOrDefault();
                        bool aval1, aval2, aval3;
                        aval1 = aval2 = aval3 = false;
                        switch (listaAvales1.Count())
                        {
                            case 0:
                                aval1 = false;
                                aval2 = false;
                                aval3 = false;
                                break;
                            case 1:
                                aval1 = true;
                                aval2 = false;
                                aval3 = false;
                                break;
                            case 2:
                                aval1 = true;
                                aval2 = true;
                                aval3 = false;
                                break;
                            case 3:
                                aval1 = true;
                                aval2 = true;
                                aval3 = true;
                                break;
                            case 4:
                                aval1 = true;
                                aval2 = true;
                                aval3 = true;
                                break;
                        }
                        //Nombres de los avales
                        documentoWord.Replace("<<NombreObligado1>>", conyuge1 != null ? conyuge1.NombreCompleto : "", true, true);
                        documentoWord.Replace("<<NombreObligado2>>", aval1 ? listaAvales1[0].NombreCompleto : "NO APLICA", true, true);
                        documentoWord.Replace("<<NombreObligado3>>", aval2 ? listaAvales1[1].NombreCompleto : "NO APLICA", true, true);
                        documentoWord.Replace("<<NombreObligado4>>", aval3 ? listaAvales1[2].NombreCompleto : "NO APLICA", true, true);
                        //Datos completos de los avales
                        //1
                        documentoWord.Replace("<<NC1>>", conyuge1 != null ? conyuge1.NumeroCliente.ToString() : "", true, true);
                        documentoWord.Replace("<<NObl1>>", conyuge1 != null ? conyuge1.Nombre : "", true, true);
                        documentoWord.Replace("<<APObl1>>", conyuge1 != null ? conyuge1.ApellidoPaterno : "", true, true);
                        documentoWord.Replace("<<AMObl1>>", conyuge1 != null ? conyuge1.ApellidoMaterno : "", true, true);
                        documentoWord.Replace("<<DCObl1>>", conyuge1 != null ? conyuge1.DireccionCompleta : "", true, true);
                        documentoWord.Replace("<<TObl1>>", conyuge1 != null ? conyuge1.Telefono : "", true, true);
                        documentoWord.Replace("<<CURPObl1>>", conyuge1 != null ? conyuge1.CURP : "", true, true);
                        documentoWord.Replace("<<RFCObl1>>", conyuge1 != null ? conyuge1.RFC : "", true, true);
                        documentoWord.Replace("<<FNObl1>>", conyuge1 != null ? conyuge1.FechaNacimiento.Substring(0, 10) : "", true, true);
                        documentoWord.Replace("<<NacObl1>>", conyuge1 != null ? conyuge1.Nacionalidad : "", true, true);
                        //2
                        documentoWord.Replace("<<NC2>>", aval1 ? listaAvales1[0].NumeroCliente.ToString() : "", true, true);
                        documentoWord.Replace("<<NObl2>>", aval1 ? listaAvales1[0].Nombre : "", true, true);
                        documentoWord.Replace("<<APObl2>>", aval1 ? listaAvales1[0].ApellidoPaterno : "", true, true);
                        documentoWord.Replace("<<AMObl2>>", aval1 ? listaAvales1[0].ApellidoMaterno : "", true, true);
                        documentoWord.Replace("<<DCObl2>>", aval1 ? listaAvales1[0].DireccionCompleta : "", true, true);
                        documentoWord.Replace("<<TObl2>>", aval1 ? listaAvales1[0].Telefono : "", true, true);
                        documentoWord.Replace("<<CURPObl2>>", aval1 ? listaAvales1[0].CURP : "", true, true);
                        documentoWord.Replace("<<RFCObl2>>", aval1 ? listaAvales1[0].RFC : "", true, true);
                        documentoWord.Replace("<<FNObl2>>", aval1 ? listaAvales1[0].FechaNacimiento.Substring(0, 10) : "", true, true);
                        documentoWord.Replace("<<NacObl2>>", aval1 ? listaAvales1[0].Nacionalidad : "", true, true);
                        //3
                        documentoWord.Replace("<<NC3>>", aval2 ? listaAvales1[1].NumeroCliente.ToString() : "", true, true);
                        documentoWord.Replace("<<NObl3>>", aval2 ? listaAvales1[1].Nombre : "", true, true);
                        documentoWord.Replace("<<APObl3>>", aval2 ? listaAvales1[1].ApellidoPaterno : "", true, true);
                        documentoWord.Replace("<<AMObl3>>", aval2 ? listaAvales1[1].ApellidoMaterno : "", true, true);
                        documentoWord.Replace("<<DCObl3>>", aval2 ? listaAvales1[1].DireccionCompleta : "", true, true);
                        documentoWord.Replace("<<TObl3>>", aval2 ? listaAvales1[1].Telefono : "", true, true);
                        documentoWord.Replace("<<CURPObl3>>", aval2 ? listaAvales1[1].CURP : "", true, true);
                        documentoWord.Replace("<<RFCObl3>>", aval2 ? listaAvales1[1].RFC : "", true, true);
                        documentoWord.Replace("<<FNObl3>>", aval2 ? listaAvales1[1].FechaNacimiento.Substring(0, 10) : "", true, true);
                        documentoWord.Replace("<<NacObl3>>", aval2 ? listaAvales1[1].Nacionalidad : "", true, true);
                        //4
                        documentoWord.Replace("<<NC4>>", aval3 ? listaAvales1[2].NumeroCliente.ToString() : "", true, true);
                        documentoWord.Replace("<<NObl4>>", aval3 ? listaAvales1[2].Nombre : "", true, true);
                        documentoWord.Replace("<<APObl4>>", aval3 ? listaAvales1[2].ApellidoPaterno : "", true, true);
                        documentoWord.Replace("<<AMObl4>>", aval3 ? listaAvales1[2].ApellidoMaterno : "", true, true);
                        documentoWord.Replace("<<DCObl4>>", aval3 ? listaAvales1[2].DireccionCompleta : "", true, true);
                        documentoWord.Replace("<<TObl4>>", aval3 ? listaAvales1[2].Telefono : "", true, true);
                        documentoWord.Replace("<<CURPObl4>>", aval3 ? listaAvales1[2].CURP : "", true, true);
                        documentoWord.Replace("<<RFCObl4>>", aval3 ? listaAvales1[2].RFC : "", true, true);
                        documentoWord.Replace("<<FNObl4>>", aval3 ? listaAvales1[2].FechaNacimiento.Substring(0, 10) : "", true, true);
                        documentoWord.Replace("<<NacObl4>>", aval3 ? listaAvales1[2].Nacionalidad : "", true, true);
                        #endregion Obligados
                        break;

                    case "<<Avales>>":
                        #region Avales

                        DataTable tablaAvales = new DataTable
                        {
                            //Sets table name as Employees for template mergefield reference.
                            TableName = "Avales"
                        };
                        //Se crean las columnas de la tabla de avales
                        tablaAvales.Columns.Add(new DataColumn("NombreCompleto", typeof(string)));
                        tablaAvales.Columns.Add(new DataColumn("Direccion", typeof(string)));
                        var listaAvales = (from a in cliente.Avales
                                           select new
                                           {
                                               a.NombreCompleto,
                                               a.DireccionCompleta,
                                               a.TipoAval
                                           }).Where(a => a.TipoAval == "OBLIGADO SOLIDARIO").Distinct().ToList();

                        var conyuge = (from a in cliente.Avales
                                       select new
                                       {
                                           a.NombreCompleto,
                                           a.DireccionCompleta,
                                           a.TipoAval
                                       }).Where(a => a.TipoAval == "CONYUGE").Distinct().FirstOrDefault();

                        //Se crea ciclo que recorre la lista de avales
                        if (listaAvales.Count() == 0)
                        {
                            //En caso de no existir avales, se quitan los campos adicionales que imprimen su información.
                            documentoWord.Replace("«NombreCompleto»", "NO APLICA", true, true);
                            documentoWord.Replace("«Direccion»", "NO APLICA", true, true);
                            documentoWord.Replace("«TableStart:Avales»", "", true, false);
                            documentoWord.Replace("«TableEnd:Avales»", "", true, false);
                        }
                        else
                        {
                            foreach (var itemAval in listaAvales)
                            {
                                //Se crea la fila de la tabla
                                DataRow filaAval = tablaAvales.NewRow();
                                //Se llena la tabla con información
                                filaAval["NombreCompleto"] = itemAval.NombreCompleto;
                                filaAval["Direccion"] = itemAval.DireccionCompleta;
                                tablaAvales.Rows.Add(filaAval);
                            }
                        }

                        if (conyuge != null)
                        {
                            DataTable tablaTestigo = new DataTable
                            {
                                //Sets table name as Employees for template mergefield reference.
                                TableName = "Testigo"
                            };
                            //Se crean las columnas de la tabla de avales
                            tablaTestigo.Columns.Add(new DataColumn("NombreCompletoTestigo", typeof(string)));
                            tablaTestigo.Columns.Add(new DataColumn("DireccionTestigo", typeof(string)));

                            //Se crea la fila de la tabla
                            DataRow filaTestigo = tablaTestigo.NewRow();
                            //Se llena la tabla con información
                            filaTestigo["NombreCompletoTestigo"] = conyuge.NombreCompleto;
                            filaTestigo["DireccionTestigo"] = conyuge.DireccionCompleta;
                            tablaTestigo.Rows.Add(filaTestigo);
                            documentoWord.MailMerge.ClearFields = true;
                            documentoWord.MailMerge.ExecuteGroup(tablaTestigo);
                        }
                        else
                        {
                            //Se quita tabla de testigo, solo para individuales
                            documentoWord.Replace("«NombreCompletoTestigo»", "NO APLICA", true, true);
                            documentoWord.Replace("«DireccionTestigo»", "NO APLICA", true, true);
                            documentoWord.Replace("«TableStart:Testigo»", "", true, false);
                            documentoWord.Replace("«TableEnd:Testigo»", "", true, false);
                        }

                        //Executes Mail Merge with groups.
                        documentoWord.MailMerge.ExecuteGroup(tablaAvales);

                        #endregion Avales
                        break;
                    case "<<EstadoCuenta>>":
                        //Lulu
                        #region EstadoCuenta
                        //documentoWord.Replace("«TotPagos»", string.Format("{0, 15:N2}", Convert.ToDecimal(estadoCuenta.TotPagos)), true, true);
                        documentoWord.Replace("«TotPagos»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotPagos)), true, true);
                        documentoWord.Replace("«TotCargos»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotCargos)), true, true);
                        documentoWord.Replace("«TotCapital»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotCapital)), true, true);
                        documentoWord.Replace("«TotComisionMora»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotComisionMora)), true, true);
                        documentoWord.Replace("«TotInteresMora»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotInteresMora)), true, true);
                        documentoWord.Replace("«TotInteres»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotInteres)), true, true);
                        documentoWord.Replace("«TotIVA»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotIVA)), true, true);
                        documentoWord.Replace("«TotSeguros»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotSeguros)), true, true);
                        documentoWord.Replace("«TotGastoSupervision»", estadoCuenta.TotGastoSupervision != "N/A" ? string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotGastoSupervision)) : estadoCuenta.TotGastoSupervision, true, true);
                        documentoWord.Replace("«TotSaldo»", string.Format("{0:C}", Convert.ToDecimal(estadoCuenta.TotSaldo)), true, true);
                        #endregion EstadoCuenta
                        break;
                    case "<<TablaAmortizacionBC>>":
                        #region TablaAmortizacionBC                   
                        DataTable dividendosBC = new DataTable
                        {
                            TableName = "Dividendos"
                        };

                        dividendosBC.Columns.Add("NumeroDividendo", typeof(string));
                        dividendosBC.Columns.Add("FechaInicioDividendo", typeof(string));
                        dividendosBC.Columns.Add("FechaFinalDividendo", typeof(string));
                        dividendosBC.Columns.Add("AhorroDividendo", typeof(string));
                        dividendosBC.Columns.Add("CuotaDividendo", typeof(string));
                        dividendosBC.Columns.Add("CapitalDividendo", typeof(string));
                        dividendosBC.Columns.Add("InteresDividendo", typeof(string));
                        dividendosBC.Columns.Add("IvaDividendo", typeof(string));
                        dividendosBC.Columns.Add("SeguroDividendo", typeof(string));
                        dividendosBC.Columns.Add("Comision", typeof(string));
                        dividendosBC.Columns.Add("IvaComision", typeof(string));
                        dividendosBC.Columns.Add("GastoSupervisionDividendo", typeof(string));
                        dividendosBC.Columns.Add("SaldoDividendo", typeof(string));
                        dividendosBC.Columns.Add("Hola", typeof(string));
                        dividendosBC.Rows.Add(0, '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-',
                                 cliente.Credito.Dividendos.ToArray()[0].Saldo.ToString("N2"));

                        foreach (var div in cliente.Credito.Dividendos)
                        {
                            decimal totalAPagar = div.Capital + div.IntereS + div.Iva;
                            dividendosBC.Rows.Add(div.NumeroDividendo,
                                div.FechaInicio,
                                CampoTipo(div.FechaVencimiento, "FechaCortaDia"),
                                Decimal.Parse(div.Ahorro).ToString("N2"), /*div.Cuota.ToString("N2")*/ totalAPagar.ToString("N2"), div.Capital.ToString("N2"),
                                div.IntereS.ToString("N2"), div.Iva.ToString("N2"), div.Seguros.ToString("N2"), div.Comision.ToString("N2"), div.ComisionIva.ToString("N2"),
                                div.GastoSupervision.ToString("N2"), (div.Saldo - div.Capital).ToString("N2"));
                        }
                        //Se mapea la tabla con el documento
                        documentoWord.MailMerge.ClearFields = true;
                        documentoWord.MailMerge.ExecuteGroup(dividendosBC);
                        //Se revisa que no queden variables visibles
                        documentoWord.Replace("«NumeroDividendo»", "", true, true);
                        documentoWord.Replace("«FechaInicioDividendo»", "", true, true);
                        documentoWord.Replace("«FechaFinalDividendo»", "", true, true);
                        documentoWord.Replace("«AhorroDividendo»", "", true, true);
                        documentoWord.Replace("«CuotaDividendo»", "", true, true);
                        documentoWord.Replace("«CapitalDividendo»", "", true, true);
                        documentoWord.Replace("«InteresDividendo»", "", true, true);
                        documentoWord.Replace("«IvaDividendo»", "", true, true);
                        documentoWord.Replace("«SeguroDividendo»", "", true, true);
                        documentoWord.Replace("«GastoSupervisionDividendo»", "", true, true);
                        documentoWord.Replace("«SaldoDividendo»", "", true, true);
                        #endregion TablaAmortizacionBC    
                        break;
                    case "<<TablaAmortizacionPagare>>":
                        #region TablaAmortizacionPagare                       
                        int totalDividendos = cliente.Credito.Dividendos.Count();
                        int registrosTablas = Convert.ToInt32(decimal.Round((totalDividendos / 3)));
                        string totalDividendosTmp = (Convert.ToDecimal(totalDividendos) / 3).ToString();
                        string[] totalDivision = totalDividendosTmp.Split('.');
                        if (totalDivision.Count() == 2)
                            registrosTablas = Convert.ToDecimal(totalDivision[1]) > 0
                                ? registrosTablas + 1
                               : registrosTablas;
                        List<Dividendo> dividendo1 =
                            cliente.Credito.Dividendos.Where(d => d.NumeroDividendo <= registrosTablas).ToList();
                        List<Dividendo> dividendo2 =
                            cliente.Credito.Dividendos.Where(
                                d =>
                                    d.NumeroDividendo > registrosTablas &&
                                    d.NumeroDividendo <= (registrosTablas * 2))
                                .ToList();
                        List<Dividendo> dividendo3 =
                            cliente.Credito.Dividendos.Where(d => d.NumeroDividendo > (registrosTablas * 2))
                                .ToList();
                        DataTable dividendos = new DataTable
                        {
                            TableName = "Dividendos"
                        };

                        dividendos.Columns.Add("Fecha1", typeof(string));
                        dividendos.Columns.Add("Cuota1", typeof(string));
                        dividendos.Columns.Add("Fecha2", typeof(string));
                        dividendos.Columns.Add("Cuota2", typeof(string));
                        dividendos.Columns.Add("Fecha3", typeof(string));
                        dividendos.Columns.Add("Cuota3", typeof(string));

                        for (int i = 0; i < registrosTablas; i++)
                        {

                            decimal totalAPagar1 = dividendo1[i].Capital + dividendo1[i].IntereS + dividendo1[i].Iva;
                            decimal totalAPagar2 = dividendo2[i].Capital + dividendo2[i].IntereS + dividendo2[i].Iva;

                            decimal totalAPagar3 = 0;
                            if(dividendo3.Count > i)
                                totalAPagar3 = dividendo3[i].Capital + dividendo3[i].IntereS + dividendo3[i].Iva;


                            string fecha1 = CampoTipo(dividendo1[i].FechaVencimiento, "FechaCortaDia");
                            string cuota1 = string.Format("{0:N2}", /*Convert.ToDecimal(dividendo1[i].Cuota)*/ totalAPagar1);
                            string fecha2 = CampoTipo(dividendo2[i].FechaVencimiento, "FechaCortaDia");
                            string cuota2 = string.Format("{0:N2}", /*Convert.ToDecimal(dividendo2[i].Cuota)*/totalAPagar2);
                            string fecha3 = dividendo3.Count > i
                                ? CampoTipo(dividendo3[i].FechaVencimiento, "FechaCortaDia") : string.Empty;
                            string cuota3 = dividendo3.Count > i
                                ? string.Format("{0:N2}", /*dividendo3[i].Cuota*/totalAPagar3)
                               : string.Empty;
                            dividendos.Rows.Add(fecha1, cuota1, fecha2, cuota2, fecha3, cuota3);
                        }
                        //Se mapea la tabla con el documento
                        documentoWord.MailMerge.ClearFields = true;
                        documentoWord.MailMerge.ExecuteGroup(dividendos);
                        //Se revisa que no queden variables visibles
                        documentoWord.Replace("«Fecha1»", "", true, true);
                        documentoWord.Replace("«Cuota1»", "", true, true);
                        documentoWord.Replace("«Fecha2»", "", true, true);
                        documentoWord.Replace("«Cuota2»", "", true, true);
                        documentoWord.Replace("«Fecha3»", "", true, true);
                        documentoWord.Replace("«Cuota3»", "", true, true);
                        #endregion TablaAmortizacionPagare                        
                        break;
                    case "<<TablaAmortizacion>>":
                        #region TablaAmortizacion

                        dividendos = new DataTable();
                        decimal totalAhorro = 0;
                        dividendos.TableName = "Dividendos";

                        dividendos.Columns.Add("NumeroDividendo", typeof(string));
                        dividendos.Columns.Add("FechaInicioDividendo", typeof(string));
                        dividendos.Columns.Add("FechaFinalDividendo", typeof(string));
                        dividendos.Columns.Add("AhorroDividendo", typeof(string));
                        dividendos.Columns.Add("CuotaDividendo", typeof(string));
                        dividendos.Columns.Add("CapitalDividendo", typeof(string));
                        dividendos.Columns.Add("InteresDividendo", typeof(string));
                        dividendos.Columns.Add("IvaDividendo", typeof(string));
                        dividendos.Columns.Add("SeguroDividendo", typeof(string));
                        dividendos.Columns.Add("GastoSupervisionDividendo", typeof(string));
                        dividendos.Columns.Add("SaldoDividendo", typeof(string));

                        List<Dividendo> dividendo = cliente.Credito.Dividendos.ToList();
                        for (int i = 0; i < cliente.Credito.Dividendos.Count(); i++)
                        {
                            string numeroDividendo = dividendo[i].NumeroDividendo.ToString();
                            string fechaInicioDividendo = CampoTipo(dividendo[i].FechaInicio, "FechaCortaDia");
                            string fechaFinalDividendo = CampoTipo(dividendo[i].FechaVencimiento, "FechaCortaDia");
                            string cuotaDividendo = string.Format("{0:N2}", dividendo[i].Cuota);
                            string capitalDividendo = string.Format("{0:N2}", dividendo[i].Capital);
                            string interesDividendo = string.Format("{0:N2}", dividendo[i].IntereS);
                            string ivaDividendo = string.Format("{0:N2}", dividendo[i].Iva);
                            string seguroDividendo = string.Format("{0:N2}", dividendo[i].Seguros);
                            string gastoSupervisionDividendo = dividendo[i].GastoSupervision == 0 ? "N/A" : string.Format("{0:N2}", dividendo[i].GastoSupervision);
                            string saldoDividendo = string.Format("{0:N2}", dividendo[i].Saldo);
                            //Ahorro dividendo
                            string ahorroDividendo = string.Empty;
                            //Se verifica el contenido de la columna de ahorro                            
                            if (dividendo[i].Ahorro == "N/A")
                            {
                                ahorroDividendo = dividendo[i].Ahorro;
                            }
                            else
                            {
                                decimal aho = Convert.ToDecimal(dividendo[i].Ahorro);
                                totalAhorro += aho;
                                ahorroDividendo = string.Format("{0:N2}", aho);
                            }
                            //Se agregan a la tabla el dividendo
                            dividendos.Rows.Add(numeroDividendo, fechaInicioDividendo, fechaFinalDividendo, ahorroDividendo, cuotaDividendo,
                                capitalDividendo, interesDividendo, ivaDividendo, seguroDividendo, gastoSupervisionDividendo, saldoDividendo);
                        }
                        //Se mapea la tabla al documento
                        documentoWord.MailMerge.ClearFields = true;
                        documentoWord.MailMerge.ExecuteGroup(dividendos);

                        ////////if (!cliente.Credito.Dividendos.Exists(d => d.GastoSupervisionDividendo > 0))
                        ////////{
                        ////////    IWTable tablaW = documentoWord.Sections[0].Tables[1];

                        ////////    for (int i = 0; i < tablaW.Rows.Count; i++)
                        ////////    {
                        ////////        tablaW.Rows[i].Cells.RemoveAt(8);
                        ////////    }
                        ////////}

                        //Se mapean los totales
                        documentoWord.Replace("<<MontoTotalAhorro>>", string.Format("{0:C}", totalAhorro), true, true);
                        decimal tot = totalAhorro + cliente.Credito.MontoTotal;
                        documentoWord.Replace("<<MontoTotalDepositar>>", string.Format("{0:C}", tot), true, true);

                        #endregion TablaAmortizacion
                        break;

                    case "<<CodigoBarras>>":
                        WSection section = documentoWord.Sections[0];
                        HeaderFooter header = section.HeadersFooters.Header;
                        IWParagraph par = header.AddParagraph();
                        IWTextRange hTextCode = par.AppendText(cliente.CodigoBarras);
                        hTextCode.CharacterFormat.FontName = "Code 128";
                        hTextCode.CharacterFormat.FontSize = 40;
                        break;

                    case "<<CodigoBarrasTicket>>":
                        IWTable tabla = documentoWord.Sections[0].Tables[0];
                        WTableRow fila = tabla.Rows[0];
                        IWParagraph parrafo = fila.Cells[0].AddParagraph();
                        IWTextRange txt = parrafo.AppendText(cliente.CodigoBarras);
                        txt.CharacterFormat.FontName = "Code 128";
                        txt.CharacterFormat.FontSize = 30;
                        txt = null;
                        fila = null;
                        parrafo = null;
                        tabla = null;
                        break;

                    case "<<CodeBR>>":
                        IWTable tablaBR = documentoWord.Sections[0].Tables[0];
                        WTableRow filaBR = tablaBR.Rows[0];
                        IWParagraph parrafoBR = filaBR.Cells[0].AddParagraph();
                        IWTextRange txtBR = parrafoBR.AppendText(Code128(datosCD));
                        txtBR.CharacterFormat.FontName = "Code 128";
                        txtBR.CharacterFormat.FontSize = 30;
                        txtBR = null;
                        filaBR = null;
                        parrafoBR = null;
                        tablaBR = null;
                        break;

                    case "<<CodeQR>>":
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(datosQR, QRCodeGenerator.ECCLevel.Q);

                        Base64QRCode qrCode = new Base64QRCode(qrCodeData);
                        var imgQR = Base64QRCode.ImageType.Jpeg;
                        string imgenQR = qrCode.GetGraphic(50, Color.Black, Color.White, true, imgQR);
                        byte[] byteImg = Convert.FromBase64String(imgenQR);

                        IWTable tablaQR = documentoWord.Sections[0].Tables[0];
                        WTableRow filaQR = tablaQR.Rows[0];
                        IWParagraph parrafoQR = filaQR.Cells[0].AddParagraph();
                        MemoryStream strImagen = new MemoryStream(byteImg);
                        IWPicture img = parrafoQR.AppendPicture(strImagen);
                        img.Height = 100;
                        img.Width = 100;

                        filaQR = null;
                        parrafoQR = null;
                        tabla = null;
                        break;

                    case "<<ReferenciaWalmartBarras>>":

                        IWTable tabla2 = documentoWord.Sections[0].Tables[3];

                        WTableRow fila3 = tabla2.Rows[1];
                        IWParagraph parrafo3 = fila3.Cells[0].AddParagraph();
                        parrafo3.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        IWTextRange txt3 = parrafo3.AppendText(cliente.Credito.ReferenciaWalmart);
                        txt3.CharacterFormat.FontName = "Arial";
                        txt3.CharacterFormat.FontSize = 9;
                        fila3 = null;
                        parrafo3 = null;
                        txt3 = null;

                        WTableRow fila2 = tabla2.Rows[2];
                        IWParagraph parrafo2 = fila2.Cells[0].AddParagraph();
                        parrafo2.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        IWTextRange txt2 = parrafo2.AppendText(cliente.Credito.CodigoBarrasWalmart);
                        txt2.CharacterFormat.FontName = "Code 128";
                        txt2.CharacterFormat.FontSize = 30;
                        txt2 = null;
                        fila2 = null;
                        parrafo2 = null;
                        tabla2 = null;
                        break;
                    case "<<DatosObligado>>":
                        #region DatosObligado
                        if (cliente.Avales.Count() > 0)
                        {
                            //string fechaNacDia = cliente.Avales[0].FechaNacimiento.Day.ToString();
                            //string fechaNacMes = cliente.Avales[0].FechaNacimiento.Month.ToString();
                            //string fechaNacAnio = cliente.Avales[0].FechaNacimiento.Year.ToString();
                            //if (cliente.Avales[0].TipoAval == "CONYUGE")
                            //{
                            //    documentoWord.Replace("<OB1>", "X", true, true);
                            //    documentoWord.Replace("<OB2>", "", true, true);
                            //}
                            //if (cliente.Avales[0].TipoAval == "OBLIGADO SOLIDARIO")
                            //{
                            //    documentoWord.Replace("<OB1>", "", true, true);
                            //    documentoWord.Replace("<OB2>", "X", true, true);
                            //}
                            //if (cliente.Avales[0].TipoIdentificacion == "IFE" || cliente.Avales[0].TipoIdentificacion == "INE")
                            //{
                            //    documentoWord.Replace("<O1>", "X", true, true);
                            //    documentoWord.Replace("<O2>", "", true, true);
                            //    documentoWord.Replace("<O3>", "", true, true);
                            //}
                            //if (cliente.Avales[0].TipoIdentificacion == "PAS")
                            //{
                            //    documentoWord.Replace("<O1>", "", true, true);
                            //    documentoWord.Replace("<O2>", "X", true, true);
                            //    documentoWord.Replace("<O3>", "", true, true);
                            //}
                            //if (cliente.Avales[0].TipoIdentificacion != "IFE" || cliente.Avales[0].TipoIdentificacion != "INE")
                            //{
                            //    documentoWord.Replace("<O1>", "", true, true);
                            //    documentoWord.Replace("<O2>", "", true, true);
                            //    documentoWord.Replace("<O3>", "X", true, true);
                            //}
                            ////Valida el sexo
                            //if (cliente.Avales[0].Sexo == "MASCULINO")
                            //{
                            //    documentoWord.Replace("<O4>", "X", true, true);
                            //    documentoWord.Replace("<O5>", "", true, true);
                            //}
                            //if (cliente.Avales[0].Sexo == "FEMENINO")
                            //{
                            //    documentoWord.Replace("<O4>", "", true, true);
                            //    documentoWord.Replace("<O5>", "X", true, true);
                            //}
                            ////Valida el estado civil
                            //if (cliente.Avales[0].EstadoCivil == "SOLTERO")
                            //{
                            //    documentoWord.Replace("<O6>", "X", true, true);
                            //    documentoWord.Replace("<O7>", "", true, true);
                            //    documentoWord.Replace("<O8>", "", true, true);
                            //    documentoWord.Replace("<O9>", "", true, true);
                            //    documentoWord.Replace("<O10>", "", true, true);
                            //}
                            //if (cliente.Avales[0].EstadoCivil == "UNION LIBRE")
                            //{
                            //    documentoWord.Replace("<O6>", "", true, true);
                            //    documentoWord.Replace("<O7>", "X", true, true);
                            //    documentoWord.Replace("<O8>", "", true, true);
                            //    documentoWord.Replace("<O9>", "", true, true);
                            //    documentoWord.Replace("<O10>", "", true, true);
                            //}
                            //if (cliente.Avales[0].EstadoCivil == "CASADO")
                            //{
                            //    documentoWord.Replace("<O6>", "", true, true);
                            //    documentoWord.Replace("<O7>", "", true, true);
                            //    documentoWord.Replace("<O8>", "X", true, true);
                            //    documentoWord.Replace("<O9>", "", true, true);
                            //    documentoWord.Replace("<O10>", "", true, true);
                            //}
                            //if (cliente.Avales[0].EstadoCivil == "VIUDO")
                            //{
                            //    documentoWord.Replace("<O6>", "", true, true);
                            //    documentoWord.Replace("<O7>", "", true, true);
                            //    documentoWord.Replace("<O8>", "", true, true);
                            //    documentoWord.Replace("<O9>", "X", true, true);
                            //    documentoWord.Replace("<O10>", "", true, true);
                            //}
                            //if (cliente.Avales[0].EstadoCivil == "DIVORCIADO")
                            //{
                            //    documentoWord.Replace("<O6>", "", true, true);
                            //    documentoWord.Replace("<O7>", "", true, true);
                            //    documentoWord.Replace("<O8>", "", true, true);
                            //    documentoWord.Replace("<O9>", "", true, true);
                            //    documentoWord.Replace("<O10>", "X", true, true);
                            //}
                            ////Valida el tipo de domicilio
                            //if (cliente.Avales[0].direccion.TipoDomicilio == "PROPIA")
                            //{
                            //    documentoWord.Replace("<O11>", "X", true, true);
                            //    documentoWord.Replace("<O12>", "", true, true);
                            //    documentoWord.Replace("<O13>", "", true, true);
                            //    documentoWord.Replace("<O14>", "", true, true);
                            //}
                            //if (cliente.Avales[0].direccion.TipoDomicilio == "RENTADA")
                            //{
                            //    documentoWord.Replace("<O11>", "", true, true);
                            //    documentoWord.Replace("<O12>", "X", true, true);
                            //    documentoWord.Replace("<O13>", "", true, true);
                            //    documentoWord.Replace("<O14>", "", true, true);
                            //}
                            //if (cliente.Avales[0].direccion.TipoDomicilio == "FISCAL")
                            //{
                            //    documentoWord.Replace("<O11>", "", true, true);
                            //    documentoWord.Replace("<O12>", "", true, true);
                            //    documentoWord.Replace("<O13>", "X", true, true);
                            //    documentoWord.Replace("<O14>", "", true, true);
                            //}
                            //if (cliente.Avales[0].direccion.TipoDomicilio == "PRESTADA")
                            //{
                            //    documentoWord.Replace("<O11>", "", true, true);
                            //    documentoWord.Replace("<O12>", "", true, true);
                            //    documentoWord.Replace("<O13>", "", true, true);
                            //    documentoWord.Replace("<O14>", "X", true, true);
                            //}
                            //documentoWord.Replace("<<NombreObl>>", cliente.Avales[0].Nombre, true, true);
                            //documentoWord.Replace("<<ApellidoPaternoObl>>", cliente.Avales[0].ApellidoPaterno, true, true);
                            //documentoWord.Replace("<<ApellidoMaternoObl>>", cliente.Avales[0].ApellidoMaterno, true, true);
                            //documentoWord.Replace("<<FechaNacObl>>", fechaNacDia + "                  " + fechaNacMes +
                            //                                                       "                  " + fechaNacAnio, true, true);
                            //documentoWord.Replace("<<LugarNacObl>>", cliente.Avales[0].LugarNacimiento, true, true);
                            //documentoWord.Replace("<<NumeroIdentificacionObl>>", cliente.Avales[0].ClaveElector, true, true);
                            //documentoWord.Replace("<<NumeroDependientesObl>>", cliente.Avales[0].NumeroDependientes, true, true);
                            //documentoWord.Replace("<<RFCObl>>", cliente.Avales[0].RFC, true, true);
                            //documentoWord.Replace("<<CURPObl>>", cliente.Avales[0].CURP, true, true);
                            //documentoWord.Replace("<<ActividadObl>>", cliente.Avales[0].Ocupacion, true, true);
                            //documentoWord.Replace("<<IngresoObl>>", cliente.Avales[0].Ingresos.ToString("C"), true, true);
                            //documentoWord.Replace("<<CalleObl>>", cliente.Avales[0].direccion.Calle, true, true);
                            //documentoWord.Replace("<<NumeroExtIntObl>>", cliente.Avales[0].direccion.NumeroExterior + "              " +
                            //    cliente.Avales[0].direccion.NumeroInterior, true, true);
                            //documentoWord.Replace("<<ColoniaObl>>", cliente.Avales[0].direccion.Colonia, true, true);
                            //documentoWord.Replace("<<CiudadObl>>", cliente.Avales[0].direccion.Ciudad, true, true);
                            //documentoWord.Replace("<<EstadoObl>>", cliente.Avales[0].direccion.Provincia, true, true);
                            //documentoWord.Replace("<<CPObl>>", cliente.Avales[0].direccion.CP, true, true);
                            //documentoWord.Replace("<<ReferenciasUbicacionObl>>", "", true, true);
                            //documentoWord.Replace("<<TelefonoCelularObl>>", cliente.Avales[0].TelefonoCelular, true, true);
                            //documentoWord.Replace("<<TelefonoObl>>", cliente.Avales[0].Telefono, true, true);
                            //documentoWord.Replace("<<IngresosObl>>", cliente.Avales[0].Ingresos.ToString("C"), true, true);
                            //documentoWord.Replace("<<TiempoResidenciaObl>>", cliente.Avales[0].direccion.TiempoResidencia.ToString(), true, true);
                        }
                        else
                        {
                            //documentoWord.Replace("<O1>", "", true, true);
                            //documentoWord.Replace("<O2>", "", true, true);
                            //documentoWord.Replace("<O3>", "", true, true);
                            //documentoWord.Replace("<O4>", "", true, true);
                            //documentoWord.Replace("<O5>", "", true, true);
                            //documentoWord.Replace("<O6>", "", true, true);
                            //documentoWord.Replace("<O7>", "", true, true);
                            //documentoWord.Replace("<O8>", "", true, true);
                            //documentoWord.Replace("<O9>", "", true, true);
                            //documentoWord.Replace("<O10>", "", true, true);
                            //documentoWord.Replace("<O11>", "", true, true);
                            //documentoWord.Replace("<O12>", "", true, true);
                            //documentoWord.Replace("<O13>", "", true, true);
                            //documentoWord.Replace("<O14>", "", true, true);
                            //documentoWord.Replace("<OB1>", "", true, true);
                            //documentoWord.Replace("<OB2>", "", true, true);
                            //documentoWord.Replace("<<NombreObl>>", "", true, true);
                            //documentoWord.Replace("<<ApellidoPaternoObl>>", "", true, true);
                            //documentoWord.Replace("<<ApellidoMaternoObl>>", "", true, true);
                            //documentoWord.Replace("<<FechaNacObl>>", "", true, true);
                            //documentoWord.Replace("<<LugarNacObl>>", "", true, true);
                            //documentoWord.Replace("<<NumeroIdentificacionObl>>", "", true, true);
                            //documentoWord.Replace("<<NumeroDependientesObl>>", "", true, true);
                            //documentoWord.Replace("<<RFCObl>>", "", true, true);
                            //documentoWord.Replace("<<CURPObl>>", "", true, true);
                            //documentoWord.Replace("<<ActividadObl>>", "", true, true);
                            //documentoWord.Replace("<<IngresoObl>>", "", true, true);
                            //documentoWord.Replace("<<CalleObl>>", "", true, true);
                            //documentoWord.Replace("<<NumeroExtIntObl>>", "", true, true);
                            //documentoWord.Replace("<<ColoniaObl>>", "", true, true);
                            //documentoWord.Replace("<<CiudadObl>>", "", true, true);
                            //documentoWord.Replace("<<EstadoObl>>", "", true, true);
                            //documentoWord.Replace("<<CPObl>>", "", true, true);
                            //documentoWord.Replace("<<ReferenciasUbicacionObl>>", "", true, true);
                            //documentoWord.Replace("<<TelefonoCelularObl>>", "", true, true);
                            //documentoWord.Replace("<<TelefonoObl>>", "", true, true);
                            //documentoWord.Replace("<<TiempoResidenciaObl>>", "", true, true);
                            //documentoWord.Replace("<<IngresosObl>>", "", true, true);
                        }
                        #endregion DatosObligado
                        break;

                    case "<<DatosSolCred>>":
                        // REVISAR
                        #region DatosSolCred


                        #endregion DatosSolCred
                        break;

                    case "<<RepresentanteLegal>>":
                        #region RepresentanteLegal
                        _ = documentoWord.Replace("<<NumeroClienteRep>>", cliente.RepresentanteLegal != null ? cliente.RepresentanteLegal.NumeroCliente.ToString() : "", true, true);
                        documentoWord.Replace("<<NombreRep>>", cliente.RepresentanteLegal != null ? cliente.RepresentanteLegal.Nombre : "", true, true);
                        documentoWord.Replace("<<ApellidoPaternoRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.ApellidoPaterno : "", true, true);
                        documentoWord.Replace("<<ApellidoMaternoRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.ApellidoMaterno : "", true, true);
                        documentoWord.Replace("<<DireccionCompletaRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.DireccionCompleta : "", true, true);
                        documentoWord.Replace("<<TelefonoRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.Telefono : "", true, true);
                        documentoWord.Replace("<<CURPRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.CURP : "", true, true);
                        documentoWord.Replace("<<RFCRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.RFC : "", true, true);
                        documentoWord.Replace("<<FechaNacimientoRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.FechaNacimiento : "", true, true);
                        documentoWord.Replace("<<NacionalidadRep>>", cliente.RepresentanteLegal != null
                            ? cliente.RepresentanteLegal.Nacionalidad : "", true, true);
                        documentoWord.Replace("<<BaseCalculo>>", cliente.Credito.TipoSaldo ?? "", true, true);
                        #endregion RepresentanteLegal
                        break;

                    case "<<ClausulaAhorro>>":
                        //if (producto == "VIVJFA" || producto == "VIVJFE")
                        //{
                        //    documentoWord.Replace("<<C>>", "QUINTA", true, true);
                        //}
                        //else if (producto == "VIVTCCONVA" || producto == "VIVTCCONVE")
                        //{
                        //    documentoWord.Replace("<<C>>", "SEXTA", true, true);
                        //}
                        //else
                        //{
                        //    documentoWord.Replace("<<C>>", "", true, true);
                        //}
                        break;

                    case "<<TipoCuentaAhorro>>":
                        //if (producto == "VIVJFA" || producto == "VIVJFE")
                        //{
                        //    documentoWord.Replace("<<TipoCuentaAhorro>>", "Cuenta de depósito en garantía.", true, true);
                        //}
                        //if (producto == "VIVCA14A" || producto == "VIVCA14E")
                        //{
                        //    documentoWord.Replace("<<TipoCuentaAhorro>>", "Cuenta de depósito.", true, true);
                        //}
                        break;

                    case "<<Disposiciones>>":
                        int d = 1;
                        foreach (decimal disp in cliente.Credito.Disposiciones)
                        {
                            documentoWord.Replace("<<Disp" + d + ">>", disp.ToString("C"), true, true);
                            d++;
                        }
                        break;

                    case "<<CartaBeneficiarios>>":
                        // REVISION
                        #region CartaBeneficiarios


                        #endregion CartaBeneficiarios
                        break;

                    case "<<TablaPagos>>":
                        #region TablaPagos
                        //Se obtienen los dividendos
                        totalDividendos = cliente.Credito.Dividendos.Count();
                        registrosTablas = Convert.ToInt32(decimal.Round((totalDividendos / 2)));
                        //Se declaran variables temporales
                        totalDividendosTmp = (Convert.ToDecimal(totalDividendos) / 2).ToString();
                        totalDivision = totalDividendosTmp.Split('.');

                        if (totalDivision.Length == 2)
                            registrosTablas = Convert.ToDecimal(totalDivision[1]) > 0 ? registrosTablas + 1 : registrosTablas;
                        //Se pasan a lista
                        dividendo1 = cliente.Credito.Dividendos.Where(d => d.NumeroDividendo <= registrosTablas).ToList();
                        dividendo2 = cliente.Credito.Dividendos.Where(d => d.NumeroDividendo > registrosTablas).ToList();

                        //Se obtiene el mayor valor entre todas las cuotas
                        decimal mayor = 0;
                        foreach (Dividendo div in cliente.Credito.Dividendos)
                        {
                            if (div.Cuota > mayor)
                            {
                                mayor = div.Cuota;
                            }
                            else
                            {
                                if ((div.Cuota - cliente.Credito.MontoCredito) > mayor)
                                {
                                    mayor = div.Cuota - cliente.Credito.MontoCredito;
                                }
                            }
                        }
                        //Se obtiene la diferencia entre los montos
                        decimal sumaDif = 0;
                        Dividendo ultimoDiv = dividendo2.Last();
                        List<decimal> listaDiferencias1 = new List<decimal>();
                        List<decimal> listaDiferencias2 = new List<decimal>();
                        foreach (Dividendo div in dividendo1)
                        {
                            decimal d1 = mayor - div.Cuota;
                            d1 = Math.Round(d1, 2);
                            listaDiferencias1.Add(d1);
                            sumaDif += d1;
                        }
                        foreach (Dividendo div in dividendo2)
                        {
                            if (div.NumeroDividendo == ultimoDiv.NumeroDividendo)
                            {
                                decimal d2 = mayor - div.Cuota + cliente.Credito.MontoCredito;
                                d2 = Math.Round(d2, 2);
                                listaDiferencias2.Add(d2);
                                sumaDif += d2;
                            }
                            else
                            {
                                decimal d3 = mayor - div.Cuota;
                                d3 = Math.Round(d3, 2);
                                listaDiferencias2.Add(d3);
                                sumaDif += d3;
                            }
                        }
                        //Se crea la tabla de pagos, se le asiga nombre y sus columnas
                        DataTable TablaPagos = new DataTable
                        {
                            //Se declaran columnas
                            TableName = "TablaPagos"
                        };
                        TablaPagos.Columns.Add("Fecha1", typeof(string));
                        TablaPagos.Columns.Add("PagoCredito1", typeof(string));
                        TablaPagos.Columns.Add("DepositoAhorro1", typeof(string));
                        TablaPagos.Columns.Add("Total1", typeof(string));
                        TablaPagos.Columns.Add("Fecha2", typeof(string));
                        TablaPagos.Columns.Add("PagoCredito2", typeof(string));
                        TablaPagos.Columns.Add("DepositoAhorro2", typeof(string));
                        TablaPagos.Columns.Add("Total2", typeof(string));
                        //Se obtiene el ultimo 
                        Dividendo ultDiv = dividendo2.Last();
                        ultimoDiv.Cuota -= cliente.Credito.MontoCredito;
                        //Se comienza el llenado de datos al dataTable
                        for (int i = 0; i < registrosTablas; i++)
                        {
                            //Se llena la priemra mitad de los datos
                            decimal ahorro1 = 0;
                            decimal ahorro2 = 0;
                            string fecha1 = Convert.ToDateTime(dividendo1[i].FechaVencimiento).ToString("dd/MM/yyyy");
                            string pagoCredito1 = string.Format("{0:C}", dividendo1[i].Cuota);
                            ahorro1 += (Math.Round(((cliente.Credito.MontoCredito - sumaDif) / cliente.Credito.Plazo), 2) + listaDiferencias1[i]);
                            string depoAhorro1 = string.Format("{0:C}", ahorro1);
                            string total1 = string.Format("{0:C}", dividendo1[i].Cuota + ahorro1);
                            //Se llena la segunda mitad de los datos
                            string fecha2 = dividendo2.Count > i ? Convert.ToDateTime(dividendo2[i].FechaVencimiento).ToString("dd/MM/yyyy") : string.Empty;
                            string pagoCredito2 = dividendo2.Count > i ? string.Format("{0:C}", dividendo2[i].Cuota) : string.Empty;
                            ahorro2 += (Math.Round(((cliente.Credito.MontoCredito - sumaDif) / cliente.Credito.Plazo), 2) + listaDiferencias2[i]);
                            string depoAhorro2 = dividendo2.Count > i ? string.Format("{0:C}", ahorro2) : string.Empty;
                            string total2 = dividendo2.Count > i ? string.Format("{0:C}", dividendo2[i].Cuota + ahorro2) : string.Empty;
                            //Se agregan a la tabla
                            TablaPagos.Rows.Add(fecha1, pagoCredito1, depoAhorro1, total1, fecha2, pagoCredito2, depoAhorro2, total2);
                        }
                        //Se mapea la tabla
                        documentoWord.MailMerge.ClearFields = true;
                        documentoWord.MailMerge.ExecuteGroup(TablaPagos);

                        #endregion TablaPagos
                        break;

                    case "<<TablaDatosIntegrantes>>":
                        // REVISION
                        #region TablaDatosIntegrantes


                        #endregion TablaDatosIntegrantes
                        break;

                    case "<<ActaFundacion>>":
                        //Lulu
                        #region ActaFundacion
                        //documentoWord.Replace("«TotPagos»", string.Format("{0, 15:N2}", Convert.ToDecimal(estadoCuenta.TotPagos)), true, true);
                        documentoWord.Replace("«desembolsodia»", cliente.Credito.FechaDesembolso.ToString("dd"), true, true);
                        documentoWord.Replace("«desembolsoaño»", cliente.Credito.FechaDesembolso.ToString("yyyy"), true, true);
                        documentoWord.Replace("«desembolsomes»", cliente.Credito.FechaDesembolso.ToString("MM"), true, true);
                        documentoWord.Replace("«reuniondias»", cliente.Credito.DiaReunion, true, true);
                        documentoWord.Replace("«reunionhora»", cliente.Credito.HoraReunion, true, true);

                        #endregion ActaFundacion
                        break;

                    case "<<TablaFirmas>>":
                        // REVISION

                        #region TablaFirmas
                        ////Se crea la tabla a mapear con sus columnas
                        //DataTable tablaFirmas = new DataTable();
                        //tablaFirmas.TableName = "TablaFirmas";
                        //tablaFirmas.Columns.Add("No", typeof(string));
                        //tablaFirmas.Columns.Add("NombreCompleto", typeof(string));
                        //tablaFirmas.Columns.Add("A1", typeof(string));
                        //tablaFirmas.Columns.Add("A2", typeof(string));
                        //tablaFirmas.Columns.Add("A3", typeof(string));
                        //tablaFirmas.Columns.Add("A4", typeof(string));
                        ////Se hace un ciclo para el llenado de la tabla
                        //int no = 1;
                        ////Se recorre la lista de integrantes para agregarlo a la tabla
                        //for (int e = 0; e < cliente.Grupo.Integrantes.Count; e++)
                        //{
                        //    string numero = no.ToString();
                        //    string nombre = cliente.Grupo.Integrantes[e].NombreCompletoIntegrante;
                        //    string a1 = "1";
                        //    string a2 = "2";
                        //    string a3 = "3";
                        //    string a4 = "4";
                        //    //Se llena la tabla
                        //    tablaFirmas.Rows.Add(numero, nombre, a1, a2, a3, a4);
                        //    no++;
                        //}
                        ////Se mapea la tabla
                        //documentoWord.MailMerge.ExecuteGroup(tablaFirmas);

                        #endregion TablaFirmas
                        break;

                    case "<<FechaReunion>>":
                        //documentoWord.Replace("<DiaReunion>", cliente.Grupo.DiaReunion != null
                        //        ? cliente.Grupo.DiaReunion: "", true, true);
                        //documentoWord.Replace("<HoraReunion>", cliente.Grupo.HoraReunion != null
                        //    ? cliente.Grupo.HoraReunion: "", true, true);
                        //documentoWord.Replace("<DiaDesembolso>", cliente.Grupo.FechaDesembolso != null
                        //        ? cliente.Grupo.FechaDesembolso.ToShortDateString(): "", true, true);
                        //documentoWord.Replace("<HoraDesembolso>", cliente.Grupo.FechaDesembolso != null
                        //    ? cliente.Grupo.FechaDesembolso.ToShortTimeString(): "", true, true);
                        break;

                    case "<<OrdenPagoBC>>":
                        // REVISION

                        #region OrdenPagoBC
                        ////Se delcara la tabla 
                        //DataTable odp = new DataTable();
                        ////Se le asigna el nombre
                        //odp.TableName = "ODP";
                        ////Se agregan las columnas 
                        //odp.Columns.Add(new DataColumn("numeroCredito", typeof(string)));
                        //odp.Columns.Add(new DataColumn("FechaDes", typeof(string)));
                        //odp.Columns.Add(new DataColumn("NombreBanco", typeof(string)));
                        //odp.Columns.Add(new DataColumn("FechaVigencia", typeof(string)));
                        //odp.Columns.Add(new DataColumn("Convenio", typeof(string)));
                        //odp.Columns.Add(new DataColumn("MontoOrdenPago", typeof(string)));
                        //odp.Columns.Add(new DataColumn("ReferenciaPago", typeof(string)));
                        //odp.Columns.Add(new DataColumn("NombreCompleto", typeof(string)));
                        //odp.Columns.Add(new DataColumn("ConceptoPago", typeof(string)));
                        //odp.Columns.Add(new DataColumn("DiasHab", typeof(string)));
                        ////Se recorren las ordenes de pago para agregarlas
                        //for (int t = 0; t < cliente.Credito.OrdenesPago.Count; t++)
                        //{
                        //    //Se crea la fila de la tabla
                        //    DataRow fila = odp.NewRow();
                        //    //Se llena la tabla con info
                        //    fila["numeroCredito"] = cliente.Credito.numeroCredito;
                        //    fila["FechaDes"] = cliente.Credito.FechaDesembolsoRenovacion.ToShortDateString();
                        //    fila["NombreBanco"] = cliente.Credito.OrdenesPago[t].NombreBanco;
                        //    fila["FechaVigencia"] = cliente.Credito.OrdenesPago[t].FechaVigencia.ToShortDateString();
                        //    fila["Convenio"] = cliente.Credito.OrdenesPago[t].Convenio;
                        //    fila["MontoOrdenPago"] = cliente.Credito.OrdenesPago[t].MontoOrdenPago.ToString("C");
                        //    fila["ReferenciaPago"] = cliente.Credito.OrdenesPago[t].ReferenciaPago;
                        //    fila["NombreCompleto"] = cliente.Credito.OrdenesPago[t].NombreCompleto;
                        //    fila["ConceptoPago"] = cliente.Credito.OrdenesPago[t].Concepto;
                        //    fila["DiasHab"] = cliente.Credito.OrdenesPago[t].DiasHabiles;
                        //    //Se agraga a la lista
                        //    odp.Rows.Add(fila);
                        //}
                        ////Se ejecuta el mapeo
                        //documentoWord.MailMerge.ExecuteGroup(odp);

                        #endregion OrdenPagoBC
                        break;


                    case "<<PlanillaRefinan>>":
                        // REVISION

                        #region PlanillaRefinan

                        ////Se obtienen los datos del encabezado de la planilla

                        #endregion PlanillaRefinan
                        break;

                    case "<<Cotitulares>>":
                        // REVISION
                        #region Cotitulares

                        List<AhorroIndividuales> cotitular = cliente.AhorroIndividual.Where(d => d.Rol == "TESORERO" || d.Rol == "SECRETARIO").ToList();
                        if (cotitular != null && cotitular.Count > 0)
                        {


                            if (cotitular[0].Rol == "SECRETARIO")
                            {
                                documentoWord.Replace("<<NombreCotitular1>>", cotitular[0].NombreCompleto, true, true);
                                documentoWord.Replace("<<1C1>>", cotitular[0].NumeroCliente.ToString(), true, true);
                                documentoWord.Replace("<<1C2>>", cotitular[0].Nombre, true, true);
                                documentoWord.Replace("<<1C3>>", cotitular[0].ApellidoPaterno, true, true);
                                documentoWord.Replace("<<1C4>>", cotitular[0].ApellidoMaterno, true, true);
                                documentoWord.Replace("<<1C5>>", cotitular[0].DireccionCompleta, true, true);
                                documentoWord.Replace("<<1C6>>", cotitular[0].Sexo, true, true);
                                documentoWord.Replace("<<1C7>>", cotitular[0].CURP, true, true);
                                documentoWord.Replace("<<1C8>>", cotitular[0].RFC, true, true);
                                documentoWord.Replace("<<1C9>>", cotitular[0].FechaNacimiento, true, true);
                                documentoWord.Replace("<<1C10>>", cotitular[0].Telefono, true, true);
                                documentoWord.Replace("<<1C11>>", string.Format("{0:C}", cotitular[0].Ingresos), true, true);
                                documentoWord.Replace("<<1C12>>", string.Format("{0:C}", cotitular[0].SaldoPromedio), true, true);
                                documentoWord.Replace("<<1C13>>", Convert.ToString(cotitular[0].MovimientosEsperados), true, true);
                                documentoWord.Replace("<<1C14>>", cotitular[0].Ocupacion, true, true);
                                documentoWord.Replace("<<1C15>>", cotitular[0].Trabajo, true, true);
                                documentoWord.Replace("<<1C16>>", cotitular[0].FormaMigratoria, true, true);
                                documentoWord.Replace("<<1C17>>", cotitular[0].NumeroExtrajero, true, true);
                                documentoWord.Replace("<<1C18>>", cotitular[0].DireccionExtrajero, true, true);
                                documentoWord.Replace("<<1C19>>", "", true, true);

                            }
                            else if (cotitular[1].Rol == "SECRETARIO")
                            {
                                documentoWord.Replace("<<NombreCotitular1>>", cotitular[1].NombreCompleto, true, true);
                                documentoWord.Replace("<<1C1>>", cotitular[0].NumeroCliente.ToString(), true, true);
                                documentoWord.Replace("<<1C2>>", cotitular[1].Nombre, true, true);
                                documentoWord.Replace("<<1C3>>", cotitular[1].ApellidoPaterno, true, true);
                                documentoWord.Replace("<<1C4>>", cotitular[1].ApellidoMaterno, true, true);
                                documentoWord.Replace("<<1C5>>", cotitular[1].DireccionCompleta, true, true);
                                documentoWord.Replace("<<1C6>>", cotitular[1].Sexo, true, true);
                                documentoWord.Replace("<<1C7>>", cotitular[1].CURP, true, true);
                                documentoWord.Replace("<<1C8>>", cotitular[1].RFC, true, true);
                                //documentoWord.Replace("<<1C9>>", cotitular[1].FechaNacimiento .Substring(0, 10).ToString("dd/MM/yyyy"), true, true);
                                documentoWord.Replace("<<1C9>>", cotitular[1].FechaNacimiento, true, true);
                                documentoWord.Replace("<<1C10>>", cotitular[1].Telefono, true, true);
                                documentoWord.Replace("<<1C11>>", string.Format("{0:C}", cotitular[0].Ingresos), true, true);
                                documentoWord.Replace("<<1C12>>", string.Format("{0:C}", cotitular[0].SaldoPromedio), true, true);
                                documentoWord.Replace("<<1C13>>", Convert.ToString(cotitular[1].MovimientosEsperados), true, true);
                                documentoWord.Replace("<<1C14>>", cotitular[1].Ocupacion, true, true);
                                documentoWord.Replace("<<1C15>>", cotitular[1].Trabajo, true, true);
                                documentoWord.Replace("<<1C16>>", cotitular[1].FormaMigratoria, true, true);
                                documentoWord.Replace("<<1C17>>", cotitular[1].NumeroExtrajero, true, true);
                                documentoWord.Replace("<<1C18>>", cotitular[1].DireccionExtrajero, true, true);
                                documentoWord.Replace("<<1C19>>", "", true, true);


                            }

                            if (cotitular[1].Rol == "TESORERO")
                            {
                                documentoWord.Replace("<<NombreCotitular2>>", cotitular[1].NombreCompleto, true, true);
                                documentoWord.Replace("<<2C1>>", cotitular[0].NumeroCliente.ToString(), true, true);
                                documentoWord.Replace("<<2C2>>", cotitular[1].Nombre, true, true);
                                documentoWord.Replace("<<2C3>>", cotitular[1].ApellidoPaterno, true, true);
                                documentoWord.Replace("<<2C4>>", cotitular[1].ApellidoMaterno, true, true);
                                documentoWord.Replace("<<2C5>>", cotitular[1].DireccionCompleta, true, true);
                                documentoWord.Replace("<<2C6>>", cotitular[1].Sexo, true, true);
                                documentoWord.Replace("<<2C7>>", cotitular[1].CURP, true, true);
                                documentoWord.Replace("<<2C8>>", cotitular[1].RFC, true, true);
                                documentoWord.Replace("<<2C9>>", cotitular[1].FechaNacimiento, true, true);
                                documentoWord.Replace("<<2C10>>", cotitular[1].Telefono, true, true);
                                documentoWord.Replace("<<2C11>>", string.Format("{0:C}", cotitular[0].Ingresos), true, true);
                                documentoWord.Replace("<<2C12>>", string.Format("{0:C}", cotitular[0].SaldoPromedio), true, true);
                                documentoWord.Replace("<<2C13>>", Convert.ToString(cotitular[1].MovimientosEsperados), true, true);
                                documentoWord.Replace("<<2C14>>", cotitular[1].Ocupacion, true, true);
                                documentoWord.Replace("<<2C15>>", cotitular[1].Trabajo, true, true);
                                documentoWord.Replace("<<2C16>>", cotitular[1].FormaMigratoria, true, true);
                                documentoWord.Replace("<<2C17>>", cotitular[1].NumeroExtrajero, true, true);
                                documentoWord.Replace("<<2C18>>", cotitular[1].DireccionExtrajero, true, true);
                                documentoWord.Replace("<<2C19>>", "", true, true);


                            }
                            else if (cotitular[0].Rol == "TESORERO")
                            {
                                documentoWord.Replace("<<NombreCotitular2>>", cotitular[0].NombreCompleto, true, true);
                                documentoWord.Replace("<<2C1>>", cotitular[0].NumeroCliente.ToString(), true, true);
                                documentoWord.Replace("<<2C2>>", cotitular[0].Nombre, true, true);
                                documentoWord.Replace("<<2C3>>", cotitular[0].ApellidoPaterno, true, true);
                                documentoWord.Replace("<<2C4>>", cotitular[0].ApellidoMaterno, true, true);
                                documentoWord.Replace("<<2C5>>", cotitular[0].DireccionCompleta, true, true);
                                documentoWord.Replace("<<2C6>>", cotitular[0].Sexo, true, true);
                                documentoWord.Replace("<<2C7>>", cotitular[0].CURP, true, true);
                                documentoWord.Replace("<<2C8>>", cotitular[0].RFC, true, true);
                                documentoWord.Replace("<<2C9>>", cotitular[0].FechaNacimiento, true, true);
                                documentoWord.Replace("<<2C10>>", cotitular[0].Telefono, true, true);
                                documentoWord.Replace("<<2C11>>", string.Format("{0:C}", cotitular[0].Ingresos), true, true);
                                documentoWord.Replace("<<2C12>>", string.Format("{0:C}", cotitular[0].SaldoPromedio), true, true);
                                documentoWord.Replace("<<2C13>>", Convert.ToString(cotitular[0].MovimientosEsperados), true, true);
                                documentoWord.Replace("<<2C14>>", cotitular[0].Ocupacion, true, true);
                                documentoWord.Replace("<<2C15>>", cotitular[0].Trabajo, true, true);
                                documentoWord.Replace("<<2C16>>", cotitular[0].FormaMigratoria, true, true);
                                documentoWord.Replace("<<2C17>>", cotitular[0].NumeroExtrajero, true, true);
                                documentoWord.Replace("<<2C18>>", cotitular[0].DireccionExtrajero, true, true);
                                documentoWord.Replace("<<2C19>>", "", true, true);
                            }
                            //Se llena la tabla con información



                        }


                        #endregion Cotitulares
                        break;

                    case "<<QuitarDocsAhorro>>":

                        //    if (quitarDocsAhorro.Contains("NO"))
                        //    {
                        //        Se obtiene la tabla
                        //        IWTable tabla = documentoWord.Sections[0].Tables[1];
                        //        Se elimina la tabla
                        //        documentoWord.Sections[0].Body.ChildEntities.Remove(tabla);
                        //    }
                        //    else
                        //    {
                        //        Se obtiene la tabla
                        //        IWTable tabla = documentoWord.Sections[0].Tables[2];
                        //        Se elimina la tabla
                        //        documentoWord.Sections[0].Body.ChildEntities.Remove(tabla);
                        //    }
                        break;

                    case "<<TablaAnexoCom>>":
                        //    if (producto.Contains("ULTRA"))
                        //    {
                        //        //Se obtiene la tabla
                        //        IWTable tabla = documentoWord.Sections[0].Tables[0];
                        //        //Se elimina la tabla
                        //        documentoWord.Sections[0].Body.ChildEntities.Remove(tabla);
                        //    }
                        //    else
                        //    {
                        //        //Se obtiene la tabla
                        //        IWTable tabla = documentoWord.Sections[0].Tables[1];
                        //        //Se elimina la tabla
                        //        documentoWord.Sections[0].Body.ChildEntities.Remove(tabla);
                        //    }
                        break;

                    case "<<RFCEXT>>":
                        if (!String.IsNullOrEmpty(cliente.NumeroExtranjero.Trim()))
                        {
                            documentoWord.Replace("<<RFCEXT>>", cliente.RFC, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<RFCEXT>>", "", true, true);
                        }
                        break;

                    case "<<NoCertBasico>>":
                        #region NoCertBasico
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroBasico != null)
                        {
                            documentoWord.Replace("<<NoCertBasico>>", cliente.SeguroBasico.Certificado, true, true);

                            if (cliente.Beneficiarios != null && cliente.Beneficiarios.Count() > 0)
                            {
                                List<Beneficiario> beneficiarios = cliente.Beneficiarios.Where(b => b.TipoBeneficiario == "S").ToList();
                                if (beneficiarios != null && beneficiarios.Count > 0)
                                {
                                    documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].ApellidoPaterno + " " + beneficiarios[0].ApellidoMaterno + " " + beneficiarios[0].Nombre, true, true);
                                    documentoWord.Replace("<<Parentesco1>>", beneficiarios[0].Relacion, true, true);
                                    documentoWord.Replace("<<Porcentaje1>>", beneficiarios[0].Porcentaje.ToString(), true, true);

                                    if (beneficiarios.Count > 1)
                                    {
                                        documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].ApellidoPaterno + " " + beneficiarios[1].ApellidoMaterno + " " + beneficiarios[1].Nombre, true, true);
                                        documentoWord.Replace("<<Parentesco2>>", beneficiarios[1].Relacion, true, true);
                                        documentoWord.Replace("<<Porcentaje2>>", beneficiarios[1].Porcentaje.ToString(), true, true);
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                        documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                        documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                                    }
                                }
                                else
                                {
                                    documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                    documentoWord.Replace("<<Parentesco1>>", "", true, true);
                                    documentoWord.Replace("<<Porcentaje1>>", "", true, true);
                                    documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                    documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                    documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                documentoWord.Replace("<<Parentesco1>>", "", true, true);
                                documentoWord.Replace("<<Porcentaje1>>", "", true, true);
                                documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                documentoWord.Replace("<<Parentesco2>>", "", true, true);
                                documentoWord.Replace("<<Porcentaje2>>", "", true, true);
                            }
                        }
                        else
                        {
                            documentoWord.Replace("<<NoCertBasico>>", "", true, true);
                        }

                        #endregion NoCertBasico
                        break;

                    case "<<NoCertVoluntario>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroVoluntario != null)
                        {
                            documentoWord.Replace("<<NoCertVoluntario>>", cliente.SeguroVoluntario.Certificado, true, true);
                        }
                        else
                        {
                            if (cliente.SeguroBasico != null)
                                documentoWord.Replace("<<NoCertVoluntario>>", cliente.SeguroBasico.Certificado, true, true);
                            else
                                documentoWord.Replace("<<NoCertVoluntario>>", "", true, true);
                        }
                        break;

                    case "<<FechaIniBasico>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroBasico != null)
                        {
                            documentoWord.Replace("<<FechaIniBasico>>", cliente.SeguroBasico.FechaInicio, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<FechaIniBasico>>", "", true, true);
                        }
                        break;

                    case "<<FechaFinBasico>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroBasico != null)
                        {
                            documentoWord.Replace("<<FechaFinBasico>>", cliente.SeguroBasico.FechaFin, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<FechaFinBasico>>", "", true, true);
                        }
                        break;
                    case "<<FechaInicioAnio>>":
                        valor = Convert.ToDateTime(cliente.Credito.FechaInicio).Year.ToString();
                        documentoWord.Replace("<<FechaInicioAnio>>", valor, true, true);
                        break;
                    case "<<FechaInicioDia>>":
                        valor = Convert.ToDateTime(cliente.Credito.FechaInicio).Day.ToString();
                        documentoWord.Replace("<<FechaInicioDia>>", valor, true, true);
                        break;
                    case "<<FechaInicioMes>>":
                        int mesNumero = Convert.ToDateTime(cliente.Credito.FechaInicio).Month;
                        DateTimeFormatInfo mesNombre = CultureInfo.CurrentCulture.DateTimeFormat;
                        string mesNom = mesNombre.GetMonthName(mesNumero);
                        valor = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mesNom);
                        documentoWord.Replace("<<FechaInicioMes>>", valor, true, true);
                        break;

                    case "<<FechaInicioLetra>>":
                        int w = cliente.Credito.FechaInicio.Month;
                        DateTimeFormatInfo M = CultureInfo.CurrentCulture.DateTimeFormat;
                        string O = M.GetMonthName(w);
                        valor = Convert.ToDateTime(cliente.Credito.FechaInicio).Day + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(O) + " de " + Convert.ToDateTime(cliente.Credito.FechaInicio).Year;
                        documentoWord.Replace("<<FechaInicioLetra>>", valor, true, true);
                        break;

                    case "<<MontoTotalLetra>>":
                        //CameDigital
                        if (cliente == null)
                        {
                            documentoWord.Replace("<<MontoTotalLetra>>", " ", true, true);
                        }
                        else
                        {
                            montoLetraT = cliente.Credito.MontoTotal.ToString();
                            temporalT = montoLetraT.Split('.');
                            centavosT = numerToText.Convert(Convert.ToDecimal("0." + temporalT[1]));
                            tempCentavosT = centavosT.Split(' ');
                            valortemp = numerToText.Convert(Convert.ToDecimal(temporalT[0])).Replace(" con 00/100", "") + " pesos " + tempCentavosT[2] + " M.N.";
                            valor = valortemp.Replace(",", "");
                            valor = valor.ToUpper();
                            documentoWord.Replace("<<MontoTotalLetra>>", valor, true, true);
                        }
                        break;

                    case "<<InteresMoratorioLetra>>":
                        montoLetraI = cliente.Credito.InteresMoratorioMensual.ToString();
                        temporalI = montoLetraI.Split('.');
                        valor = numerToText.Convert(Convert.ToDecimal(temporalI[0])).Replace(" con 00/100", "") + " punto " +
                            numerToText.Convert(Convert.ToDecimal(temporalI[1])).Replace(" con 00/100", "") + " por ciento";
                        valor = valor.ToLower();
                        documentoWord.Replace("<<InteresMoratorioLetra>>", valor, true, true);
                        break;

                    case "<<InteresMoratorio>>":
                        valor = cliente.Credito.InteresMoratorio.ToString("F");
                        documentoWord.Replace("<<InteresMoratorio>>", valor, true, true);
                        break;

                    case "<<PaqBasico>>":
                        //if (documSolicitud.ProcesoNombre == "CreditoCameDigital" && cliente == null)
                        //{ 
                        //    CamposEspecialesCAMEDigital(item, documentoWord, numInt, numVol);
                        //}                        
                        //else
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroBasico != null)
                        {
                            documentoWord.Replace("<<PaqBasico>>", "X", true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<PaqBasico>>", "", true, true);
                        }
                        break;

                    case "<<PaqPremium>>":
                        //if (documSolicitud.ProcesoNombre == "CreditoCameDigital" && cliente == null)
                        //{
                        //    CamposEspecialesCAMEDigital(item, documentoWord, numInt, numVol);
                        //}
                        //else 
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroVoluntario != null && cliente.SeguroVoluntario.Tipo == 1)
                        {
                            documentoWord.Replace("<<PaqPremium>>", "X", true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<PaqPremium>>", "", true, true);
                        }
                        break;

                    case "<<PaqPlatino>>":
                        //if (documSolicitud.ProcesoNombre == "CreditoCameDigital" && cliente == null)
                        //{
                        //    CamposEspecialesCAMEDigital(item, documentoWord, numInt, numVol);
                        //}
                        //else 
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroVoluntario != null && cliente.SeguroVoluntario.Tipo == 2)
                        {
                            documentoWord.Replace("<<PaqPlatino>>", "X", true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<PaqPlatino>>", "", true, true);
                        }
                        break;

                    case "<<FechaIniVoluntario>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroVoluntario != null)
                        {
                            documentoWord.Replace("<<FechaIniVoluntario>>", cliente.SeguroVoluntario.FechaInicio, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<FechaIniVoluntario>>", "", true, true);
                        }
                        break;

                    case "<<FechaFinVoluntario>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroVoluntario != null)
                        {
                            documentoWord.Replace("<<FechaFinVoluntario>>", cliente.SeguroVoluntario.FechaFin, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<FechaFinVoluntario>>", "", true, true);
                        }
                        break;

                    case "<<NombreBeneficiarios>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        else if (cliente.SeguroBasico != null)
                        {
                            if (cliente.Beneficiarios != null && cliente.Beneficiarios.Count() > 0)
                            {
                                List<Beneficiario> beneficiarios = cliente.Beneficiarios.Where(b => b.TipoBeneficiario == "S").ToList();
                                if (beneficiarios != null && beneficiarios.Count > 0)
                                {
                                    documentoWord.Replace("<<Beneficiario1>>", beneficiarios[0].NombreCompleto, true, true);

                                    if (beneficiarios.Count > 1)
                                    {
                                        documentoWord.Replace("<<Beneficiario2>>", beneficiarios[1].NombreCompleto, true, true);
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                    }
                                }
                                else
                                {
                                    documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                    documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<Beneficiario1>>", "", true, true);
                                documentoWord.Replace("<<Beneficiario2>>", "", true, true);
                            }
                        }
                        else
                        {
                            documentoWord.Replace("<<NoCertBasico>>", "", true, true);
                        }
                        break;

                    case "<<PaqueteId>>":
                        if (cliente.PlanConexion.PaqueteId == 1) { documentoWord.Replace("<<plan1>>", "X", true, true); }
                        if (cliente.PlanConexion.PaqueteId == 2) { documentoWord.Replace("<<plan2>>", "X", true, true); }
                        if (cliente.PlanConexion.PaqueteId == 3) { documentoWord.Replace("<<plan3>>", "X", true, true); }
                        documentoWord.Replace("<<plan1>>", "", true, true);
                        documentoWord.Replace("<<plan2>>", "", true, true);
                        documentoWord.Replace("<<plan3>>", "", true, true);
                        break;

                    case "<<NombreCompletoVoluntarios>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }

                        break;
                    case "<<NombreBeneficiariosVoluntarios>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);

                        }
                        break;

                    case "<<NombreCompletoContratoBeneficiarios>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);
                        }

                        break;
                    case "<<ConcentimientoCredito>>":
                        if (numInt > 0)
                        {
                            CamposEspecialesGrupal(item, documentoWord, numInt, numVol);
                        }
                        break;

                    //G-EngineMambu
                    case "<<G-Beneficiarios>>":
                        if (clienteJson != null)
                        {
                            if (clienteJson.Beneficiarios.Count > 0)
                            {
                                int no = 0;

                                for (int i = 0; i < clienteJson.Beneficiarios.Count; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<" + no + "B01>>", clienteJson.Beneficiarios[i].Numero, true, true);
                                    documentoWord.Replace("<<" + no + "B02>>", clienteJson.Beneficiarios[i].NombreCompleto, true, true);
                                    documentoWord.Replace("<<" + no + "B03>>", clienteJson.Beneficiarios[i].FechaNacimiento, true, true);
                                    documentoWord.Replace("<<" + no + "B04>>", clienteJson.Beneficiarios[i].Telefono, true, true);
                                    documentoWord.Replace("<<" + no + "B05>>", clienteJson.Beneficiarios[i].Porcentaje, true, true);
                                    documentoWord.Replace("<<" + no + "B06>>", clienteJson.Beneficiarios[i].Alta, true, true);
                                    documentoWord.Replace("<<" + no + "B07>>", clienteJson.Beneficiarios[i].Baja, true, true);

                                }

                                for (int i = clienteJson.Beneficiarios.Count; i < 6; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<" + no + "B01>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B02>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B03>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B04>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B05>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B06>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "B07>>", "", true, true);
                                }
                            }
                        }

                        break;

                    case "<<G-AvalesObligados>>":
                        if (clienteJson != null)
                        {
                            if (clienteJson.AvalesObligados.Count > 0)
                            {
                                int no = 0;

                                for (int i = 0; i < clienteJson.AvalesObligados.Count; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<G-AvalObligado" + no + ">>", clienteJson.AvalesObligados[i].NombreCompleto, true, true);
                                }

                                for (int i = clienteJson.AvalesObligados.Count; i < 4; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<G-AvalObligado" + no + ">>", "", true, true);
                                }


                            }

                        }
                        break;
                    case "<<G-ApoderadoCotitular>>":

                        if (clienteJson != null)
                        {
                            if (clienteJson.Cotitulares.Count > 0)
                            {

                                int no = 0;

                                for (int i = 0; i < clienteJson.Cotitulares.Count; i++)
                                {
                                    no = i + 1;

                                    if (clienteJson.Cotitulares[i].Ingresos <= 10000)
                                    {
                                        documentoWord.Replace("<<" + no + "AC13>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC14>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC15>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].Ingresos > 10000 && clienteJson.Cotitulares[i].Ingresos <= 50000)
                                    {
                                        documentoWord.Replace("<<" + no + "AC13>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC14>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC15>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].Ingresos > 51000)
                                    {
                                        documentoWord.Replace("<<" + no + "AC13>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC14>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC15>>", "X", true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AC01>>", clienteJson.Cotitulares[i].NumeroCliente, true, true);
                                    documentoWord.Replace("<<" + no + "AC02>>", clienteJson.Cotitulares[i].Nombre, true, true);
                                    documentoWord.Replace("<<" + no + "AC03>>", clienteJson.Cotitulares[i].ApellidoPaterno, true, true);
                                    documentoWord.Replace("<<" + no + "AC04>>", clienteJson.Cotitulares[i].ApellidoMaterno, true, true);
                                    documentoWord.Replace("<<" + no + "AC05>>", clienteJson.Cotitulares[i].Nacionalidad, true, true);
                                    documentoWord.Replace("<<" + no + "AC06>>", clienteJson.Cotitulares[i].CURP, true, true);
                                    documentoWord.Replace("<<" + no + "AC07>>", clienteJson.Cotitulares[i].RFC, true, true);
                                    documentoWord.Replace("<<" + no + "AC08>>", clienteJson.Cotitulares[i].FechaNacimiento, true, true);
                                    documentoWord.Replace("<<" + no + "AC09>>", clienteJson.Cotitulares[i].TelefonoFijo, true, true);
                                    documentoWord.Replace("<<" + no + "AC10>>", clienteJson.Cotitulares[i].DireccionCompleta, true, true);
                                    documentoWord.Replace("<<" + no + "AC11>>", clienteJson.Cotitulares[i].Genero, true, true);
                                    documentoWord.Replace("<<" + no + "AC12>>", clienteJson.Cotitulares[i].CorreoElectronico, true, true);
                                    documentoWord.Replace("<<" + no + "AC16>>", string.Format("{0:C}", clienteJson.Cotitulares[i].SaldoPromedio), true, true);
                                    documentoWord.Replace("<<" + no + "AC17>>", clienteJson.Cotitulares[i].MovimientosEsperados, true, true);
                                    documentoWord.Replace("<<" + no + "AC18>>", clienteJson.Cotitulares[i].Profesion, true, true);
                                    documentoWord.Replace("<<" + no + "AC19>>", clienteJson.Cotitulares[i].Trabajo, true, true);
                                    documentoWord.Replace("<<" + no + "AC20>>", clienteJson.Cotitulares[i].FormaMigratoria, true, true);
                                    documentoWord.Replace("<<" + no + "AC21>>", clienteJson.Cotitulares[i].TelefonoExtranjero, true, true);
                                    documentoWord.Replace("<<" + no + "AC22>>", clienteJson.Cotitulares[i].DireccionCompletaExtranjero, true, true);
                                    documentoWord.Replace("<<" + no + "AC23>>", clienteJson.Cotitulares[i].RFCExtranjero, true, true);
                                    documentoWord.Replace("<<" + no + "AC24>>", clienteJson.Cotitulares[i].Baja, true, true);
                                    documentoWord.Replace("<<" + no + "AC25>>", clienteJson.Cotitulares[i].Alta, true, true);
                                    documentoWord.Replace("<<" + no + "AC26>>", clienteJson.Cotitulares[i].Modificacion, true, true);

                                    documentoWord.Replace("<<" + no + "AC27>>", clienteJson.Cotitulares[i].FechaNacimiento.Substring(0, 2), true, true);
                                    documentoWord.Replace("<<" + no + "AC28>>", clienteJson.Cotitulares[i].FechaNacimiento.Substring(3, 2), true, true);
                                    documentoWord.Replace("<<" + no + "AC29>>", clienteJson.Cotitulares[i].FechaNacimiento.Substring(6, 2), true, true);

                                    documentoWord.Replace("<<" + no + "AC30>>", string.Format("{0:C}", clienteJson.Cotitulares[i].Ingresos), true, true);
                                    documentoWord.Replace("<<" + no + "AC31>>", clienteJson.Cotitulares[i].LugarNacimiento, true, true);
                                    documentoWord.Replace("<<" + no + "AC32>>", clienteJson.Cotitulares[i].PaisNacimiento, true, true);
                                    documentoWord.Replace("<<" + no + "AC33>>", clienteJson.Cotitulares[i].Edad, true, true);
                                    if (clienteJson.Cotitulares[i].Genero.ToUpper() == "FEMENINO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC34>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC35>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].Genero.ToUpper() == "MASCULINO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC34>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC35>>", "X", true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AC36>>", clienteJson.Cotitulares[i].TelefonoOficina, true, true);
                                    documentoWord.Replace("<<" + no + "AC37>>", clienteJson.Cotitulares[i].TelefonoCelular, true, true);
                                    documentoWord.Replace("<<" + no + "AC38>>", clienteJson.Cotitulares[i].Identificacion, true, true);
                                    documentoWord.Replace("<<" + no + "AC39>>", clienteJson.Cotitulares[i].NivelEstudio, true, true);
                                    documentoWord.Replace("<<" + no + "AC40>>", clienteJson.Cotitulares[i].TiempoRadicandoDomicilio, true, true);
                                    documentoWord.Replace("<<" + no + "AC41>>", clienteJson.Cotitulares[i].AntigüedadEmpleoNegocio, true, true);
                                    documentoWord.Replace("<<" + no + "AC42>>", string.Format("{0:C}", clienteJson.Cotitulares[i].IngresoMensual), true, true);
                                    documentoWord.Replace("<<" + no + "AC43>>", string.Format("{0:C}", clienteJson.Cotitulares[i].ImporteInicialDepositado), true, true);
                                    documentoWord.Replace("<<" + no + "AC44>>", clienteJson.Cotitulares[i].OrigenRecursos, true, true);

                                    if (clienteJson.Cotitulares[i].CargoPoliticoUltimos12Meses.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC45>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC46>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].CargoPoliticoUltimos12Meses.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC45>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC46>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].RelacionPolitico12Meses.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC47>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC48>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].RelacionPolitico12Meses.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC47>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC48>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ResidenciaOtroPais.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC49>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC50>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ResidenciaOtroPais.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC49>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC50>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].PropietarioRecursos.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC51>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC52>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC76>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].PropietarioRecursos.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC51>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC52>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC76>>", clienteJson.Cotitulares[i].QuienEsPropietarioRecursos, true, true);

                                    }
                                    if (clienteJson.Cotitulares[i].ProveedorRecursos.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC53>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC54>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC77>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ProveedorRecursos.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC53>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC54>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC77>>", clienteJson.Cotitulares[i].QuienEsProveedorRecursos, true, true);

                                    }
                                    if (clienteJson.Cotitulares[i].IncrementoInversionMensual.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC55>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC56>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].IncrementoInversionMensual.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC55>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC56>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].RetirosMensuales.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC57>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC58>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].RetirosMensuales.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC57>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC58>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ActuaCuentaPropia.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC59>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC60>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ActuaCuentaPropia.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC59>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC60>>", "X", true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AC61>>", clienteJson.Cotitulares[i].Cargo, true, true);
                                    documentoWord.Replace("<<" + no + "AC62>>", clienteJson.Cotitulares[i].NombreFuncionario, true, true);
                                    documentoWord.Replace("<<" + no + "AC63>>", clienteJson.Cotitulares[i].ParentescoPolitico, true, true);
                                    documentoWord.Replace("<<" + no + "AC64>>", clienteJson.Cotitulares[i].NumeroIdentificacionFiscal, true, true);
                                    documentoWord.Replace("<<" + no + "AC65>>", clienteJson.Cotitulares[i].PaisAsignado, true, true);
                                    documentoWord.Replace("<<" + no + "AC66>>", clienteJson.Cotitulares[i].PropietarioRecursos, true, true);
                                    documentoWord.Replace("<<" + no + "AC67>>", clienteJson.Cotitulares[i].ProveedorRecursos, true, true);
                                    documentoWord.Replace("<<" + no + "AC68>>", clienteJson.Cotitulares[i].DestinoRecursos, true, true);
                                    documentoWord.Replace("<<" + no + "AC69>>", clienteJson.Cotitulares[i].RelacionProveedor, true, true);
                                    documentoWord.Replace("<<" + no + "AC70>>", clienteJson.Cotitulares[i].RelacionDueno, true, true);
                                    documentoWord.Replace("<<" + no + "AC71>>", clienteJson.Cotitulares[i].IncrementoInversionMensual, true, true);
                                    documentoWord.Replace("<<" + no + "AC72>>", string.Format("{0:C}", clienteJson.Cotitulares[i].MontoIncrementoInversionMensual), true, true);
                                    documentoWord.Replace("<<" + no + "AC73>>", clienteJson.Cotitulares[i].RetirosMensuales, true, true);
                                    documentoWord.Replace("<<" + no + "AC74>>", string.Format("{0:C}", clienteJson.Cotitulares[i].MontoRetirosMensuales), true, true);
                                    documentoWord.Replace("<<" + no + "AC75>>", clienteJson.Cotitulares[i].ActuaCuentaPropia, true, true);

                                    documentoWord.Replace("<<" + no + "AC78>>", clienteJson.Cotitulares[i].NumeroSerieFirmaElectronica, true, true);


                                    if (clienteJson.Cotitulares[i].ConsentimientoCredito == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AC79>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AC80>>", "", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].ConsentimientoCredito == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AC79>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AC80>>", "X", true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].DomicilioParticular != null)
                                    {
                                        documentoWord.Replace("<<" + no + "AC90>>", clienteJson.Cotitulares[i].DomicilioParticular.Calle, true, true);
                                        documentoWord.Replace("<<" + no + "AC91>>", clienteJson.Cotitulares[i].DomicilioParticular.NumeroInterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC92>>", clienteJson.Cotitulares[i].DomicilioParticular.NumeroExterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC93>>", clienteJson.Cotitulares[i].DomicilioParticular.Colonia, true, true);
                                        documentoWord.Replace("<<" + no + "AC94>>", clienteJson.Cotitulares[i].DomicilioParticular.Municipio, true, true);
                                        documentoWord.Replace("<<" + no + "AC95>>", clienteJson.Cotitulares[i].DomicilioParticular.Ciudad, true, true);
                                        documentoWord.Replace("<<" + no + "AC96>>", clienteJson.Cotitulares[i].DomicilioParticular.Estado, true, true);
                                        documentoWord.Replace("<<" + no + "AC97>>", clienteJson.Cotitulares[i].DomicilioParticular.CP, true, true);
                                        documentoWord.Replace("<<" + no + "AC98>>", clienteJson.Cotitulares[i].DomicilioParticular.Pais, true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].DomicilioCorrespondencia != null)
                                    {
                                        documentoWord.Replace("<<" + no + "AC99>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Calle, true, true);
                                        documentoWord.Replace("<<" + no + "AC100>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.NumeroInterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC101>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.NumeroExterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC102>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Colonia, true, true);
                                        documentoWord.Replace("<<" + no + "AC103>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Municipio, true, true);
                                        documentoWord.Replace("<<" + no + "AC104>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Ciudad, true, true);
                                        documentoWord.Replace("<<" + no + "AC105>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Estado, true, true);
                                        documentoWord.Replace("<<" + no + "AC106>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.CP, true, true);
                                        documentoWord.Replace("<<" + no + "AC107>>", clienteJson.Cotitulares[i].DomicilioCorrespondencia.Pais, true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].DomicilioFiscal != null)
                                    {
                                        documentoWord.Replace("<<" + no + "AC108>>", clienteJson.Cotitulares[i].DomicilioFiscal.Calle, true, true);
                                        documentoWord.Replace("<<" + no + "AC109>>", clienteJson.Cotitulares[i].DomicilioFiscal.NumeroInterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC110>>", clienteJson.Cotitulares[i].DomicilioFiscal.NumeroExterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC111>>", clienteJson.Cotitulares[i].DomicilioFiscal.Colonia, true, true);
                                        documentoWord.Replace("<<" + no + "AC112>>", clienteJson.Cotitulares[i].DomicilioFiscal.Municipio, true, true);
                                        documentoWord.Replace("<<" + no + "AC113>>", clienteJson.Cotitulares[i].DomicilioFiscal.Ciudad, true, true);
                                        documentoWord.Replace("<<" + no + "AC114>>", clienteJson.Cotitulares[i].DomicilioFiscal.Estado, true, true);
                                        documentoWord.Replace("<<" + no + "AC115>>", clienteJson.Cotitulares[i].DomicilioFiscal.CP, true, true);
                                        documentoWord.Replace("<<" + no + "AC116>>", clienteJson.Cotitulares[i].DomicilioFiscal.Pais, true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].DomicilioExtranjero != null)
                                    {
                                        documentoWord.Replace("<<" + no + "AC117>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Calle, true, true);
                                        documentoWord.Replace("<<" + no + "AC118>>", clienteJson.Cotitulares[i].DomicilioExtranjero.NumeroInterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC119>>", clienteJson.Cotitulares[i].DomicilioExtranjero.NumeroExterior, true, true);
                                        documentoWord.Replace("<<" + no + "AC120>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Colonia, true, true);
                                        documentoWord.Replace("<<" + no + "AC121>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Municipio, true, true);
                                        documentoWord.Replace("<<" + no + "AC122>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Ciudad, true, true);
                                        documentoWord.Replace("<<" + no + "AC123>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Estado, true, true);
                                        documentoWord.Replace("<<" + no + "AC124>>", clienteJson.Cotitulares[i].DomicilioExtranjero.CP, true, true);
                                        documentoWord.Replace("<<" + no + "AC125>>", clienteJson.Cotitulares[i].DomicilioExtranjero.Pais, true, true);
                                    }
                                    if (clienteJson.Cotitulares[i].NombreCompleto != null || clienteJson.Cotitulares[i].NombreCompleto != "")
                                    {

                                        documentoWord.Replace("<<G-ApoderadoCotitular" + no + ">>", clienteJson.Cotitulares[i].NombreCompleto, true, true);
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<G-ApoderadoCotitular" + no + ">>", "", true, true);
                                    }

                                }




                                for (int i = clienteJson.Cotitulares.Count; i < 4; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<" + no + "AC01>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC02>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC03>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC04>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC05>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC06>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC07>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC08>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC09>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC10>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC11>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC12>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC13>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC14>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC15>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC16>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC17>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC18>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC19>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC20>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC21>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC22>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC23>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC24>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC25>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC26>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC27>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC28>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC29>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC30>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC31>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC32>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC33>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC34>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC35>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC36>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC37>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC38>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC39>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC40>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC41>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC42>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC43>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC44>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC45>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC46>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC47>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC48>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC49>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC50>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC51>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC52>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC53>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC54>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC55>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC56>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC57>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC58>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC59>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC60>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC61>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC62>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC63>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC64>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC65>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC66>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC67>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC68>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC69>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC60>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC61>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC62>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC63>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC64>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC65>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC66>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC67>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC68>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC69>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC70>>", "", true, true);

                                    documentoWord.Replace("<<" + no + "AC71>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC72>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC73>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC74>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC75>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC76>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC77>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC78>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC79>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC80>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC81>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC82>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC83>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC84>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC85>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC86>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC87>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC88>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC89>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC90>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC91>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC92>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC93>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC94>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC95>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC96>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC97>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC98>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC99>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC100>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC101>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC102>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC103>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC104>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC105>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC106>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC107>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC108>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC109>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC110>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC111>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC112>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC113>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC114>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC115>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC116>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC117>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC118>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC119>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC120>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC121>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC122>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC123>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC124>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AC125>>", "", true, true);
                                    documentoWord.Replace("<<G-ApoderadoCotitular" + no + ">>", "", true, true);



                                }


                            }

                        }

                        break;
                    case "<<G-Checks>>":
                        if (clienteJson != null)
                        {
                            if (clienteJson.PersonalidadJuridica != null)
                            {

                                if (clienteJson.PersonalidadJuridica.ToUpper() == "ASALARIADO")
                                {
                                    documentoWord.Replace("<<PFAS>>", "X", true, true);
                                    documentoWord.Replace("<<PFCO>>", "", true, true);
                                    documentoWord.Replace("<<PFIN>>", "", true, true);
                                    documentoWord.Replace("<<PFNC>>", "", true, true);
                                }
                                if (clienteJson.PersonalidadJuridica.ToUpper() == "COMPROBABLE")
                                {
                                    documentoWord.Replace("<<PFAS>>", "", true, true);
                                    documentoWord.Replace("<<PFCO>>", "X", true, true);
                                    documentoWord.Replace("<<PFIN>>", "", true, true);
                                    documentoWord.Replace("<<PFNC>>", "", true, true);
                                }
                                if (clienteJson.PersonalidadJuridica.ToUpper() == "INDEPENDIENTE")
                                {
                                    documentoWord.Replace("<<PFAS>>", "", true, true);
                                    documentoWord.Replace("<<PFCO>>", "", true, true);
                                    documentoWord.Replace("<<PFIN>>", "X", true, true);
                                    documentoWord.Replace("<<PFNC>>", "", true, true);
                                }
                                if (clienteJson.PersonalidadJuridica.ToUpper() == "NOCOMPROBABLE")
                                {
                                    documentoWord.Replace("<<PFAS>>", "", true, true);
                                    documentoWord.Replace("<<PFCO>>", "", true, true);
                                    documentoWord.Replace("<<PFIN>>", "", true, true);
                                    documentoWord.Replace("<<PFNC>>", "X", true, true);
                                }

                            }
                            else
                            {
                                documentoWord.Replace("<<PFAS>>", "", true, true);
                                documentoWord.Replace("<<PFIN>>", "", true, true);
                                documentoWord.Replace("<<PFIN>>", "", true, true);
                                documentoWord.Replace("<<PFNC>>", "", true, true);
                            }

                            if (clienteJson.EstadoCivil != null)
                            {
                                if (clienteJson.EstadoCivil.ToUpper() == "CASADO")
                                {
                                    documentoWord.Replace("<<ECCA>>", "X", true, true);
                                    documentoWord.Replace("<<ECSO>>", "", true, true);
                                }
                                if (clienteJson.EstadoCivil.ToUpper() == "SOLTERO")
                                {
                                    documentoWord.Replace("<<ECCA>>", "", true, true);
                                    documentoWord.Replace("<<ECSO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<ECCA>>", "", true, true);
                                documentoWord.Replace("<<ECSO>>", "", true, true);
                            }
                            if (clienteJson.Genero != null)
                            {

                                if (clienteJson.Genero.ToUpper() == "FEMENINO")
                                {
                                    documentoWord.Replace("<<GEFE>>", "X", true, true);
                                    documentoWord.Replace("<<GEMA>>", "", true, true);
                                }
                                if (clienteJson.Genero.ToUpper() == "MASCULINO")
                                {
                                    documentoWord.Replace("<<GEFE>>", "", true, true);
                                    documentoWord.Replace("<<GEMA>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<GEFE>>", "", true, true);
                                documentoWord.Replace("<<GEMA>>", "", true, true);
                            }
                            if (clienteJson.Invercamex.TipoRenovacion != null)
                            {
                                if (clienteJson.Invercamex.TipoRenovacion.ToUpper() == "SOLO CAPITAL")
                                {
                                    documentoWord.Replace("<<TRCI>>", "X", true, true);
                                    documentoWord.Replace("<<TRSC>>", "", true, true);
                                }
                                if (clienteJson.Invercamex.TipoRenovacion.ToUpper() == "CAPITAL E INTERÉS GENERADO")
                                {
                                    documentoWord.Replace("<<TRCI>>", "", true, true);
                                    documentoWord.Replace("<<TRSC>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<TRCI>>", "", true, true);
                                documentoWord.Replace("<<TRSC>>", "", true, true);
                            }
                            if (clienteJson.Credito.Plazo != 0)
                            {
                                if (clienteJson.Credito.Plazo == 7)
                                {
                                    documentoWord.Replace("<<PI7>>", "X", true, true);
                                    documentoWord.Replace("<<PI28>>", "", true, true);
                                    documentoWord.Replace("<<PI91>>", "", true, true);
                                    documentoWord.Replace("<<PI182>>", "", true, true);
                                    documentoWord.Replace("<<PI364>>", "", true, true);
                                }
                                if (clienteJson.Credito.Plazo == 28)
                                {
                                    documentoWord.Replace("<<PI7>>", "", true, true);
                                    documentoWord.Replace("<<PI28>>", "X", true, true);
                                    documentoWord.Replace("<<PI91>>", "", true, true);
                                    documentoWord.Replace("<<PI182>>", "", true, true);
                                    documentoWord.Replace("<<PI364>>", "", true, true);
                                }
                                if (clienteJson.Credito.Plazo == 91)
                                {
                                    documentoWord.Replace("<<PI7>>", "", true, true);
                                    documentoWord.Replace("<<PI28>>", "", true, true);
                                    documentoWord.Replace("<<PI91>>", "X", true, true);
                                    documentoWord.Replace("<<PI182>>", "", true, true);
                                    documentoWord.Replace("<<PI364>>", "", true, true);
                                }
                                if (clienteJson.Credito.Plazo == 182)
                                {
                                    documentoWord.Replace("<<PI7>>", "", true, true);
                                    documentoWord.Replace("<<PI28>>", "", true, true);
                                    documentoWord.Replace("<<PI91>>", "", true, true);
                                    documentoWord.Replace("<<PI182>>", "X", true, true);
                                    documentoWord.Replace("<<PI364>>", "", true, true);
                                }
                                if (clienteJson.Credito.Plazo == 364)
                                {
                                    documentoWord.Replace("<<PI7>>", "", true, true);
                                    documentoWord.Replace("<<PI28>>", "", true, true);
                                    documentoWord.Replace("<<PI91>>", "", true, true);
                                    documentoWord.Replace("<<PI182>>", "", true, true);
                                    documentoWord.Replace("<<PI364>>", "X", true, true);
                                }
                                else
                                {
                                    documentoWord.Replace("<<PI7>>", "", true, true);
                                    documentoWord.Replace("<<PI28>>", "", true, true);
                                    documentoWord.Replace("<<PI91>>", "", true, true);
                                    documentoWord.Replace("<<PI182>>", "", true, true);
                                    documentoWord.Replace("<<PI364>>", "", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<PI7>>", "", true, true);
                                documentoWord.Replace("<<PI28>>", "", true, true);
                                documentoWord.Replace("<<PI91>>", "", true, true);
                                documentoWord.Replace("<<PI182>>", "", true, true);
                                documentoWord.Replace("<<PI364>>", "", true, true);
                            }
                            if (clienteJson.Invercamex.ConsentimientoCredito != null)
                            {
                                if (clienteJson.Invercamex.ConsentimientoCredito.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<CTSI>>", "X", true, true);
                                    documentoWord.Replace("<<CTNO>>", "", true, true);
                                }
                                if (clienteJson.Invercamex.ConsentimientoCredito.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<CTSI>>", "", true, true);
                                    documentoWord.Replace("<<CTNO>>", "X", true, true);
                                }

                            }
                            else
                            {
                                documentoWord.Replace("<<CTSI>>", "", true, true);
                                documentoWord.Replace("<<CTNO>>", "", true, true);
                            }
                            if (clienteJson.CargoPoliticoUltimos12Meses != null)
                            {
                                if (clienteJson.CargoPoliticoUltimos12Meses.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<CPSI>>", "X", true, true);
                                    documentoWord.Replace("<<CPNO>>", "", true, true);
                                }
                                if (clienteJson.CargoPoliticoUltimos12Meses.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<CPSI>>", "", true, true);
                                    documentoWord.Replace("<<CPNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<CPSI>>", "", true, true);
                                documentoWord.Replace("<<CPNO>>", "", true, true);
                            }
                            if (clienteJson.RelacionPolitico12Meses != null)
                            {
                                if (clienteJson.RelacionPolitico12Meses.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<RPSI>>", "X", true, true);
                                    documentoWord.Replace("<<RPNO>>", "", true, true);
                                }
                                if (clienteJson.RelacionPolitico12Meses.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<RPSI>>", "", true, true);
                                    documentoWord.Replace("<<RPNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<RPSI>>", "", true, true);
                                documentoWord.Replace("<<RPNO>>", "", true, true);
                            }
                            if (clienteJson.ResidenciaOtroPais != null)
                            {
                                if (clienteJson.ResidenciaOtroPais.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<ROPSI>>", "X", true, true);
                                    documentoWord.Replace("<<ROPNO>>", "", true, true);
                                }
                                if (clienteJson.ResidenciaOtroPais.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<ROPSI>>", "", true, true);
                                    documentoWord.Replace("<<ROPNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<ROPSI>>", "", true, true);
                                documentoWord.Replace("<<ROPNO>>", "", true, true);
                            }
                            if (clienteJson.PropietarioRecursos != null)
                            {
                                if (clienteJson.PropietarioRecursos.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<RRSI>>", "X", true, true);
                                    documentoWord.Replace("<<RRNO>>", "", true, true);
                                    documentoWord.Replace("<<G-QuienEsPropietarioRecursos>>", "", true, true);


                                }
                                if (clienteJson.PropietarioRecursos.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<RRSI>>", "", true, true);
                                    documentoWord.Replace("<<RRNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<RRSI>>", "", true, true);
                                documentoWord.Replace("<<RRNO>>", "", true, true);
                            }
                            if (clienteJson.ProveedorRecursos != null)
                            {
                                if (clienteJson.ProveedorRecursos.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<PRSI>>", "X", true, true);
                                    documentoWord.Replace("<<PRNO>>", "", true, true);
                                    documentoWord.Replace("<<G-QuienEsProveedorRecursos>>", "", true, true);


                                }
                                if (clienteJson.ProveedorRecursos.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<PRSI>>", "", true, true);
                                    documentoWord.Replace("<<PRNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<PRSI>>", "", true, true);
                                documentoWord.Replace("<<PRNO>>", "", true, true);
                            }
                            if (clienteJson.IncrementoInversionMensual != null)
                            {
                                if (clienteJson.IncrementoInversionMensual.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<IISI>>", "X", true, true);
                                    documentoWord.Replace("<<IINO>>", "", true, true);


                                }
                                if (clienteJson.IncrementoInversionMensual.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<IISI>>", "", true, true);
                                    documentoWord.Replace("<<IINO>>", "X", true, true);
                                    //documentoWord.Replace("<<G-MontoIncrementoInversionMensual>>", "", true, true); << G - MontoRetirosMensuales >>
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<IISI>>", "", true, true);
                                documentoWord.Replace("<<IINO>>", "", true, true);
                            }
                            if (clienteJson.RetirosMensuales != null)
                            {
                                if (clienteJson.RetirosMensuales.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<RMSI>>", "X", true, true);
                                    documentoWord.Replace("<<RMNO>>", "", true, true);
                                }
                                if (clienteJson.RetirosMensuales.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<RMSI>>", "", true, true);
                                    documentoWord.Replace("<<RMNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<RMSI>>", "", true, true);
                                documentoWord.Replace("<<RMNO>>", "", true, true);
                            }
                            if (clienteJson.ActuaCuentaPropia != null)
                            {
                                if (clienteJson.ActuaCuentaPropia.ToUpper() == "SI")
                                {
                                    documentoWord.Replace("<<ACSI>>", "X", true, true);
                                    documentoWord.Replace("<<ACNO>>", "", true, true);
                                }
                                if (clienteJson.ActuaCuentaPropia.ToUpper() == "NO")
                                {
                                    documentoWord.Replace("<<ACSI>>", "", true, true);
                                    documentoWord.Replace("<<ACNO>>", "X", true, true);
                                }
                            }
                            else
                            {
                                documentoWord.Replace("<<ACSI>>", "", true, true);
                                documentoWord.Replace("<<ACNO>>", "", true, true);
                            }



                        }
                        break;

                    case "<<G-DomiciliosInverCamex>>":
                        if (clienteJson.DomicilioParticular != null)
                        {
                            documentoWord.Replace("<<DPCalle>>", clienteJson.DomicilioParticular.Calle, true, true);
                            documentoWord.Replace("<<DPNoInt>>", clienteJson.DomicilioParticular.NumeroInterior, true, true);
                            documentoWord.Replace("<<DPNoExt>>", clienteJson.DomicilioParticular.NumeroExterior, true, true);
                            documentoWord.Replace("<<DPColonia>>", clienteJson.DomicilioParticular.Colonia, true, true);
                            documentoWord.Replace("<<DPMunicipio>>", clienteJson.DomicilioParticular.Municipio, true, true);
                            documentoWord.Replace("<<DPCiudad>>", clienteJson.DomicilioParticular.Ciudad, true, true);
                            documentoWord.Replace("<<DPEstado>>", clienteJson.DomicilioParticular.Estado, true, true);
                            documentoWord.Replace("<<DPCP>>", clienteJson.DomicilioParticular.CP, true, true);
                            documentoWord.Replace("<<DPPais>>", clienteJson.DomicilioParticular.Pais, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<DPCalle>>", "", true, true);
                            documentoWord.Replace("<<DPNumeroInterior>>", "", true, true);
                            documentoWord.Replace("<<DPNumeroExterior>>", "", true, true);
                            documentoWord.Replace("<<DPColonia>>", "", true, true);
                            documentoWord.Replace("<<DPMunicipio>>", "", true, true);
                            documentoWord.Replace("<<DPCiudad>>", "", true, true);
                            documentoWord.Replace("<<DPEstado>>", "", true, true);
                            documentoWord.Replace("<<DPCP>>", "", true, true);
                            documentoWord.Replace("<<DPPais>>", "", true, true);
                        }
                        if (clienteJson.DomicilioCorrespondencia != null)
                        {
                            documentoWord.Replace("<<DCCalle>>", clienteJson.DomicilioCorrespondencia.Calle, true, true);
                            documentoWord.Replace("<<DCNoInt>>", clienteJson.DomicilioCorrespondencia.NumeroInterior, true, true);
                            documentoWord.Replace("<<DCNoExt>>", clienteJson.DomicilioCorrespondencia.NumeroExterior, true, true);
                            documentoWord.Replace("<<DCColonia>>", clienteJson.DomicilioCorrespondencia.Colonia, true, true);
                            documentoWord.Replace("<<DCMunicipio>>", clienteJson.DomicilioCorrespondencia.Municipio, true, true);
                            documentoWord.Replace("<<DCCiudad>>", clienteJson.DomicilioCorrespondencia.Ciudad, true, true);
                            documentoWord.Replace("<<DCEstado>>", clienteJson.DomicilioCorrespondencia.Estado, true, true);
                            documentoWord.Replace("<<DCCP>>", clienteJson.DomicilioCorrespondencia.CP, true, true);
                            documentoWord.Replace("<<DCPais>>", clienteJson.DomicilioCorrespondencia.Pais, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<DCCalle>>", "", true, true);
                            documentoWord.Replace("<<DCNumeroInterior>>", "", true, true);
                            documentoWord.Replace("<<DCNumeroExterior>>", "", true, true);
                            documentoWord.Replace("<<DCColonia>>", "", true, true);
                            documentoWord.Replace("<<DCMunicipio>>", "", true, true);
                            documentoWord.Replace("<<DCCiudad>>", "", true, true);
                            documentoWord.Replace("<<DCEstado>>", "", true, true);
                            documentoWord.Replace("<<DCCP>>", "", true, true);
                            documentoWord.Replace("<<DCPais>>", "", true, true);
                        }
                        if (clienteJson.DomicilioFiscal != null)
                        {
                            documentoWord.Replace("<<DFCalle>>", clienteJson.DomicilioFiscal.Calle, true, true);
                            documentoWord.Replace("<<DFNoInt>>", clienteJson.DomicilioFiscal.NumeroInterior, true, true);
                            documentoWord.Replace("<<DFNoExt>>", clienteJson.DomicilioFiscal.NumeroExterior, true, true);
                            documentoWord.Replace("<<DFColonia>>", clienteJson.DomicilioFiscal.Colonia, true, true);
                            documentoWord.Replace("<<DFMunicipio>>", clienteJson.DomicilioFiscal.Municipio, true, true);
                            documentoWord.Replace("<<DFCiudad>>", clienteJson.DomicilioFiscal.Ciudad, true, true);
                            documentoWord.Replace("<<DFEstado>>", clienteJson.DomicilioFiscal.Estado, true, true);
                            documentoWord.Replace("<<DFCP>>", clienteJson.DomicilioFiscal.CP, true, true);
                            documentoWord.Replace("<<DFPais>>", clienteJson.DomicilioFiscal.Pais, true, true);

                        }
                        else
                        {
                            documentoWord.Replace("<<DFCalle>>", "", true, true);
                            documentoWord.Replace("<<DFNumeroInterior>>", "", true, true);
                            documentoWord.Replace("<<DFNumeroExterior>>", "", true, true);
                            documentoWord.Replace("<<DFColonia>>", "", true, true);
                            documentoWord.Replace("<<DFMunicipio>>", "", true, true);
                            documentoWord.Replace("<<DFCiudad>>", "", true, true);
                            documentoWord.Replace("<<DFEstado>>", "", true, true);
                            documentoWord.Replace("<<DFCP>>", "", true, true);
                            documentoWord.Replace("<<DFPais>>", "", true, true);
                        }
                        if (clienteJson.DomicilioExtranjero != null)
                        {
                            documentoWord.Replace("<<DECalle>>", clienteJson.DomicilioExtranjero.Calle, true, true);
                            documentoWord.Replace("<<DENoInt>>", clienteJson.DomicilioExtranjero.NumeroInterior, true, true);
                            documentoWord.Replace("<<DENoExt>>", clienteJson.DomicilioExtranjero.NumeroExterior, true, true);
                            documentoWord.Replace("<<DEColonia>>", clienteJson.DomicilioExtranjero.Colonia, true, true);
                            documentoWord.Replace("<<DEMunicipio>>", clienteJson.DomicilioExtranjero.Municipio, true, true);
                            documentoWord.Replace("<<DECiudad>>", clienteJson.DomicilioExtranjero.Ciudad, true, true);
                            documentoWord.Replace("<<DEEstado>>", clienteJson.DomicilioExtranjero.Estado, true, true);
                            documentoWord.Replace("<<DECP>>", clienteJson.DomicilioExtranjero.CP, true, true);
                            documentoWord.Replace("<<DEPais>>", clienteJson.DomicilioExtranjero.Pais, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<DECalle>>", "", true, true);
                            documentoWord.Replace("<<DENumeroInterior>>", "", true, true);
                            documentoWord.Replace("<<DENumeroExterior>>", "", true, true);
                            documentoWord.Replace("<<DEColonia>>", "", true, true);
                            documentoWord.Replace("<<DEMunicipio>>", "", true, true);
                            documentoWord.Replace("<<DECiudad>>", "", true, true);
                            documentoWord.Replace("<<DEEstado>>", "", true, true);
                            documentoWord.Replace("<<DECP>>", "", true, true);
                            documentoWord.Replace("<<DEPais>>", "", true, true);
                        }
                        if (clienteJson.Invercamex.DomicilioComercial != null)
                        {
                            documentoWord.Replace("<<DCoCalle>>", clienteJson.Invercamex.DomicilioComercial.Calle, true, true);
                            documentoWord.Replace("<<DCoNoInt>>", clienteJson.Invercamex.DomicilioComercial.NumeroInterior, true, true);
                            documentoWord.Replace("<<DCoNoExt>>", clienteJson.Invercamex.DomicilioComercial.NumeroExterior, true, true);
                            documentoWord.Replace("<<DCoColonia>>", clienteJson.Invercamex.DomicilioComercial.Colonia, true, true);
                            documentoWord.Replace("<<DCoMunicipio>>", clienteJson.Invercamex.DomicilioComercial.Municipio, true, true);
                            documentoWord.Replace("<<DCoCiudad>>", clienteJson.Invercamex.DomicilioComercial.Ciudad, true, true);
                            documentoWord.Replace("<<DCoEstado>>", clienteJson.Invercamex.DomicilioComercial.Estado, true, true);
                            documentoWord.Replace("<<DCoCP>>", clienteJson.Invercamex.DomicilioComercial.CP, true, true);
                            documentoWord.Replace("<<DCoPais>>", clienteJson.Invercamex.DomicilioComercial.Pais, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<DCoCalle>>", "", true, true);
                            documentoWord.Replace("<<DCoNoInt>>", "", true, true);
                            documentoWord.Replace("<<DCoNoExt>>", "", true, true);
                            documentoWord.Replace("<<DCoColonia>>", "", true, true);
                            documentoWord.Replace("<<DCoMunicipio>>", "", true, true);
                            documentoWord.Replace("<<DCoCiudad>>", "", true, true);
                            documentoWord.Replace("<<DCoEstado>>", "", true, true);
                            documentoWord.Replace("<<DCoCP>>", "", true, true);
                            documentoWord.Replace("<<DCoPais>>", "", true, true);
                        }



                        break;

                    case "<<G-ApoderadoRepresentante>>":
                        //gestorLog.Registrar(Nivel.Debug, "ApoderadoRepresentante   ");

                        if (clienteJson != null)
                        {
                            if (clienteJson.Representantes.Count > 0)
                            {
                                int no = 0;

                                for (int i = 0; i < clienteJson.Representantes.Count; i++)
                                {
                                    no = i + 1;

                                    documentoWord.Replace("<<" + no + "AR02>>", clienteJson.Representantes[i].Nombre, true, true);
                                    documentoWord.Replace("<<" + no + "AR03>>", clienteJson.Representantes[i].ApellidoPaterno, true, true);
                                    documentoWord.Replace("<<" + no + "AR04>>", clienteJson.Representantes[i].ApellidoMaterno, true, true);
                                    documentoWord.Replace("<<" + no + "AR05>>", clienteJson.Representantes[i].Nacionalidad, true, true);
                                    documentoWord.Replace("<<" + no + "AR08>>", clienteJson.Representantes[i].FechaNacimiento, true, true);
                                    documentoWord.Replace("<<" + no + "AR20>>", clienteJson.Representantes[i].FormaMigratoria, true, true);

                                    documentoWord.Replace("<<" + no + "AR27>>", clienteJson.Representantes[i].FechaNacimiento.Substring(0, 2), true, true);
                                    documentoWord.Replace("<<" + no + "AR28>>", clienteJson.Representantes[i].FechaNacimiento.Substring(3, 2), true, true);
                                    documentoWord.Replace("<<" + no + "AR29>>", clienteJson.Representantes[i].FechaNacimiento.Substring(6, 4), true, true);

                                    documentoWord.Replace("<<" + no + "AR38>>", clienteJson.Representantes[i].Identificacion, true, true);

                                    if (clienteJson.Representantes[i].ResidenciaOtroPais.ToUpper() == "SI")
                                    {
                                        documentoWord.Replace("<<" + no + "AR49>>", "X", true, true);
                                        documentoWord.Replace("<<" + no + "AR50>>", "", true, true);
                                    }
                                    if (clienteJson.Representantes[i].ResidenciaOtroPais.ToUpper() == "NO")
                                    {
                                        documentoWord.Replace("<<" + no + "AR49>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR50>>", "X", true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AR64>>", clienteJson.Representantes[i].NumeroIdentificacionFiscal, true, true);
                                    documentoWord.Replace("<<" + no + "AR65>>", clienteJson.Representantes[i].PaisAsignado, true, true);

                                    if (clienteJson.Representantes[i].Domicilio != null)
                                    {
                                        documentoWord.Replace("<<" + no + "AR90>>", clienteJson.Representantes[i].Domicilio.Calle, true, true);
                                        documentoWord.Replace("<<" + no + "AR91>>", clienteJson.Representantes[i].Domicilio.NumeroInterior, true, true);
                                        documentoWord.Replace("<<" + no + "AR92>>", clienteJson.Representantes[i].Domicilio.NumeroExterior, true, true);
                                        documentoWord.Replace("<<" + no + "AR93>>", clienteJson.Representantes[i].Domicilio.Colonia, true, true);
                                        documentoWord.Replace("<<" + no + "AR94>>", clienteJson.Representantes[i].Domicilio.Municipio, true, true);
                                        documentoWord.Replace("<<" + no + "AR95>>", clienteJson.Representantes[i].Domicilio.Ciudad, true, true);
                                        documentoWord.Replace("<<" + no + "AR96>>", clienteJson.Representantes[i].Domicilio.Estado, true, true);
                                        documentoWord.Replace("<<" + no + "AR97>>", clienteJson.Representantes[i].Domicilio.CP, true, true);
                                        documentoWord.Replace("<<" + no + "AR98>>", clienteJson.Representantes[i].Domicilio.Pais, true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AR99>>", clienteJson.Representantes[i].FormaMigratoria, true, true);
                                    documentoWord.Replace("<<" + no + "AR100>>", clienteJson.Representantes[i].FolioIdentificacion, true, true);
                                    documentoWord.Replace("<<" + no + "AR101>>", clienteJson.Representantes[i].NumeroEscritura, true, true);
                                    documentoWord.Replace("<<" + no + "AR102>>", clienteJson.Representantes[i].FechaOtorgamiento, true, true);
                                    documentoWord.Replace("<<" + no + "AR103>>", clienteJson.Representantes[i].TipoPoder, true, true);
                                    documentoWord.Replace("<<" + no + "AR104>>", clienteJson.Representantes[i].NombreNotario, true, true);
                                    documentoWord.Replace("<<" + no + "AR105>>", clienteJson.Representantes[i].NumeroNotario, true, true);
                                    documentoWord.Replace("<<" + no + "AR106>>", clienteJson.Representantes[i].Firma, true, true);

                                    if (clienteJson.Representantes[i].PersonalidadJuridica != null)
                                    {
                                        if (clienteJson.Representantes[i].PersonalidadJuridica.ToUpper() == "ASALARIADO")
                                        {
                                            documentoWord.Replace("<<" + no + "AR107>>", "X", true, true);
                                            documentoWord.Replace("<<" + no + "AR108>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR109>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR110>>", "", true, true);
                                        }
                                        if (clienteJson.Representantes[i].PersonalidadJuridica.ToUpper() == "COMPROBABLE")
                                        {
                                            documentoWord.Replace("<<" + no + "AR107>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR108>>", "X", true, true);
                                            documentoWord.Replace("<<" + no + "AR109>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR110>>", "", true, true);
                                        }
                                        if (clienteJson.Representantes[i].PersonalidadJuridica.ToUpper() == "INDEPENDIENTE")
                                        {
                                            documentoWord.Replace("<<" + no + "AR107>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR108>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR109>>", "X", true, true);
                                            documentoWord.Replace("<<" + no + "AR110>>", "", true, true);
                                        }
                                        if (clienteJson.Representantes[i].PersonalidadJuridica.ToUpper() == "NOCOMPROBABLE")
                                        {
                                            documentoWord.Replace("<<" + no + "AR107>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR108>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR109>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR110>>", "X", true, true);
                                        }
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<" + no + "AR107>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR108>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR109>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR110>>", "", true, true);
                                    }

                                    documentoWord.Replace("<<" + no + "AR111>>", clienteJson.Representantes[i].RepresentanteLegal, true, true);
                                    documentoWord.Replace("<<" + no + "AR112>>", clienteJson.Representantes[i].NumeroInscripcion, true, true);

                                    if (clienteJson.Representantes[i].TipoFirma != null)
                                    {
                                        if (clienteJson.Representantes[i].TipoFirma.ToUpper() == "MANCOMUNADA")
                                        {
                                            documentoWord.Replace("<<" + no + "AR113>>", "X", true, true);
                                            documentoWord.Replace("<<" + no + "AR114>>", "", true, true);
                                        }
                                        if (clienteJson.Representantes[i].TipoFirma.ToUpper() == "INDISTINTA")
                                        {
                                            documentoWord.Replace("<<" + no + "AR113>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR114>>", "X", true, true);
                                        }
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<" + no + "AR113>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR114>>", "", true, true);
                                    }


                                    if (clienteJson.Representantes[i].ConsentimientoDatosPersonales != null)
                                    {
                                        if (clienteJson.Representantes[i].ConsentimientoDatosPersonales.ToUpper() == "SI")
                                        {
                                            documentoWord.Replace("<<" + no + "AR115>>", "X", true, true);
                                            documentoWord.Replace("<<" + no + "AR116>>", "", true, true);
                                        }
                                        if (clienteJson.Representantes[i].ConsentimientoDatosPersonales.ToUpper() == "NO")
                                        {
                                            documentoWord.Replace("<<" + no + "AR115>>", "", true, true);
                                            documentoWord.Replace("<<" + no + "AR116>>", "X", true, true);
                                        }

                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<" + no + "AR115>>", "", true, true);
                                        documentoWord.Replace("<<" + no + "AR116>>", "", true, true);
                                    }
                                    if (clienteJson.Representantes[i].NombreCompleto != null || clienteJson.Representantes[i].NombreCompleto != "")
                                    {

                                        documentoWord.Replace("<<G-ApoderadoRepresentante" + no + ">>", clienteJson.Representantes[i].NombreCompleto, true, true);
                                    }
                                    else
                                    {
                                        documentoWord.Replace("<<G-ApoderadoRepresentante" + no + ">>", "", true, true);
                                    }

                                }


                                for (int i = clienteJson.Representantes.Count; i < 4; i++)
                                {
                                    no = i + 1;
                                    documentoWord.Replace("<<" + no + "AR01>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR02>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR03>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR04>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR05>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR06>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR07>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR08>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR09>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR10>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR11>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR12>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR13>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR14>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR15>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR16>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR17>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR18>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR19>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR20>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR21>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR22>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR23>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR24>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR25>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR26>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR27>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR28>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR29>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR30>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR31>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR32>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR33>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR34>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR35>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR36>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR37>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR38>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR39>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR40>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR41>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR42>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR43>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR44>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR45>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR46>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR47>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR48>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR49>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR50>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR51>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR52>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR53>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR54>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR55>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR56>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR57>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR58>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR59>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR60>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR61>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR62>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR63>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR64>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR65>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR66>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR67>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR68>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR69>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR60>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR61>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR62>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR63>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR64>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR65>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR66>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR67>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR68>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR69>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR70>>", "", true, true);

                                    documentoWord.Replace("<<" + no + "AR71>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR72>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR73>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR74>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR75>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR76>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR77>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR78>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR79>>", "", true, true);
                                    documentoWord.Replace("<<" + no + "AR80>>", "", true, true);
                                    documentoWord.Replace("<<G-ApoderadoRepresentante" + no + ">>", "", true, true);





                                }


                            }

                        }

                        break;
                    case "<<NombreCotitular1>>":
                        if (cliente.Cotitulares != null && cliente.Cotitulares.Count > 0)
                        {
                            documentoWord.Replace("<<NombreCotitular1>>", cliente.Cotitulares[0].NombreCompleto, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<NombreCotitular1>>", "", true, true);
                        }
                        break;
                    case "<<NombreCotitular2>>":
                        if (cliente.Cotitulares != null && cliente.Cotitulares.Count > 1)
                        {
                            documentoWord.Replace("<<NombreCotitular2>>", cliente.Cotitulares[1].NombreCompleto, true, true);
                        }
                        else
                        {
                            documentoWord.Replace("<<NombreCotitular2>>", "", true, true);
                        }
                        break;
                    default:
                        resultado = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                //  resultado = false;
                // throw;
            }

            return resultado;
        }


        public ResultadoUrlDocumeto ObtieneRutaDocumento(SolictudDocumentoDto solicitud)
        {
            gestorLog.Entrar();

            documSolicitud = solicitud;
            ObtenerDatos();
            MemoryStream documento = GeneraDocumento();
            ResultadoUrlDocumeto resultado = ResultadoRutas(documento);

            gestorLog.Salir();

            return resultado;
        }

        private ResultadoUrlDocumeto ResultadoRutas(MemoryStream documento)
        {
            gestorLog.Entrar();
            ResultadoUrlDocumeto resultado = new ResultadoUrlDocumeto();
            ArchivoGuardado archivoGuardado = new ArchivoGuardado();
            List<DatosUrl> ListaDatosUrl = new List<DatosUrl>();
            List<DatosUrl> DatoUrl = new List<DatosUrl>();
            string nombre = "";

            DatosUrl datosUrl = null;

            try
            {
                if (documSolicitud.Separado)
                {
                    string nombres = "";
                    foreach (var item in ArchivoPDF)
                    {
                        datosUrl = new DatosUrl();
                        nombres += (nombres == "" ? "" : "|") + item.Key.ToString() + "_" + documSolicitud.NumeroCredito;
                        var nombreArchivo = item.Key.ToString() + "_" + documSolicitud.NumeroCredito + ".pdf";
                        archivoGuardado = GuardarDocumentoEnDigipro(Convert.ToBase64String(item.Value.ToArray()), nombreArchivo, "PDF");
                        datosUrl.Id = archivoGuardado.Id;
                        datosUrl.UrlVisor = archivoGuardado.Url;
                        ListaDatosUrl.Add(datosUrl);
                    }
                    resultado.Mensaje = nombres;


                    if (documSolicitud.Comprimido == true)
                    {
                        documento = ArchivosToZiP(ArchivoPDF);

                        nombre = string.Format("Documentos_{0}{1}", documSolicitud.NumeroCredito, documSolicitud.Comprimido ? ".zip" : ".pdf");
                        archivoGuardado = GuardarDocumentoEnDigipro(Convert.ToBase64String(documento.ToArray()), nombre, documSolicitud.Comprimido ? "ZIP" : "PDF");
                        datosUrl.Id = archivoGuardado.Id;
                        datosUrl.UrlVisor = archivoGuardado.Url;
                        DatoUrl.Add(datosUrl);

                        resultado.Dato = DatoUrl;
                        resultado.Mensaje = resultado.Mensaje + "|" + nombre;
                    }
                    else
                    {
                        resultado.ListaDatos = ListaDatosUrl;
                    }
                }
                else
                {
                    resultado.Mensaje = string.Format("Documento_{0}{1}", documSolicitud.NumeroCredito, documSolicitud.Comprimido ? ".zip" : ".pdf");

                    if (documSolicitud.Comprimido == true)
                    {
                        documento = ArchivoToZiP(documento);

                        datosUrl = new DatosUrl();
                        archivoGuardado = GuardarDocumentoEnDigipro(Convert.ToBase64String(documento.ToArray()), resultado.Mensaje, documSolicitud.Comprimido ? "ZIP" : "PDF");
                        datosUrl.Id = archivoGuardado.Id;
                        datosUrl.UrlVisor = archivoGuardado.Url;
                        DatoUrl.Add(datosUrl);
                        resultado.Dato = DatoUrl;

                    }
                    else
                    {
                        datosUrl = new DatosUrl();
                        archivoGuardado = GuardarDocumentoEnDigipro(Convert.ToBase64String(documento.ToArray()), resultado.Mensaje, documSolicitud.Comprimido ? "ZIP" : "PDF");
                        datosUrl.Id = archivoGuardado.Id;
                        datosUrl.UrlVisor = archivoGuardado.Url;
                        DatoUrl.Add(datosUrl);
                        resultado.Dato = DatoUrl;

                    }
                }
            }
            catch (Exception)
            {
                throw new BusinessException("No se pudo crear el archivo de respuesta.");
            }

            gestorLog.Salir();
            return resultado;
        }

        public bool ValidarFecha(string fecha, out DateTime fechaValida)
        {
            bool respuesta = false;
            fechaValida = default;
            string dia = string.Empty;
            string mes = string.Empty;
            string año = string.Empty;

            try
            {

                fechaValida = DateTime.Parse(fecha);

            }
            catch (Exception)
            {
                mes = fecha.Substring(0, 2);
                dia = fecha.Substring(3, 2);
                año = fecha.Substring(6, 4);

                try
                {
                    fechaValida = DateTime.Parse(dia + "/" + mes + "/" + año);
                    respuesta = true;

                }
                catch (Exception)
                {

                    respuesta = false;
                }

            }

            return respuesta;
        }
        public ResultadoDocumentoDto ObtieneEstadoCuenta(ObtenerPlantillasProcesoDto solicitud)
        {
            gestorLog.Entrar();

            documSolicitud = serviceProvider("TCR").ServicioDatosPlantillas.AsignarValores(solicitud);

            datosDocumento = serviceProvider("TCR").ServicioDatosPlantillas.EstadoCuentaObtenerDatos(documSolicitud);

            if (datosDocumento != null)
            {
                // estadoCuenta = serviceProvider("TCR").ServicioDatosPlantillas.ObtenerDatosJsonEstadoCuenta(documSolicitud);
                ObtenerDatosDto solic = new ObtenerDatosDto
                {
                    // Proceso = datosDocumento.ProcesoId,
                    NumeroCredito = documSolicitud.NumeroCredito,
                    NumeroCliente = documSolicitud.NumeroCliente,
                    Usuario = documSolicitud.Usuario
                };

                datosJson = serviceProvider("TCR").ServicioDatosPlantillas.ObtenerDatosEstadoCuenta(solic);
                SeleccionaPlantillas();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }

            MemoryStream documento = GeneraDocumento();
            //ResultadoEstadoCuenta resultado = null;
            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            //ResultadoUrlDocumeto resultado = ResultadoRutas(documento);
            gestorLog.Salir();
            return resultado;
        }

        public int EstadosCuentaMensualProcesa(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            int procesados = 0;
            try
            {
                var serviceFactory = serviceProvider(solicitud.Empresa.ToString());
                var inicioMes = new DateTime(solicitud.Fecha.Year, solicitud.Fecha.Month, 01, 00, 00, 01);
                var finalMes = new DateTime(solicitud.Fecha.Year, solicitud.Fecha.Month, DateTime.DaysInMonth(solicitud.Fecha.Year, solicitud.Fecha.Month), 23, 59, 59);
                var productos = _mambuFactory.Crear(solicitud.Empresa).RepositorioProductosAhorro.ObtenerProductos().Result;
                var validProducts = productos
                    .Where(p => p.Id.ToUpper() == "PATRIMONIAL" || p.Id.ToUpper() == "Patrimonial_D")
                    .Select(p => p.EncodedKey);

                var cuentas = ObtenerAhorros(solicitud.Empresa, validProducts);
                var bitacoras = factory.ServicioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuentaTodos(solicitud.Empresa, solicitud.Fecha);

                cuentas = cuentas
                    .Where(c => c.AccountHolderType == "CLIENT" && !bitacoras.Any(b => b.NumeroCliente == c.AccountHolderKey || b.NumeroCuenta == c.Id || b.NumeroCuenta == c.EncodedKey)
                        && ((c.AccountState == "ACTIVE" && Convert.ToDateTime(c.ActivationDate) <= finalMes)
                        || ((c.AccountState == "CLOSED" && Convert.ToDateTime(c.ClosedDate) >= inicioMes && Convert.ToDateTime(c.ClosedDate) <= finalMes))
                        || ((c.AccountState == "MATURED" && Convert.ToDateTime(c.maturityDate) >= inicioMes && Convert.ToDateTime(c.maturityDate) <= finalMes))
                    ));

                foreach (var cuenta in cuentas)
                {
                    try
                    {
                        var insertado = factory.ServicioBitacoraEstadoCuenta
                            .InsertarEstadoDeCuenta(solicitud.Empresa, cuenta.AccountHolderKey, cuenta.Id, solicitud.Fecha, "Inicial");
                        if (insertado)
                            procesados++;
                        else
                            gestorLog.Registrar(Nivel.Information, $"No se pudo procesar el estado de cuenta con el Id {cuenta.Id} del cliente: {cuenta.AccountHolderKey}");

                    }
                    catch (Exception ex)
                    {
                        gestorLog.RegistrarError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                gestorLog.RegistrarError(ex);
                throw;
            }
            finally
            {
                gestorLog.Salir();
            }
            return procesados;
        }
        public async Task<int> EstadosCuentaMensualProcesaAsync(EstadosCuentaMensualProcSol solicitud) =>
            await Task.FromResult(EstadosCuentaMensualProcesa(solicitud));
        /// <summary>
        /// Obtener los estados de cuenta por procesar
        /// </summary>
        /// <param name="solicitud">Informacion para filtrar los estados de cuenta</param>
        /// <returns>Coleccion de estados de cuenta por procesar</returns>
        public List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud)
        {
            gestorLog.Entrar();
            List<EstadoCuentaMensualProcResp> result = null;
            //result = factory.ServicioBitacoraEstadoCuenta.ObtenerBitacorasEstadoCuenta(solicitud.Empresa, solicitud.Fecha);
            result = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.EstadosCuentaMensualObtiene(solicitud);
            gestorLog.Salir();
            return result;
        }
        /// <summary>
        /// Obtener los estados de cuenta por procesar
        /// </summary>
        /// <param name="solicitud">Informacion para filtrar los estados de cuenta</param>
        /// <returns>Coleccion de estados de cuenta por procesar</returns>
        public async Task<List<EstadoCuentaMensualProcResp>> EstadosCuentaMensualObtieneAsync(EstadosCuentaMensualProcSol solicitud) =>
            await Task.FromResult(EstadosCuentaMensualObtiene(solicitud));
        /// <summary>
        /// Generar estado de cuenta
        /// </summary>
        /// <param name="solicitud">Informacion del estado de cuenta a generar</param>
        /// <returns>Instancia de <see cref="ResultadoDocumentoDto"/></returns>
        public ResultadoDocumentoDto EstadoCuentaMensualGenera(EstadoCuentaMensualSol solicitud)
        {
            gestorLog.Entrar();

            // ResultadoDocumentoDto resultado = new ResultadoDocumentoDto();

            documSolicitud = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.AsignarValores(solicitud);

            datosDocumento = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.EstadoCuentaObtenerDatos(documSolicitud);
            if (datosDocumento != null)
            {
                if (documSolicitud.SubProcesoNombre == "EstadosCuentaCAME_SPEI" || documSolicitud.SubProcesoNombre.ToLower() == "edocuenta_inversiones")
                {
                    datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuentaDescripcion(solicitud);
                }
                else if(documSolicitud.SubProcesoNombre == "EstadosCuentaCAME_GRUPAL")
                {

                    datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuentaGrupal(solicitud);
                }
                else
                {
                    datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuenta(solicitud);
                }
                
                ObtenerGeneralesJson();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }
            MemoryStream documento = GeneraDocumento();
            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            gestorLog.Salir();
            return resultado;
        }
        /// <summary>
        /// Generar estado de cuenta
        /// </summary>
        /// <param name="solicitud">Informacion del estado de cuenta a generar</param>
        /// <returns>Instancia de <see cref="ResultadoDocumentoDto"/></returns>
        public async Task<ResultadoDocumentoDto> EstadoCuentaMensualGeneraAsync(EstadoCuentaMensualSol solicitud) =>
            await Task.FromResult(EstadoCuentaMensualGenera(solicitud));
        /// <summary>
        /// Envia un estado de cuenta por correo
        /// </summary>
        /// <param name="solicitud">Informacion del estado de cuenta a enviar</param>
        /// <returns>Instancia de <see cref="ResultadoDocumentoDto"/></returns>
        public ResultadoDocumentoDto EstadoCuentaMensualEnvia(EstadoCuentaMensualSol solicitud)
        {
            gestorLog.Entrar();
            var sp = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual;
            ActualizaEstadoCuentaDto actualizaEdo = new ActualizaEstadoCuentaDto();
            MemoryStream documento = new MemoryStream();
            try
            {

                // ResultadoDocumentoDto resultado = new ResultadoDocumentoDto();
                documSolicitud = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.AsignarValores(solicitud);
                datosDocumento = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.EstadoCuentaObtenerDatos(documSolicitud);
                if (datosDocumento != null)
                {
                    if (documSolicitud.SubProcesoNombre == "EstadosCuentaCAME_SPEI" || documSolicitud.SubProcesoNombre.ToLower() == "edocuenta_inversiones")
                    {
                        datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuentaDescripcion(solicitud);
                    }
                    else
                    {
                        datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuenta(solicitud);
                    }                    
                    ObtenerGeneralesJson();
                    AjustaPlantillasCampos();
                }
                else
                {
                    throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
                }
                documento = GeneraDocumento();
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "PDFGenerado",
                    Observaciones = "",
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
            }
            catch (Exception ex)
            {
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "ErrorEnProceso",
                    Observaciones = ex.Message,
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
                gestorLog.Registrar(Nivel.Information, "Error en procesar");
            }
            //ENVIA CORREO
            try
            {
                _ = PreparaCorreoASync(solicitud, documento, datosJson);
            }
            catch (Exception ex)
            {
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "ErrorEnviado",
                    Observaciones = ex.Message,
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
                gestorLog.Registrar(Nivel.Information, "Error en enviar correo");
            }

            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            gestorLog.Salir();
            return resultado;
        }
        /// <summary>
        /// Envia un estado de cuenta por correo
        /// </summary>
        /// <param name="solicitud">Informacion del estado de cuenta a enviar</param>
        /// <returns>Instancia de <see cref="ResultadoDocumentoDto"/></returns>
        public async Task<ResultadoDocumentoDto> EstadoCuentaMensualEnviaAsync(EstadoCuentaMensualSol solicitud)
        {

            gestorLog.Entrar();
            var sp = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual;
            ActualizaEstadoCuentaDto actualizaEdo = new ActualizaEstadoCuentaDto();
            actualizaEdo = new ActualizaEstadoCuentaDto
            {
                Empresa = solicitud.Empresa,
                FechaEnvio = null,
                Fecha = solicitud.Fecha,
                Estatus = "EnProceso",
                NumeroCuenta = solicitud.NumeroCuenta,
                NumeroCliente = solicitud.NumeroCliente
            };

            factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
            MemoryStream documento = new MemoryStream();
            try
            {

                // ResultadoDocumentoDto resultado = new ResultadoDocumentoDto();
                documSolicitud = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.AsignarValores(solicitud);
                datosDocumento = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.EstadoCuentaObtenerDatos(documSolicitud);
                if (datosDocumento != null)
                {
                    if (documSolicitud.SubProcesoNombre == "EstadosCuentaCAME_SPEI")
                    {
                        datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuentaDescripcion(solicitud);
                    }
                    else
                    {
                        datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.ObtenerDatosEstadoCuenta(solicitud);
                    }                    
                    ObtenerGeneralesJson();
                    AjustaPlantillasCampos();
                }
                else
                {
                    throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
                }
                documento = GeneraDocumento();
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "PDFGenerado",
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
            }
            catch (Exception ex)
            {
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "ErrorEnProceso",
                    Observaciones = ex.Message,
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
                gestorLog.Registrar(Nivel.Information, "Error en procesar");
                throw;

            }
            //ENVIA CORREO

            try
            {
                await PreparaCorreoASync(solicitud, documento, datosJson);
            }
            catch (Exception ex)
            {
                actualizaEdo = new ActualizaEstadoCuentaDto()
                {
                    Empresa = solicitud.Empresa,
                    FechaEnvio = null,
                    Fecha = solicitud.Fecha,
                    Estatus = "ErrorEnviado",
                    Observaciones = ex.Message,
                    NumeroCuenta = solicitud.NumeroCuenta,
                    NumeroCliente = solicitud.NumeroCliente
                };

                factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);

                gestorLog.Registrar(Nivel.Information, "Error en enviar correo");
                throw;
            }

            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            gestorLog.Salir();
            return resultado;
        }

        private async Task<int> PreparaCorreoASync(EstadoCuentaMensualSol solicitud, MemoryStream documento, string dJson)
        {
            var sp = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual;

            ActualizaEstadoCuentaDto actualizaEdo = new ActualizaEstadoCuentaDto();

            var servCorreo = configuration.GetSection("ServicioCorreoNotificaciones");
            List<AdjuntoV2> adj = new List<AdjuntoV2>();
            adj.Add(new AdjuntoV2 { Nombre = "Estado de cuenta.pdf", Contenido = Convert.ToBase64String(documento.ToArray()) });
            var plantillaDinamico = solicitud.Empresa.ToString() + "Plantilla";
            var clienteJson = JsonConvert.DeserializeObject<mambu.ClienteJson>(dJson);
            var correo = new CorreoSengridPost
            {
                Adjuntos = adj,
                Plantilla = new PlantillaSengrid { Contenido = "", Identificador = plantillaDinamico },
                Destinatarios = new List<Destinatario>(),
                DestinatariosCCO = new List<Destinatario>(),
                Asunto = $"{servCorreo["Asunto"].ToString()} {clienteJson.NombreCliente ?? ""} {DateTime.Now.ToString("d")}",
                DatosPlantilla = new List<MapaCampos>(),
                Contenido = ""
                // cantenidoDinamico.Replace("AnoCambio", DateTime.Now.AddMonths(-1).ToString("yyyy")).Replace("MesCambio", DateTime.Now.AddMonths(-1).ToString("MMMM"))
            };
            correo.DatosPlantilla.Add(new MapaCampos
            {
                Tag = "MesCambio",
                Tipo = "String",
                Valor = solicitud.Fecha.ToString("MMMM")
            });
            correo.DatosPlantilla.Add(new MapaCampos
            {
                Tag = "AnoCambio",
                Tipo = "String",
                Valor = solicitud.Fecha.ToString("yyyy")
            });

            List<Parametro> parametros = gestorParametros.RecuperarPorGrupo("SERVICIOEDOCTA");
            var parametrosConfig = parametros.Distinct().ToDictionary(x => x.Codigo, x => x.Valor);

            var correoDemo = parametrosConfig.ContainsKey("CorreoDemo") ? parametrosConfig["CorreoDemo"] : "sistemas@tecas.com.mx";
            var destinatario = parametrosConfig.ContainsKey("CorreoPruebas") ? parametrosConfig["CorreoPruebas"] : clienteJson.Correo;

            correo.DestinatariosCCO.Add(new Destinatario { Alias = "", Email = correoDemo });
            correo.Destinatarios.Add(new Destinatario { Alias = "", Email = destinatario });
            var resCorreo = await UnitOfWork.RepositorioEnviaCorreo.EnviarSenGridPostAsync(correo, servCorreo["NotificacionesURL"].ToString());
            actualizaEdo = new ActualizaEstadoCuentaDto()
            {
                Empresa = solicitud.Empresa,
                FechaEnvio = resCorreo.FechaEnvio,
                Fecha = solicitud.Fecha,
                Estatus = resCorreo.Estatus,
                Observaciones = resCorreo.Observaciones,
                NumeroCuenta = solicitud.NumeroCuenta,
                NumeroCliente = solicitud.NumeroCliente
            };

            factory.ServicioBitacoraEstadoCuenta.ActualizaEstadoCuenta(actualizaEdo);
            return 1;
        }
        private SolictudDocumentoDto AsignarValores(SolictudDocumentoJsonDto solicitud)
        {
            gestorLog.Entrar();
            SolictudDocumentoDto resultado = new SolictudDocumentoDto
            {
                Base64 = solicitud.Base64,
                ProcesoNombre = solicitud.ProcesoNombre,
                SubProcesoNombre = solicitud.SubProcesoNombre,
                NumeroCredito = solicitud.NumeroCredito,
                NumeroCliente = solicitud.NumeroCliente,
                Separado = solicitud.Separado,
                Comprimido = solicitud.Comprimido,
                Usuario = solicitud.Usuario,
                ListaPlantillasIds = solicitud.ListaPlantillasIds,
                TipoPersona = solicitud.TipoPersona,
                Alfresco = solicitud.Alfresco,
                GuardarDocumento = solicitud.GuardarDocumento,
                TipoComprobante = solicitud.TipoComprobante
            };

            gestorLog.Salir();
            return resultado;
        }
        private SolictudDocumentoDto AsignarReciboValores(SolicitudDocumentoReciboJson solicitud)
        {
            gestorLog.Entrar();
            SolictudDocumentoDto resultado = new SolictudDocumentoDto
            {
                ProcesoNombre = solicitud.ProcesoNombre,
                SubProcesoNombre = solicitud.SubProcesoNombre,
                NumeroCredito = solicitud.NumeroCredito,
                NumeroCliente = solicitud.NumeroCliente,
                Separado = solicitud.Separado,
                Comprimido = solicitud.Comprimido,
                Usuario = solicitud.Usuario,
                ListaPlantillasIds = solicitud.ListaPlantillasIds
            };
            gestorLog.Salir();
            return resultado;
        }
        ///Campos Especiales CAME Digital
        ///
        private bool CamposEspecialesJson(Campo item, WordDocument documentoWord, int numInt, long numVol)
        {
            bool resultado = true;
            string valorClienteBen = string.Empty;
            try
            {

                switch (item.CampoNombre)
                {

                    case "<<CAME-TablaAmortizacionPagare>>":
                        #region TablaAmortizacionPagare

                        //Se revisa que no queden variables visibles
                        documentoWord.Replace("«BeginGroup:TablaAmortizacionPagare»", "", true, true);
                        documentoWord.Replace("«NumeroMensualNon»", "", true, true);
                        documentoWord.Replace("«FechaNon»", "", true, true);
                        documentoWord.Replace("«MontoNon»", "", true, true);
                        documentoWord.Replace("«NumeroMensualPar»", "", true, true);
                        documentoWord.Replace("«FechaPar»", "", true, true);
                        documentoWord.Replace("«MontoPar»", "", true, true);
                        documentoWord.Replace("«EndGroup:TablaAmortizacionPagare»", "", true, true);
                        #endregion TablaAmortizacionPagare
                        break;

                    case "<<CAME-TablaAvalesObligados>>":
                        #region TablaAvalesObligados
                        documentoWord.Replace("«BeginGroup:TablaAvalesObligados»", "", true, true);
                        documentoWord.Replace("«Numero»", "", true, true);
                        documentoWord.Replace("«Nombre»", "", true, true);
                        documentoWord.Replace("«Direccion»", "", true, true);
                        documentoWord.Replace("«Estado»", "", true, true);
                        documentoWord.Replace("«Telefono»", "", true, true);
                        documentoWord.Replace("«EndGroup:TablaAvalesObligados»", "", true, true);
                        #endregion TablaAvalesObligados
                        break;
                    case "<<CAME-TablaAmortizacion>>":
                        #region TablaAmortizacion
                        documentoWord.Replace("«BeginGroup:TablaAmortizacion»", "", true, true);
                        documentoWord.Replace("«NumeroPago»", "", true, true);
                        documentoWord.Replace("«FechaLimitePago»", "", true, true);
                        documentoWord.Replace("«SaldoInicial»", "", true, true);
                        documentoWord.Replace("«PagoInteres»", "", true, true);
                        documentoWord.Replace("«IVAInteres»", "", true, true);
                        documentoWord.Replace("«PagoPricipal»", "", true, true);
                        documentoWord.Replace("«PagoTotal»", "", true, true);
                        documentoWord.Replace("«SaldoInsolutoPricipal»", "", true, true);
                        documentoWord.Replace("«EndGroup:TablaAmortizacion»", "", true, true);
                        #endregion TablaAmortizacion
                        break;
                    case "<<CAME-TablaBeneficiarios>>":
                        #region TablaBeneficiarios
                        documentoWord.Replace("«BeginGroup:TablaBeneficiarios»", "", true, true);
                        documentoWord.Replace("«Nombre»", "", true, true);
                        documentoWord.Replace("«Parentesco»", "", true, true);
                        documentoWord.Replace("«FechaNacimiento»", "", true, true);
                        documentoWord.Replace("«Porcentaje»", "", true, true);
                        documentoWord.Replace("«EndGroup:TablaBeneficiarios»", "", true, true);
                        #endregion TablaBeneficiarios
                        break;
                    case "<<PaqPlatino>>":
                        #region PaqPlatino
                        documentoWord.Replace("<<PaqPlatino>>", "", true, true);
                        #endregion PaqPlatino
                        break;
                    case "<<PaqBasico>>":
                        #region PaqBasico
                        documentoWord.Replace("<<PaqBasico>>", "", true, true);
                        #endregion PaqBasico
                        break;
                    case "<<PaqPremium>>":
                        #region PaqPremium
                        documentoWord.Replace("<<PaqPremium>>", "", true, true);
                        #endregion PaqPremium
                        break;



                    default:
                        resultado = false;
                        break;
                }

            }
            catch (Exception)
            {
                //  resultado = false;
                // throw;
            }

            return resultado;
        }
        public ResultadoDocumentoDto EstadoCuentaCredito(EstadoCuentaCreditoSolDto solicitud)
        {
            gestorLog.Entrar();
            documSolicitud = _mapper.Map<SolictudDocumentoDto>(solicitud);
            datosDocumento = serviceProvider(solicitud.Empresa.ToString()).ServicioDatosPlantillas.EstadoCuentaObtenerDatos(documSolicitud);
            if (datosDocumento != null)
            {
                datosJson = serviceProvider(solicitud.Empresa.ToString()).ServicioEstadoCuentaMensual.EstadoCuentaCredito(solicitud);
                ObtenerGeneralesJson();
                AjustaPlantillasCampos();
            }
            else
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);

            MemoryStream documento = GeneraDocumento();
            gestorLog.Salir();
            return PreparaResultado(documento);
        }
        public ResultadoDocumentoDto ObtenerDocumentosJson(SolictudDocumentoJsonDto solicitud)
        {
            gestorLog.Entrar();

            documSolicitudJson = solicitud;
            documSolicitud = AsignarValores(solicitud);
            ObtenerJson();
            MemoryStream documento = GeneraDocumento();
            DocumentoGuarda();
            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            resultado = GuardaResultado(resultado);
            gestorLog.Salir();
            return resultado;
        }

        public ResultadoUrlDocumeto ObtieneRutaDocumentoJson(SolictudDocumentoJsonDto solicitud)
        {
            gestorLog.Entrar();
            documSolicitudJson = solicitud;
            documSolicitud = AsignarValores(solicitud);
            ObtenerJson();
            MemoryStream documento = GeneraDocumento();

            ResultadoUrlDocumeto resultado = ResultadoRutas(documento);
            gestorLog.Salir();
            return resultado;
        }

        public ResultadoUrlDocumeto ObtenerRutaReciboJson(SolicitudDocumentoReciboJson solicitud)
        {
            gestorLog.Entrar();
            documentoReciboJson = solicitud;
            documSolicitud = AsignarReciboValores(solicitud);
            ObtenerJsonSolicitud();
            MemoryStream documento = GeneraDocumento();

            ResultadoUrlDocumeto resultado = ResultadoRutas(documento);

            gestorLog.Salir();
            return resultado;

        }

        private void ObtenerJsonSolicitud()
        {
            gestorLog.Entrar();

            datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            if (datosDocumento != null)
            {
                obtieneReciboJson();
                //SeleccionaPlantillas();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }

            gestorLog.Salir();

        }
        private void ObtenerJson()
        {
            gestorLog.Entrar();

            datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            if (datosDocumento != null)
            {
                ObtenerGeneralesJson();
                ObtieneJson();
                SeleccionaPlantillas();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }

            gestorLog.Salir();
        }

        private void obtieneReciboJson()
        {
            gestorLog.Entrar();

            ObtenerDatosJsonDto solicitud = new ObtenerDatosJsonDto
            {
                Empresa = datosDocumento.Empresa,
                // Proceso = datosDocumento.ProcesoId,
                ProcesoNombre = datosDocumento.ProcesoNombre,
                SubProcesoNombre = datosDocumento.SubProcesoNombre,
                NumeroCredito = documentoReciboJson.NumeroCredito,
                NumeroCliente = documentoReciboJson.NumeroCliente,
                Usuario = documentoReciboJson.Usuario,
                JsonSolicitud = documentoReciboJson.JsonSolicitud
            };

            clienteRecibo = JsonConvert.DeserializeObject<Dcame.Recibo>(documentoReciboJson.JsonSolicitud);
            datosJson = documentoReciboJson.JsonSolicitud;
            datosQR = "NoVeces:" + clienteRecibo.NumeroContador + " Fecha:" + clienteRecibo.Fecha + " " + clienteRecibo.Hora + " Folio:" + clienteRecibo.Folio + " LugardePago:" + clienteRecibo.LugaPago + " NumeroCliente:" + clienteRecibo.NumeroCliente + " NombreCliente: " + clienteRecibo.NombreCompleto + " NumeroGrupo:" + clienteRecibo.NumeroGrupo + " NombreGrupo:" + clienteRecibo.NombreGrupo + " Cantidad: " + clienteRecibo.Monto + " " + clienteRecibo.Cantidad;
            datosCD = clienteRecibo.BarcodeOxxo;
            gestorLog.Salir();
        }
        private void ObtieneJson()
        {
            gestorLog.Entrar();

            ObtenerDatosJsonDto solicitud = new ObtenerDatosJsonDto
            {
                Empresa = datosDocumento.Empresa,
                // Proceso = datosDocumento.ProcesoId,
                ProcesoNombre = datosDocumento.ProcesoNombre,
                SubProcesoNombre = datosDocumento.SubProcesoNombre,
                NumeroCredito = documSolicitudJson.NumeroCredito,
                NumeroCliente = documSolicitudJson.NumeroCliente,
                Usuario = documSolicitudJson.Usuario,
                JsonSolicitud = documSolicitudJson.JsonSolicitud
            };

            //   datosJson = factory.ServicioDatosPlantillas.ObtenerDatosPlantilla();
            if (solicitud.SubProcesoNombre == "RenovacionIndividual" || solicitud.SubProcesoNombre.ToLower() == "clubcamecompinv" || solicitud.SubProcesoNombre.ToLower() == "crecemascompinv" || solicitud.SubProcesoNombre.ToLower() == "clubcameticketinv" || solicitud.SubProcesoNombre.ToLower() == "crecemasticketinv" 
                || solicitud.SubProcesoNombre.ToLower() == "crecemasempresas")
            {
                try
                {
                    factory.ServicioDatosPlantillas.ResetearDatos(solicitud);
                }
                catch (BusinessException buex)
                {
                    Console.WriteLine(buex.Message);
                }

            }

            datosJson = factory.ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);

            //Se deserializa el objeto para mapear la documentación
            clienteJson = JsonConvert.DeserializeObject<mambu.ClienteJson>(datosJson);

            gestorLog.Salir();
        }

        private void ObtenerGeneralesJson()
        {
            gestorLog.Entrar();
            datosGenerales = factory.ServicioGenerales.Obtiene(datosDocumento.ProcesoId);
            gestorLog.Salir();
        }
        private string ObtieneDecimales(decimal monto, int decimales)
        {
            string resultado = "";
            try
            {
                // decimal.TryParse(valor, out monto);
                int enteros = (int)monto;
                decimal factor = (decimal)Math.Pow(10, decimales);
                decimal trunc = Math.Truncate(factor * monto) / factor;
                string sTrunc = (trunc - enteros).ToString();
                resultado = sTrunc.Split('.')[1];
            }
            catch (Exception)
            {
                resultado = "";
            }
            return resultado;
        }
        private string TruncarDecimales(decimal monto, int decimales)
        {
            var resultado = "";
            try
            {
                int enteros = (int)monto;
                decimal factor = (decimal)Math.Pow(10, decimales);
                resultado = (Math.Truncate(factor * monto) / factor).ToString();
            }
            catch (Exception)
            {
                resultado = "";
            }
            return resultado;
        }
        private string Code128(string NoReferencia)
        {

            string resp = "";
            int i;
            int checksum = 0;
            int mini;
            int dummy;
            Boolean tableB;
            resp = "";


            Encoding ANSI = Encoding.GetEncoding(1252);
            //dummy = ANSI.GetBytes(letra)[0];

            string salidafor = "";

            if (NoReferencia.Length > 0)
            {
                for (i = 1; i <= NoReferencia.Length; i += 1)
                {
                    int varint = ANSI.GetBytes(NoReferencia.Substring(i - 1, 1))[0];
                    if ((varint >= 32 && varint <= 126) || varint == 203)
                    {
                    }
                    else
                    {
                        i = 0;
                        salidafor = "si";
                        break;
                    }

                    if (salidafor == "si")
                        break;
                }

                resp = "";
                tableB = true;

                if (i > 0)
                {
                    i = 1;
                    while (i <= (NoReferencia.Length))
                    {
                        if (tableB)
                        {
                            if ((i == 1) || (i + 3 == NoReferencia.Length))
                                mini = 4;
                            else
                                mini = 6;

                            EfuncionCod128 r = new EfuncionCod128();
                            r = testnum(mini, i, NoReferencia);
                            mini = r.mini;
                            i = r.i;
                            NoReferencia = r.NoReferencia;

                            if (mini < 0)
                            {
                                if (i == 1)
                                    resp = Convert.ToString((char)210);
                                else
                                    resp = resp + (char)204;

                                tableB = false;
                            }
                            else
                            {
                                if (i == 1)
                                    resp = Convert.ToString((char)209);
                            }
                        }

                        if (tableB == false)
                        {
                            mini = 2;

                            EfuncionCod128 r = new EfuncionCod128();
                            r = testnum(mini, i, NoReferencia);
                            mini = r.mini;
                            i = r.i;
                            NoReferencia = r.NoReferencia;

                            if (mini < 0)
                            {
                                try
                                {
                                    dummy = Convert.ToInt32(NoReferencia.Substring(i - 1, 2));
                                }
                                catch
                                {
                                    dummy = 0;
                                }

                                if (dummy < 95)
                                    dummy = dummy + 32;
                                else
                                    dummy = dummy + 105;
                                resp = resp + (char)dummy;
                                i = i + 2;
                            }
                            else
                            {
                                resp = resp + (char)205;
                                tableB = true;
                            }
                        }

                        if (tableB)
                        {
                            resp = resp + NoReferencia.Substring(i - 1, 1);
                            i = i + 1;
                        }
                    }

                    for (i = 1; i <= resp.Length; i += 1)
                    {
                        string letra = resp.Substring(i - 1, 1);
                        //dummy = Encoding..ASCII.GetBytes(letra)[0];
                        dummy = ANSI.GetBytes(letra)[0];
                        if (dummy < 127)
                            dummy = dummy - 32;
                        else
                            dummy = dummy - 105;

                        if (i == 1)
                            checksum = dummy;
                        checksum = (checksum + (i - 1) * dummy) % 103;
                    }

                    if (checksum < 95)
                        checksum = checksum + 32;
                    else
                        checksum = checksum + 105;
                    resp = resp + (char)checksum + (char)211;
                }
            }




            return resp;
        }

        private class EfuncionCod128
        {
            public int mini { get; set; }

            public int i { get; set; }

            public string NoReferencia { get; set; }
        }

        private EfuncionCod128 testnum(int mini, int i, string NoReferencia)
        {

            Encoding ANSI = Encoding.GetEncoding(1252);
            EfuncionCod128 r = new EfuncionCod128();

            mini = mini - 1;
            if (i + mini <= NoReferencia.Length)
            {
                while (mini >= 0)
                {
                    if (((ANSI.GetBytes(NoReferencia.Substring(i - 1 + mini, 1))[0]) < 48) ||
                        ((ANSI.GetBytes(NoReferencia.Substring(i - 1 + mini, 1))[0]) > 57))
                        break;
                    mini = mini - 1;
                }
            }
            r.mini = mini;
            r.i = i;
            r.NoReferencia = NoReferencia;


            return r;
        }

        public ResultadoDocumentoDto VistaPrevia(SolictudVistaPreviaDto solicitud)
        {
            gestorLog.Entrar();
            ResultadoDocumentoDto resultado = null;

            documSolicitud = new SolictudDocumentoDto()
            {
                NumeroCliente = solicitud.NumeroCliente,
                NumeroCredito = solicitud.NumeroCredito,
            };

            MemoryStream documento = GeneraDocumentoVistaPrevia(solicitud);
            resultado = PreparaResultado(documento);

            gestorLog.Salir();
            return resultado;
        }

        private MemoryStream GeneraDocumentoVistaPrevia(SolictudVistaPreviaDto solicitud)
        {
            gestorLog.Entrar();

            MemoryStream resultado = new MemoryStream();
            try
            {
                //Se crea el objeto que contendrá todos los documentos
                PdfDocument documentos = new PdfDocument
                {
                    EnableMemoryOptimization = true,
                    Compression = PdfCompressionLevel.Best
                };

                WordDocument documentoWord = CreaDocumentoWordfromBase64(solicitud.PlantillaBase64);   /// 
                MapeaDatosDocumentoWordVistaPrevia(documentoWord, solicitud.ListaCampos);

                //Se crea el objeto que convierte el documento word a pdf
                MemoryStream streamPDF = WordToPDF(documentoWord);
                documentoWord.Close();
                documentoWord.Dispose();
                documentoWord = null;

                PdfDocumentBase.Merge(documentos, streamPDF);

                //Se cierra el stream
                streamPDF.Close();
                streamPDF.Dispose();

                documentos.Save(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // GestorErrores.Manejar(ex, CapaExcepcion.LogicaNegocios);
            }
            resultado.Position = 0;

            gestorLog.Salir();
            return resultado;
        }

        private WordDocument CreaDocumentoWordfromBase64(string ArchivoBase64)
        {
            gestorLog.Entrar();
            //Se crea el documento word que almacena el documento que contiene el stream
            WordDocument documentoWord = null;
            try
            {

                byte[] bytes = Convert.FromBase64String(ArchivoBase64);
                MemoryStream ms = new MemoryStream(bytes);
                documentoWord = CreaWordfromStream(ms);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new BusinessException("No se pudo obtener  el documento Word base64.");
            }
            gestorLog.Salir();
            return documentoWord;
        }

        private WordDocument CreaWordfromStream(Stream stream)
        {
            gestorLog.Entrar();
            //Se crea el documento word que almacena el documento que contiene el stream
            WordDocument documentoWord = null;
            try
            {

                if (stream != null)
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    documentoWord = new WordDocument(ms, FormatType.Doc);
                    documentoWord.MailMerge.ClearFields = false;

                    //// Comprobacion = OK
                    //FileStream file = new FileStream(@"C:\ServDocsPruebas\Plantilla.doc", FileMode.Create, System.IO.FileAccess.Write);
                    //documentoWord.Save(file, FormatType.Doc); //streamDocumento.WriteTo(file);
                    //file.Close();

                    ms.Close();
                    ms.Dispose();
                }
                stream.Close();
                stream.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new BusinessException("No se pudo generar el documento Word.");
            }
            gestorLog.Salir();
            return documentoWord;
        }

        private void MapeaDatosDocumentoWordVistaPrevia(WordDocument documentoWord, IEnumerable<CampoPreviewDto> campos)
        {
            gestorLog.Entrar();
            try
            {
                //Ciclo que recorre la lista de campos que se van a mapear en el documento
                foreach (CampoPreviewDto item in campos)
                {

                    string valor = "";
                    if (item.Tipo == "Tabla")
                    {
                        DataTable dtTable = CreaTabla(item);
                        documentoWord.MailMerge.ClearFields = true;
                        documentoWord.MailMerge.ExecuteGroup(dtTable);
                    }
                    else
                    {
                        // valor = ObtenerValorPropiedad(item);
                        valor = item.Ejemplo;
                    }
                    valor = CampoTipo(valor, item.Tipo);
                    documentoWord.Replace(item.CampoNombre, valor, true, true);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // GestorErrores.Manejar(ex, CapaExcepcion.LogicaNegocios);
            }
            gestorLog.Salir();
        }

        private DataTable CreaTabla(CampoPreviewDto dataCampo)
        {
            DataTable Tabla = new DataTable();
            try
            {

                var jDatosTabla = JObject.Parse(dataCampo.DatoCampo);
                string valor = jDatosTabla.SelectToken("Tabla").ToString();
                Tabla.TableName = valor;
                string token = string.Empty;
                JArray datos = new JArray();

                valor = jDatosTabla.SelectToken("Encabezado").ToString();
                string[] Columns = valor.Split(',');
                foreach (string Titulo in Columns)
                {
                    Tabla.Columns.Add(Titulo, typeof(String));
                }

                var jDatosJson = JObject.Parse(dataCampo.Ejemplo);
                JArray jDatosEjemplo = (JArray)jDatosJson.SelectToken("Ejemplo");

                JArray jCampos = (JArray)jDatosTabla.SelectToken("Datos");
                DataRow dtRow = Tabla.NewRow();
                int Cols = 0;
                string valorCampo = "";
                foreach (string Titulo in Columns)
                {
                    token = jCampos[Cols]["Campo"].ToString();
                    string formato = jCampos[Cols]["Tipo"].ToString();

                    valorCampo = jDatosEjemplo[Cols][Titulo].ToString();
                    valorCampo = CampoTipo(valorCampo, formato.Trim());
                    dtRow[Titulo] = valorCampo;
                    Cols++;
                }
                Tabla.Rows.Add(dtRow);

            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
                //throw;
            }
            return Tabla;
        }

        private DataTable CreaForma(Campo dataCampo, int numInt)
        {
            DataTable Tabla = new DataTable();

            try
            {
                var datos = JObject.Parse(dataCampo.DatoCampo);
                JArray campos = (JArray)datos.SelectToken("Datos");
                string tablanombre = (string)datos.SelectToken("Tabla");
                Tabla.TableName = tablanombre;

                foreach (JToken campo in campos.Children())
                {
                    string etiqueta = (string)campo["Etiqueta"];
                    Tabla.Columns.Add(etiqueta, typeof(String));
                }

                int numRegistro = 0;
                var solicitud = JObject.Parse(datosJson);
                string token = $"solicitudJSON." + datos.SelectToken("DatoConjunto").ToString();
                var arrayDato = solicitud.SelectToken(token.Replace("[", "").Replace("]", ""));

                JArray arrayDatos = new JArray();
                if (arrayDato.Type == JTokenType.Array) { arrayDatos = (JArray)arrayDato; } else { arrayDatos.Add(arrayDato); }

                //registro es un cotitular
                foreach (JToken registro in arrayDatos.Children())
                {
                    int i = 0;
                    DataRow dtRow = Tabla.NewRow();
                    foreach (JToken campo in campos.Children())
                    {
                        string tmpDatoConjunto = (string)campo["DatoConjunto"];
                        string tmpDatoCampo = (string)campo["DatoCampo"];
                        string tmpTipo = (string)campo["Tipo"];
                        string tmpOpciones = (string)campo["Opciones"];
                        string valorCampo = "";

                        if (tmpDatoCampo == "Consecutivo")
                        {
                            valorCampo = ToRoman(numRegistro);
                        }
                        else
                        {
                            string correcionValorConjunto = string.IsNullOrEmpty(tmpDatoConjunto) ? $"{token}" : $"{ token}.{tmpDatoConjunto}";

                            var tempCampo = new Campo()
                            {
                                DatoCampo = tmpDatoCampo,
                                DatoConjunto = correcionValorConjunto
                                //DatoConjunto = $"{token}.{tmpDatoConjunto}"
                                //DatoConjunto = dataCampo.DatoConjunto + "." + tmpDatoConjunto
                            };

                            valorCampo = ObtenerValorPropiedad(tempCampo, numRegistro);
                            valorCampo = CampoTipo(valorCampo, tmpTipo, tmpOpciones);
                        }
                        dtRow[i] = valorCampo;
                        i++;
                    }
                    Tabla.Rows.Add(dtRow);
                    numRegistro++;
                }

            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
                //throw;
            }
            return Tabla;
        }

        private string ObtenerValorPropiedadV2NOSEUSA(Campo Campo, int numInt)
        {
            gestorLog.Entrar();
            string valor = "";
            string conjuntoJson = string.Empty;
            string token = string.Empty;
            string tokenGen = string.Empty;

            JToken General = null;


            try
            {
                string campo = Campo.DatoCampo.Replace("<", "").Replace(">", "");
                var jDatos = JObject.Parse(datosJson);
                if (Campo.Tipo == "Seleccion")
                {
                    string opciones = "";
                    var jDatosSolicitud = jDatos.SelectToken("solicitudJSON");
                    var jCampo = JObject.Parse(Campo.DatoCampo);
                    token = Campo.DatoConjunto + (Campo.DatoConjunto == null ? "" : ".") + jCampo.SelectToken("DatoCampo").ToString();
                    string datoselecccion = jDatosSolicitud.SelectToken(token)?.ToString() ?? "";

                    string datosOpciones = jCampo.SelectToken("Opciones").ToString();
                    string[] ListaOpciones = datosOpciones.Split("|");
                    foreach (string opcion in ListaOpciones)
                    {
                        string cadenaValor = opcion;
                        var temp = cadenaValor.Split("&");

                        string seleccion = string.Empty;
                        if (temp.Length > 1)
                        {
                            seleccion = (temp[0] == datoselecccion ? "X" : " ");
                            cadenaValor = cadenaValor.Replace("&", "");
                        }
                        else
                        {
                            seleccion = (cadenaValor == datoselecccion ? "X" : " ");
                        }

                        opciones = opciones + $"  [{seleccion}]" + cadenaValor;
                    }

                    valor = opciones;
                }
                else if (Campo.DatoConjunto == "Generales")
                {
                    if (!string.IsNullOrEmpty(datosGenerales))
                    {
                        try
                        {
                            JArray jdata = JArray.Parse(datosGenerales);
                            tokenGen = string.Format("$[?(@.CampoNombre == '{0}')]", Campo.DatoCampo);
                            General = (JToken)jdata.SelectToken(tokenGen);
                        }
                        catch (Exception)
                        {
                            General = null;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(Campo.DatoConjuntoGrupal))
                {
                    if (Campo.DatoConjunto.StartsWith("solicitudJSON."))
                    {
                        if (Campo.DatoConjunto.Contains("[]"))
                        {
                            Campo.DatoConjunto = Campo.DatoConjunto.Replace("[]", $"[{numInt}]");
                        }
                    }
                    token = (Campo.DatoConjunto == "" ? "" : Campo.DatoConjunto + ".") + campo;
                }
                else
                {
                    if (numInt > 0)
                    {
                        numInt = numInt - 1;
                        conjuntoJson = Campo.DatoConjuntoGrupal; // AhorroIndividual[2]
                        conjuntoJson = conjuntoJson.Replace("[]", "[" + numInt.ToString() + "]");
                        token = (conjuntoJson == "" ? "" : conjuntoJson + ".") + campo;
                    }
                    else
                    {
                        token = (Campo.DatoConjunto == "" ? "" : Campo.DatoConjunto + ".") + campo;
                    }
                }
                if (Campo.DatoConjunto == "Generales")
                {
                    if (General != null)
                        valor = (((JValue)General["Valor"]).Value).ToString();
                }
                else
                {
                    if (valor == "")
                        //valor = jDatos.SelectToken(token)?.ToString() ?? "";
                        valor = (string)jDatos.SelectToken(token) ?? "";
                }
            }
            catch (Exception)
            {
                valor = "";
            }

            gestorLog.Salir();
            return valor;
        }

        private string CampoTipo(string valorOriginal, string tipo, string valores)
        {
            gestorLog.Entrar();
            string valor = "";
            try
            {
                switch (tipo)
                {
                    case "Seleccion":
                        var selecciones = "";
                        string[] listOpciones = valores.Split("|");
                        foreach (string opcion in listOpciones)
                        {
                            string cadenaValor = opcion;
                            var temp = cadenaValor.Split("&");

                            string seleccionado = "";
                            string[] connumerador = cadenaValor.Split(')');

                            if (temp.Length > 1)
                            {
                                seleccionado = (temp[0].ToUpper().Equals(valorOriginal.ToUpper()) ? "X" : " ");
                                cadenaValor = cadenaValor.Replace("&", " ");

                                if (seleccionado.Equals("X"))
                                {
                                    selecciones = $"[{seleccionado}]{cadenaValor} ";
                                }
                            }
                            else if (connumerador.Length > 1)
                            {
                                seleccionado = valorOriginal.ToUpper().Equals(connumerador[0].ToUpper()) ? "X" : "_";
                                selecciones = $"{selecciones} [{seleccionado}]{cadenaValor} " + "\r\n";
                            }
                            else
                            {
                                seleccionado = valorOriginal.ToUpper().Equals(cadenaValor.ToUpper()) ? "X" : "_";
                                selecciones = $"{selecciones} [{seleccionado}]{cadenaValor} ";
                            }

                        }
                        valor = selecciones.ToUpper();
                        break;
                    case "Predeterminado":
                        var selecciones2 = "";
                        string[] listOpciones2 = valores.Split("|");
                        foreach (string opcion in listOpciones2)
                        {
                            string cadenaValor = opcion;
                            string seleccionado = "";

                            seleccionado = valorOriginal.ToUpper().Equals(cadenaValor.ToUpper()) ? "X" : "_";
                            selecciones2 = $"{selecciones2} {cadenaValor}[{seleccionado}] ";
                        }
                        valor = selecciones2;
                        break;
                    default:
                        valor = valorOriginal;
                        break;
                }

            }
            catch (Exception)
            {
                valor = "";
            }

            gestorLog.Salir();
            return valor;
        }

        private static string ToRoman(int number)
        {
            string[] numbers = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX", "XXXI", "XXXII", "XXXIII", "XXXIV", "XXXV", "XXXVI", "XXXVII", "XXXVIII", "XXXIX", "XL", "XLI", "XLII", "XLIII", "XLIV", "XLV", "XLVI", "XLVII", "XLVIII", "XLIX", "L", "LI", "LII", "LIII", "LIV", "LV", "LVI", "LVII", "LVIII", "LIX", "LX", "LXI", "LXII", "LXIII", "LXIV", "LXV", "LXVI", "LXVII", "LXVIII", "LXIX", "LXX", "LXXI", "LXXII", "LXXIII", "LXXIV", "LXXV", "LXXVI", "LXXVII", "LXXVIII", "LXXIX", "LXXX", "LXXXI", "LXXXII", "LXXXIII", "LXXXIV", "LXXXV", "LXXXVI", "LXXXVII", "LXXXVIII", "LXXXIX", "XC", "XCI", "XCII", "XCIII", "XCIV", "XCV", "XCVI", "XCVII", "XCVIII", "XCIX", "C" };
            return numbers[number];
        }

        private string ValidarFecha(string date)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(date))
            {
                return date;
            }

            CultureInfo culture = new CultureInfo("es-Mx");
            string[] arrayFormats = { "dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy/MM/dd", "yyyy-MM-dd" };

            try
            {
                string valor = date.Substring(0, 10);

                if (!(valor.Contains("-") || valor.Contains("/")))
                {
                    return date;
                }

                string separador = valor.Contains("-") ? "-" : "/";
                result = DateTime.ParseExact(valor, arrayFormats, culture).ToString($"dd{separador}MM{separador}yyyy");
            }
            catch (Exception)
            {
                result = date;

                try
                {
                    string valor = date.Substring(0, 8);

                    if (!(valor.Contains("-") || valor.Contains("/")))
                    {
                        return date;
                    }
                    else
                    {
                        string separador = valor.Contains("-") ? "-" : "/";

                        string[] array = valor.Split(separador);

                        for (int i = 0; i < array.Length - 1; i++)
                        {
                            array[i] = (array[i].Length == 1 ? "0" : "") + array[i];
                        }

                        result = DateTime.ParseExact($"{array[0]}{separador}{array[1]}{separador}{array[2]}", arrayFormats, culture).ToString($"dd{separador}MM{separador}yyyy");
                    }
                }
                catch (Exception)
                {
                    result = date;
                }
            }

            return result;
        }

        private List<int> AjustaTechreoSeguroVida()
        {
            List<int> list = new List<int>();
            PlantillaDto plantilla = null;
            ProcesoSubDto obtenerPlantillas = new ProcesoSubDto()
            {
                ProcesoNombre = documSolicitud.ProcesoNombre,
                SubProcesoNombre = documSolicitud.SubProcesoNombre
            };
            var jsonSeguros = JObject.Parse(datosJson);
            JArray seguros = (JArray)jsonSeguros.SelectToken("Seguro");

            var plantillas = UnitOfWork.RepositorioPlantillas.ObtenerListadoPorSubProceso(obtenerPlantillas);
            var plantillasIds = UnitOfWork.RepositorioPlantillas.ObtenerListadoPorSubProceso(obtenerPlantillas).Select(x => x.PlantillaId);

            foreach (var seguro in seguros)
            {
                plantilla = plantillas.Where(x => x.DescripcionDocumentos == seguro["CodigoSeguroDescripcion"].ToString()).FirstOrDefault();
                if (plantilla != null)
                {
                    list.Add(plantilla.PlantillaId);
                }
            }

            return list;
        }

        private void AgregaSoporteTipoOpciones(ref string DatosOpciones, string TipoOpciones, ref string DatosSeleccion)
        {
            string datosOpcionesTipo = string.Empty;
            int cont = 0;

            string[] opciones = DatosOpciones.Split('|');
            foreach (var opcion in opciones)
            {
                if (cont < opciones.Length - 1)
                {
                    datosOpcionesTipo = $"{datosOpcionesTipo}{CampoTipo(opcion, TipoOpciones)}|";
                }
                else
                {
                    datosOpcionesTipo = $"{datosOpcionesTipo}{CampoTipo(opcion, TipoOpciones)}";
                }
                cont++;
            }

            DatosOpciones = datosOpcionesTipo;
            DatosSeleccion = CampoTipo(DatosSeleccion, TipoOpciones);
        }

        public ResultadoDocumentoDto ObtenerDocumentosJsonDatos(SolictudDocumentoJsonDatosDto solicitud)
        {
            gestorLog.Entrar();
            documSolicitudJsonDatos = solicitud;
            documSolicitud = AsignarValores(solicitud);
            ObtenerJsonDatos();
            MemoryStream documento = GeneraDocumento();
            ResultadoDocumentoDto resultado = PreparaResultado(documento);
            gestorLog.Salir();
            return resultado;
        }

        private SolictudDocumentoDto AsignarValores(SolictudDocumentoJsonDatosDto solicitud)
        {
            gestorLog.Entrar();
            SolictudDocumentoDto resultado = new SolictudDocumentoDto
            {
                Base64 = solicitud.Base64,
                ProcesoNombre = solicitud.ProcesoNombre,
                SubProcesoNombre = solicitud.SubProcesoNombre,
                Separado = solicitud.Separado,
                Comprimido = solicitud.Comprimido,
                Usuario = solicitud.Usuario,
                ListaPlantillasIds = solicitud.ListaPlantillasIds
            };
            gestorLog.Salir();
            return resultado;
        }

        private void ObtenerJsonDatos()
        {
            gestorLog.Entrar();

            datosDocumento = UnitOfWork.RepositorioPlantillas.DocumentoDatosPorSubProceso(documSolicitud);
            if (datosDocumento != null)
            {
                ObtenerGeneralesJson();
                ObtieneJsonDatos();
                AjustaPlantillasCampos();
            }
            else
            {
                throw new BusinessException(MensajesServicios.ErrorObtenerDocumentos);
            }

            gestorLog.Salir();
        }

        private void ObtieneJsonDatos()
        {
            gestorLog.Entrar();

            ObtenerDatosJsonDatosDto solicitud = new ObtenerDatosJsonDatosDto
            {
                Empresa = datosDocumento.Empresa,
                ProcesoNombre = datosDocumento.ProcesoNombre,
                SubProcesoNombre = datosDocumento.SubProcesoNombre,
                Usuario = documSolicitudJsonDatos.Usuario,
                JsonSolicitudData = documSolicitudJsonDatos.JsonSolicitudData,
                Gat = documSolicitudJsonDatos.Gat
            };

            datosJson = factory.ServicioDatosPlantillas.ObtenerDatosPlantilla(solicitud);
            gestorLog.Salir();
        }
        /// <summary>
        /// Obtiene el producto configurado en la base de datos de parametros
        /// </summary>
        /// <returns>Nombre del producto digipro</returns>
        private string ObtenerProductoDigipro()
        {
            if (string.IsNullOrEmpty(_productoDigipro))
            {
                var parametro = gestorParametros.RecuperarPorCodigo("Digipro", "Producto");
                _productoDigipro = parametro?.Valor ?? string.Empty;
            }
            return _productoDigipro;
        }
        /// <summary>
        /// Obtiene la empresa configurada en la base de datos de parametros
        /// </summary>
        /// <returns>Nombre de la empresa para guardar en digipro</returns>
        private string ObtenerEmpresaDigipro()
        {
            if (string.IsNullOrEmpty(_empresaDigpro))
            {
                var parametro = gestorParametros.RecuperarPorCodigo("Digipro", "Empresa");
                _empresaDigpro = parametro?.Valor ?? string.Empty;
            }
            return _empresaDigpro;
        }
        /// <summary>
        /// Obtiene todos las cuentas de ahorro repartiendo la carga en multiples peticiones por la limitacion en mambu
        /// </summary>
        /// <param name="empresa">Empresa</param>
        /// <param name="productosValidos">Productos validos</param>
        /// <returns>Coleccion de ahorros</returns>
        private IEnumerable<Core.Dtos.DatosMambu.AhorroDto> ObtenerAhorros(string empresa, IEnumerable<string> productosValidos)
        {
            var filtros = new SearchCriteria();
            filtros.AgregarFiltroIn("accountState", new List<string>() { "ACTIVE", "CLOSED", "MATURED" });
            filtros.AgregarFiltroIn("productTypeKey", productosValidos);
            filtros.AgregarLimite(LimiteCuentas);
            filtros.AgregarOrdenamiento("encodedKey", "DESC");
            filtros.AgregarFiltroEsVacio("_Datos_Adicionales_Cuenta._Origen");
            var repositorio = _mambuFactory.Crear(empresa).RepositorioAhorros;
            var cuentas = repositorio.Search(filtros).Result.ToList();

            if (cuentas.Count() < LimiteCuentas)
                return cuentas;

            while (true)
            {
                try
                {
                    filtros.AgregarOffset(cuentas.Count());
                    var cuentasNuevas = repositorio.Search(filtros).Result;
                    cuentas.AddRange(cuentasNuevas);
                    if (cuentasNuevas.Count() < LimiteCuentas)
                        break;
                }
                catch (Exception ex)
                {
                    gestorLog.RegistrarError(ex);
                    break;
                }
            }
            var filtroCuentas = ObtenerFiltroCuentasAhorroPruebas();
            return filtroCuentas != null && filtroCuentas.Any() ? cuentas.Where(c => filtroCuentas.Contains(c.EncodedKey) || filtroCuentas.Contains(c.Id)) : cuentas;
        }
        /// <summary>
        /// Obtener lista de id o encoded keys de las cuentas de ahorro para pruebas
        /// </summary>
        /// <returns>Coleccion de cuentas</returns>
        private IEnumerable<string> ObtenerFiltroCuentasAhorroPruebas()
        {
            var parametro = gestorParametros.RecuperarPorCodigo("SERVICIOEDOCTA", "CuentasPruebas");
            if (parametro != null && !string.IsNullOrEmpty(parametro.Valor))
                return parametro.Valor.Split(",").Select(c => c.Trim());
            return Enumerable.Empty<string>();

        }
    }
}

public static class WordDocumentExtensions
{
    public static T SingleOrDefault<T>(this WordDocument wordDocument, Func<T, bool> func)
    {
        foreach (WSection section in wordDocument.Sections)
        {
            foreach (WParagraph paragraph in section.Paragraphs)
            {
                var tmp = paragraph.ChildEntities.OfType<T>();

                var item = paragraph.ChildEntities.OfType<T>().SingleOrDefault(func);
                if (item != null)
                {
                    return item;
                }
            }
        }

        return default(T);
    }

    public static WChart FindChartByTitle(this WordDocument wordDocument, string chartTitle)
    {
        if (wordDocument == null) throw new NullReferenceException($"{nameof(wordDocument)} can't be null");
        var chart = wordDocument.SingleOrDefault<WChart>(c => c.ChartTitle == chartTitle);
        return chart;
    }
}

