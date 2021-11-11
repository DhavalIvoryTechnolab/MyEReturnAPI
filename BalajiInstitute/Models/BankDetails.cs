using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class BankDetails
    {
        public long RegistrationId { get; set; }
        public string Bank_Name { get; set; }
        public string Bank_Account_Number { get; set; }
        public string Account_Holder_Name { get; set; }
        public string IFSC_Code { get; set; }
        public string Account_Type { get; set; }
        public string Bank_Address { get; set; }
        public string CancelledChequePhoto { get; set; }
        public string CancelledChequeName { get; set; }
    }

    public class Response
    {
        public bool Status { get; set; }
        public long StatusCode { get; set; }
        public long Otp { get; set; }
        public string Message { get; set; }
    }

    public class ResponseAeps
    {
        public bool Status { get; set; }
        public long StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class MoneyTransferResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public decimal MainWallet { get; set; }
        public decimal TreadWallet { get; set; }
    }

    public class RechargeIntialResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public decimal MainWallet { get; set; }
        public decimal TreadWallet { get; set; }
    }

    public class TResponse
    {
        public bool Status { get; set; }
        public long ClientId { get; set; }
        public string Message { get; set; }
        public string WalletResponse { get; set; }
        public decimal MainWallet { get; set; }
        public decimal TreadWallet { get; set; }
    }

    public class SearchResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public long SenderId { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
    }

    public class UpdateTransactionStatus
    {
        public long Client_Id { get; set; }
        public long status_id { get; set; }
        public string utr { get; set; }
        public string report_id { get; set; }
        public long orderid { get; set; }
        public string message { get; set; }
    }
}