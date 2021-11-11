using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class ComplaintDataModel
    {
        public string MobileNo { get; set; }
        public long? TableId { get; set; }
        public long? TransactionId { get; set; }
        public string Complaint { get; set; }
        public string Status { get; set; }
    }

    public class ComplaintDataModel2
    {
        public string MobileNo { get; set; }
        public long? TableId { get; set; }
        public long? TransactionId { get; set; }
        public string Complaint { get; set; }
        public string Status { get; set; }
        public long RegistrationId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ComplaintType { get; set; }
        public long ComplaintId { get; set; }
        public DateTime Cdate { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
    }

    public class ReportDataModel
    {
        public long Id { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Profit { get; set; }
        public string UTR { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}