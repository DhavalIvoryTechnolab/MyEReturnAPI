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
    
    public partial class TB_ServiceOrderIncome
    {
        public long IncomeId { get; set; }
        public Nullable<long> OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public string IncomeType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Attachment { get; set; }
        public string Remarks { get; set; }
    }
}
