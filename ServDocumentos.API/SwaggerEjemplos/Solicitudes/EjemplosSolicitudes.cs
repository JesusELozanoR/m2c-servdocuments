using ServDocumentos.Core.Dtos.Comun.Plantillas;
using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using ServDocumentos.Core.Enumeradores;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace ServDocumentos.API.SwaggerEjemplos.Respuestas
{
    class EjemplosSolicitudes
    {
    }


    //public class Ejemplo : IExamplesProvider<>
    //{
    //    public  GetExamples()
    //    {
    //        return new 
    //        {
    //            Empresa = Empresa.CAME,
    //        };
    //    }
    //}


    #region Datos

    public class ObtenerDatosSolicitudEjemplo : IExamplesProvider<ObtenerDatosDto>
    {
        public ObtenerDatosDto GetExamples()
        {
            return new ObtenerDatosDto
            {
                Empresa = "",
                Proceso = "TodoSi",
                Usuario = "dsi00107",
                NumeroCredito = "8a81871b72eb872b0172f263117a5cf6",
                NumeroCliente = "8a81871b72eb872b0172f26311195ce3"
            };
        }
    }

    #endregion

    #region Documentos

    public class ResetearDatosSolicitudEjemplo : IExamplesProvider<SolicitudBaseDto>
    {
        public SolicitudBaseDto GetExamples()
        {
            return new SolicitudBaseDto
            {
             //   proceso = 2,
                Usuario = "dsi00100",
                NumeroCredito = "8a81871b72eb872b0172f263117a5cf6",
                NumeroCliente = "8a81871b72eb872b0172f26311195ce3"
            };
        }
    }

    public class ObtenenerDocumentosSolicitudEjemplo : IExamplesProvider<SolictudDocumentoDto>
    {
        public SolictudDocumentoDto GetExamples()
        {
            return new SolictudDocumentoDto
            {
                ProcesoNombre = "TodoSi",
                SubProcesoNombre = "PALAPURO",
                NumeroCredito = "8a81867a71690bd101717a39a5a14494",
                NumeroCliente = "8a81867a71690bd10171759cc00a4766",
                Separado = false,
                Comprimido = false,
                Usuario = "USD01700",
                ListaPlantillasIds = new List<int>(){10, 12, 18},
                ImpresionesPlantillas = new List<PlantillasImpresiones>(){
                    new PlantillasImpresiones(){Id = 10, NumeroImpresion = 3 },
                    new PlantillasImpresiones(){Id = 12},
                    new PlantillasImpresiones(){Id = 18, NumeroImpresion = 2 },
                },
                NumerosClientes = new List<int>() {8327468,230948023,023840 }, 
                NumerosDividendos = new List<int>() {20,21,22,23} 
            };
        }
    }
    public class ObtenerPlantillasPorProcesoSolicitudEjemplo : IExamplesProvider<ObtenerPlantillasProcesoDto>
    {
        public ObtenerPlantillasProcesoDto GetExamples()
        {
            return new ObtenerPlantillasProcesoDto
            {
                Proceso= "TodoSi",
                SubProcesoNombre= "CreditosTeCreemos",
                NumeroCredito = "8a81871b72eb872b0172f263117a5cf6",
                NumeroCliente = "8a81871b72eb872b0172f26311195ce3",                
                Usuario = "dsi00107"
            };
        }
    }
    public class ObtenenerDocumentosJsonSolicitudEjemplo : IExamplesProvider<SolictudDocumentoJsonDto>
    {
        public SolictudDocumentoJsonDto GetExamples()
        {
            return new SolictudDocumentoJsonDto
            {
                JsonSolicitud = "{'NumeroCliente': '857436','NombreCompleto': 'Salvador Manuel Cruz Sifuentes','Nombre': 'Salvador Manuel','ApellidoPaterno': 'Cruz','ApellidoMaterno': 'Sifcouentes','FechaNacimiento': '06/09/1961','LugarNacimiento': 'Tultitlan, Edo. Mex., México','Edad': '59','Genero': 'Masculino','EstadoCivil': 'Soltero','DireccionCompleta': 'Calle Felipe II, 518,  Ext 10,  Colinas del Rey, Guadalupe, Nuevo León, CP 54940 México','DomicilioParticular': {'Calle': 'Particular II','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioCorrespondencia': {'Calle': 'Correspondencia II','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioFiscal': {'Calle': 'Fiscal II','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioExtranjero': {'Calle': 'Extranjero II','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'Telefono': '55-5555-5555','TelefonoFijo': '55-5555-5555','TelefonoOficina': '55-5555-5555','TelefonoCelular': '55-5555-5555','CorreoElectronico': 'salvador.gancedo4@gmail.com','Identificacion': 'INE','RFC': 'CUSS6109066R2','CURP': 'CUSS610906HDFNRN01','Nacionalidad': 'Mexicana','FormaMigratoria': '2314535','CuentaBancaria': '54987600154678','Banco': 'Banamex','CuentaCLABE': '5469875105641350','Sucursal': '2-Ixtapaluca','Ciudad': 'Mexico','PromotorId': 'AUMA30380','NombreCompletoPromotor': 'Juan Sánchez Flores','FechaApertura': '06/09/2020','NumeroNomina': '15497584','Puesto': 'Gerente Comercial','PersonalidadJuridica': 'Asalariado','Profesion': 'Programador','Trabajo': 'Trasportes S.A.','TelefonoExtranjero': '55-5555-5555','DireccionCompletaExtranjero':'',  'RFCExtranjero': 'CUSS9009206R2','Ingresos': 20.00,'SaldoPromedio': 20.00,'MovimientosEsperados':'2','NivelEstudio': 'Universidad','TiempoRadicandoDomicilio': '5','AntigüedadEmpleoNegocio': '6','IngresoMensual': 25000.0,'ImporteInicialDepositado': 7000.0,'OrigenRecursos': 'Empleo','CargoPoliticoUltimos12Meses': 'Si','Cargo': 'Dueño','RelacionPolitico12Meses': 'Si','NombreFuncionario': 'Salvador García','ParentescoPolitico': 'Primo','ResidenciaOtroPais': 'Si','NumeroIdentificacionFiscal': '35643416741','PaisAsignado': 'España, Mexico, Italia','PropietarioRecursos': 'Si','RelacionDueno': 'Suegro','ProveedorRecursos': 'Si','RelacionProveedor': 'Jefe','DestinoRecursos': 'Pago de maestros estatales','IncrementoInversionMensual': 'Si','MontoIncrementoInversionMensual': 10000.0,'RetirosMensuales': 'Si','MontoRetirosMensuales': 2000.0,'QuienEsPropietarioRecursos': 'Otro','QuienEsProveedorRecursos': 'Otro','ActuaCuentaPropia': 'Si',  'DirecionRegional': 'AVENIDA COLONIA DEL VALLE 615, COL. DEL VALLE CENTRO, BENITO JUAREZ, CIUDAD DE MEXICO, C.P.03100',  'DirecionSucursal': 'AVENIDA COLONIA DEL VALLE NO EXT. 615 NO INT.   ENTRE   Y   COL. DEL VALLE CENTRO C.P. 03100', 'DirecionCorporativo': 'AVENIDA COLONIA DEL VALLE NO EXT. 615 NO INT.   ENTRE   Y   COL. DEL VALLE CENTRO C.P. 03100', 'NombreAseguradora': 'HIR Compañía de seguros, S.A. de C.V.', 'NombreSeguro': 'Seguro de Vida a Socios', 'DireccionCompletaCorporativo': 'Avenida Colonia del Valle 615, Colonia del Valle, C.P. 03100, Alcaldía Benito Juárez, Ciudad de México', 'Beneficiarios': [{'Numero': '1','NombreCompleto': 'Luis Mejia Ruiz','Nombre': 'Luis','ApellidoPaterno': 'Mejia','ApellidoMaterno': 'Ruiz','FechaNacimiento': '11/10/1980','Telefono': '55-5555-5555','CorreoElectronico': 'luis.mejia@gmail.com','DireccionCompleta': ' Naranjo, Mz 21, Lt 23, San Antonio, Valle de ChalcoSolidaridad,Edo. Mex., 566614 México','Parentesco': 'Amigo','Porcentaje': '50','Alta':'X','Baja':''},{'Numero': '2','NombreCompleto': 'Marcos Rodriguez Méndez','Nombre': 'Marcos','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','FechaNacimiento': '25/03/1986','Telefono': '55-5555-5555','CorreoElectronico': 'marcos.rodriguez.mendez@gmail.com','DireccionCompleta': ' Lima, Mz 1, Lt 3, San Marcos, Valle de ChalcoSolidaridad,Edo. Mex., 566618 México','Parentesco': 'Amigo','Porcentaje': '50','Alta':'X','Baja':''}],'AvalesObligados': [{'Numero': '1','NombreCompleto': 'Javier Rodriguez Méndez','Nombre': 'Javier','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','FechaNacimiento': '25/03/1986','Telefono': '55-5555-5555','CorreoElectronico': 'javier.rodriguez.mendez@gmail.com','DireccionCompleta': ' Naranjo, Mz 21, Lt 23, San Javier, Valle de ChalcoSolidaridad,Edo. Mex., 56614 México','Estado': 'México'},{'Numero': '2','NombreCompleto': 'Marcos Rodriguez Méndez','Nombre': 'Marcos','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','FechaNacimiento': '25/03/1986','Telefono': '55-5555-5555','CorreoElectronico': 'marcos.rodriguez.mendez@gmail.com','DireccionCompleta': ' Naranjo, Mz 21, Lt 23, San Marcos, Valle de ChalcoSolidaridad,Edo. Mex., 56610 México','Estado': 'México'}],'Cotitulares': [{'Numero': '1','NumeroCliente':'1234','Ingresos': 100.00,'NombreCompleto': 'Sergio Rodriguez Méndez','Nombre': 'Sergio','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','FechaNacimiento': '20/09/1990','LugarNacimiento': 'Tultitlan, Edo. Mex., México','PaisNacimiento': 'México','Edad': '30','Genero': 'Masculino','EstadoCivil': 'Casado','DireccionCompleta': 'Calle Sergio II, 518,  Ext 10,  Colinas del Rey, Guadalupe, Nuevo León, CP 54940 México','DomicilioParticular': {'Calle': 'Felipe II','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioCorrespondencia': {'Calle': 'Felipe III','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioFiscal': {'Calle': 'Felipe IV','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioExtranjero': {'Calle': 'Felipe V','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'TelefonoFijo': '55-5555-5555','TelefonoOficina': '55-5555-5555','TelefonoCelular': '55-5555-5555','CorreoElectronico': 'sergio.rodriguez.mendez@gmail.com','Identificacion': 'INE','RFC': 'CUSS9009206R2','CURP': 'CUSS900920HDFNRN01','Nacionalidad': 'Mexicana','FormaMigratoria': '121321','TelefonoExtranjero': '55-5555-5555','DireccionCompletaExtranjero':'',  'RFCExtranjero': 'CUSS9009206R2','Profesion': 'Chofer','Trabajo': 'Trasportes S.A.','NivelEstudio': 'Universidad','TiempoRadicandoDomicilio': '5','AntigüedadEmpleoNegocio': '6','IngresoMensual': 25000.0,'ImporteInicialDepositado': 7000.0,'OrigenRecursos': 'Empleo','CargoPoliticoUltimos12Meses': 'Si','Cargo': 'Dueño','RelacionPolitico12Meses': 'Si','NombreFuncionario': 'Salvador García','ParentescoPolitico': 'Primo','ResidenciaOtroPais': 'Si','NumeroIdentificacionFiscal': '35643416741','PaisAsignado': 'España, Mexico, Italia','PropietarioRecursos': 'Si','RelacionDueno': 'Suegro','ProveedorRecursos': 'Si','RelacionProveedor': 'Jefe','DestinoRecursos': 'Pago de maestros estatales','IncrementoInversionMensual': 'Si','MontoIncrementoInversionMensual': 10000.0,'RetirosMensuales': 'Si','MontoRetirosMensuales': 2000.0,'ActuaCuentaPropia': 'Si','SaldoPromedio': 200.00, 'MovimientosEsperados': '2','Alta':'X','Baja':'','Modificacion':'','NumeroSerieFirmaElectronica':'1213221','ConsentimientoCredito': 'SI','QuienEsPropietarioRecursos': 'otro','QuienEsProveedorRecursos': 'otro'},{'Numero': '2','NumeroCliente':'12345','Ingresos': 100.00,'NombreCompleto': 'Carlos Morales Sifuentes','Nombre': 'Carlos','ApellidoPaterno': 'Morales','ApellidoMaterno': 'Sifuentes','FechaNacimiento': '20/09/1990','LugarNacimiento': 'Tultitlan, Edo. Mex., México','PaisNacimiento': 'México','Edad': '30','Genero': 'Masculino','EstadoCivil': 'Casado','DireccionCompleta': 'Calle Carlos II, 518,  Ext 10,  Colinas del Rey, Guadalupe, Nuevo León, CP 54940 México','DomicilioParticular': {'Calle': 'Felipe III','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioCorrespondencia': {'Calle': 'Felipe IV','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioFiscal': {'Calle': 'Felipe V','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'DomicilioExtranjero': {'Calle': 'Felipe VI','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'TelefonoFijo': '55-5555-5555','TelefonoOficina': '55-5555-5555','TelefonoCelular': '55-5555-5555','CorreoElectronico': 'carlos.morales.sifuentes@gmail.com','Identificacion': 'INE','RFC': 'CUSS9009206R2','CURP': 'CUSS900920HDFNRN01','Nacionalidad': 'Mexicana','FormaMigratoria': '121321','TelefonoExtranjero': '55-5555-5555','DireccionCompletaExtranjero':'',  'RFCExtranjero': 'CUSS9009206R2','Profesion': 'Chofer','Trabajo': 'Trasportes S.A.','NivelEstudio': 'Universidad','TiempoRadicandoDomicilio': '3','AntigüedadEmpleoNegocio': '1','IngresoMensual': 25000.0,'ImporteInicialDepositado': 7000.0,'OrigenRecursos': 'Empleo','CargoPoliticoUltimos12Meses': 'Si','Cargo': 'Dueño','RelacionPolitico12Meses': 'Si','NombreFuncionario': 'Salvador García','ParentescoPolitico': 'Primo','ResidenciaOtroPais': 'Si','NumeroIdentificacionFiscal': '35643416741','PaisAsignado': 'España, Mexico, Italia','PropietarioRecursos': 'Si','RelacionDueno': 'Suegro','ProveedorRecursos': 'Si','RelacionProveedor': 'Jefe','DestinoRecursos': 'Pago de maestros estatales','IncrementoInversionMensual': 'Si','MontoIncrementoInversionMensual': 10000.0,'RetirosMensuales': 'Si','MontoRetirosMensuales': 2000.0,'ActuaCuentaPropia': 'Si','SaldoPromedio': 200.00, 'MovimientosEsperados': '2','Alta':'X','Baja':'','Modificacion':'','NumeroSerieFirmaElectronica':'1213221','ConsentimientoCredito': '','QuienEsPropietarioRecursos': 'otro','QuienEsProveedorRecursos': 'otro'}],'Representantes': [{'Numero': '1','NombreCompleto': 'Sergio Rodriguez Méndez','Nombre': 'Sergio','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','PersonalidadJuridica': 'Asalariado','CalidadMigratoria': '998675','FechaNacimiento': '20/09/1990','Nacionalidad': 'Mexicana','Identificacion': 'INE','FolioIdentificacion': '998675','NumeroEscritura': '998675','FechaOtorgamiento': '20/09/1990','TipoPoder': 'tipoPoder','NombreNotario': '998675','NumeroNotario': '998675','Firma': '998675','DireccionCompleta': 'Calle Sergio II, 518,  Ext 10,  Colinas del Rey, Guadalupe, Nuevo León, CP 54940 México','Domicilio': {'Calle': 'Felipe VI','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Gucadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'ResidenciaOtroPais': 'Si','NumeroIdentificacionFiscal': '35643416741','PaisAsignado': 'España, Mexico, Italia','RepresentanteLegal': 'RepresentanteLegal','NumeroInscripcion': '12132','TipoFirma': 'Indistinta','ConsentimientoDatosPersonales': 'SI'},{'Numero': '2','NombreCompleto': 'Sandro Rodriguez Méndez','Nombre': 'Sandro','ApellidoPaterno': 'Rodriguez','ApellidoMaterno': 'Méndez','PersonalidadJuridica': 'Asalariado','CalidadMigratoria': '998675','FechaNacimiento': '20/09/1990','Nacionalidad': 'Mexicana','Identificacion': 'Pasaporte','FolioIdentificacion': '998675','NumeroEscritura': '998675','FechaOtorgamiento': '20/09/1990','TipoPoder': 'tipoPoder','NombreNotario': '998675','NumeroNotario': '998675','Firma': '998675','DireccionCompleta': 'Calle Sergio II, 518,  Ext 10,  Colinas del Rey, Guadalupe, Nuevo León, CP 54940 México','Domicilio': {'Calle': 'Felipe VI','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Gucadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'ResidenciaOtroPais': 'Si','NumeroIdentificacionFiscal': '35643416741','PaisAsignado': 'España, Mexico, Italia','RepresentanteLegal': 'RepresentanteLegal','NumeroInscripcion': '12132','TipoFirma': 'Indistinta','ConsentimientoDatosPersonales': 'SI'}],'Credito': {'NumeroCredito': '8a81862e73487e0c017348ec876102b6','NumeroCreditos': 2,'NumeroCliente': '8a81860c7332512901733637afd45f68','FechaInicio': '10/01/2020','Producto': 'CAMEDIGITAL','MontoTotal': 5000.0,'MontoCredito': 5000.0,'EstatusCredito': 'Pendiente','TasaInteresAnual': 20,'Plazo': '16','PlazoGarcia': '','FechaDesembolso': '12/01/2020','FrecuenciaPago': 'Mensual','CuotaAumento': 400.0,'CuotaMensual': 300.0,'PaqBasico': 'X','PaqPlatino': '','PaqPremium': '','CAT': 17.00, 'GATNominal': 17.00, 'GATReal': 17.00, 'MedioDisposicionEfectivo': 'X', 'MedioDisposicionChequera': '', 'MedioDisposicionTransferencia': '', 'LugarEfectuarRetiroVentanilla': 'X', 'EnvioEstadoCuentaDom': 'X', 'EnvioEstadoCuentaEmail': '','ReferenciaCame': '0200000062245246','ReferenciaBBVA ': '02000000000062245244 ','ReferenciaCitibanamex': '02000000000062245244','ReferenciaBanBajio': '02000000000622452411','ReferenciaScontiabank': '02000000000622452411','ReferenciaTelecomm': '02000000000622452411','ReferenciaOXXO': '01020622452400000004','ReferenciaBANSEFI': '02000000000622452411'},'TablaAmortizacion': [{'NumeroPago' : 1,'FechaLimitePago' : '13/07/2020','SaldoInicial' : 5000.00,'PagoInteres' : 500.00,'IVAInteres' : 160.00,'PagoPricipal' : 160.00,'PagoTotal' : 160.00,'SaldoInsolutoPricipal' : 160.00},{'NumeroPago' : 2,'FechaLimitePago' : '13/08/2020','SaldoInicial' : 600.00,'PagoInteres' : 600.00,'IVAInteres' : 10.00,'PagoPricipal' : 160.00,'PagoTotal' : 160.00,'SaldoInsolutoPricipal' : 160.00},{'NumeroPago' : 3,'FechaLimitePago' : '13/09/2020','SaldoInicial' : 3000.00,'PagoInteres' : 300.00,'IVAInteres' : 160.00,'PagoPricipal' : 160.00,'PagoTotal' : 160.00,'SaldoInsolutoPricipal' : 160.00}],'TablaAmortizacionPagare': [{'NumeroMensual' : 1,'Fecha' : '13/07/2020','Monto' : 500.00},{'NumeroMensual' : 2,'Fecha' : '13/08/2020','Monto' : 800.00},{'NumeroMensual' : 3,'Fecha' : '13/09/2020','Monto' : 700.00}],'Ahorro':{'NumeroCuentaInicial': '635436584','RazonSocial': 'Juan Garcia Garica','Giro': 'servicios','FechaConstitucion': '01/09/2006','NumeroSerieFirmaElectronica':'1213221','PeriodoInversion3m':'','PeriodoInversion6m':'X','PeriodoInversion12m':'','PeriodoInversion18m':'','PeriodoInversion24m':'','PeriodoInversionOtro':'','PeriodoInversion':'','DiasPreestablecidos':'Lunes,Martes','RenovacionAutoSI': '','RenovacionAutoNO': 'X','ReinversionAutoSI': '','ReinversionAutoNO': 'X','ManejoCuentaFirmaIndistintas': 'X','ManejoCuentaFirmaMancomunadas': '','ManejoCuentaFirmaIndividual': ''},'Invercamex':{'Folio': '23546','TipoRenovacion': 'Solo Capital','ConsentimientoCredito': 'NO','ActividadObjetoSocial': 'Comercio','NumeroEscrituraConstitutiva': '29383874','PaisConstitucion': 'Mexico','NombreNotario': 'Luis Fuentes','NumeroInscripcionRegistro': '7286276','RepresentanteLegal': 'NO','DomicilioComercial': {'Calle': 'Felipe VI','NumeroInterior': '518','NumeroExterior': '10','Colonia': 'Colinas del Rey','Municipio': 'Rey','Ciudad': 'Guadalupe','Estado': 'Nuevo León','CP': '54940','Pais': 'México'},'RefereciaBancaria': [{'Numero': '1','NumeroCuenta': '2345667544','Banco': 'Banamex','Telefono': '55-5555-5555'},{'Numero': '2','NumeroCuenta': '2754434566','Banco': 'Banamex','Telefono': '55-5555-5555'}],'RefereciaComercial': [{'Numero': '1','NumeroCuenta': '2345667544','Institucion': 'CAME','Telefono': '55-5555-5555'},{'Numero': '2','NumeroCuenta': '667544','Institucion': 'TECAS','Telefono': '55-5555-5555'}],'EstructuraAccionaria': [{'Nombre': 'Juan','ApellidoPaterno': 'Lopez','ApellidoMaterno': 'Lopez','Porcentaje': '40','Identificacion': 'INE'},{'Nombre': 'Jose','ApellidoPaterno': 'Perez','ApellidoMaterno': 'Perez','Porcentaje': '50','Identificacion': 'INE'},{'Nombre': 'Luis','ApellidoPaterno': 'Carmona','ApellidoMaterno': 'Montijo','Porcentaje': '10','Identificacion': 'INE'}],'EstructuraCorporativa': [{'Nombre': 'Luis','ApellidoPaterno': 'Lopez','ApellidoMaterno': 'Lopez','Cargo': 'Cargo','Identificacion': 'INE'},{'Nombre': 'Saul','ApellidoPaterno': 'Perez','ApellidoMaterno': 'Perez','Cargo': 'Cargo','Identificacion': 'INE'}],'ConsejoAdministracion': [{'Nombre': 'Luis','ApellidoPaterno': 'Lopez','ApellidoMaterno': 'Lopez','Posicion': 'Posicion','Identificacion': 'INE'},{'Nombre': 'Saul','ApellidoPaterno': 'Perez','ApellidoMaterno': 'Perez','Posicion': 'Posicion','Identificacion': 'INE'}],'TipoSolicitud': 'Fisica'}}",
                ProcesoNombre = "CreditoCameDigital",
                SubProcesoNombre = "RenovacionIndividual",
                NumeroCredito = "8a81867a71690bd101717a39a5a14494",
                NumeroCliente = "8a81867a71690bd10171759cc00a4766",
                Separado = false,
                Comprimido = false,
                Usuario = "cot00102",
                ListaPlantillasIds = new List<int>() { 10, 12, 18 },
            };
        }
    }
    public class ObtenerDocumentosVistaPreviaEjemplo : IExamplesProvider<SolictudVistaPreviaDto>
    {
        public SolictudVistaPreviaDto GetExamples()
        {
            return new SolictudVistaPreviaDto
            {
                NumeroCredito = "8a81867a71690bd101717a39a5a14494",
                NumeroCliente = "8a81867a71690bd10171759cc00a4766",
                PlantillaBase64 = "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",
                ListaCampos = new List<CampoPreviewDto>() {
                    new CampoPreviewDto(){ CampoNombre = "<<RECA>>" , Tipo = "Texto" , DatoCampo = "", Ejemplo ="WEF4578-H" },
                    new CampoPreviewDto(){ CampoNombre = "<<G-Sucursal>>" , Tipo = "Texto" , DatoCampo = "", Ejemplo ="CAME Del Valle" },
                    new CampoPreviewDto(){ CampoNombre = "<<FechaActualLarga>>" , Tipo = "Fecha" , DatoCampo = "", Ejemplo ="12 de Febrero del 2020." },
                    new CampoPreviewDto(){ CampoNombre = "<<MontoCredito>>" , Tipo = "Monto" , DatoCampo = "", Ejemplo ="345.45" },
                    new CampoPreviewDto(){ CampoNombre = "<<TablaAmortizacionPagare>>" , Tipo = "Tabla" , 
                        DatoCampo = "{\"Tabla\":\"TablaAmortizacionPagare\", \"Encabezado\" : \"NumeroMensual,Fecha,Monto\", \"Datos\" : [{ \"Campo\" : \"NumeroMensual\" , \"Tipo\":\"Texto\" } , { \"Campo\" : \"Fecha\" , \"Tipo\":\"Texto\" },{ \"Campo\" : \"Monto\" , \"Tipo\":\"Monto\"}] }", 
                        Ejemplo ="{ \"Ejemplo\" : [{ \"NumeroMensual\" :\"7689-IUIY\" } , {  \"Fecha\" :\"12-dic-2019\"},{ \"Monto\" : \"123,456.35\" }] }" },
                },
            };
        }
    }

    public class ObtenerReciboPagoEjemplo : IExamplesProvider<SolicitudDocumentoReciboJson>
    {

        public SolicitudDocumentoReciboJson GetExamples()
        {
            return new SolicitudDocumentoReciboJson
            {
                JsonSolicitud = "{'NumeroCliente': '857436','NombreCompleto': 'ERIKA MENDOZA ROBLES','NumeroGrupo': '1234','NombreGrupo': 'LAS FLORES','Folio': '1234567898','Monto': '275.00','Cantidad': 'DOSCIENTOS SETENTA Y CINCO PESOS 00 / 100 M.N','Fecha': '06/09/1961','Hora': '14:55:26','LugaPago': 'CIUDAD DE MEXICO','BanamexCta': '870-591515','ReferenciaBanamex': '00040176193507421788','BancomerCta': '734128','ReferenciasBancomer': '00040176193507431788','Gestor': 'MEDINA GONZALEZ ADRIAN','NumeroContador': '2','FechaImpresion':'18/10/2021','HoraImpresion': '05:20:01','Estatus': ''}",
                ProcesoNombre = "CreditoCameDigital",
                SubProcesoNombre = "Recibo",
                NumeroCredito = "345678909876",
                NumeroCliente = "3344456666",
                Separado = false,
                Comprimido = false,
                Usuario = "cot00102",
                ListaPlantillasIds = new List<int>() { 10, 12, 18 },
            };
        }

    }


    #endregion

    #region Generales

    public class GeneralesInsDtoEjemplo : IExamplesProvider<GeneralesInsDto>
    {
        public GeneralesInsDto GetExamples()
        {
            return new GeneralesInsDto
            {
                ProcesoId = 1,
                CampoNombre = "DireccionEmpresarial",
                Valor = "Av. Colonial Izatapalapa No. 234",
                Usuario = "dsi00107",
            };
        }
    }
    public class GeneralesNombreDtoEjemplo : IExamplesProvider<GeneralesNombreDto>
    {
        public GeneralesNombreDto GetExamples()
        {
            return new GeneralesNombreDto
            {
                ProcesoId = 1,
                CampoNombre = "DireccionEmpresarial",
                Usuario = "dsi00107",
            };
        }
    }
    public class GeneralesIdDtoEjemplo : IExamplesProvider<GeneralesIdDto>
    {
        public GeneralesIdDto GetExamples()
        {
            return new GeneralesIdDto
            {
                GeneralId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class GeneralesGetDtoEjemplo : IExamplesProvider<GeneralesGetDto>
    {
        public GeneralesGetDto GetExamples()
        {
            return new GeneralesGetDto
            {
                CampoNombre = "DireccionEmpresarial",
                GeneralId = 1,
                ProcesoId = 1,
            };
        }
    }
    public class GeneralesUpdDtoEjemplo : IExamplesProvider<GeneralesUpdDto>
    {
        public GeneralesUpdDto GetExamples()
        {
            return new GeneralesUpdDto
            {
                CampoNombre = "DireccionEmpresarial",
                GeneralId = 1,
                ProcesoId = 1,
                Valor = "Av. Colonial Izatapalapa No. 234-23, Col. Iztapaluca",
                Usuario = "dsi00107",
            };
        }
    }

    #endregion

    #region Clasificaciones

    public class ClasificacionInsDtoEjemplo : IExamplesProvider<ClasificacionInsDto>
    {
        public ClasificacionInsDto GetExamples()
        {
            return new ClasificacionInsDto
            {
                Clasificacion = "Creditos",
                Usuario = "dsi00107",
            };
        }
    }
    public class ClasificacionNombreDtoEjemplo : IExamplesProvider<ClasificacionNombreDto>
    {
        public ClasificacionNombreDto GetExamples()
        {
            return new ClasificacionNombreDto
            {
                Clasificacion = "Creditos",
                Usuario = "dsi00107",
            };
        }
    }
    public class ClasificacionIdDtoEjemplo : IExamplesProvider<ClasificacionIdDto>
    {
        public ClasificacionIdDto GetExamples()
        {
            return new ClasificacionIdDto
            {
                ClasificacionId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ClasificacionGetDtoEjemplo : IExamplesProvider<ClasificacionGetDto>
    {
        public ClasificacionGetDto GetExamples()
        {
            return new ClasificacionGetDto
            {
                ClasificacionId = 1,
                Clasificacion = "Creditos",
                Empresa = EmpresaSel.CAME
            };
        }
    }
    public class ClasificacionUpdDtoEjemplo : IExamplesProvider<ClasificacionUpdDto>
    {
        public ClasificacionUpdDto GetExamples()
        {
            return new ClasificacionUpdDto
            {
                ClasificacionId = 1,
                Clasificacion = "Creditos",
                Usuario = "dsi00107",
            };
        }
    }

    #endregion

    #region Campos

    public class CampoInsDtoEjemplo : IExamplesProvider<CampoInsDto>
    {
        public CampoInsDto GetExamples()
        {
            return new CampoInsDto
            {
                CampoNombre = "<<MontoTotal>>",
                DatoConjunto = "Credito",
                DatoCampo = "CreditTotalAmount",
                Descripcion = "Monto total del Crédito",
                Tipo = "Monto",
                Ejemplo = "125,155.00",
                Usuario = "dsi00107",
                DatoConjuntoGrupal = "",
                ProcesoId = 0,
                SubProcesoId = 0
            };
        }
    }
    public class CampoNombreDtoEjemplo : IExamplesProvider<CampoNombreDto>
    {
        public CampoNombreDto GetExamples()
        {
            return new CampoNombreDto
            {
                CampoNombre = "<<MontoTotal>>",
                Usuario = "dsi00107",
            };
        }
    }
    public class CampoIdDtoEjemplo : IExamplesProvider<CampoIdDto>
    {
        public CampoIdDto GetExamples()
        {
            return new CampoIdDto
            {
                CampoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class CampoGetDtoEjemplo : IExamplesProvider<CampoGetDto>
    {
        public CampoGetDto GetExamples()
        {
            return new CampoGetDto
            {
                CampoId = 1,
                CampoNombre = "<<MontoTotal>>",
                DatoCampo = "Credito",
                Tipo = "Monto",
            };
        }
    }
    public class CampoUpdDtoEjemplo : IExamplesProvider<CampoUpdDto>
    {
        public CampoUpdDto GetExamples()
        {
            return new CampoUpdDto
            {
                CampoId = 1,
                CampoNombre = "<<MontoTotal>>",
                DatoConjunto = "Credito",
                DatoCampo = "CreditTotalAmount",
                Tipo = "Monto",
                Descripcion = "Monto total del credito",
                Ejemplo = "456.00",
                Usuario = "dsi001017",
            };
        }
    }

    #endregion

    #region Procesos

    public class ProcesoDtoEjemplo : IExamplesProvider<ProcesoDto>
    {
        public ProcesoDto GetExamples()
        {
            return new ProcesoDto
            {
                Empresa = Empresa.TCR,
                ProcesoNombre = "AhorroCAME",
                Descripcion = "Cuentas de Ahorro",
                ClasificacionId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoNombreDtoEjemplo : IExamplesProvider<ProcesoNombreDto>
    {
        public ProcesoNombreDto GetExamples()
        {
            return new ProcesoNombreDto
            {
                Empresa = Empresa.TCR,
                ProcesoNombre = "AhorroCAME",
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoIdDtoEjemplo : IExamplesProvider<ProcesoIdDto>
    {
        public ProcesoIdDto GetExamples()
        {
            return new ProcesoIdDto
            {
                Empresa = Empresa.TCR,
                ProcesoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoUpdDtoEjemplo : IExamplesProvider<ProcesoUpdDto>
    {
        public ProcesoUpdDto GetExamples()
        {
            return new ProcesoUpdDto
            {
                Empresa = EmpresaSel.TCR,
                ProcesoId = 1,
                ProcesoNombre = "AhorroCAME",
                Descripcion = "Cuentas de Ahorro",
                ClasificacionId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoGetDtoEjemplo : IExamplesProvider<ProcesoGetDto>
    {
        public ProcesoGetDto GetExamples()
        {
            return new ProcesoGetDto
            {
                Empresa = Empresa.TCR,
                ProcesoId = 1,
                ProcesoNombre = "AhorroCAME",
                ClasificacionId = 1,
                ClasificacionNombre = "Ahorro",
            };
        }
    }

    #endregion

    #region ProcesosCampos

    public class ProcesoCampoInsDtoEjemplo : IExamplesProvider<ProcesoCampoInsDto>
    {
        public ProcesoCampoInsDto GetExamples()
        {
            return new ProcesoCampoInsDto
            {
                ProcesoId = 1,
                CampoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoCampoDelDtoEjemplo : IExamplesProvider<ProcesoCampoDelDto>
    {
        public ProcesoCampoDelDto GetExamples()
        {
            return new ProcesoCampoDelDto
            {
                ProcesoId = 1,
                CampoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoCampoIdDtoEjemplo : IExamplesProvider<ProcesoCampoIdDto>
    {
        public ProcesoCampoIdDto GetExamples()
        {
            return new ProcesoCampoIdDto
            {
                ProcesoCampoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class ProcesoCampoGetDtoEjemplo : IExamplesProvider<ProcesoCampoGetDto>
    {
        public ProcesoCampoGetDto GetExamples()
        {
            return new ProcesoCampoGetDto
            {
                ProcesoCampoId = 145,
                ProcesoId = 1,
                CampoId = 665,
            };
        }
    }
    public class ProcesoCampoUpdDtoEjemplo : IExamplesProvider<ProcesoCampoUpdDto>
    {
        public ProcesoCampoUpdDto GetExamples()
        {
            return new ProcesoCampoUpdDto
            {
                ProcesoCampoId = 145,
                ProcesoId = 1,
                CampoId = 665,
                Usuario = "dsi00107",
            };
        }
    }
    
    #endregion

    #region SubProcesos

    public class SubProcesoDtoEjemplo : IExamplesProvider<SubProcesoDto>
    {
        public SubProcesoDto GetExamples()
        {
            return new SubProcesoDto
            {
                SubProcesoNombre = "PALDENTISTA",
                Descripcion = "Apoyo para Consultorio",
                ProcesoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoNombreDtoEjemplo : IExamplesProvider<SubProcesoNombreDto>
    {
        public SubProcesoNombreDto GetExamples()
        { 
            return new SubProcesoNombreDto
            {
                SubProcesoNombre = "PALDENTISTA",
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoIdDtoEjemplo : IExamplesProvider<SubProcesoIdDto>
    {
        public SubProcesoIdDto GetExamples()
        {
            return new SubProcesoIdDto
            {
                SubProcesoId = 2,
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoGetDtoEjemplo : IExamplesProvider<SubProcesoGetDto>
    {
        public SubProcesoGetDto GetExamples()
        {
            return new SubProcesoGetDto
            {
                ProcesoId = 2,
                SubProcesoId = 1,
                SubProcesoNombre = "PALDENTISTA",
            };
        }
    }
    public class SubProcesoUpdDtoEjemplo : IExamplesProvider<SubProcesoUpdDto>
    {
        public SubProcesoUpdDto GetExamples()
        {
            return new SubProcesoUpdDto
            {
                SubProcesoId = 1,
                SubProcesoNombre = "PALDENTISTA",
                ProcesoId = 2,
                Descripcion = "Apoyo para Consultorios",
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoClasDtoEjemplo : IExamplesProvider<SubProcesoClasDto>
    {
        public SubProcesoClasDto GetExamples()
        {
            return new SubProcesoClasDto
            {
                Empresa = EmpresaSel.CAME,
                ProcesoId = 0,
                SubProcesoId = 0,
                ClasificacionId = 1,
            };
        }
    }

    #endregion

    #region SubProcesosCampos

    public class SubProcesoCampoInsDtoEjemplo : IExamplesProvider<SubProcesoCampoInsDto>
    {
        public SubProcesoCampoInsDto GetExamples()
        {
            return new SubProcesoCampoInsDto
            {
                SubProcesoId = 2,
                CampoId = 132,
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoCampoDelDtoEjemplo : IExamplesProvider<SubProcesoCampoDelDto>
    {
        public SubProcesoCampoDelDto GetExamples()
        {
            return new SubProcesoCampoDelDto
            {
                SubProcesoId = 2,
                CampoId = 132,
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoCampoIdDtoEjemplo : IExamplesProvider<SubProcesoCampoIdDto>
    {
        public SubProcesoCampoIdDto GetExamples()
        {
            return new SubProcesoCampoIdDto
            {
                SubProcesoCampoId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class SubProcesoCampoGetDtoEjemplo : IExamplesProvider<SubProcesoCampoGetDto>
    {
        public SubProcesoCampoGetDto GetExamples()
        {
            return new SubProcesoCampoGetDto
            {
                SubProcesoCampoId = 2,
                SubProcesoId = 2,
                CampoId = 234,
            };
        }
    }
    public class SubProcesoCampoUpdDtoEjemplo : IExamplesProvider<SubProcesoCampoUpdDto>
    {
        public SubProcesoCampoUpdDto GetExamples()
        {
            return new SubProcesoCampoUpdDto
            {
                SubProcesoCampoId = 2,
                SubProcesoId = 2,
                CampoId = 234,
                Usuario = "dsi00107",
            };
        }
    }

    #endregion

    #region Plantillas

    public class ObtenerPlantillasProcesoDtoEjemplo : IExamplesProvider<ObtenerPlantillasProcesoDto>
    {
        public ObtenerPlantillasProcesoDto GetExamples()
        {
            return new ObtenerPlantillasProcesoDto
            {
                Empresa = "TSI",
                NumeroCliente = "8a81867a71690bd10171759cc00a4766",
                NumeroCredito = "8a81867a71690bd101717a39a5a14494",
                Proceso = "TeCreemos",
                SubProcesoNombre = "NEGULTRA",
                Usuario = "dsi00107",                
            };
        }
    }
    public class ProcesoSubDtoEjemplo : IExamplesProvider<ProcesoSubDto>
    {
        public ProcesoSubDto GetExamples()
        {
            return new ProcesoSubDto
            {
                ProcesoNombre = "TeCreemos",
                SubProcesoNombre = "NEGULTRA",
            };
        }
    }
    public class PlantillaInsDtoEjemplo : IExamplesProvider<PlantillaInsDto>
    {
        public PlantillaInsDto GetExamples()
        {
            return new PlantillaInsDto
            {
                Archivo64 = "HYUJ67==",
                Extension = ArchivosExtension.docx,
                Version = "1.0",
                Descripcion = "CRED_Contrato",
                DescripcionDocumentos = "Contrato del Credito",
                Usuario = "dsi00107",
                RECA = "AGSSG7676",
                SubProcesoId = 1,
                Tipo = "I",
            };
        }
    }
    public class PlantillaIdDtoEjemplo : IExamplesProvider<PlantillaIdDto>
    {
        public PlantillaIdDto GetExamples()
        {
            return new PlantillaIdDto
            {
                PlantillaId = 1,
                Usuario = "dsi00107",
            };
        }
    }
    public class PlantillaNombreDtoEjemplo : IExamplesProvider<PlantillaNombreDto>
    {
        public PlantillaNombreDto GetExamples()
        {
            return new PlantillaNombreDto
            {
                PlantillaNombre = "5007F571-6337-4193-94F2-05F1CDF78637.doc",
                Usuario = "dsi00107",
            };
        }
    }
    public class PlantillaGetDtoEjemplo : IExamplesProvider<PlantillaGetDto>
    {
        public PlantillaGetDto GetExamples()
        {
            return new PlantillaGetDto
            { 
                AlfrescoId = "39585e3b-43d7-4c61-a3c3-7c787e12eac3",
                Descripcion = "NEGULTRA_ReferenciaPago",                 
                PlantillaId = 1,
                PlantillaNombre = "5007F571-6337-4193-94F2-05F1CDF78637.doc",
                Estado = true,
            };
        }
    }
    public class PlantillaUpdDtoEjemplo : IExamplesProvider<PlantillaUpdDto>
    {
        public PlantillaUpdDto GetExamples()
        {
            return new PlantillaUpdDto
            {
                AlfrescoId = "39585e3b-43d7-4c61-a3c3-7c787e12eac3",
                AlfrescoUrl = @"https://192.168.101.185:8443/share/s/CDc0sEqbQkan0h22xB3mtw",
                Archivo64 = "HYJUIOLPFDER67FFFDMBGF556GBVFR45RFCDE43EDFCVY56OI98JKNB65RDGVUJ67==",
                DescripcionDocumentos = "Contrato del Credito",
                Version = "1.0",
                Descripcion = "NEGULTRA_ReferenciaPago",
                PlantillaId = 1,
                PlantillaNombre = "5007F571-6337-4193-94F2-05F1CDF78637.doc",
                usuario = "dsi00107",
            };
        }
    }
    public class PlantillaRegDtoEjemplo : IExamplesProvider<PlantillaRegDto>
    {
        public PlantillaRegDto GetExamples()
        {
            return new PlantillaRegDto
            {
                DescripcionDocumentos = "Contrato del Credito",
                Extension = ArchivosExtension.docx,
                Version = "1.0",
                Descripcion = "NEGULTRA_ReferenciaPago",
                Usuario = "dsi00107",
            };
        }
    }

    #endregion

    #region PlantillasCampos

    public class PlantillaCampoInsDtoEjemplo : IExamplesProvider<PlantillaCampoInsDto>
    {
        public PlantillaCampoInsDto GetExamples()
        {
            return new PlantillaCampoInsDto
            {
                PlantillaId = 2,
                ListaCampoIds = new System.Collections.Generic.List<int>() { 345, 765, 983, 234, 123 },
                Usuario = "dsi00107",
            };
        }
    }
    public class PlantillaCampoIdDtoEjemplo : IExamplesProvider<PlantillaCampoIdDto>
    {
        public PlantillaCampoIdDto GetExamples()
        {
            return new PlantillaCampoIdDto
            {
                PlantillaCampoId = 2,
                Usuario = "dsi00107",
            };
        }
    }    
    public class PlantillaCampoIdcDtoEjemplo : IExamplesProvider<PlantillaCampoIdcDto>
    {
        public PlantillaCampoIdcDto GetExamples()
        {
            return new PlantillaCampoIdcDto
            {
                PlantillaId = 2,
                CampoId = 234,
                Usuario = "dsi00107",
            };
        }
    }
    public class PlantillaCampoGetDtoEjemplo : IExamplesProvider<PlantillaCampoGetDto>
    {
        public PlantillaCampoGetDto GetExamples()
        {
            return new PlantillaCampoGetDto
            {
                PlantillaId = 2,
                CampoId = 1233,
            };
        }
    }
    public class PlantillaCampoUpdDtoEjemplo : IExamplesProvider<PlantillaCampoUpdDto>
    {
        public PlantillaCampoUpdDto GetExamples()
        {
            return new PlantillaCampoUpdDto
            {
                // Empresa = Empresa.CAME,
                 
            };
        }
    }
    public class PlantillaCampoPSGetEjemplo : IExamplesProvider<PlantillaCampoPSGet>
    {
        public PlantillaCampoPSGet GetExamples()
        {
            return new PlantillaCampoPSGet
            {
                ProcesoId = 1,
                SubprocesoId = 2,
                CampoNombre = "<<>CAT>",
                CampoDescripcion = "CAT del Documento",
            };
        }
    }

    #endregion

    #region SubProcesoPlantillas

    public class SubProcesoPlantillaInsDtoEjemplo : IExamplesProvider<SubProcesoPlantillaInsDto>
    {
        public SubProcesoPlantillaInsDto GetExamples()
        {
            return new SubProcesoPlantillaInsDto
            {
                SubProcesoId = 2,
                listadoPlantillas = new System.Collections.Generic.List<SubProcesoPlantilla>() { 
                     new SubProcesoPlantilla(){ PlantillaId = 10, RECA = "GFD56", Tipo = "I" },
                     new SubProcesoPlantilla(){ PlantillaId = 12, RECA = "KJU78", Tipo = "I" },
                     new SubProcesoPlantilla(){ PlantillaId = 15, RECA = "RECA-2", Tipo = "I" },
                },
                Usuario = "dsi00107",
            };
        }
    }
    public class SubprocesoPlantillaDelDtoEjemplo : IExamplesProvider<SubprocesoPlantillaDelDto>
    {
        public SubprocesoPlantillaDelDto GetExamples()
        {
            return new SubprocesoPlantillaDelDto
            {
                SubProcesoId = 2,
                PlantillaId = 232,
                Usuario = "dsi00107",
            };
        }
    }    
    public class SubprocesoPlantillaGetDtoEjemplo : IExamplesProvider<SubprocesoPlantillaGetDto>
    {
        public SubprocesoPlantillaGetDto GetExamples()
        {
            return new SubprocesoPlantillaGetDto
            {
                SubpPantId = 2,
                SubProcesoId = 3,
                PlantillaId = 10,
                Reca = "Reca-1233",
                Tipo = "I",
            };
        }
    }
    public class SubProcesoPlantillaUpdDtoEjemplo : IExamplesProvider<SubProcesoPlantillaUpdDto>
    {
        public SubProcesoPlantillaUpdDto GetExamples()
        {
            return new SubProcesoPlantillaUpdDto
            {
                SubProcesoPlantillaId = 2,
                SubProcesoId = 3,
                PlantillaId = 10,
                RECA = "Reca-1233",
                Tipo = "I",
                Usuario = "dsi00107", 
            };
        }
    }

    #endregion


}





