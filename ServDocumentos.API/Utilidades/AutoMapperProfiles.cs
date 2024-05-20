using AutoMapper;
using ServDocumentos.Core.Dtos.Comun.Solicitudes;

namespace cmn.std.Servicio.Seguridad.Api.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<EstadoCuentaCreditoSolDto, SolictudDocumentoDto>();
        }
    }
}
