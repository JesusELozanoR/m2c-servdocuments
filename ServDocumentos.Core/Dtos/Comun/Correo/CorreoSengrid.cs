using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.Comun.Correo
{
    public class CorreoSengridPost
    {
        public List<Destinatario> Destinatarios { get; set; }
        public List<Destinatario> DestinatariosCC { get; set; }
        public List<Destinatario> DestinatariosCCO { get; set; }
        public string Asunto { get; set; }
        public string Contenido { get; set; }
        public List<AdjuntoV2> Adjuntos { get; set; }
        public PlantillaSengrid Plantilla { get; set; }

        public List<MapaCampos> DatosPlantilla { get; set; }
    }
    public class PlantillaSengrid
    {
        public string Identificador { get; set; }
        public string Contenido { get; set; }
    }
    public class MapaCampos
    {
        public string Tag { get; set; }
        public string Tipo { get; set; }
        public object Valor { get; set; }
    }
    public class CorreoSengrid
    {
        public string Destinatario { get; set; }
        public List<Destinatario> DestinatariosCC { get; set; }
        public string Asunto { get; set; }
        public string Contenido { get; set; }
        public string ApiKey { get; set; }
        public string Remitente { get; set; }
        public List<AdjuntoV2> Adjuntos { get; set; }
    }
    public class Destinatario
    {
        public string Email { get; set; }
        public string Alias { get; set; }
    }
    public class AdjuntoV2
    {
        public string Nombre { get; set; }
        public string Contenido { get; set; }
    }
    public class PlantillaV2
{
    public string Identificador { get; set; }
    public string Contenido { get; set; }
}
}

