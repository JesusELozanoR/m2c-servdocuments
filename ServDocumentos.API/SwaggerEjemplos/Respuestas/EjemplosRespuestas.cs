

using ServDocumentos.Core.Dtos.Comun.Respuestas;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace ServDocumentos.API.SwaggerEjemplos.Respuestas
{
    class EjemplosRespuestas
    {
    }

    public class MensajeErrorFuncionalDtoRespuestaEjemplo : IExamplesProvider<MensajeErrorFuncionalDto>
    {
        public MensajeErrorFuncionalDto GetExamples()
        {
            return new MensajeErrorFuncionalDto
            {
                Origen = "Servicio donde se origina el Conflicto",
                Mensajes = new[] { "Descripción del tipo de conflicto" }
            };
        }
    }

    public class MensajeErrorCriticoDtoEjemplo : IExamplesProvider<MensajeErrorCriticoDto>
    {
        public MensajeErrorCriticoDto GetExamples()
        {
            return new MensajeErrorCriticoDto
            {
                Origen = "Servicio donde se origina la Excepción",
                Mensajes = new[] { "Descripción de la Excepción" },
                CodigoRastreo = "Codigo para identificar la excepción"
            };
        }
    }


    public class ObtenerDocumentosDtoEjemplo : IExamplesProvider<ResultadoDocumentoDto>
    {
        public ResultadoDocumentoDto GetExamples()
        {
            return new ResultadoDocumentoDto
            {
                Mensaje = "archvi.pdf",
                Dato = "4469+89++",
                ListaDatos = new List<string>() {
                      "564646451515616565",
                      "sdadasdas"
                }
            };
        }
    }


    public class ObtenerPlantillasPorProcesoRespuestaEjemplo : IExamplesProvider<IEnumerable<ArchivoPlantillaDto>>
    {
        public IEnumerable<ArchivoPlantillaDto> GetExamples()
        {
            return new List<ArchivoPlantillaDto>()
            {
                new ArchivoPlantillaDto(){
                    Base64 = "0M8R4KGxGuEAAAAAAAAAAAAAAAAA..........",
                    Nombre = "TodoSi_Credito.doc"
                }

            };
        }
    }


}
