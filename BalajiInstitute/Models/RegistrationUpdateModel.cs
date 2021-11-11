using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class RegistrationUpdateModel
    {
        public long RegistrationId { get; set; }
        public string MobileNo { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<long> ProfessionId { get; set; }
        public string ProfilePhoto { get; set; }
        public Nullable<long> TahsilsId { get; set; }
        public Nullable<long> StateId { get; set; }
        public Nullable<long> CityId { get; set; }
        public string Gender { get; set; }
        public string Pincode { get; set; }
        public string profileImageName { get; set; }
    }
}