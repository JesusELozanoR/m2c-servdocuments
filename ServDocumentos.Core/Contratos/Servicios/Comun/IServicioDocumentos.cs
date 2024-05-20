using ServDocumentos.Core.Dtos.Comun.Respuestas;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Servicios.Comun
{
    public interface IServicioDocumentos
    {
        ResultadoDocumentoDto ObtieneDocumento(SolictudDocumentoDto solicitud);
        ResultadoUrlDocumeto ObtieneRutaDocumento(SolictudDocumentoDto solicitud);
        ResultadoDocumentoDto ObtieneEstadoCuenta(ObtenerPlantillasProcesoDto solicitud);
        int EstadosCuentaMensualProcesa(EstadosCuentaMensualProcSol solicitud);
        Task<int> EstadosCuentaMensualProcesaAsync(EstadosCuentaMensualProcSol solicitud);
        List<EstadoCuentaMensualProcResp> EstadosCuentaMensualObtiene(EstadosCuentaMensualProcSol solicitud);
        Task<List<EstadoCuentaMensualProcResp>> EstadosCuentaMensualObtieneAsync(EstadosCuentaMensualProcSol solicitud);
        ResultadoDocumentoDto EstadoCuentaMensualGenera(EstadoCuentaMensualSol solicitud);
        ResultadoDocumentoDto EstadoCuentaMensualEnvia(EstadoCuentaMensualSol solicitud);
        Task<ResultadoDocumentoDto> EstadoCuentaMensualEnviaAsync(EstadoCuentaMensualSol solicitud);
        Task<ResultadoDocumentoDto> EstadoCuentaMensualGeneraAsync(EstadoCuentaMensualSol solicitud);
        ResultadoUrlDocumeto ObtieneRutaDocumentoJson(SolictudDocumentoJsonDto solicitud);
        ResultadoDocumentoDto ObtenerDocumentosJson(SolictudDocumentoJsonDto solicitud);
        ResultadoDocumentoDto VistaPrevia(SolictudVistaPreviaDto solicitud);
        ResultadoUrlDocumeto ObtenerRutaReciboJson(SolicitudDocumentoReciboJson solicitud);
        ResultadoDocumentoDto EstadoCuentaCredito(EstadoCuentaCreditoSolDto solicitud);
        ResultadoDocumentoDto ObtenerDocumentosJsonDatos(SolictudDocumentoJsonDatosDto solicitud);
    }
}
