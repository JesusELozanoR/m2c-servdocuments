using AdaptiveCards;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosEngine
{
    public class DatosCredito
    {
        public string NumeroCredito { get; set; }
        public int NumeroCreditos { get; set; }
        public string NumeroCliente { get; set; }
        public string FechaInicio { get; set; }
        public string Producto { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoCredito { get; set; }
        public string EstatusCredito { get; set; }
        public decimal TasaInteresAnual { get; set; }
        public int Plazo { get; set; }
        public string PlazoGarcia { get; set; }
        public string FechaDesembolso { get; set; }
        public string FrecuenciaPago { get; set; }
        public double CuotaAumento { get; set; }
        public decimal CuotaMensual { get; set; }
        public string PaqBasico { get; set; }
        public string PaqPlatino { get; set; }
        public string PaqPremium { get; set; }
        public decimal CAT { get; set; }
        public decimal GATNominal { get; set; }
        public decimal GATReal { get; set; }
       // public string MediosDisposicion { get; set; }
        public string LugarEfectuarRetiroVentanilla { get; set; }
        public string ReferenciaNumeroRetiro { get; set; }
        public string EnvioEstadoCuentaDom { get; set; }
        public string EnvioEstadoCuentaEmail { get; set; }
        public string MedioDisposicionEfectivo { get; set; }
        public string MedioDisposicionChequera { get; set; }
        public string MedioDisposicionTransferencia { get; set; }

        public string ReferenciaCame { get; set; }
        public string ReferenciaTCR { get; set; }
        public string ReferenciaBBVA { get; set; }
        public string ReferenciaCitibanamex { get; set; }
        public string ReferenciaBanBajio { get; set; }
        public string ReferenciaWalmart { get; set; }

        public string ReferenciaScontiabank { get; set; }
        public string ReferenciaTelecomm { get; set; }
        public string ReferenciaOXXO { get; set; }
        public string ReferenciaBANSEFI { get; set; }       
        public string FechaPrimerPago { get; set; }
        public decimal TasaMoratoriaAnual { get; set; }
        public decimal InteresMoratorioMensual { get; set; }
        public int DiasFrecuenciaPago { get; set; }
        public double MontoCreditoNeto { get; set; }
        public bool ConCuentaAhorroLigada { get; set; }
        public string FechaFinal { get; set; }
    }
}
