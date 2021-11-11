using BalajiDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class RootClass
    {
    }

    public class AdminChangePassword
    {
        public string userId { get; set; }
        public string password { get; set; }
        public string newPassword { get; set; }
    }

    public class Message
    {
        public string MsgText { get; set; }
        public long SID { get; set; }
    }

    public class ChangePassword
    {
        public string password { get; set; }
        public long SID { get; set; }
    }

 

    public class UpdateStatus
    {
        public long sid { set; get; }
        public string status { set; get; }
    }


    public class Webhooks
    {
        public long status_id { get; set; }
        public string utr { get; set; }
        public string report_id { get; set; }
        public long client_id { get; set; }
        public string number { get; set; }
    }

    public class CallBackAeps
    {
        public long status_id { get; set; }
        public string utr { get; set; }
        public decimal amount { get; set; }
        public string report_id { get; set; }
        public long payment_id { get; set; }
        public string number { get; set; }
        public string message { get; set; }
        public string outlet_id { get; set; }
    }


}