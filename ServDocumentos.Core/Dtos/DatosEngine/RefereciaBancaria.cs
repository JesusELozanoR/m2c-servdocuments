using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class RefereciaBancaria : Referencias
    {
        public string Banco { get; set; }  
    }
}

