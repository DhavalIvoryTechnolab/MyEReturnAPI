using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class PancardResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string LogginId { get; set; }
        public string Password { get; set; }
        public string MemberId { get; set; }
    }

    public class Generaldetails
    {
        [Required]
        public string ShopName { get; set; }
        [Required]
        public string ShopAddress1 { get; set; }
        [Required]
        public string ShopAddress2 { get; set; }
        [Required]
        public string ShopCategory { get; set; }
        [Required]
        public string ShopPincode { get; set; }
        [Required]
        public string Adhar_Number { get; set; }
        [Required]
        public string Pan_Number { get; set; }
        [Required]
        public string Adhar_Front_Image { get; set; }
        [Required]
        public string Adhar_Back_Image { get; set; }
        [Required]
        public string Pan_Card_Image { get; set; }
        [Required]
        public string Bank_Name { get; set; }
        [Required]
        public string Bank_Account_Number { get; set; }
        [Required]
        public string Account_Holder_Name { get; set; }
        [Required]
        public string IFSC_Code { get; set; }
    }

    public class PanCheckModel
    {
        [Required]
        public string MobieNo { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string ShopName { get; set; }
        [Required]
        public string ShopAddress1 { get; set; }
        [Required]
        public string ShopAddress2 { get; set; }
        [Required]
        public string Adahr_Number { get; set; }
        [Required]
        public string Pan_Number { get; set; }
    }
}