using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ServDocumentos.Core.Enumeradores
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Estado
    {
        Todos = 2,
        Activo = 1,
        Inactivo = 0
    }
}
