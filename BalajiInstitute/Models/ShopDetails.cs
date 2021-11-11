using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class ShopDetails
    {
        public long RegistrationId { get; set; }
        public string ShopPhoto { get; set; }
        public string ShopImageName { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress1 { get; set; }
        public string ShopAddress2 { get; set; }
        public string ShopCategory { get; set; }
        public string ShopPincode { get; set; }
        public string ShopPostOffice { get; set; }
        public long ShopCity { get; set; }
        public long ShopState { get; set; }
        public string ShopAreaLocality { get; set; }
        public string OutlateIsStatus { get; set; }
        public string UpdateDetailsFor { get; set; }
    }
}