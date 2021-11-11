using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiInstitute.Models;
using System.Web;
using BalajiDataAccess;

namespace BalajiInstitute.Controllers
{
    public class UploadDocumentController : ApiController
    {
        [HttpPost]
        public AppResponce Post()
        {
            AppResponce res = new AppResponce();
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var request = HttpContext.Current.Request;
                var photo = request.Files["photo"];
                var orderId = request.Form["OrderId"];
                var orderNumber = request.Form["OrderNumber"];
                var registrationId = request.Form["RegistrationId"];
                var documentType = request.Form["DocumentType"];
                var documentNumber = request.Form["DocumentNumber"];
                var DocumentName = request.Form["DocumentName"];
                var Remarks = request.Form["Remarks"];
                string Tofile = "";
                if (photo != null && photo.ContentLength != 0)
                {
                    string[] str = photo.FileName.Split('.');
                    Tofile = timeStamp + "." + str[1];
                    photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/UploadDocument/Order/ServiceOrderDocument/" + Tofile));
                    using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                    {
                        TB_ServiceOrderDocument tb = new TB_ServiceOrderDocument();
                        tb.Attachment = Tofile;
                        tb.DocumentName = DocumentName;
                        tb.DocumentNumber = documentNumber;
                        tb.DocumentType = documentType;
                        if(orderId !=null && orderId.Length != 0)
                        {
                            tb.OrderId = Convert.ToInt32(orderId);
                        }
                        if (registrationId != null && registrationId.Length != 0)
                        {
                            tb.RegistrationId = Convert.ToInt32(registrationId);
                        }
                        tb.Remarks = Remarks;
                        tb.OrderNumber = orderNumber;
                        entities.TB_ServiceOrderDocument.Add(tb);
                        entities.SaveChanges();
                    }
                    res.status = true;
                    res.message = "Complete Process Successfully";
                    res.FileName = photo.FileName;
                    return res;
                }
                else
                {
                    res.status = false;
                    res.message = "Complete Process Failed";
                    res.FileName = photo.FileName;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                Console.Write(ex.Message);
                return res;
            }
        }
    }
}
