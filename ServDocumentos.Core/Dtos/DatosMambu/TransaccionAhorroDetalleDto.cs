using ServDocumentos.Core.Dtos.DatosMambu.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    /// <summary>
    /// Transaccion de ahorro del api de mambu
    /// </summary>
    public class TransaccionAhorroDetalleDto
    {
        /// <summary>
        /// Id de la transaccion
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Encoded Key de la transaccion
        /// </summary>
        public string EncodedKey { get; set; }
        /// <summary>
        /// Value date
        /// </summary>
        public DateTime ValueDate { get; set; }
        /// <summary>
        /// Tipo de transaccion
        /// </summary>
        public TipoTransaccionAhorro? Type { get; set; }
        /// <summary>
        /// Cantidad de la transaccion
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Moneda
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Comentario de la transaccion
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Balances
        /// </summary>
        public AccountBalances AccountBalances { get; set; }
        /// <summary>
        /// Detalles de la transaccion
        /// </summary>
        public TransactionDetails TransactionDetails { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Transferencia_SPEI _Transferencia_SPEI { get; set; }
        public Datos_Adicionales_Transaccion _Datos_Adicionales_Transaccion { get; set; }
        public Caja _Caja { get; set; }
        public Reversas_PTS _Reversas_PTS { get; set; }
        public Compras_Card _Compras_Card { get; set; }
        
        /// <summary>
        /// Descripcion del tipo de transaccion
        /// </summary>
        [JsonIgnore]
        public string TypeDescription
        {
            get
            {
                return Type
                    ?.GetType()
                    .GetMember(Type.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description ?? Type?.ToString();
            }
        }

        public class Transferencia_SPEI
        {
            public string S_conceptoPago { get; set; }

            public string S_cuentaConcentradora { get; set; }
            public string S_refNum { get; set; }
            public string S_clabeOrigen { get; set; }
            public string S_cveRastreo { get; set; }

            public string S_rfcDestino { get; set; }
            public string S_rfcOrigen { get; set; }
            public string S_fhOperacion { get; set; }
            public string S_nombreDestino { get; set; }
            public string S_folio_paquete { get; set; }
            public string S_idTipoCtaDestino { get; set; }
            public string S_folio { get; set; }
            public string S_causaDev { get; set; }
            public string S_iva { get; set; }
            public string S_refCob { get; set; }

            public string S_nombOrigen { get; set; }
            public string S_idTipoPago { get; set; }
            public string S_ctaDestino { get; set; }
            public string S_bancoDestino { get; set; }
            

        }

        public class Datos_Adicionales_Transaccion
        {
            public string Concepto_Transaccion { get; set; }

            public string S_Referencia_TCH { get; set; }
            public string S_Producto_TCH { get; set; }
            public string Tipo_transaccion { get; set; }


        }
        public class Caja
        {
            public string Caja_Origen { get; set; }

            public string Caja_Fecha_Transaccion { get; set; }
            public string Caja_Id_Transaccion { get; set; }
            public string Caja_Num_Sucursal { get; set; }


        }

        public class Reversas_PTS
        {
            public string P_MotivoReverso { get; set; }
            public string P_FechaTranOriginal { get; set; }
            public string P_IdTranOriginal { get; set; }            
        }

        public class Compras_Card
        {
            public string _mcMotivo { get; set; }
            public string _mcNombreComercio { get; set; }
            public string _mcFechaOperacion { get; set; }
            public string _mcIdentificador { get; set; }
            public string _mcTerminal { get; set; }
        }
        

    }
}
