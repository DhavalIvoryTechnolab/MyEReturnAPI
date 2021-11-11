using BalajiDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class RegistrationResponse
    {
        public TB_Registration Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
    }

    public class Responses
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class RResponse
    {
        public string username { get; set; }
        public string password { get; set; }
        public string message { get; set; }
        public bool Status { get; set; }
    }

    public class NewRegistration
    {
        public string ShopName { get; set; }
        public string Category { get; set; }
        public string Address { get; set; }
        public long ShopNo { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public long TahsilsId { get; set; }
        public long State { get; set; }
        public long City { get; set; }
        public string EmailId { get; set; }
        public string PostOffice { get; set; }
        public string Pincode { get; set; }
        public string ContactPersoneName { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public long RegistrationId { get; set; }
    }
}