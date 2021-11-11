using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalajiInstitute.Models
{
    public class DocumentsModel
    {
        public long RegistrationId { get; set; }
        public string Adhar_Number { get; set; }
        public string Pan_Number { get; set; }
        public string Adhar_Front_Image { get; set; }
        public string Adhar_Back_Image { get; set; }
        public string Pan_Card_Image { get; set; }
        public string Adhar_Front_Image_Name { get; set; }
        public string Adhar_Back_Image_Name { get; set; }
        public string Pan_Card_Image_Name { get; set; }
    }
}