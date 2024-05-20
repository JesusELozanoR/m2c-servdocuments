using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class DireccionClientes
    {
        public string Municipio_Direccion_Cliente { get; set; }
        public string Numero_Ext_Direccion_Cliente { get; set; }
        public string Numero_Int_Direccion_Cliente { get; set; }
        public string Estado_Direccion_Cliente { get; set; }
        public string Calle_Direccion_Cliente { get; set; }
        public string CP_Direccion_Cliente { get; set; }
        public string Colonia_Direccion_Cliente { get; set; }
    }

    public class DatosPersonalesClientes
    {
        public string Genero_Datos_Personales_Cliente { get; set; }
        public string Segundo_Apellido { get; set; }
        public string Primer_Apellido { get; set; }
        public string RFC_Datos_Personales_Cliente { get; set; }
        public string CURP_Datos_Personales_Cliente { get; set; }
        public string Estado_Civil_Datos_Cliente { get; set; }
    }

    public class Cliente
    {
        public string encodedKey { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public DateTime activationDate { get; set; }
        public DateTime approvedDate { get; set; }

        public string groupName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string mobilePhone { get; set; }
        public string emailAddress { get; set; }
        public string preferredLanguage { get; set; }
        public string birthDate { get; set; }
        public string clientRoleKey { get; set; }
        public int loanCycle { get; set; }
        public int groupLoanCycle { get; set; }
        public string assignedBranchKey { get; set; }
        public IList<object> groupKeys { get; set; }
        public IList<Address> addresses { get; set; }
        public IList<object> idDocuments { get; set; }
        public DireccionClientes _Direccion_Clientes { get; set; }
        public DatosPersonalesClientes _Datos_Personales_Clientes { get; set; }
        public Credito Credito { get; set; }
        public AhorroDto Ahorro { get; set; }
        public List<Pagos> Pagos { get; set; }
        public List<Member> groupMembers { get; set; }
    }
    public class Address
    {
        public string city { get; set; }
        public string country { get; set; }
        public string encodedKey { get; set; }
        public int indexInList { get; set; }
        public int latitude { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public int longitude { get; set; }
        public string parentKey { get; set; }
        public string postcode { get; set; }
        public string region { get; set; }

    }

    public class Member
    {
        public string clientKey { get; set; }
        public List<Rol> roles { get; set; }
    }

    public class Rol
    {
        public string encodedKey { get; set; }
        public string groupRoleNameKey { get; set; }
        public string roleName { get; set; }
        public string roleNameId { get; set; }
    }

}
