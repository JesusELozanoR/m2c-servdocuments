using cmn.std.Log;
using Dapper;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Documento;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioDocumentos : RepositorioBase, IRepositorioDocumentos
    {

        public RepositorioDocumentos(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }

        public Int64 DocumentosRegistroGuarda(DocGuarda docGuarda)
        {
            gestorLog.Entrar();
            Int64 lastId = 0;
            try
            {
                int result = connection.Execute(
                       "docp_DocumentosRegistro_ins"
                     , new
                     {
                         @CreditoNumero = docGuarda.NumeroCredito
                         ,
                         @ClienteNumero = docGuarda.ClienteNumero
                         ,
                         @DatosJsonId = docGuarda.DatosJsonId == null ? "" : docGuarda.DatosJsonId
                         ,
                         @DatosJaonUrl = docGuarda.DatosJaonUrl == null ? "" : docGuarda.DatosJaonUrl,
                         @UsuarioCreacion = docGuarda.Usuario,
                         @EstadoRegistro = 1
                     }
                     , commandType: CommandType.StoredProcedure);

                lastId = result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return lastId;
        }

        public async Task<Int64> EstadosCuentaGenera()
        {
            gestorLog.Entrar();
            Int64 lastId = 0;
            try
            {
                int result = await connection.ExecuteAsync(
                       "docp_DocumentosGerneraEdoCta"
                     , new
                     {
                         @Usuario = ""
                     }
                     , commandType: CommandType.StoredProcedure);

                lastId = result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                gestorLog.Salir();
            }
            return lastId;
        }
    }
}
