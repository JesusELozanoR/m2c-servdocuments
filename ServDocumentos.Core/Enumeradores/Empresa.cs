
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace ServDocumentos.Core.Enumeradores
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Empresa
    {
        TCR = 1,
        TSI = 2,
        CAME = 3,
        DEMO = 4
    }

    public enum EmpresaSel
    {
        NINGUNA = 0,
        TCR = 1,
        TSI = 2,
        CAME = 3,
        DEMO = 4
    }
}
