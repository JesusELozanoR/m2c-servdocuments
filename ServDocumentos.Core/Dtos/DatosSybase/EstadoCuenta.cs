using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosSybase
{
    public class EstadoCuenta
    {
        public string NumeroCredito { get; set; }
        public string TipoOperacion { get; set; }
        public decimal MontoCredito { get; set; }
        public decimal SaldoCapital { get; set; }
        public string EstadoCredito { get; set; }
        public int NumeroCliente { get; set; }
        public string FechaInicio { get; set; }
        public decimal CAT { get; set; }
        public decimal TasaInteres { get; set; }
        public decimal InteresMoratorio { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Direccion { get; set; }      
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Provincia { get; set; }
        public string Telefono { get; set; }
        public string Sucursal { get; set; }
        public string TelefonoSucursal { get; set; }
        public string DireccionSucursal { get; set; }
        public string DireccionRegional { get; set; }
        public string TelefonoRegional { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal CuotaActual { get; set; }
        public decimal PagoMinimo { get; set; }
        public decimal PagoLiquidar { get; set; }       
        public string Periodo { get; set; }    
        public string FechaImpresion { get; set; }
        public string FechaLimitePago { get; set; }
        public IEnumerable<TablaPagos> TablaEstadoCuenta { get; set; }


        public string TotPagos { get; set; }
        public string TotCargos { get; set; }

        public string TotCapital { get; set; }
        public string TotComisionMora{ get; set; }
        public string TotInteresMora { get; set; }
        public string TotInteres { get; set; }
        public string TotIVA { get; set; }
        public string TotSeguros { get; set; }
        public string TotGastoSupervision { get; set; }
        public string TotSaldo { get; set; }




    }
}
