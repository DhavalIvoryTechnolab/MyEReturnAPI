using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiDataAccess;
using BalajiInstitute.Models;
using System.Transactions;
using System.Data.Entity;
using System.Web;
using System.IO;

namespace BalajiInstitute.Controllers
{
    public class PaymentController : ApiController
    {
        [HttpGet]
        public List<AddMoneyComplaintModel> Get(long regId,int PageNo)
        {
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    int iPageNum = PageNo;
                    var ListCount = entities.TB_BranchSendPayment.Where(e => e.RegistrationId == regId).ToList().Count();
                    int iPageSize = Convert.ToInt32(ListCount / PageNo);
                    var PaymentLists = entities.TB_BranchSendPayment.Where(e => e.RegistrationId == regId).ToList();
                    var ComplaintList = PaymentLists.Skip((iPageNum - 1) * iPageSize).ToList();
                    List<AddMoneyComplaintModel> AddmoneyList = new List<AddMoneyComplaintModel>();
                    foreach (var item in ComplaintList)
                    {
                        AddMoneyComplaintModel rdm = new AddMoneyComplaintModel();
                        rdm.Complaint = new Cmodel();
                        rdm.PaymentId = item.PaymentId;
                        rdm.RegistrationId = item.RegistrationId;
                        rdm.PaymentMode = item.PaymentMode;
                        rdm.PaymentFor = item.PaymentFor;
                        rdm.PaidAmount = item.PaidAmount;
                        rdm.ReferenceNo = item.ReferenceNo;
                        rdm.Remarks = item.Remarks;
                        rdm.PaidDate = item.PaidDate;
                        rdm.PaymentStatus = item.PaymentStatus;
                        rdm.PaymentType = item.PaymentType;
                        rdm.RejectReason = item.RejectReason;
                        rdm.PaymentBy = item.PaymentBy;
                        rdm.PaymentByCode = item.PaymentByCode;

                        var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.PaymentId).FirstOrDefault();
                        if (Cres != null)
                        {
                            rdm.Complaint.ComplaintId = Cres.ComplaintId;
                            rdm.Complaint.ComplaintType = Cres.ComplaintType;
                            rdm.Complaint.Subject = Cres.Subject;
                            rdm.Complaint.TransactionId = Cres.TransactionId;
                            rdm.Complaint.Status = Cres.Status;
                            rdm.Complaint.Message = Cres.Message;
                            rdm.Complaint.CreatedBy = Cres.CreatedBy;
                            rdm.Complaint.CreatedDate = Cres.CreatedDate;
                            rdm.Complaint.UpdatedBy = Cres.UpdatedBy;
                            rdm.Complaint.UpdatedDate = Cres.UpdatedDate;
                            rdm.Complaint.RegistrationId = Cres.RegistrationId;
                        }
                        AddmoneyList.Add(rdm);
                    }
                    var ResList = (from res in AddmoneyList
                                   where res.RegistrationId == regId
                                   select res).OrderByDescending(e => e.PaymentId).ToList();
                    return ResList;
                }
            }
            catch (Exception)
            {
                return new List<AddMoneyComplaintModel>();
            }
        }

        [HttpDelete]
        public AppResponce Delete(long paymentId)
        {
            AppResponce res = new AppResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        var entity = entities.TB_BranchSendPayment.Where(e => e.PaymentId == paymentId).FirstOrDefault();
                        entities.TB_BranchSendPayment.Remove(entity);
                        entities.SaveChanges();
                        res.status = true;
                        s.Complete();
                        return res;
                    }
                }

            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return res;
            }
        }

        [HttpPost]
        public ResultResponse Post(BranchSendPayment custData)
        {
            ResultResponse res = new ResultResponse();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        string imageName = "";
                        if (custData.UploadFile != "" && custData.UploadFile != null && custData.UploadFileName != "" && custData.UploadFileName != null)
                        {
                            String path = HttpContext.Current.Server.MapPath("~/Content/Upload/Files");
                            if (!System.IO.Directory.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                            }
                            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                            imageName = custData.UploadFileName;
                            string[] str = imageName.Split('.');
                            imageName = baseUrl + "/Content/Upload/Files/" + imageName;
                            string imgPath = Path.Combine(path, custData.UploadFileName);
                            byte[] imageBytes = Convert.FromBase64String(custData.UploadFile);
                            File.WriteAllBytes(imgPath, imageBytes);
                        }
                        TB_BranchSendPayment tB_ = new TB_BranchSendPayment();
                        tB_.PaymentId = custData.PaymentId;
                        tB_.RegistrationId = custData.RegistrationId;
                        tB_.PaymentMode = custData.PaymentMode;
                        tB_.PaymentType = custData.PaymentType;
                        tB_.PaymentFor = custData.PaymentFor;
                        tB_.PaidAmount = Convert.ToDecimal(custData.PaidAmount);
                        tB_.ReferenceNo = custData.ReferenceNo;
                        tB_.Remarks = custData.Remarks;
                        //tB_.PaidDate = custData.PaidDate;
                        tB_.PaidDate = DateTime.Now;
                        tB_.UploadFile = imageName;
                        tB_.PaymentStatus = custData.PaymentStatus;
                        tB_.PaymentBy = custData.PaymentBy; ;
                        tB_.PaymentByCode = custData.PaymentByCode; 
                        tB_.RejectReason = custData.RejectReason;
                        var entity = entities.TB_BranchSendPayment.Add(tB_);
                        entities.SaveChanges();
                        res.status = true;
                        res.message = "Your add payment request sent successfully";
                        res.FileName = imageName;
                        s.Complete();
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return res;
            }
        }

        [HttpPut]
        public AppResponce Put([FromBody] PayentModel custData)
        {
            AppResponce res = new AppResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        var entity = entities.TB_BranchSendPayment.Where(e => e.PaymentId == custData.PaymentId).FirstOrDefault();
                        entity.PaymentMode = custData.PaymentMode;
                        entity.PaymentFor = custData.PaymentFor;
                        entity.PaidAmount = custData.PaidAmount;
                        entity.ReferenceNo = custData.ReferenceNo;
                        entity.Remarks = custData.Remarks;
                        entity.PaidDate = custData.PaidDate;
                        entity.PaymentStatus = custData.PaymentStatus;
                        entities.Entry(entity).State = EntityState.Modified;
                        entities.SaveChanges();
                        var wentity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == custData.RegistrationId).FirstOrDefault();
                        if(custData.PaymentType == "main")
                        {
                            wentity.MainWallet = wentity.MainWallet + custData.PaidAmount;
                        }
                        else
                        {
                            wentity.TreadWallet = wentity.TreadWallet + custData.PaidAmount;
                        }
                        entities.Entry(wentity).State = EntityState.Modified;
                        entities.SaveChanges();
                        res.status = true;
                        s.Complete();
                        return res;


                    }
                }

            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return res;
            }
        }

        [Route("api/Payment/AddedMoneyHistory")]
        [HttpGet]
        public IHttpActionResult AddedMoneyHistory(DateTime? fromdate, DateTime? Todate, int PageNo, long RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_BranchSendPayment.ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var PaymentLists = entities.TB_BranchSendPayment.Where(e => e.RegistrationId == RegistrationId).ToList();
                var ComplaintList = PaymentLists.Skip((iPageNum - 1) * iPageSize).ToList();
                List<AddMoneyComplaintModel> AddmoneyList = new List<AddMoneyComplaintModel>();
                foreach (var item in ComplaintList)
                {
                    AddMoneyComplaintModel rdm = new AddMoneyComplaintModel();
                    rdm.Complaint = new Cmodel();
                    rdm.PaymentId = item.PaymentId;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.PaymentMode = item.PaymentMode;
                    rdm.PaymentFor = item.PaymentFor;
                    rdm.PaidAmount = item.PaidAmount;
                    rdm.ReferenceNo = item.ReferenceNo;
                    rdm.Remarks = item.Remarks;
                    rdm.PaidDate = Convert.ToDateTime( item.PaidDate).Date;
                    rdm.PaymentStatus = item.PaymentStatus;
                    rdm.PaymentType = item.PaymentType;
                    rdm.RejectReason = item.RejectReason;
                    rdm.PaymentBy = item.PaymentBy;
                    rdm.PaymentByCode = item.PaymentByCode;

                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.PaymentId).FirstOrDefault();
                    if (Cres != null)
                    {
                        rdm.Complaint.ComplaintId = Cres.ComplaintId;
                        rdm.Complaint.ComplaintType = Cres.ComplaintType;
                        rdm.Complaint.Subject = Cres.Subject;
                        rdm.Complaint.TransactionId = Cres.TransactionId;
                        rdm.Complaint.Status = Cres.Status;
                        rdm.Complaint.Message = Cres.Message;
                        rdm.Complaint.CreatedBy = Cres.CreatedBy;
                        rdm.Complaint.CreatedDate = Cres.CreatedDate;
                        rdm.Complaint.UpdatedBy = Cres.UpdatedBy;
                        rdm.Complaint.UpdatedDate = Cres.UpdatedDate;
                        rdm.Complaint.RegistrationId = Cres.RegistrationId;
                    }
                    AddmoneyList.Add(rdm);
                }
                if (fromdate != null && Todate != null)
                {
                    var ResList = (from res in AddmoneyList
                                   where res.PaidDate >= Convert.ToDateTime(fromdate).Date && res.PaidDate <= Convert.ToDateTime(Todate).Date && res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PaymentId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in AddmoneyList
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PaymentId).ToList();
                    return Ok(ResList);
                }
            }
        }


        [Route("api/Payment/AddQRUrlToRetailer")]
        [HttpGet]
        public IHttpActionResult AddQRUrlToRetailer(DateTime? fromdate, DateTime? Todate, int PageNo, long RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_BranchSendPayment.ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var PaymentLists = entities.TB_BranchSendPayment.Where(e => e.RegistrationId == RegistrationId).ToList();
                var ComplaintList = PaymentLists.Skip((iPageNum - 1) * iPageSize).ToList();
                List<AddMoneyComplaintModel> AddmoneyList = new List<AddMoneyComplaintModel>();
                foreach (var item in ComplaintList)
                {
                    AddMoneyComplaintModel rdm = new AddMoneyComplaintModel();
                    rdm.Complaint = new Cmodel();
                    rdm.PaymentId = item.PaymentId;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.PaymentMode = item.PaymentMode;
                    rdm.PaymentFor = item.PaymentFor;
                    rdm.PaidAmount = item.PaidAmount;
                    rdm.ReferenceNo = item.ReferenceNo;
                    rdm.Remarks = item.Remarks;
                    rdm.PaidDate = Convert.ToDateTime(item.PaidDate).Date;
                    rdm.PaymentStatus = item.PaymentStatus;
                    rdm.PaymentType = item.PaymentType;
                    rdm.RejectReason = item.RejectReason;
                    rdm.PaymentBy = item.PaymentBy;
                    rdm.PaymentByCode = item.PaymentByCode;

                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.PaymentId).FirstOrDefault();
                    if (Cres != null)
                    {
                        rdm.Complaint.ComplaintId = Cres.ComplaintId;
                        rdm.Complaint.ComplaintType = Cres.ComplaintType;
                        rdm.Complaint.Subject = Cres.Subject;
                        rdm.Complaint.TransactionId = Cres.TransactionId;
                        rdm.Complaint.Status = Cres.Status;
                        rdm.Complaint.Message = Cres.Message;
                        rdm.Complaint.CreatedBy = Cres.CreatedBy;
                        rdm.Complaint.CreatedDate = Cres.CreatedDate;
                        rdm.Complaint.UpdatedBy = Cres.UpdatedBy;
                        rdm.Complaint.UpdatedDate = Cres.UpdatedDate;
                        rdm.Complaint.RegistrationId = Cres.RegistrationId;
                    }
                    AddmoneyList.Add(rdm);
                }
                if (fromdate != null && Todate != null)
                {
                    var ResList = (from res in AddmoneyList
                                   where res.PaidDate >= Convert.ToDateTime(fromdate).Date && res.PaidDate <= Convert.ToDateTime(Todate).Date && res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PaymentId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in AddmoneyList
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PaymentId).ToList();
                    return Ok(ResList);
                }
            }
        }

    }
}


