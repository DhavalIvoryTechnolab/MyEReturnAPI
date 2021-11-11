using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class Pay2AllClass
    {
    }

    public class Recharge
    {
        public string number { get; set; }
        public decimal amount { get; set; }
        public long? provider_id { get; set; }
        public long? client_id { get; set; }
    }

    public class TransferMoney
    {
        public long mobile_number { get; set; }
        public int amount { get; set; }
        public string beneficiary_name { get; set; }
        public long? account_number { get; set; }
        public string ifsc { get; set; }
        public int channel_id { get; set; }
        public long client_id { get; set; }
        public int provider_id { get; set; }
    }

    public class Presponse
    {
        public long? status_id { get; set; }
        public string utr { get; set; }
        public string orderid { get; set; }
        public long? report_id { get; set; }
        public string Message { get; set; }
    }
}