using BalajiDataAccess;
using BalajiInstitute.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BalajiInstitute.Controllers
{
    public class EreturnController : ApiController
    {
        [Route("api/Ereturn/WalletDetails")]
        [HttpGet]
        public IHttpActionResult WalletDetails(long RegistrationId) 
        {
            WalletResponse wres = new WalletResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                if(entity != null)
                {
                    wres.RegistrationId = entity.RegistrationId;
                    wres.WalletId = entity.WalletId;
                    wres.MainWallet = entity.MainWallet;
                    wres.TreadWallet = entity.TreadWallet;
                    wres.status = true;
                    wres.message = "Find Wallet Details";
                }
                else
                {
                    wres.status = false;
                    wres.message = "Error While Fetching Wallet Data";
                }
                return Ok(wres);
            }   
        }


        [Route("api/Ereturn/AddAmountWalletDetails")]
        [HttpGet]
        public IHttpActionResult AddAmountWalletDetails(long RegistrationId, decimal Amount)
        {
            WalletResponse wres = new WalletResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                if (entity != null)
                {
                    wres.MainWallet = entity.MainWallet;
                    wres.RegistrationId = RegistrationId;
                    var AddAmount = Convert.ToDecimal(Amount);
                    decimal TotalAmount = Convert.ToDecimal(entity.TreadWallet + Amount);
                    entity.TreadWallet = TotalAmount;
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();
                    wres.status = true;
                    wres.TreadWallet = TotalAmount;
                    wres.message = "TreadWallet Updated Successfully";
                }
                else
                {
                    wres.status = false;
                    wres.message = "Wallet Data Not Found";
                }
                return Ok(wres);
            }
        }

        [Route("api/Ereturn/InternalWalletTransfer")]
        [HttpGet]
        public IHttpActionResult InternalWalletTransfer(long RegistrationId, decimal Amount , string FromWallet)
        {
            WalletResponse wres = new WalletResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                if (entity != null)
                {
                    if(!string.IsNullOrEmpty(FromWallet) && FromWallet == "MainWallet")
                    {
                        if(Amount < (decimal)entity.MainWallet)
                        {
                            entity.MainWallet = Convert.ToDecimal(entity.MainWallet - Amount);
                            entity.TreadWallet = Convert.ToDecimal(entity.TreadWallet + Amount);
                            entities.Entry(entity).State = EntityState.Modified;
                            entities.SaveChanges();


                            wres.status = true;
                            wres.message = "Amount added to TreadWallet Successfully";
                        }
                        else
                        {
                            wres.status = false;
                            wres.message = "Existing MainWallet balance is low";
                        }
                    }
                    else if(!string.IsNullOrEmpty(FromWallet) && FromWallet == "TradeWallet")
                    {
                        if (Amount < (decimal)entity.TreadWallet)
                        {
                            entity.TreadWallet = Convert.ToDecimal(entity.TreadWallet - Amount);
                            entity.MainWallet = Convert.ToDecimal(entity.MainWallet + Amount);
                            entities.Entry(entity).State = EntityState.Modified;
                            entities.SaveChanges();
                            wres.status = true;
                            wres.message = "Amount added to MainWallet Successfully";
                        }
                        else
                        {
                            wres.status = false;
                            wres.message = "Existing TreadWallet balance is low";
                        }
                    }
                    else
                    {
                        wres.status = false;
                        wres.message = "Wallet Type is not proper selected.";
                    }

                }
                else
                {
                    wres.status = false;
                    wres.message = "Wallet Data Not Found";
                }
                return Ok(wres);
            }
        }

        [Route("api/Ereturn/DeductionAmounts")]
        [HttpPost]
        public IHttpActionResult DeductionAmounts(long RegistrationId,string Type,decimal Amount)
        {
            WalletResponse wres = new WalletResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).SingleOrDefault();
                if (entity != null)
                {
                    if(Type == "AEPS")
                    { 
                        var AddAmount = Convert.ToDecimal(Amount);
                        decimal TotalAmount = Convert.ToDecimal(entity.TreadWallet + Amount);
                        entity.TreadWallet = TotalAmount;
                        entities.Entry(entity).State = EntityState.Modified;
                        entities.SaveChanges();
                        wres.RegistrationId = entity.RegistrationId;
                        wres.WalletId = entity.WalletId;
                        wres.MainWallet = entity.MainWallet;
                        wres.TreadWallet = TotalAmount; 
                        wres.status = true;
                        wres.message = "Wallet Update SuccessFully";
                    }
                    else if(Type == "Money Transfer")
                    {
                        var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                        if(ent != null)
                        {
                            if (ent.TreadWallet >= Amount)
                            {
                                var Amnt = Convert.ToDecimal(Amount);
                                decimal TotalAmount = Convert.ToDecimal(ent.TreadWallet - Amount);
                                ent.TreadWallet = TotalAmount;
                                entities.Entry(ent).State = EntityState.Modified;
                                entities.SaveChanges();
                                wres.RegistrationId = ent.RegistrationId;
                                wres.WalletId = ent.WalletId;
                                wres.MainWallet = ent.MainWallet;
                                wres.TreadWallet = TotalAmount;
                                wres.status = true;
                                wres.message = "Wallet Update SuccessFully";
                            }
                            else
                            {
                                wres.status = false;
                                wres.message = "You Don't Have Sufficient Balance";
                            }
                        }
                        else
                        {
                            wres.status = false;
                            wres.message = "Wallet Data Not Found";
                        }
                    }
                    else if(Type == "Mobile Recharge" || Type == "DTH Recharge" || Type == "Gas Bill" || Type == "Light Bill")
                    {
                        var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                        if (ent != null)
                        {
                            if (ent.TreadWallet >= Amount)
                            {
                                var Amnt = Convert.ToDecimal(Amount);
                                decimal TotalAmount = Convert.ToDecimal(ent.MainWallet - Amount);
                                ent.MainWallet = TotalAmount;
                                entities.Entry(ent).State = EntityState.Modified;
                                entities.SaveChanges();
                                wres.RegistrationId = ent.RegistrationId;
                                wres.WalletId = ent.WalletId;
                                wres.MainWallet = TotalAmount;
                                wres.TreadWallet = ent.TreadWallet;
                                wres.status = true;
                                wres.message = "Wallet Update SuccessFully";
                            }
                            else
                            {
                                wres.status = false;
                                wres.message = "You Don't Have Sufficient Balance";
                            }
                        }
                        else
                        {
                            wres.status = false;
                            wres.message = "Wallet Data Not Found";
                        }
                    }
                    else
                    {
                        wres.status = false;
                        wres.message = "Services Type Miss Matched";
                    }
                }
                else
                {
                    wres.status = false;
                    wres.message = "Wallet Data Not Found";
                }
                return Ok(wres);
            }
        }

        [Route("api/Ereturn/AddPanCard")]
        [HttpPost]
        public IHttpActionResult AddPanCard(TB_PanCard pmodel)
        {
            AppCommonResponce wres = new AppCommonResponce();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var TW = entities.TB_CustomerWallet.Where(e => e.RegistrationId == pmodel.RegistrationId).FirstOrDefault()?.TreadWallet;
                if(TW != null)
                {
                    if (TW >= pmodel.TotalAmount)
                    {
                        var Walletdata = entities.TB_CustomerWallet.Where(e => e.RegistrationId == pmodel.RegistrationId).SingleOrDefault();
                        if (Walletdata != null)
                        {
                            decimal Amount = Convert.ToDecimal(Walletdata.TreadWallet - pmodel.TotalAmount);
                            Walletdata.TreadWallet = Amount;
                            entities.Entry(Walletdata).State = EntityState.Modified;
                            pmodel.Date = DateTime.Now;
                            pmodel.Status = "Process";
                            entities.TB_PanCard.Add(pmodel);
                            entities.SaveChanges();
                            wres.status = true;
                            wres.message = "Send Request Successfully";
                            wres.MainWallet = Convert.ToDecimal(Walletdata.MainWallet);
                            wres.TreadWallet = Convert.ToDecimal(Walletdata.TreadWallet);
                            return Ok(wres);
                        }
                        else
                        {
                            wres.status = false;
                            wres.message = "Something Went Wrong";
                            return Ok(wres);
                        }
                    }
                    else
                    {
                        wres.status = false;
                        wres.message = "You Don't Have Sufficient Balance";
                        return Ok(wres);
                    }
                }
                else
                {
                    wres.status = false;
                    wres.message = "WalletData Not Found";
                    return Ok(wres);
                }
            }
        }

        [Route("api/Ereturn/Aepsstatus")]
        [HttpGet]
        public IHttpActionResult Aepsstatus(long RegistrationId)
        {
            RegistrationResponse response = new RegistrationResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                response.Data = entities.TB_Registration.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                if(response.Data != null)
                {
                   var OutId = entities.TB_Registration.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault()?.OutlateId;
                    if(OutId != null)
                    {
                        response.Status = true;
                        response.Message = "OutletId Already Exists";
                        return Ok(response);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "OutletId Approval Pending";
                        return Ok(response);
                    }
                }
                else
                {
                    response.Data = null;
                    response.Status = false;
                    response.Message = "Something Went Wrong";
                    return Ok(response);
                }
            }
        }

        [Route("api/Ereturn/Updateshopedetails")]
        [HttpPost]
        public IHttpActionResult Updateshopedetails(ShopDetails shop)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RegistrationResponse response = new RegistrationResponse();
                var entity = entities.TB_Registration.Find(shop.RegistrationId);
                if(entity != null)
                {
                    entity.ShopAddress1 = shop.ShopAddress1 /*httpRequest.Form["ShopAddress1"]*/;
                    entity.ShopAddress2 = shop.ShopAddress2 /*httpRequest.Form["ShopAddress2"]*/;
                    entity.ShopCategory = Convert.ToString(shop.ShopCategory) /* httpRequest.Form["ShopCategory"]*/;
                    entity.ShopPincode = shop.ShopPincode /*httpRequest.Form["ShopPincode"]*/;
                    entity.ShopPostOffice = shop.ShopPostOffice /*httpRequest.Form["ShopPostOffice"]*/;
                    entity.ShopCity = Convert.ToInt64(shop.ShopCity) /*httpRequest.Form["ShopCity"]*/;
                    entity.ShopState = Convert.ToInt64(shop.ShopState) /*httpRequest.Form["ShopState"]*/;
                    entity.ShopAreaLocality = shop.ShopAreaLocality /*httpRequest.Form["ShopAreaLocality"]*/;
                    entity.ShopName = shop.ShopName /*httpRequest.Form["ShopName"]*/;
                    entity.OutlateIdStatus = "pending";
                    if (shop.ShopPhoto != "" && shop.ShopPhoto != null && shop.ShopImageName != "" && shop.ShopImageName != null)
                    {
                        String path = HttpContext.Current.Server.MapPath("~/Content/ShopImage/");
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                        }
                        var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string imageName = shop.ShopImageName;
                        string[] str = imageName.Split('.');
                        imageName = baseUrl + "/Content/ShopImage/" + imageName;
                        string imgPath = Path.Combine(path, shop.ShopImageName);
                        byte[] imageBytes = Convert.FromBase64String(shop.ShopPhoto);
                        File.WriteAllBytes(imgPath, imageBytes);
                        entity.ShopPhoto = imageName;
                    }
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();
                    response.StateName = entities.TB_State.Where(e => e.StateId == shop.ShopState).FirstOrDefault()?.StateName;
                    response.CityName = entities.TB_City.Where(e => e.CityId == shop.ShopCity).FirstOrDefault()?.CityName;
                    response.CountryName = entities.TB_Country.Where(e => e.CountryId == 1).FirstOrDefault()?.CountryName;
                    response.Data = entity;
                    response.Status = true;
                    response.Message = "Update Shop Details Successfully";
                    if(shop.UpdateDetailsFor == "Pancard")
                    {
                        TB_PancardServiceRequest tbpsr = new TB_PancardServiceRequest();
                        tbpsr.RegistrationId = shop.RegistrationId;
                        tbpsr.RequestDate = DateTime.Now.Date;
                        tbpsr.Status = "Pending";
                        entities.TB_PancardServiceRequest.Add(tbpsr);
                        entities.SaveChanges();
                    }
                    return Ok(response);
                }
                else
                {
                    response.Data = entity;
                    response.Status = false;
                    response.Message = "Something Went Wrong";
                    return Ok(response);
                }
            }
        }

        [Route("api/Ereturn/AddBankDetails")]
        [HttpPost]
        public IHttpActionResult AddBankDetails(BankDetails Bmodel)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RegistrationResponse response = new RegistrationResponse();
                var entity = entities.TB_Registration.Find(Bmodel.RegistrationId);
                if(entity != null)
                {
                    entity.Bank_Name = Bmodel.Bank_Name;
                    entity.Bank_Address = Bmodel.Bank_Address;
                    entity.Bank_Account_Number = Bmodel.Bank_Account_Number;
                    entity.Account_Holder_Name = Bmodel.Account_Holder_Name;
                    entity.Account_Type = Bmodel.Account_Type;
                    entity.IFSC_Code = Bmodel.IFSC_Code;
                    if (Bmodel.CancelledChequePhoto != "" && Bmodel.CancelledChequePhoto != null && Bmodel.CancelledChequeName != "" && Bmodel.CancelledChequeName != null)
                    {
                        String path = HttpContext.Current.Server.MapPath("~/Content/DocumentsImages/");
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                        }
                        var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string imageName = Bmodel.CancelledChequeName;
                        string[] str = imageName.Split('.');
                        imageName = baseUrl + "/Content/DocumentsImages/" + imageName;
                        string imgPath = Path.Combine(path, Bmodel.CancelledChequeName);
                        byte[] imageBytes = Convert.FromBase64String(Bmodel.CancelledChequePhoto);
                        File.WriteAllBytes(imgPath, imageBytes);
                        entity.CancelledChequePhoto = imageName;
                        entity.CancelledChequeName = Bmodel.CancelledChequeName;
                    }
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();

                    response.Data = entity;
                    response.Status = true;
                    response.Message = "Bank Details Added Successfully";
                }
                else
                {
                    response.Data = null;
                    response.Status = false;
                    response.Message = "Data Not Found For Update";
                }
                return Ok(response);
            }
        }

        [Route("api/Ereturn/AddDocuments")]
        [HttpPost]
        public IHttpActionResult AddDocuments(DocumentsModel Dmodel)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RegistrationResponse response = new RegistrationResponse();

                var entity = entities.TB_Registration.Find(Dmodel.RegistrationId);
                entity.Pan_Number = Convert.ToString(Dmodel.Pan_Number);
                entity.Adhar_Number = Convert.ToString(Dmodel.Adhar_Number);

                //Add Images To Server
                var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                String path = HttpContext.Current.Server.MapPath("~/Content/Upload/Documents/");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                //Add Adhar Front Image
                if (Dmodel.Adhar_Front_Image != "" && Dmodel.Adhar_Front_Image != null && Dmodel.Adhar_Front_Image_Name != "" && Dmodel.Adhar_Front_Image_Name != null)
                {
                    string FrontImage = Dmodel.Adhar_Front_Image_Name;
                    FrontImage = baseUrl + "/Content/Upload/Documents/" + FrontImage;
                    string imgPathFrontImage = Path.Combine(path, Dmodel.Adhar_Front_Image_Name);
                    byte[] imageBytesFrontImage = Convert.FromBase64String(Dmodel.Adhar_Front_Image);
                    File.WriteAllBytes(imgPathFrontImage, imageBytesFrontImage);
                    entity.Adhar_Front_Image = FrontImage;
                }

                //Add Adhar Back Image
                if (Dmodel.Adhar_Back_Image != "" && Dmodel.Adhar_Back_Image != null && Dmodel.Adhar_Back_Image_Name != "" && Dmodel.Adhar_Back_Image_Name != null)
                {
                    string BackImage = Dmodel.Adhar_Back_Image_Name;
                    BackImage = baseUrl + "/Content/Upload/Documents/" + BackImage;
                    string imgPathBackImage = Path.Combine(path, Dmodel.Adhar_Back_Image_Name);
                    byte[] imageBytesBackImage = Convert.FromBase64String(Dmodel.Adhar_Back_Image);
                    File.WriteAllBytes(imgPathBackImage, imageBytesBackImage);
                    entity.Adhar_Back_Image = BackImage;
                }

                //Add Pancard Image
                if (Dmodel.Pan_Card_Image != "" && Dmodel.Pan_Card_Image != null && Dmodel.Pan_Card_Image_Name != "" && Dmodel.Pan_Card_Image_Name != null)
                {
                    string Pancard = Dmodel.Pan_Card_Image_Name;
                    Pancard = baseUrl + "/Content/Upload/Documents/" + Pancard;
                    string imgPathPancard = Path.Combine(path, Dmodel.Pan_Card_Image_Name);
                    byte[] imageBytesPancard = Convert.FromBase64String(Dmodel.Pan_Card_Image);
                    File.WriteAllBytes(imgPathPancard, imageBytesPancard);
                    entity.Pan_Card_Image = Pancard;
                }
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
                response.Data = entity;
                response.Status = true;
                response.Message = "Documents Added Successfully";
                return Ok(response);
            }
        }

        [Route("api/Ereturn/Getpanlist")]
        [HttpGet]
        public IHttpActionResult getallpanlist(long? RegistrationId,int PageNo)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_PanCard.ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var ComplaintList = entities.TB_PanCard.ToList().Skip((iPageNum - 1) * iPageSize).ToList();
                List<PanComplaintModel> PanList = new List<PanComplaintModel>();
                foreach (var item in ComplaintList)
                {
                    PanComplaintModel rdm = new PanComplaintModel();
                    rdm.Complaint = new Cmodel();
                    rdm.PanId = item.PanId;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.TotalAmount = item.TotalAmount;
                    rdm.NoOfQty = item.NoOfQty;
                    rdm.Date = item.Date;
                    rdm.ProcessBy = item.ProcessBy;
                    rdm.ProcessByCode = item.ProcessByCode;
                    rdm.Price = item.Price;
                    rdm.Status = item.Status;
                    rdm.Reason = item.Reason;
                    rdm.RegistrationId = item.RegistrationId;
                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.PanId).FirstOrDefault();
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
                    PanList.Add(rdm);
                }
                if (RegistrationId != null)
                {
                    var ResList = (from res in PanList
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PanId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in PanList
                                   select res).OrderByDescending(e => e.PanId).ToList();
                    return Ok(ResList);
                }
            }
        }

        [Route("api/Ereturn/GetpanlistByDate")]
        [HttpGet]
        public IHttpActionResult GetpanlistByDate(DateTime? fromdate,DateTime? Todate,long? RegistrationId,int PageNo)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_PanCard.ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var ComplaintList = entities.TB_PanCard.ToList().Skip((iPageNum - 1) * iPageSize).ToList();
                List<PanComplaintModel> PanList = new List<PanComplaintModel>();
                foreach (var item in ComplaintList)
                {
                    PanComplaintModel rdm = new PanComplaintModel();
                    rdm.Complaint = new Cmodel();
                    rdm.PanId = item.PanId;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.TotalAmount = item.TotalAmount;
                    rdm.NoOfQty = item.NoOfQty;
                    rdm.Date = Convert.ToDateTime(item.Date);
                    rdm.Pdate = Convert.ToDateTime(item.Date).Date;
                    rdm.ProcessBy = item.ProcessBy;
                    rdm.ProcessByCode = item.ProcessByCode;
                    rdm.Price = item.Price;
                    rdm.Status = item.Status;
                    rdm.Reason = item.Reason;
                    rdm.RegistrationId = item.RegistrationId;
                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.PanId).FirstOrDefault();
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
                    PanList.Add(rdm);
                }
                DateTime Fromtimestamp = Convert.ToDateTime(fromdate).Date;
                DateTime Totimestamp = Convert.ToDateTime(Todate).Date;
                if (fromdate != null && Todate != null && RegistrationId != null)
                {
                    var ResList = (from res in PanList
                                   where res.Pdate >= Fromtimestamp && res.Pdate <= Totimestamp && res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PanId).ToList();
                    return Ok(ResList);
                }
                else if(fromdate == null && Todate == null && RegistrationId != null)
                {
                    var ResList = (from res in PanList
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.PanId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in PanList
                                   select res).OrderByDescending(e => e.PanId).ToList();
                    return Ok(ResList);
                }
            }
        }

        [Route("api/Ereturn/PanRequest")]
        [HttpGet]
        public IHttpActionResult PanRequest(long RegistrationID)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                PanCheckModel pcm = new PanCheckModel();
                var entity = entities.TB_Registration.Where(e => e.RegistrationId == RegistrationID).FirstOrDefault();
                PancardResponse prs = new PancardResponse();
                try
                {
                    pcm.FullName = entity.FullName;
                    pcm.EmailId = entity.EmailId;
                    pcm.MobieNo = entity.MobileNo;
                    pcm.ShopName = entity.ShopName;
                    pcm.ShopAddress1 = entity.ShopAddress1;
                    pcm.ShopAddress2 = entity.ShopAddress2;
                    pcm.Adahr_Number = entity.Adhar_Number;
                    pcm.Pan_Number = entity.Pan_Number;

                    bool isNull = pcm.GetType().GetProperties()
                            .All(p => p.GetValue(pcm) != null);

                    if (isNull == true)
                    {
                        var statusentity = entities.TB_PancardServiceRequest.Where(e => e.RegistrationId == RegistrationID).FirstOrDefault();
                        if(statusentity != null)
                        {
                            if (statusentity.Status == "Active")
                            {
                                prs.Status = true;
                                prs.Message = "Profile Complete";
                                prs.LogginId = statusentity.LoginId;
                                prs.Password = statusentity.Password;
                                prs.MemberId = statusentity.MemberId;
                            }
                            else
                            {
                                prs.Status = false;
                                prs.Message = "PanCard Services Request Is Pending";
                                prs.LogginId = null;
                                prs.Password = null;
                                prs.MemberId = null;
                            }
                        }
                        else
                        {
                            TB_PancardServiceRequest tB_PancardServiceRequest = new TB_PancardServiceRequest();
                            tB_PancardServiceRequest.RegistrationId = entity.RegistrationId;
                            tB_PancardServiceRequest.RequestDate = DateTime.Now;
                            tB_PancardServiceRequest.Status = "Active";
                            tB_PancardServiceRequest.MemberId = entity.LoginId;
                            entities.TB_PancardServiceRequest.Add(tB_PancardServiceRequest);
                            entities.SaveChanges();

                            prs.Status = true;
                            prs.Message = "PanCard Services Request Sended";
                            prs.LogginId = null;
                            prs.Password = null;
                            prs.MemberId = null;

                        }
                        
                    }
                    else
                    {
                        prs.Status = false;
                        prs.Message = "Please Complete Your Profile First";
                        prs.LogginId = null;
                        prs.Password = null;
                        prs.MemberId = null;
                    }
                    return Ok(prs);
                }
                catch
                {
                    return Ok("Something Went Wrong");
                }
            }
        }

        [Route("api/Ereturn/UpdateBankDetails")]
        [HttpPost]
        public IHttpActionResult UpdateBankDetails(BankDetails Bmodel)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RegistrationResponse response = new RegistrationResponse();
                var entity = entities.TB_Registration.Find(Bmodel.RegistrationId);
                entity.Bank_Name = Bmodel.Bank_Name;
                entity.Bank_Address = Bmodel.Bank_Address;
                entity.Bank_Account_Number = Bmodel.Bank_Account_Number;
                entity.Account_Holder_Name = Bmodel.Account_Holder_Name;
                entity.Account_Type = Bmodel.Account_Type;
                entity.IFSC_Code = Bmodel.IFSC_Code;
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();

                response.Data = entity;
                response.Status = true;
                response.Message = "Bank Details Update Successfully";
                return Ok(response);
            }
        }

        [Route("api/Ereturn/MoneyTransferHistory")]
        [HttpGet]
        public IHttpActionResult MoneyTransferHistory(long RegistrationId,DateTime? Fromdate,DateTime? Todate,int PageNo)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var listcount = entities.TB_OrderMoneyTransfer.Where(e =>e.RegistrationId == RegistrationId).ToList().Count();
                int iPageSize = Convert.ToInt32(listcount / iPageNum);
                var TransferHistory = entities.TB_OrderMoneyTransfer.Where(e => e.RegistrationId == RegistrationId).ToList();
                var list = TransferHistory.Skip((iPageNum - 1) * iPageSize).ToList();
                List<OMTransfer> mTransfers = new List<OMTransfer>();

                foreach (var item in list)
                {
                    OMTransfer transfer = new OMTransfer();
                    transfer.Complaint = new Cmodel();
                    transfer.OrderTransferId = item.OrderTransferId;
                    transfer.TransferDateTime = Convert.ToDateTime(item.TransferDateTime);
                    transfer.Tdate = Convert.ToDateTime(item.TransferDateTime).Date;
                    transfer.OrderId = item.OrderId;
                    transfer.Sender = item.Sender;
                    transfer.Beneficiry = item.Beneficiry;
                    transfer.Accountno = item.Accountno;
                    transfer.Amount = item.Amount;
                    transfer.Servicecharge = item.Servicecharge;
                    transfer.retailerCharges = Convert.ToDecimal(item.retailerCharges);
                    transfer.retailerCommission = Convert.ToDecimal(item.retailerCommission);
                    transfer.Type = item.Type;
                    transfer.Status = item.Status;
                    transfer.BankRefNo = item.BankRefNo;
                    transfer.RegistrationId = item.RegistrationId;
                    transfer.OpenBalance = item.OpenBalance;
                    transfer.CloseBalance = item.CloseBalance;
                    transfer.ProcessBy = item.ProcessBy;
                    transfer.ProcessByCode = item.ProcessByCode;
                    transfer.AdminStatus = item.AdminStatus;
                    transfer.SenderId = item.SenderId;
                    transfer.RetailerId = item.RetailerId;
                    transfer.ReportId = item.ReportId;
                    transfer.Message = item.Message;
                    transfer.Client_Id = item.Client_Id;
                    transfer.Utr = item.Utr;
                    transfer.TransactionId = item.ReportId;

                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.OrderTransferId).FirstOrDefault();
                    if (Cres != null)
                    {
                        transfer.Complaint.ComplaintId = Cres.ComplaintId;
                        transfer.Complaint.ComplaintType = Cres.ComplaintType;
                        transfer.Complaint.Subject = Cres.Subject;
                        transfer.Complaint.TransactionId = Cres.TransactionId;
                        transfer.Complaint.Status = Cres.Status;
                        transfer.Complaint.Message = Cres.Message;
                        transfer.Complaint.CreatedBy = Cres.CreatedBy;
                        transfer.Complaint.CreatedDate = Cres.CreatedDate;
                        transfer.Complaint.UpdatedBy = Cres.UpdatedBy;
                        transfer.Complaint.UpdatedDate = Cres.UpdatedDate;
                        transfer.Complaint.RegistrationId = Cres.RegistrationId;
                    }
                    mTransfers.Add(transfer);
                }

                DateTime Fromtimestamp = Convert.ToDateTime(Fromdate).Date;
                DateTime Totimestamp = Convert.ToDateTime(Todate).Date;
                if (Fromdate != null && Todate != null)
                {
                    var ResList = (from res in mTransfers
                                   //where res.TransferDateTime >= Convert.ToDateTime(Fromdate).Date && res.TransferDateTime <= Convert.ToDateTime(Todate).Date && res.RegistrationId == RegistrationId
                                   where res.Tdate >= Fromtimestamp && res.Tdate <= Totimestamp && res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.OrderTransferId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in mTransfers
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.OrderTransferId).ToList();
                    return Ok(ResList);
                }
            }
        }

        [Route("api/Ereturn/MoneyTransferHistoryById")]
        [HttpGet]
        public IHttpActionResult MoneyTransferHistoryById(long RegistrationId, int PageNo)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var listcount = entities.TB_OrderMoneyTransfer.ToList().Count();
                int iPageSize = Convert.ToInt32(listcount / iPageNum);
                var list = entities.TB_OrderMoneyTransfer.ToList().Skip((iPageNum - 1) * iPageSize).ToList();
                List<OMTransfer> mTransfers = new List<OMTransfer>();

                foreach (var item in list)
                {
                    OMTransfer transfer = new OMTransfer();
                    transfer.Complaint = new Cmodel();
                    transfer.OrderTransferId = item.OrderTransferId;
                    transfer.TransferDateTime = item.TransferDateTime;
                    transfer.OrderId = item.OrderId;
                    transfer.Sender = item.Sender;
                    transfer.Beneficiry = item.Beneficiry;
                    transfer.Accountno = item.Accountno;
                    transfer.Amount = item.Amount;
                    transfer.Servicecharge = item.Servicecharge;
                    transfer.Type = item.Type;
                    transfer.Status = item.Status;
                    transfer.BankRefNo = item.BankRefNo;
                    transfer.RegistrationId = item.RegistrationId;
                    transfer.OpenBalance = item.OpenBalance;
                    transfer.CloseBalance = item.CloseBalance;
                    transfer.ProcessBy = item.ProcessBy;
                    transfer.ProcessByCode = item.ProcessByCode;
                    transfer.AdminStatus = item.AdminStatus;
                    transfer.SenderId = item.SenderId;
                    transfer.RetailerId = item.RetailerId;
                    transfer.ReportId = item.ReportId;
                    transfer.Message = item.Message;
                    transfer.Client_Id = item.Client_Id;
                    transfer.Utr = item.Utr;

                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.OrderTransferId).FirstOrDefault();
                    if (Cres != null)
                    {
                        transfer.Complaint.ComplaintId = Cres.ComplaintId;
                        transfer.Complaint.ComplaintType = Cres.ComplaintType;
                        transfer.Complaint.Subject = Cres.Subject;
                        transfer.Complaint.TransactionId = Cres.TransactionId;
                        transfer.Complaint.Status = Cres.Status;
                        transfer.Complaint.Message = Cres.Message;
                        transfer.Complaint.CreatedBy = Cres.CreatedBy;
                        transfer.Complaint.CreatedDate = Cres.CreatedDate;
                        transfer.Complaint.UpdatedBy = Cres.UpdatedBy;
                        transfer.Complaint.UpdatedDate = Cres.UpdatedDate;
                        transfer.Complaint.RegistrationId = Cres.RegistrationId;
                    }
                    mTransfers.Add(transfer);
                }

                var ResList = (from res in mTransfers
                               where res.RegistrationId == RegistrationId
                               select res).OrderByDescending(e => e.OrderTransferId).ToList();
                return Ok(ResList);
            }
        }

        // New Api After discusion on date 20/03/2021 
        [Route("api/Ereturn/AddComplaintRegister")]
        [HttpPost]
        public IHttpActionResult AddComplaintRegister(TB_ComplaintRegister _ComplaintRegister)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                _ComplaintRegister.Status = "Pending";
                _ComplaintRegister.CreatedDate = DateTime.Now;
                entities.TB_ComplaintRegister.Add(_ComplaintRegister);
                entities.SaveChanges();
                return Ok(_ComplaintRegister);
            }
        }

        [Route("api/Ereturn/UpdateComplaintStatus")]
        [HttpPost]
        public IHttpActionResult UpdateComplaintStatus(long ComplaintId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                var cmdata = entities.TB_ComplaintRegister.Where(e => e.ComplaintId == ComplaintId).FirstOrDefault();
                if(cmdata != null)
                {
                    cmdata.Status = "Success";
                    entities.Entry(cmdata).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Message = "Update Complaint Status Successfully Done";
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 400;
                    res.Message = "ComplaintId Not Matched";
                }
                return Ok(res);
            }
        }

        [Route("api/Ereturn/GetAllComplaintRegister")]
        [HttpGet]
        public IHttpActionResult GetAllComplaintRegister()
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var ComplaintList = entities.TB_ComplaintRegister.ToList();
                List<ComplaintDataModel> cdmlist = new List<ComplaintDataModel>();
                foreach (var item in ComplaintList)
                {
                    ComplaintDataModel cdm = new ComplaintDataModel();
                    cdm.Complaint = item.Subject;
                    cdm.MobileNo = entities.TB_Registration.Where(e => e.RegistrationId == item.RegistrationId).FirstOrDefault()?.MobileNo;
                    cdm.TableId = Convert.ToInt64(item.TransactionId);
                    if(item.ComplaintType == "Pan Card")
                    {
                        cdm.TransactionId = Convert.ToInt64(item.TransactionId);
                    }
                    else if(item.ComplaintType == "Money Transfer")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_OrderMoneyTransfer.Where(e => e.OrderTransferId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else if (item.ComplaintType == "Mobile Recharge")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_Recharge.Where(e => e.OrderId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else if (item.ComplaintType == "DTH Recharge")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_Recharge.Where(e => e.OrderId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else
                    {
                        cdm.TransactionId = Convert.ToInt64(item.TransactionId);
                    }
                    cdm.Status = item.Status;
                    cdmlist.Add(cdm);
                }
                return Ok(cdmlist.OrderByDescending(e => e.TransactionId));
            }
        }

        [Route("api/Ereturn/GetComplaintRegister")]
        [HttpGet]
        public IHttpActionResult GetComplaintRegister(DateTime? fromdate, DateTime? Todate,string Type,int PageNo,long RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_ComplaintRegister.Where(e => e.RegistrationId == RegistrationId).ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var Relists = entities.TB_ComplaintRegister.Where(e => e.RegistrationId == RegistrationId).ToList();
                var ComplaintList = Relists.Skip((iPageNum - 1) * iPageSize).ToList();
                List<ComplaintDataModel2> cdmlist = new List<ComplaintDataModel2>();
                foreach (var item in ComplaintList)
                {
                    ComplaintDataModel2 cdm = new ComplaintDataModel2();
                    cdm.Complaint = item.Subject;
                    cdm.MobileNo = entities.TB_Registration.Where(e => e.RegistrationId == item.RegistrationId).FirstOrDefault()?.MobileNo;
                    cdm.TableId = Convert.ToInt64(item.TransactionId);
                    cdm.ComplaintId = item.ComplaintId;
                    cdm.ComplaintType = item.ComplaintType;
                    cdm.CreatedDate = Convert.ToDateTime(item.CreatedDate);
                    cdm.Message = item.Message;
                    cdm.Subject = item.Subject;
                    cdm.RegistrationId = item.RegistrationId;
                    cdm.Cdate = Convert.ToDateTime(item.CreatedDate).Date;
                    if (item.ComplaintType == "Pan Card")
                    {
                        cdm.TransactionId = Convert.ToInt64(item.TransactionId);
                    }
                    else if (item.ComplaintType == "Money Transfer")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_OrderMoneyTransfer.Where(e => e.OrderTransferId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else if (item.ComplaintType == "Mobile Recharge")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_Recharge.Where(e => e.OrderId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else if (item.ComplaintType == "DTH Recharge")
                    {
                        cdm.TransactionId = Convert.ToInt64(entities.TB_Recharge.Where(e => e.OrderId == item.TransactionId).FirstOrDefault()?.ReportId);
                    }
                    else
                    {
                        cdm.TransactionId = Convert.ToInt64(item.TransactionId);
                    }
                    cdm.Status = item.Status;
                    cdmlist.Add(cdm);
                }
                DateTime Fromtimestamp = Convert.ToDateTime(fromdate).Date;
                DateTime Totimestamp = Convert.ToDateTime(Todate).Date;
                if (Type == "All")
                {
                    if (fromdate != null && Todate != null)
                    {
                        var ResList = (from res in cdmlist
                                       where res.Cdate >= Fromtimestamp && res.Cdate <= Totimestamp && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.ComplaintId).ToList();
                        return Ok(ResList);
                    }
                    else
                    {
                        var ResList = (from res in cdmlist
                                       where res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.ComplaintId).ToList();
                        return Ok(ResList);
                    }
                }
                else
                {
                    if (fromdate != null && Todate != null)
                    {
                        var ResList = (from res in cdmlist
                                       where res.Cdate >= Fromtimestamp && res.Cdate <= Totimestamp && res.ComplaintType == Type && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.ComplaintId).ToList();
                        return Ok(ResList);
                    }
                    else
                    {
                        var ResList = (from res in cdmlist
                                       where res.ComplaintType == Type && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.ComplaintId).ToList();
                        return Ok(ResList);
                    }
                }
            }
        }

        [Route("api/Ereturn/GetComplaintSearch")]
        [HttpGet]
        public IHttpActionResult GetComplaintSearch(string Search)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var ComplaintList = entities.TB_ComplaintRegister.ToList();
                List<ComplaintDataModel> cdmlist = new List<ComplaintDataModel>();
                foreach (var item in ComplaintList)
                {
                    ComplaintDataModel cdm = new ComplaintDataModel();
                    cdm.Complaint = item.Subject;
                    cdm.MobileNo = entities.TB_Registration.Where(e => e.RegistrationId == item.RegistrationId).FirstOrDefault()?.MobileNo;
                    cdm.TransactionId = item.TransactionId;
                    cdm.Status = item.Status;
                    cdmlist.Add(cdm);
                }
                var Resultlist =    (from ts in cdmlist
                                    where ts.MobileNo == Search || ts.Status == Search ||
                                    ts.Complaint == Search
                                    select ts).ToList();

                return Ok(Resultlist.OrderByDescending(e =>e.TransactionId));
            }
        }

        [Route("api/Ereturn/GetAllReport")]
        [HttpGet]
        public IHttpActionResult GetAllReport()
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var ComplaintList = entities.TB_Recharge.ToList();
                List<ReportDataModel> reportlist = new List<ReportDataModel>();
                foreach (var item in ComplaintList)
                {
                    ReportDataModel rdm = new ReportDataModel();
                    rdm.Id = item.OrderId;
                    rdm.PurchaseDate = item.Date;
                    rdm.Number = item.CustomerMobile;
                    rdm.Status = item.RechargeStatus;
                    rdm.Profit = Convert.ToDecimal(item.CommissionAmt);
                    rdm.UTR = item.TransactionId;
                    rdm.Type = item.RechargeType;
                    reportlist.Add(rdm);
                }
                return Ok(reportlist.OrderByDescending(e => e.Id));
            }
        }

        [Route("api/Ereturn/RechargeHistory")]
        [HttpGet]
        public IHttpActionResult RechargeHistory(DateTime? fromdate, DateTime? Todate, string Type, int PageNo,long RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_Recharge.Where(e => e.RegistrationId == RegistrationId).ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount/PageNo);
                var Relists = entities.TB_Recharge.Where(e => e.RegistrationId == RegistrationId).ToList();
                var ComplaintList = Relists.Skip((iPageNum - 1) * iPageSize).ToList(); 
                List<RechargeLists> reportlist = new List<RechargeLists>();
                foreach (var item in ComplaintList)
                {
                    RechargeLists rdm = new RechargeLists();
                    rdm.Complaint = new Cmodel();
                    rdm.OrderId = item.OrderId;
                    rdm.OrderNumber = item.OrderNumber;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.CustomerMobile = item.CustomerMobile;
                    rdm.OperatorCode = item.OperatorCode;
                    rdm.OperatorName = item.OperatorName;
                    rdm.BillAmount = item.BillAmount;
                    rdm.Date = Convert.ToDateTime(item.Date);
                    rdm.Rdate = Convert.ToDateTime(item.Date).Date;
                    rdm.RechargeAmount = item.RechargeAmount;
                    rdm.CommissionAmt = item.CommissionAmt;
                    rdm.PaymentType = item.PaymentType;
                    rdm.TransactionId = item.TransactionId;
                    rdm.RechargeType = item.RechargeType;
                    rdm.RechargeStatus = item.RechargeStatus;
                    rdm.OrderType= item.OrderType;
                    rdm.OpenBalance = item.OpenBalance;
                    rdm.CloseBalance = item.CloseBalance;

                    var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.OrderId).FirstOrDefault();
                    if(Cres != null)
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
                    reportlist.Add(rdm);
                }

                DateTime Fromtimestamp = Convert.ToDateTime(fromdate).Date;
                DateTime Totimestamp = Convert.ToDateTime(Todate).Date;
                if (Type == "All")
                {
                    if(fromdate != null && Todate != null)
                    {
                        var ResList = (from res in reportlist
                                       where res.Rdate >= Fromtimestamp && res.Rdate <= Todate && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.OrderId).ToList();
                        return Ok(ResList);
                    }
                    else
                    {
                        var ResList = (from res in reportlist
                                       where res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.OrderId).ToList();
                        return Ok(ResList);
                    }
                }
                else
                {
                    if(fromdate != null && Todate != null)
                    {
                        var ResList = (from res in reportlist
                                       where res.Rdate >= Fromtimestamp && res.Rdate <= Totimestamp && res.OrderType == Type && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.OrderId).ToList();
                        return Ok(ResList);
                    }
                    else
                    {
                        var ResList = (from res in reportlist
                                       where res.OrderType == Type && res.RegistrationId == RegistrationId
                                       select res).OrderByDescending(e => e.OrderId).ToList();
                        return Ok(ResList);
                    }
                }
            }
        }

        [Route("api/Ereturn/GetReportSearch")]
        [HttpGet]
        public IHttpActionResult GetReportSearch(string Search)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var ComplaintList = entities.TB_Recharge.ToList();
                List<ReportDataModel> reportlist = new List<ReportDataModel>();
                foreach (var item in ComplaintList)
                {
                    ReportDataModel rdm = new ReportDataModel();
                    rdm.Id = item.OrderId;
                    rdm.PurchaseDate = item.Date;
                    rdm.Number = item.CustomerMobile;
                    rdm.Status = item.RechargeStatus;
                    rdm.Profit = Convert.ToDecimal(item.CommissionAmt);
                    rdm.UTR = item.TransactionId;

                    reportlist.Add(rdm);
                }
                var Resultlist = (from ts in reportlist
                                  where ts.Number == Search || ts.Status == Search ||
                                  ts.UTR == Search
                                  select ts).ToList();
                return Ok(Resultlist.OrderByDescending(e => e.Id));
            }
        }

        [Route("api/Ereturn/GetComplaintType")]
        [HttpGet]
        public IHttpActionResult GetComplaintType()
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                return Ok(entities.TB_ComplaintType.ToList());
            }
        }

        [Route("api/Ereturn/CheckWallet")]
        [HttpGet]
        public IHttpActionResult CheckWallet(long RegistrationId,decimal TotalAmount, string Type)
        {
            WResponse wres = new WResponse();
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegistrationId).FirstOrDefault();
                if (entity != null)
                {
                    if (Type == "Money Transfer")
                    {
                        if (entity.TreadWallet > TotalAmount)
                        {
                            wres.Status = true;
                            wres.StatusCode = 200;
                            wres.Message = "You Have sufficient Balance To Complete Transaction";
                        }
                        else if (entity.TreadWallet == TotalAmount)
                        {
                            wres.Status = true;
                            wres.StatusCode = 200;
                            wres.Message = "You Have Equal And sufficient Balance To Complete Transaction";
                        }
                        else
                        {
                            wres.Status = false;
                            wres.StatusCode = 400;
                            wres.Message = "You Don't Have Enough Balance To Complete Transaction";
                        }
                    }
                    else if (Type == "Mobile Recharge" || Type == "DTH Recharge" || Type == "Gas Bill" || Type == "Light Bill")
                    {
                        if (entity.MainWallet > TotalAmount)
                        {
                            wres.Status = true;
                            wres.StatusCode = 200;
                            wres.Message = "You Have sufficient Balance To Complete Transaction";
                        }
                        else if (entity.MainWallet == TotalAmount)
                        {
                            wres.Status = true;
                            wres.StatusCode = 200;
                            wres.Message = "You Have Equal And sufficient Balance To Complete Transaction";
                        }
                        else
                        {
                            wres.Status = false;
                            wres.StatusCode = 400;
                            wres.Message = "You Don't Have Enough Balance To Complete Transaction";
                        }
                    }
                    else
                    {
                        wres.Status = false;
                        wres.StatusCode = 404;
                        wres.Message = "Service Type Miss Matched";
                    }
                }
                else
                {
                    wres.Status = false;
                    wres.StatusCode = 404;
                    wres.Message = "Error While Fetching Wallet Data";
                }
                return Ok(wres);
            }
        }

        [Route("api/Ereturn/YourEarning")]
        [HttpGet]
        public IHttpActionResult YoutEarning(long? RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                YourEarnings earnings = new YourEarnings();
                var Recharegs = entities.TB_Recharge.Where(e => e.RegistrationId == RegistrationId && (e.RechargeStatus == "Sucess" || e.RechargeStatus == "Success"));
                decimal RechargeCommision = 0;

                if(Recharegs != null)
                {
                    foreach(var item in Recharegs)
                    {
                        RechargeCommision = Convert.ToDecimal(RechargeCommision + item.CommissionAmt);
                    }

                    var Travel = 0;
                    var AEPS = 0;
                    decimal TotalEarns = Convert.ToDecimal(RechargeCommision + Travel + AEPS);
                    decimal Percentage = Convert.ToDecimal(5);
                    decimal FixedValue = 100;
                    decimal TAmount = Convert.ToDecimal((TotalEarns / FixedValue) * Percentage);
                    earnings.Recharges = Convert.ToDecimal(RechargeCommision);
                    earnings.Travels = Convert.ToDecimal(Travel);
                    earnings.AEPS = Convert.ToDecimal(AEPS);
                    earnings.TotalEarning = Convert.ToDecimal(TotalEarns);
                    earnings.TDS = Convert.ToDecimal(TAmount);
                    earnings.NetCommsions = Convert.ToDecimal(TotalEarns - TAmount);
                    return Ok(earnings);

                }
                else
                {
                    return Ok(earnings);
                }

                //var Recharegs = entities.TB_Recharge.Sum(e => e.CommissionAmt);
               
            }
        }


        [Route("api/Ereturn/CallbackAEPS")]
        [HttpPost]
        public IHttpActionResult CallbackAEPS(CallBackAeps backAeps)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                ResponseAeps res = new ResponseAeps();
                try
                {
                    var RegId = entities.TB_Registration.Where(e => e.MobileNo == backAeps.number).FirstOrDefault()?.RegistrationId;
                    if (backAeps.status_id == 1)
                    {
                        var Walletdata = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RegId).FirstOrDefault();
                        var Mwallet = Walletdata.MainWallet;
                        var Addwallet = backAeps.amount;
                        var TotalAmount = Convert.ToDecimal(Mwallet) + Convert.ToDecimal(Addwallet);
                        Walletdata.MainWallet = Convert.ToDecimal(TotalAmount);
                        entities.Entry(Walletdata).State = EntityState.Modified;
                        entities.SaveChanges();

                        TB_AEPSHistory aeps = new TB_AEPSHistory();
                        aeps.status_id = Convert.ToInt64(backAeps.status_id);
                        aeps.amount = Convert.ToDecimal(backAeps.amount);
                        aeps.utr = Convert.ToString(backAeps.utr);
                        aeps.report_id = Convert.ToString(backAeps.report_id);
                        aeps.number = Convert.ToString(backAeps.number);
                        aeps.payment_id = Convert.ToInt64(backAeps.payment_id);
                        aeps.outlet_id = Convert.ToString(backAeps.outlet_id);
                        aeps.TransactionDate = DateTime.Now;
                        aeps.RegistrationId = Convert.ToInt64(RegId);
                        aeps.DistributorCommision = 0;
                        aeps.RetailerCommision = 0;
                        entities.TB_AEPSHistory.Add(aeps);
                        entities.SaveChanges();
                    }
                    else
                    {
                        TB_AEPSHistory aeps = new TB_AEPSHistory();
                        aeps.status_id = Convert.ToInt64(backAeps.status_id);
                        aeps.amount = Convert.ToDecimal(backAeps.amount);
                        aeps.utr = Convert.ToString(backAeps.utr);
                        aeps.report_id = Convert.ToString(backAeps.report_id);
                        aeps.number = Convert.ToString(backAeps.number);
                        aeps.payment_id = Convert.ToInt64(backAeps.payment_id);
                        aeps.outlet_id = Convert.ToString(backAeps.outlet_id);
                        aeps.TransactionDate = DateTime.Now;
                        aeps.RegistrationId = Convert.ToInt64(RegId);
                        aeps.DistributorCommision = 0;
                        aeps.RetailerCommision = 0;
                        entities.TB_AEPSHistory.Add(aeps);
                        entities.SaveChanges();

                    }
                    res.Status = true;
                    res.Message = "Update AEPS Data Successfully";
                    res.StatusCode = 200;
                    return Ok(res);

                }
    
                catch(Exception e)
                {
                    res.Status = false;
                    res.Message = e.Message;
                    res.StatusCode = 404;
                    return Ok(res);
                }
            }
        }

        [Route("api/Ereturn/AEPSHistory")]
        [HttpGet]
        public IHttpActionResult AEPSHistory(DateTime? fromdate, DateTime? Todate, int PageNo, long RegistrationId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                int iPageNum = PageNo;
                var ListCount = entities.TB_AEPSHistory.Where(e => e.RegistrationId == RegistrationId).ToList().Count();
                int iPageSize = Convert.ToInt32(ListCount / PageNo);
                var Relists = entities.TB_AEPSHistory.Where(e => e.RegistrationId == RegistrationId).ToList();
                var ComplaintList = Relists.Skip((iPageNum - 1) * iPageSize).ToList();
                List<AEPSList> reportlist = new List<AEPSList>();
                foreach (var item in ComplaintList)
                {
                    AEPSList rdm = new AEPSList();
                    rdm.AepsId = item.AepsId;
                    rdm.status_id = item.status_id;
                    rdm.amount = item.amount;
                    rdm.utr = item.utr;
                    rdm.report_id = item.report_id;
                    rdm.number = item.number;
                    rdm.outlet_id = item.outlet_id;
                    rdm.payment_id = item.payment_id;
                    rdm.TransactionDate = Convert.ToDateTime(item.TransactionDate).Date;
                    rdm.RegistrationId = item.RegistrationId;
                    rdm.RetailerCommision = item.RetailerCommision;
                    rdm.DistributorCommision = item.DistributorCommision;
                    reportlist.Add(rdm);
                }
                DateTime Fromtimestamp = Convert.ToDateTime(fromdate).Date;
                DateTime Totimestamp = Convert.ToDateTime(Todate).Date;
                if (fromdate != null && Todate != null)
                {
                    var ResList = (from res in reportlist
                                   where res.TransactionDate >= Fromtimestamp && res.TransactionDate <= Totimestamp && res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.AepsId).ToList();
                    return Ok(ResList);
                }
                else
                {
                    var ResList = (from res in reportlist
                                   where res.RegistrationId == RegistrationId
                                   select res).OrderByDescending(e => e.AepsId).ToList();
                    return Ok(ResList);
                }
            }
        }

        [Route("api/Ereturn/CallbackUPI")]
        [HttpGet]
        public IHttpActionResult CallbackUPI(int status_id = 0 , string payid = ""  , string client_id = "", string operator_ref = "", string number = "", double amount =0, string virtual_account_number = "", string name = "")
        {
            string responseMessage = "";
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
               
                try 
                {
                    TB_CallBackUPI tB_CallBackUPI = new TB_CallBackUPI();
                    tB_CallBackUPI.status_id = status_id;
                    tB_CallBackUPI.payid = payid;
                    tB_CallBackUPI.client_id = client_id;
                    tB_CallBackUPI.operator_ref = operator_ref;
                    tB_CallBackUPI.number = number;
                    tB_CallBackUPI.amount = (decimal)amount;
                    tB_CallBackUPI.virtual_account_number = virtual_account_number;
                    tB_CallBackUPI.name = name;
                    entities.TB_CallBackUPI.Add(tB_CallBackUPI);
                    entities.SaveChanges();

                    responseMessage = "added successfully";

                    if(!string.IsNullOrEmpty(client_id))
                    {
                        var entity = entities.TB_UPICollect.Where(e => e.client_id.ToString() == client_id).FirstOrDefault();
                        if (entity != null)
                        {
                            var entityWallet = entities.TB_CustomerWallet.Where(e => e.RegistrationId == entity.RegistrationId).FirstOrDefault();
                            if (entityWallet != null)
                            {
                                var AddAmount = Convert.ToDecimal(amount);
                                decimal TotalAmount = Convert.ToDecimal(entityWallet.TreadWallet + AddAmount);
                                entityWallet.TreadWallet = TotalAmount;
                                entities.Entry(entityWallet).State = EntityState.Modified;
                                entities.SaveChanges();
                            }

                            entity.payment_status = "Completed";
                            entities.Entry(entity).State = EntityState.Modified;
                            entities.SaveChanges();


                            TB_BranchSendPayment tB_BranchSendPayment = new TB_BranchSendPayment();
                            tB_BranchSendPayment.PaidAmount = Convert.ToDecimal(amount);
                            tB_BranchSendPayment.PaidDate = DateTime.Now;
                            tB_BranchSendPayment.PaymentFor = "Payment";
                            tB_BranchSendPayment.PaymentMode = "UPI";
                            tB_BranchSendPayment.PaymentStatus = "Approve";
                            tB_BranchSendPayment.PaymentType = "trade";
                            tB_BranchSendPayment.ReferenceNo = entity.upi_id;
                            tB_BranchSendPayment.RegistrationId = entity.RegistrationId;
                            entities.TB_BranchSendPayment.Add(tB_BranchSendPayment);
                            entities.SaveChanges();
                        }
                    }

                    return Ok(responseMessage);
                }
                catch (Exception e)
                {
                    //responseMessage = "Not added successfully";
                    return Ok(e.Message);
                }

            }
        }

        //Add UPI request Details 
        [Route("api/Ereturn/AddUPIRequest")]
        [HttpGet]
        public IHttpActionResult AddUPIRequest(long client_id , int provider_id , string upi_id , string amount , string note,int status_id,string message , string order_id ,string payment_status,long RegistrationId)
        {
            string responseMessage = "";
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {

                try
                {
                    TB_UPICollect tB_UPICollect = new TB_UPICollect();
                    tB_UPICollect.client_id = client_id;
                    tB_UPICollect.provider_id = provider_id;
                    tB_UPICollect.upi_id = upi_id;
                    tB_UPICollect.amount = !string.IsNullOrEmpty(amount) ? Convert.ToDecimal(amount) : 0;
                    tB_UPICollect.note = note;
                    tB_UPICollect.status_id = status_id;
                    tB_UPICollect.message = message;
                    tB_UPICollect.order_id = order_id;
                    tB_UPICollect.payment_status = payment_status;
                    tB_UPICollect.RegistrationId = RegistrationId;
                    
                    entities.TB_UPICollect.Add(tB_UPICollect);
                    entities.SaveChanges();

                    responseMessage = "UPI Collect data added successfully";
                    return Ok(responseMessage);

                }
                catch (Exception e)
                {
                    //responseMessage = "Not added successfully";
                    return Ok(e.Message);
                }

            }
        }

        //Get All Distributor
        [Route("api/Ereturn/GetAllDistributor")]
        [HttpGet]
        public IHttpActionResult GetAllDistributor()
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                List<TB_Registration> distrbutors = new List<TB_Registration>();
                distrbutors = entities.TB_Registration.Where(e => e.MemberType == "Distributor").ToList();

                return Ok(distrbutors);
            }
        }


        [Route("api/Ereturn/GetPayoutChargesByAmount")]
        [HttpGet]
        public IHttpActionResult GetPayoutChargesByAmount(decimal Amount)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                TB_PayoutCharges tb_charges = new TB_PayoutCharges();
                tb_charges = entities.TB_PayoutCharges.Where(i => (decimal)i.Payout_SLABSFrom <= Amount && (decimal)i.Payout_SLABSTo >= Amount).FirstOrDefault();

                return Ok(tb_charges);
            }
        }




    }
}

