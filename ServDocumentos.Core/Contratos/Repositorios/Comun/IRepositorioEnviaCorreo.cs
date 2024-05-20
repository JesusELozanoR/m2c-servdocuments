using ServDocumentos.Core.Dtos.Comun.Correo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Contratos.Repositorios.Comun
{
  public  interface IRepositorioEnviaCorreo
    {
        /*public EstatusCorreoDto Enviar(CorreoDto c);
        public Task<EstatusCorreoDto> EnviarAsync(CorreoDto c);
         public  Task<EstatusCorreoDto> EnviarSenGridAsync(CorreoSengrid mail);*/
        public Task<EstatusCorreoDto> EnviarSenGridPostAsync(CorreoSengridPost mail, string url);
       
    }
}
