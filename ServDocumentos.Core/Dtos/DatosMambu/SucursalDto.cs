using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class SucursalDto
    {
        public string encodedKey { get; set; }
        public string id { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string notes { get; set; }
        //public branchHolidays branchHolidays { get; set; }
        public IList<Addresses> addresses { get; set; }
        public _Datos_Adicionales_Sucursal _Datos_Adicionales_Sucursal { get; set; }

    }
    public class branchHolidays
    {
        public DateTime creationDate { get; set; }
    }
    public class Addresses
    {
        public string encodedKey { get; set; }
        public string parentKey { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        
    }
    public class _Datos_Adicionales_Sucursal
    {
        public string Sucursal_Frontera { get; set; }
    }
   
}
