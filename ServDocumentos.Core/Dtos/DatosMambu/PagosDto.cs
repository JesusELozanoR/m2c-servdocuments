using System;
using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosMambu
{
    public class Pagos
    {
        public string encodedKey { get; set; }
        public string parentAccountKey { get; set; }
        public DateTime dueDate { get; set; }
        public string principalDue { get; set; }
        public string principalPaid { get; set; }
        public string interestDue { get; set; }
        public string interestPaid { get; set; }
        public string feesDue { get; set; }
        public string feesPaid { get; set; }
        public string penaltyDue { get; set; }
        public string penaltyPaid { get; set; }
        public string state { get; set; }
        public string taxInterestDue { get; set; }
        public string taxInterestPaid { get; set; }
        public string taxFeesDue { get; set; }
        public string taxFeesPaid { get; set; }
        public string taxPenaltyDue { get; set; }
        public string taxPenaltyPaid { get; set; }
        public IList<object> repaymentUnappliedFeeDetails { get; set; }      


    }
}
