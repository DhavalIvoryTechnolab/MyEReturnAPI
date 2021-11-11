using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class BranchSendPayment
    {
        public long PaymentId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentFor { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public string UploadFile { get; set; }
        public string UploadFileName { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public string RejectReason { get; set; }
        public string PaymentBy { get; set; }
        public string PaymentByCode { get; set; }
    }
}