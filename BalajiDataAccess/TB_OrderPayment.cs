//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BalajiDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_OrderPayment
    {
        public long OPaymentId { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string PaymentReference { get; set; }
        public string PayRemarks { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public Nullable<decimal> CommissionAmount { get; set; }
    }
}