using AdoNetCore.AseClient;
using cmn.std.Log;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ServDocumentos.Core.Contratos.Factories.TCR.Servicio;
using ServDocumentos.Core.Contratos.Repositorios.TCR;
using ServDocumentos.Core.Dtos.DatosExpress;
using ServDocumentos.Core.Dtos.DatosSybase;
using ServDocumentos.Core.Dtos.DatosTcrCaja;
using ServDocumentos.Core.Entidades.Comun;
using ServDocumentos.Core.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ServDocumentos.Repositorios.TCR.Repositorios.Sybase
{
    public class RepositorioDatosPlantillas : RepositorioBase, IRepositorioDatosPlantillasSybase
    {
        public RepositorioDatosPlantillas(AseConnection aseConnection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(aseConnection, transaction, gestorLog)
        {
        }


        private List<TablaAcelerada> _tablaAcelerada(string numeroCredito)
        {
            gestorLog.Entrar();
            List<TablaAcelerada> aceledada = new List<TablaAcelerada>();
            try
            {
                var parameters1 = new DynamicParameters();
                parameters1.Add("@i_banco", numeroCredito, DbType.AnsiStringFixedLength);
                parameters1.Add("@tipo_apli", 6, DbType.Int32);

                var result1 = aseConnection.QueryMultiple(
                          "cob_cartera.dbo.tcrp_amortizacion_acelerada"
                        , parameters1
                        , commandType: CommandType.StoredProcedure);


                aceledada = (List<TablaAcelerada>)result1.Read<TablaAcelerada>();


            }   
            finally
            {
                gestorLog.Salir();
            }

            return aceledada;


        }

       
        public Cliente ObtenerDatosPlantilla(string numeroCredito, List<int> numeroClientes, List<int> numeroDividendos, RespuestaOrdenPago ordenPago)
        {
            gestorLog.Entrar();
            Cliente cliente = new Cliente();
            List<Seguro> seguroBasico = new List<Seguro>();
            List<Seguro> seguroVoluntarios = new List<Seguro>();
            List<PlanConexion> paquete = new List<PlanConexion>();
            List<AhorroIndividuales> ahorroIndividual = new List<AhorroIndividuales>();
            List<AhorroIndividuales> filtroAhorroIndividual = new List<AhorroIndividuales>();
            List<Beneficiario> beneficiarios = new List<Beneficiario>();
            List<PagoPorDividendo> pagoPorDividendo = new List<PagoPorDividendo>();
            List<PagoPorDividendo> filtroPagoPorDividendo = new List<PagoPorDividendo>();
            List<NombrePorDividendo> nombrePorDividendos = new List<NombrePorDividendo>();
            
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@numeroCredito", numeroCredito, DbType.AnsiStringFixedLength);
             
                // parameters.Add("@numConsulta", 2, DbType.AnsiStringFixedLength);

                var result = aseConnection.QueryMultiple(
                          "cob_cartera.dbo.tcrp_DatosDocumentosPlantillas"
                        , parameters
                        , commandType: CommandType.StoredProcedure);

                cliente = result.ReadFirstOrDefault<Cliente>();
                if (cliente== null)
                {
                    throw new BusinessException("El número de crédito no existe");
                }

                cliente.Avales = result.Read<Aval>();
                cliente.Credito = result.ReadFirstOrDefault<Credito>();             
                cliente.Credito.Dividendos = result.Read<Dividendo>();
                cliente.Beneficiarios = result.Read<Beneficiario>();
                cliente.Ahorro = result.ReadFirstOrDefault<Ahorro>();
                cliente.RepresentanteLegal = result.ReadFirstOrDefault<RepresentanteLegal>();
                cliente.Credito.Comisiones = result.Read<Comisiones>();
                seguroBasico = (List<Seguro>)result.Read<Seguro>();
                seguroVoluntarios= (List<Seguro>)result.Read<Seguro>();
                paquete = (List<PlanConexion>)result.Read<PlanConexion>();

                if(cliente.BanderaExpres == 0)
                {
                    cliente.Credito.DatoSolicitudCredito = result.ReadFirstOrDefault<DatosSolicitudCredito>();
                }
              
                if (cliente.Credito.TipoCredito == "G")
                {                    
                    
                    if (!result.IsConsumed)
                    {
                        beneficiarios = (List<Beneficiario>)result.Read<Beneficiario>();

                        
                        if (numeroClientes.Count==0 || numeroClientes==null)
                        {                            
                            ahorroIndividual = (List<AhorroIndividuales>)result.Read<AhorroIndividuales>();
                        }
                        else 
                        {
                            ahorroIndividual = new List<AhorroIndividuales>();
                            filtroAhorroIndividual = (List<AhorroIndividuales>)result.Read<AhorroIndividuales>();
                            //int[] filtroAhorroIndividual = numeroClientes.ToArray();
                            //var ahorroIndivi = filtroAhorroIndividual.FirstOrDefault();
                            // ahorroIndividual = ahorroIndividual.Where(a => a.NumeroCliente.CompareTo(numeroClientes)).ToList();
                            foreach (AhorroIndividuales aho in filtroAhorroIndividual)
                            {
                                if (numeroClientes.Contains(aho.NumeroCliente))
                                {
                                    ahorroIndividual.Add(aho);
                                }
                            }
                        }

                        if (numeroDividendos.Count == 0 || numeroDividendos == null)
                        {
                            pagoPorDividendo = (List<PagoPorDividendo>)result.Read<PagoPorDividendo>();
                        }
                        else 
                        {
                            pagoPorDividendo = new List<PagoPorDividendo>();
                            filtroPagoPorDividendo = (List<PagoPorDividendo>)result.Read<PagoPorDividendo>();

                            foreach (PagoPorDividendo pag in filtroPagoPorDividendo)
                            {
                                if (numeroDividendos.Contains(pag.NumeroDividendo))
                                {
                                    pagoPorDividendo.Add(pag);
                                }
                            }
                        }
                        

                        nombrePorDividendos = (List<NombrePorDividendo>)result.Read<NombrePorDividendo>();
                        cliente.Credito.TablaAcelerada = _tablaAcelerada(numeroCredito);
                        cliente.Credito.TablaActaFundacion = result.Read<ActaFundacion>();
                        if (cliente.BanderaCreditosSubsecuentes > 0)
                        {
                            cliente.Credito.BCCreditosSubsecuentes = _SolicitudGrupalBCCreditosSubsecuentes(numeroCredito);
                        }
                        // Seagrega plantilla Orden de pago
                       // var ordenPago = FactoryPago.ServicioOrdenPago.ObtieneDatosOrdenPagoAsync("2940017317").Result; //2940017317
                       //var ordenPago = FactoryPago.ServicioOrdenPago.ObtieneDatosOrdenPagoAsync(numeroCredito).Result; //2940017317
                        if (ordenPago != null)
                        {
                            cliente.Credito.OrdenesPago = new List<OrdenDePago>();
                            foreach (ServDocumentos.Core.Dtos.DatosTcrCaja.DatosOrdenPago  datoOrdenPago in ordenPago.ListaDatosOrdenPago) 
                            {                          

                                OrdenDePago datoOrden = new OrdenDePago()
                                {                                    
                                    Concepto = datoOrdenPago.Concepto,
                                    DiasVigencia = datoOrdenPago.DiasVigencia,                                    
                                    FechaVigencia =   (datoOrdenPago.FechaVigencia.ToString()).Substring(0,10),
                                    NumeroConvenio = datoOrdenPago.NumeroConvenio,
                                    MontoOrdenPago = datoOrdenPago.Monto,
                                    NombreBanco = datoOrdenPago.Banco,
                                    NombreCompleto = datoOrdenPago.Nombre,
                                    NumeroCliente = datoOrdenPago.NumeroCliente,
                                    ReferenciaPago =datoOrdenPago.Referencia,
                                    FechaOperacion = (datoOrdenPago.FechaOperacion.ToString()).Substring(0,10),
                                    NumeroCredito = numeroCredito
                                }   
                                ;
                                cliente.Credito.OrdenesPago.Add(datoOrden);
                            }
                           
                        }

                    }                   

                }

                if (cliente.BanderaAcelerada>0)
                {
                    cliente.Credito.TablaAcelerada = _tablaAcelerada(numeroCredito);
                }


                if (!MapeoDatos(ahorroIndividual, seguroBasico, seguroVoluntarios, paquete, cliente.Credito.TipoCredito,cliente, beneficiarios, pagoPorDividendo, nombrePorDividendos))
                {
                    throw new BusinessException("No existe seguros");
                }




            }
            finally
            {
                gestorLog.Salir();
            }
            return cliente;
        }
        public bool MapeoDatos(List<AhorroIndividuales> datosAhorro, List<Seguro> seguroBasico, List<Seguro> seguroVoluntario, List<PlanConexion> paquete, string tipoCredito, Cliente cliente,List<Beneficiario> beneficiarios,List<PagoPorDividendo> pagoPorDividendo, List<NombrePorDividendo> dividendoPorCliente)
        {
            gestorLog.Entrar();
            bool respuesta = true;
            long idCliente = 0;
            int contador = 0;
            Seguro segurosBasicos = new Seguro();           
            try
            {
                if (tipoCredito == "G")
                {
                    foreach (var datosInd in datosAhorro)
                    {

                        idCliente = datosInd.NumeroCliente;

                        foreach (var segBasico in seguroBasico)
                        {
                            if(idCliente == segBasico.ClienteId)
                            {
                                datosInd.SeguroBasico = new Seguro();
                                datosInd.SeguroBasico.ClienteId = segBasico.ClienteId;
                                datosInd.SeguroBasico.Certificado = segBasico.Certificado;
                                datosInd.SeguroBasico.FechaFin = segBasico.FechaFin;
                                datosInd.SeguroBasico.FechaInicio = segBasico.FechaInicio;
                                datosInd.SeguroBasico.Paquete = segBasico.Paquete;
                                datosInd.SeguroBasico.Tipo = segBasico.Tipo;
                                datosInd.SeguroBasico.TipoSeguroId = segBasico.TipoSeguroId;
                                
                            }

                        }
                        foreach (var segVoluntario in seguroVoluntario)
                        {
                            if(idCliente== segVoluntario.ClienteId)
                            {
                                datosInd.SeguroVoluntario = new Seguro();
                                datosInd.SeguroVoluntario.ClienteId = segVoluntario.ClienteId;
                                datosInd.SeguroVoluntario.Certificado = segVoluntario.Certificado;
                                datosInd.SeguroVoluntario.FechaFin = segVoluntario.FechaFin;
                                datosInd.SeguroVoluntario.FechaInicio = segVoluntario.FechaInicio;
                                datosInd.SeguroVoluntario.Paquete = segVoluntario.Paquete;
                                datosInd.SeguroVoluntario.Tipo = segVoluntario.Tipo;
                                datosInd.SeguroVoluntario.TipoSeguroId = segVoluntario.TipoSeguroId;
                            }
                            
                        }

                        foreach (var paqCencel in paquete)
                        {
                            if (idCliente == paqCencel.ClienteId)
                            {
                                datosInd.PlanConexion = new PlanConexion();
                                datosInd.PlanConexion.ClienteId = paqCencel.ClienteId;
                                datosInd.PlanConexion.CodigoCompra = paqCencel.CodigoCompra;
                                datosInd.PlanConexion.MontoPaquete = paqCencel.MontoPaquete;
                                datosInd.PlanConexion.PaqueteId = paqCencel.PaqueteId;
                            }
                        }
                        List<Beneficiario> agregarBeneficiarios = new List<Beneficiario>();

                        foreach (var beneficiariosInd in beneficiarios)
                        {
                            if(idCliente == beneficiariosInd.NumeroCliente)
                            {
                                Beneficiario todoBeneficiarios = new Beneficiario();
                                todoBeneficiarios.NumeroCliente = beneficiariosInd.NumeroCliente;
                                todoBeneficiarios.NombreCompleto = beneficiariosInd.NombreCompleto;
                                todoBeneficiarios.Nombre = beneficiariosInd.Nombre;
                                todoBeneficiarios.ApellidoPaterno = beneficiariosInd.ApellidoPaterno;
                                todoBeneficiarios.ApellidoMaterno = beneficiariosInd.ApellidoMaterno;
                                todoBeneficiarios.Porcentaje = beneficiariosInd.Porcentaje;
                                todoBeneficiarios.TipoBeneficiario = beneficiariosInd.TipoBeneficiario;
                                todoBeneficiarios.Relacion = beneficiariosInd.Relacion;
                                todoBeneficiarios.DireccionCompleta = beneficiariosInd.DireccionCompleta;
                                todoBeneficiarios.Telefono = beneficiariosInd.Telefono;
                                todoBeneficiarios.FechaNacimiento = beneficiariosInd.FechaNacimiento;
                                todoBeneficiarios.Observacion = beneficiariosInd.Observacion;
                                agregarBeneficiarios.Add(todoBeneficiarios);


                            }

                        }
                        datosInd.Beneficiarios = agregarBeneficiarios;



                    }

                    cliente.AhorroIndividual = datosAhorro;
                    contador = 1;
                    ///plantilla por pago
                    foreach (var pagoDiv in pagoPorDividendo)
                    {
                        contador = 1;
                        List<NombrePorDividendo> agregarPorDividiendo = new List<NombrePorDividendo>();
                        foreach (var nombreDiv in dividendoPorCliente)
                        {
                            if(pagoDiv.NumeroDividendo == nombreDiv.NumeroDividendo)
                            {
                                
                                NombrePorDividendo nomPorDiv = new NombrePorDividendo();
                                nomPorDiv.Id = contador;
                                nomPorDiv.NumeroDividendo = nombreDiv.NumeroDividendo;                               
                                nomPorDiv.NombreCompleto = nombreDiv.NombreCompleto;
                                nomPorDiv.Cuota = nombreDiv.Cuota;
                                nomPorDiv.Ahorro = nombreDiv.Ahorro;
                                nomPorDiv.Total = nombreDiv.Total;
                                agregarPorDividiendo.Add(nomPorDiv);

                                contador = contador + 1;

                            }

                        }
                        pagoDiv.ClientePorDividendo = agregarPorDividiendo;
                    }
                    cliente.Credito.PagoPorDividendo = pagoPorDividendo;
                }
                else
                {
                    foreach (var item in seguroBasico)
                    {
                        Seguro segurosInd = new Seguro();
                        segurosInd.ClienteId = item.ClienteId;
                        segurosInd.Certificado = item.Certificado;
                        segurosInd.FechaFin = item.FechaFin;
                        segurosInd.FechaInicio = item.FechaInicio;
                        segurosInd.Paquete = item.Paquete;
                        segurosInd.Tipo = item.Tipo;
                        segurosInd.TipoSeguroId = item.TipoSeguroId;

                        cliente.SeguroBasico = segurosInd;
                    }
                    foreach (var item in seguroVoluntario)
                    {
                        Seguro segurosVol = new Seguro();
                        segurosVol.ClienteId = item.ClienteId;
                        segurosVol.Certificado = item.Certificado;
                        segurosVol.FechaFin = item.FechaFin;
                        segurosVol.FechaInicio = item.FechaInicio;
                        segurosVol.Paquete = item.Paquete;
                        segurosVol.Tipo = item.Tipo;
                        segurosVol.TipoSeguroId = item.TipoSeguroId;

                        cliente.SeguroVoluntario = segurosVol;
                    }
                    foreach (var paqCencel in paquete)
                    {
                        PlanConexion paqueteInvidual = new PlanConexion();
                        paqueteInvidual.ClienteId = paqCencel.ClienteId;
                        paqueteInvidual.CodigoCompra = paqCencel.CodigoCompra;
                        paqueteInvidual.MontoPaquete = paqCencel.MontoPaquete;
                        paqueteInvidual.PaqueteId = paqCencel.PaqueteId;
                        cliente.PlanConexion = paqueteInvidual;
                    }

                   
                }
                

            }
            finally
            {
                gestorLog.Salir();
            }
            return respuesta;
        }

        public EstadoCuenta ObtenerDatosEstadoCuenta(string numeroCredito)
        {
            gestorLog.Entrar();
            EstadoCuenta estadoCuenta = new EstadoCuenta();
            
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@numeroCredito", numeroCredito, DbType.AnsiStringFixedLength);
                parameters.Add("@dividendo", 0, DbType.Int32);

                var result = aseConnection.QueryMultiple(
                          "cob_cartera.dbo.tcrp_DatosEstadoCuenta"
                        , parameters
                        , commandType: CommandType.StoredProcedure);

                estadoCuenta = result.ReadFirstOrDefault<EstadoCuenta>();
                if (estadoCuenta == null)
                {
                    throw new BusinessException("El número de crédito no existe");
                }

                estadoCuenta.TablaEstadoCuenta = result.Read<TablaPagos>();               
             
            }
            finally
            {
                gestorLog.Salir();
            }
            return estadoCuenta;
        }
        private SolicitudGrupalBCCreditosSubsecuentes _SolicitudGrupalBCCreditosSubsecuentes(string numeroCredito)
        {
            gestorLog.Entrar();
            SolicitudGrupalBCCreditosSubsecuentes creditosSubsecuentes = new SolicitudGrupalBCCreditosSubsecuentes();

            try
            {
                var parameters1 = new DynamicParameters();
                parameters1.Add("@i_nobanco", numeroCredito, DbType.AnsiStringFixedLength);


                var result1 = aseConnection.QueryMultiple(
                          "cob_cartera.dbo.tcrp_creditos_subcecuentes_BC"
                        , parameters1
                        , commandType: CommandType.StoredProcedure);


                creditosSubsecuentes = result1.ReadFirstOrDefault<SolicitudGrupalBCCreditosSubsecuentes>();

                creditosSubsecuentes.CredSubsecuentes = result1.Read<CreditosSubsecuentes>();


                creditosSubsecuentes.TotalCredSubs = result1.ReadFirstOrDefault<TotalesCreditosSubsecuentes>();



            }
            finally
            {
                gestorLog.Salir();
            }

            return creditosSubsecuentes;


        }


    }
}
