

using ServDocumentos.Core.Enumeradores;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServDocumentos.Core.ValidacionesPersonalizadas
{
    class RequiredTipoProcesoAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string str = value as string;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }


            var resultado = false;
            //foreach (var elemento in Enum.GetNames(typeof(TipoProcesos)))
            //{
            //    if (elemento.ToUpper().Equals(str.ToUpper()))
            //    {
            //        resultado = true;
            //        break;
            //    }
            //}

            return resultado;

        }


    }
}
