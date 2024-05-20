using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Core.Helpers
{
    public class Conversiones
    {
        public class NumberToText
        {
            private readonly string[] _ones =
            {
            "cero",
            "un",
            "dos",
            "tres",
            "cuatro",
            "cinco",
            "seis",
            "siete",
            "ocho",
            "nueve"
            };

            private readonly string[] _teens =
            {
            "diez",
            "once",
            "doce",
            "trece",
            "catorce",
            "quince",
            "dieciseis",
            "diecisiete",
            "dieciocho",
            "diecinueve"
            };

            private readonly string[] _tens =
            {
            "",
            "diez",
            "veinte",
            "treinta",
            "cuarenta",
            "cincuenta",
            "sesenta",
            "setenta",
            "ochenta",
            "noventa"
            };

            private readonly string[] _centenas =
            {
            "",
            "ciento",
            "doscientos",
            "trescientos",
            "cuatrocientos",
            "quinientos",
            "seiscientos",
            "setecientos",
            "ochocientos",
            "novecientos"
            };

            private readonly string[] _thousands =
            {
            "",
            "mil",
            "millon",
            "billon",
            "trillon",
            "cuatrillon"
            };

            /// <summary>
            /// Converts a numeric value to words suitable for the portion of
            /// a check that writes out the amount.
            /// </summary>
            /// <param name="value">Value to be converted</param>
            /// <returns></returns>
            public string Convert(decimal value)
            {
                StringBuilder builder = new StringBuilder(Convert((long)value));                

                // Append fractional portion/cents
                builder.AppendFormat("con {0:00}/100", (value - (long)value) * 100);

                // Capitalize first letter
                return String.Format("{0}{1}",
                Char.ToUpper(builder[0]),
                builder.ToString(1, builder.Length - 1));
            }
            public string Convert(long value)
            {
                string digits,  tempO, tempT, tempH;
                bool allZeros = false;
                tempT = "";
                tempO = "";
                // Use StringBuilder to build result
                StringBuilder builder = new StringBuilder();
                // Convert integer portion of value to string
                digits = ((long)value).ToString();
                // Traverse characters in reverse order
                for (int i = digits.Length - 1; i >= 0; i--)
                {
                    int ndigit = digits[i] - '0';
                    int column = (digits.Length - (i + 1));

                    // Determine if ones, tens, or hundreds column
                    switch (column % 3)
                    {
                        case 0:        // Ones position
                            bool showThousands = true;
                            if (i == 0)
                            {
                                // First digit in number (last in loop)
                                tempO = String.Format("{0} ", _ones[ndigit]);
                            }
                            else if (digits[i - 1] == '1')
                            {
                                // This digit is part of "teen" value
                                tempO = String.Format("{0} ", _teens[ndigit]);
                                // Skip tens position
                                i--;
                            }
                            else if (ndigit != 0)
                            {
                                // Any non-zero digit                               
                                tempO = String.Format("{0} ", _ones[ndigit]);
                            }
                            else
                            {
                                // This digit is zero. If digit in tens and hundreds
                                // column are also zero, don't show "thousands"
                                tempO = String.Empty;
                                // Test for non-zero digit in this grouping
                                if (digits[i - 1] != '0' || (i > 1 && digits[i - 2] != '0'))
                                    showThousands = true;
                                else
                                    showThousands = false;
                            }

                            // Show "thousands" if non-zero in grouping
                            if (showThousands)
                            {
                                if (column > 0)
                                {
                                    var thous = _thousands[column / 3];
                                    if (!tempO.ToLower().Trim().Contains("un"))
                                    {
                                        if (thous != "" && thous.ToLower().Trim() != "mil")
                                        {
                                            thous += "es";
                                        }
                                    }

                                    builder.Insert(0, thous + " ");

                                }
                                else if (column == 0)
                                {
                                    if (tempO.ToLower().Trim().Contains("un"))
                                    {
                                        tempO = "uno ";
                                    }
                                }
                                // Indicate non-zero digit encountered
                                allZeros = false;
                            }
                            builder.Insert(0, tempO);
                            break;

                        case 1:        // Tens column
                            if (ndigit > 0)
                            {
                                tempT = String.Format("{0}{1}",
                                _tens[ndigit],
                                (digits[i + 1] != '0') ? " y " : " ");
                                builder.Insert(0, tempT);
                            }
                            break;

                        case 2:        // Hundreds column
                            if (ndigit > 0)
                            {                                

                                tempH = String.Format("{0} ", _centenas[ndigit]);
                                if (tempO == "" && tempT == "" && ndigit == 1)
                                {
                                    tempH = "cien ";
                                }
                                builder.Insert(0, tempH);
                            }
                            tempT = "";
                            tempO = "";
                            break;
                    }
                }
                var temp2 = String.Format("{0}{1}",
                Char.ToUpper(builder[0]),
                builder.ToString(1, builder.Length - 1));               
                // Capitalize first letter
                return temp2;
            }

        }

    }
}
