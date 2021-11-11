using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiInstitute.Models;
using System.Web;
//using System.Web.Http;
namespace BalajiInstitute.Controllers
{
    public class UploadFileController : ApiController
    {
        [HttpPost]
        public ResultResponse Post()
        {
            ResultResponse res = new ResultResponse();
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var request = HttpContext.Current.Request;
                var photo = request.Files["photo"];
                var type = request.Form["type"];
                string Tofile = "";
                if (photo != null && photo.ContentLength != 0)
                {
                    string[] str = photo.FileName.Split('.');
                    Tofile = timeStamp + "." + str[1];
                    if(type == "Profile")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Upload/Profile/" + Tofile));
                    }

                    if(type == "Payment")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Upload/Payment/" + Tofile));
                    }

                    if(type == "Shop")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Upload/Shop/" + Tofile));
                    }


                    if(type == "Income")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/Income/" + Tofile));
                    }

                    if (type == "Discussion")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/Discussion/" + Tofile));
                    }

                    if (type == "Deduction")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/Deduction/" + Tofile));
                    }

                    if (type == "OtherServices")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/OtherServices/" + Tofile));
                    }

                    if (type == "Digital")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/Digital/" + Tofile));
                    }

                    if(type== "GST_and_Account")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/GST_and_Account/" + Tofile));
                    }

                    if (type == "GST")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/GST/" + Tofile));
                    }

                    if (type == "ITR")
                    {
                        photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/ITR/" + Tofile));
                    }
                    res.status = true;
                    res.message = "Upload Image Sucessfully Done";
                    res.FileName = photo.FileName;
                   return res;
                }
                else
                {
                    res.status = false;
                    res.message = "Something Went Wrong";
                    res.FileName = photo.FileName;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return res;
            }
        }
    }
}
