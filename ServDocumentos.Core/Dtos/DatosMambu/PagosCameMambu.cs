using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class PagosCameMambu
    {
        public List<Installment> installments { get; set; }
    }
    public class Installment
    {
        public string encodedKey { get; set; }
        public string number { get; set; }
        public string dueDate { get; set; }
        public string state { get; set; }
        public Principal principal { get; set; }
        public Interest interest { get; set; }
        public Interest fee { get; set; }
        public Interest penalty { get; set; }
    }

    public class Principal
    {
        public Amount amount { get; set; }


    }
    public class Amount
    {
        public double expected { get; set; }
        public double paid { get; set; }
        public double due { get; set; }
    }
    public class Interest
    {
        public Amount amount { get; set; }
        public Amount tax { get; set; }


    }
}
