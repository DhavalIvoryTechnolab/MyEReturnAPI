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
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace BalajiInstitute.Controllers
{
    public class RechrageController : ApiController
    {
        [Route("api/Rechrage")]
        [HttpPost]
        public IHttpActionResult Rechrage(TB_Recharge _Recharge)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == _Recharge.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(_Recharge.RechargeAmount);
                if (entity.MainWallet >= TotalAmount)
                {
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    _Recharge.Date = DateTime.Now.Date;
                    _Recharge.Client_Id = myRandomNo;
                    _Recharge.RechargeStatus = "Process";
                    entities.TB_Recharge.Add(_Recharge);
                    entities.SaveChanges();
                    res.status = true;
                    res.WalletResponse = "You Have Sufficient Balance To Complete Transaction";
                    res.message = "Transaction Intialization Successfully";
                    res.ClientId = myRandomNo;
                    return Ok(res);
                }
                else
                {
                    res.status = false;
                    res.message = "Transaction Intialization Failed";
                    res.WalletResponse = "You Don't Have Enough Balance To Complete Transaction";
                    return Ok(res);
                }
            }
        }

        [Route("api/Rechrage/IntialTransaction")]
        [HttpPost]
        public IHttpActionResult IntialTransaction(TB_Recharge _Recharge)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == _Recharge.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(_Recharge.RechargeAmount);
                decimal OPBalance = 0;
                decimal CLBalance = 0;
                if (entity.MainWallet >= TotalAmount)
                {
                    var Lists = entities.TB_Recharge.ToList();
                    var code = Lists.Last()?.OrderNumber;
                    if(code == null)
                    {
                        _Recharge.OrderNumber = "OR" + 1001;
                    }
                    else
                    {
                        string mystr = Regex.Replace(code, @"[a-zA-Z]", "");
                        var Ncode = Convert.ToInt32(mystr) + 1;
                        _Recharge.OrderNumber = "OR" + Ncode;
                    }
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    _Recharge.Date = DateTime.Now.Date;
                    _Recharge.Client_Id = myRandomNo;
                    _Recharge.RechargeStatus = "Process";
                    OPBalance = Convert.ToDecimal(entity.MainWallet);
                    decimal Amount = Convert.ToDecimal(_Recharge.RechargeAmount);
                    decimal TotalAmountu = Convert.ToDecimal(entity.MainWallet - Amount);
                    entity.MainWallet = Convert.ToDecimal(TotalAmountu);
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();
                    CLBalance = Convert.ToDecimal(entity.MainWallet);
                    _Recharge.OpenBalance = OPBalance;
                    _Recharge.CloseBalance = CLBalance;
                    entities.TB_Recharge.Add(_Recharge);
                    entities.SaveChanges();
                    res.status = true;
                    res.WalletResponse = "You Have Sufficient Balance To Complete Transaction";
                    res.message = "Transaction Intialization Successfully";
                    res.ClientId = myRandomNo;
                    res.MainWallet = Convert.ToDecimal(entity.MainWallet);
                    res.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                    return Ok(res);
                }
                else
                {
                    res.status = false;
                    res.message = "Transaction Intialization Failed";
                    res.WalletResponse = "You Don't Have Enough Balance To Complete Transaction";
                    res.ClientId = 0;
                    res.MainWallet = Convert.ToDecimal(entity.MainWallet);
                    res.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                    return Ok(res);
                }
            }
        }

        [Route("api/Rechrage/UpdateStatus")]
        [HttpPost]
        public IHttpActionResult UpdateStatus(UpdateTransactionStatus transactionStatus)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                MoneyTransferResponse response = new MoneyTransferResponse();
                decimal OPBalance = 0;
                decimal CLBalance = 0;
                var BTdate = entities.TB_Recharge.Where(e => e.Client_Id == transactionStatus.Client_Id).FirstOrDefault();
                if(BTdate != null)
                {
                    var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == BTdate.RegistrationId).FirstOrDefault();
                    if (transactionStatus.status_id == 1)
                    {
                        BTdate.RechargeStatus = "Sucess";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if (transactionStatus.status_id == 2)
                    {
                        BTdate.RechargeStatus = "Failure";
                        OPBalance = Convert.ToDecimal(ent.MainWallet);
                        decimal Amount = Convert.ToDecimal(BTdate.RechargeAmount);
                        decimal TotalAmount = Convert.ToDecimal(ent.MainWallet + Amount);
                        ent.MainWallet = Convert.ToDecimal(TotalAmount);
                        entities.Entry(ent).State = EntityState.Modified;
                        entities.SaveChanges();
                        CLBalance = Convert.ToDecimal(ent.MainWallet);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if(transactionStatus.status_id == 3)
                    {
                        BTdate.RechargeStatus = "Pending";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if (transactionStatus.status_id == 4)
                    {
                        BTdate.RechargeStatus = "Refund";
                        OPBalance = Convert.ToDecimal(ent.MainWallet);
                        decimal Amount = Convert.ToDecimal(BTdate.RechargeAmount);
                        decimal TotalAmount = Convert.ToDecimal(ent.MainWallet + Amount);
                        ent.MainWallet = Convert.ToDecimal(TotalAmount);
                        entities.Entry(ent).State = EntityState.Modified;
                        entities.SaveChanges();
                        CLBalance = Convert.ToDecimal(ent.MainWallet);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Status Id Not Matched";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        return Ok(response);
                    }
                    BTdate.ReportId = Convert.ToString(transactionStatus.report_id);
                    BTdate.Utr = Convert.ToString(transactionStatus.utr);
                    BTdate.TransactionId = Convert.ToString(transactionStatus.report_id);
                    BTdate.Message = transactionStatus.message;
                    BTdate.OpenBalance = OPBalance;
                    BTdate.CloseBalance = CLBalance;
                    entities.Entry(BTdate).State = EntityState.Modified;
                    entities.SaveChanges();
                    response.Status = true;
                    response.Message = "Update Status Successfully";
                }
                else
                {
                    response.Status = false;
                    response.Message = "Data Not Found";
                }
                return Ok(response);
            }
        }

        [HttpGet]
        public List<RechargeLists> Get(long regId,DateTime? Fromdate,DateTime? Todate,int PageNo)
        {
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        int iPageNum = PageNo;
                        var ListCount = entities.TB_Recharge.Where(e =>e.RegistrationId == regId).ToList().Count();
                        int iPageSize = Convert.ToInt32(ListCount / PageNo);
                        var Rlists = entities.TB_Recharge.Where(e => e.RegistrationId == regId).ToList();
                        var ComplaintList = Rlists.Skip((iPageNum - 1) * iPageSize).ToList();
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
                            rdm.Date = Convert.ToDateTime(item.Date).Date;
                            rdm.RechargeAmount = item.RechargeAmount;
                            rdm.CommissionAmt = item.CommissionAmt;
                            rdm.PaymentType = item.PaymentType;
                            rdm.TransactionId = item.TransactionId;
                            rdm.RechargeType = item.RechargeType;
                            rdm.RechargeStatus = item.RechargeStatus;
                            rdm.OrderType = item.OrderType;
                            rdm.OpenBalance = item.OpenBalance;
                            rdm.CloseBalance = item.CloseBalance;

                            var Cres = entities.TB_ComplaintRegister.Where(e => e.TransactionId == item.OrderId).FirstOrDefault();
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
                            reportlist.Add(rdm);
                        }
                        if (Fromdate != null && Todate != null)
                        {
                            var ResList = (from res in reportlist
                                              where res.Date >= Convert.ToDateTime(Fromdate).Date && res.Date <= Convert.ToDateTime(Todate) && res.RegistrationId == regId
                                              select res).OrderByDescending(e => e.OrderId).ToList();
                            return ResList;

                        }
                        else
                        {
                            var ResList = (from res in reportlist
                                           where res.RegistrationId == regId
                                           select res).OrderByDescending(e => e.OrderId).ToList();
                            return ResList;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new List<RechargeLists>();
            }
        }

        [Route("api/Rechrage/RechargeIntial")]
        [HttpPost]
        public IHttpActionResult RechargeIntial(TB_Recharge _Recharge)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var typeid = 0;
                var RetilerWallet = entities.TB_CustomerWallet.Where(e => e.RegistrationId == _Recharge.RegistrationId).FirstOrDefault();
                if(RetilerWallet.MainWallet >= _Recharge.RechargeAmount)
                {
                    var Lists = entities.TB_Recharge.ToList();
                    if (Lists.Count > 0)
                    {
                        var code = Lists.Last()?.OrderNumber;
                        string mystr = Regex.Replace(code, @"[a-zA-Z]", "");
                        var Ncode = Convert.ToInt32(mystr) + 1;
                        _Recharge.OrderNumber = "OR" + Ncode;
                    }
                    else
                    {
                        _Recharge.OrderNumber = "OR1001";
                    }
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    _Recharge.Date = DateTime.Now;
                    _Recharge.Client_Id = myRandomNo;
                    _Recharge.RechargeStatus = "Process";
                    if (_Recharge.RechargeType == "Prepaid")
                    {
                        typeid = 1;
                        //For Retailer
                        var Cmsn = entities.tb_MobileRecharge_Commission.Where(e => e.ServiceName == _Recharge.OperatorName && e.PrepostId == typeid).FirstOrDefault()?.Retailer;
                        decimal Commission = Convert.ToDecimal((_Recharge.RechargeAmount * Cmsn) / 100);
                        _Recharge.RetailerCommision = Convert.ToDecimal(Commission);

                        //For Distributor
                        var CmsnDist = entities.tb_MobileRecharge_Commission.Where(e => e.ServiceName == _Recharge.OperatorName && e.PrepostId == typeid).FirstOrDefault()?.Distributor;
                        decimal CommissionDist = Convert.ToDecimal((_Recharge.RechargeAmount * CmsnDist) / 100);
                        _Recharge.DistributorCommision = Convert.ToDecimal(CommissionDist);
                    }
                    else if(_Recharge.RechargeType == "Dth")
                    {
                        //For Retailer
                        var Cmsn = entities.TB_DTHRechargeCommission.Where(e => e.ServiceName == _Recharge.OperatorName).FirstOrDefault()?.Retailer;
                        decimal Commission = Convert.ToDecimal((_Recharge.RechargeAmount * Cmsn) / 100);
                        _Recharge.RetailerCommision = Convert.ToDecimal(Commission);

                        //For Distributor
                        var CmsnDist = entities.TB_DTHRechargeCommission.Where(e => e.ServiceName == _Recharge.OperatorName).FirstOrDefault()?.Distributor;
                        decimal CommissionDist = Convert.ToDecimal((_Recharge.RechargeAmount * CmsnDist) / 100);
                        _Recharge.DistributorCommision = Convert.ToDecimal(CommissionDist);
                    }
                    else
                    {
                        typeid = 2;
                        //For Retailer
                        var Cmsn = entities.tb_MobileRecharge_Commission.Where(e => e.ServiceName == _Recharge.OperatorName && e.PrepostId == typeid).FirstOrDefault()?.Retailer;
                        decimal Commission = Convert.ToDecimal((_Recharge.RechargeAmount * Cmsn) / 100);
                        _Recharge.RetailerCommision = Convert.ToDecimal(Commission);

                        //For Distributor
                        var CmsnDist = entities.tb_MobileRecharge_Commission.Where(e => e.ServiceName == _Recharge.OperatorName && e.PrepostId == typeid).FirstOrDefault()?.Distributor;
                        decimal CommissionDist = Convert.ToDecimal((_Recharge.RechargeAmount * CmsnDist) / 100);
                        _Recharge.DistributorCommision = Convert.ToDecimal(CommissionDist);
                    }
                    _Recharge.OpenBalance = RetilerWallet.MainWallet;
                    _Recharge.CloseBalance = Convert.ToDecimal(RetilerWallet.MainWallet) - Convert.ToDecimal(_Recharge.RechargeAmount);
                    _Recharge.CloseBalance = Convert.ToDecimal(_Recharge.CloseBalance) + Convert.ToDecimal(_Recharge.RetailerCommision);
                    _Recharge.CommissionAmt = Convert.ToDecimal(_Recharge.RetailerCommision);
                    entities.TB_Recharge.Add(_Recharge);
                    entities.SaveChanges();

                    //Pay2All Api Call And Get Response
                    RechargeIntialResponse response = new RechargeIntialResponse();
                    Presponse transactionStatus = Pay2AllRecharge(_Recharge);
                    if (transactionStatus != null)
                    {
                        var RetailerData = entities.TB_Recharge.Where(e => e.Client_Id == _Recharge.Client_Id).FirstOrDefault();

                        if (transactionStatus.status_id == 0 || transactionStatus.status_id == 1 || transactionStatus.status_id == 3)
                        {
                            //Update Recharge Data
                            if(transactionStatus.status_id == 0 || transactionStatus.status_id == 1)
                            {
                                RetailerData.RechargeStatus = "Sucess";
                                RetailerData.Message = "Recharge Successfully Done";

                                RetilerWallet.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet) - Convert.ToDecimal(_Recharge.RechargeAmount);
                                RetilerWallet.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet) + Convert.ToDecimal(_Recharge.RetailerCommision);
                                entities.Entry(RetilerWallet).State = EntityState.Modified;
                                entities.SaveChanges();
                            }
                            else if(transactionStatus.status_id == 3)
                            {
                                RetailerData.RechargeStatus = "Failed";
                                RetailerData.Message = "Recharge Fail ";
                            }
                            else
                            {
                                RetailerData.RechargeStatus = "Pending";
                                RetailerData.Message = "Recharge Pending";

                                RetilerWallet.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet) - Convert.ToDecimal(_Recharge.RechargeAmount);
                                RetilerWallet.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet) + Convert.ToDecimal(_Recharge.RetailerCommision);
                                entities.Entry(RetilerWallet).State = EntityState.Modified;
                                entities.SaveChanges();
                            }

                            //Update Wallet Data
                           

                            //Update Recharge Data
                            RetailerData.Utr = Convert.ToString(transactionStatus.utr);
                            RetailerData.ReportId = Convert.ToString(transactionStatus.report_id);
                            RetailerData.TransactionId = Convert.ToString(transactionStatus.report_id);
                            entities.Entry(RetailerData).State = EntityState.Modified;
                            entities.SaveChanges();
                        }

                        else if(transactionStatus.status_id == 2)
                        {
                            RetailerData.RechargeStatus = "Failed";
                            RetailerData.Message = "Recharge Failed";
                            RetailerData.Utr = Convert.ToString(transactionStatus.utr);
                            RetailerData.ReportId = Convert.ToString(transactionStatus.report_id);
                            RetailerData.TransactionId = Convert.ToString(transactionStatus.report_id);
                            RetailerData.OpenBalance = RetailerData.CloseBalance;
                            RetailerData.CloseBalance = RetailerData.OpenBalance;
                            entities.Entry(RetailerData).State = EntityState.Modified;
                            entities.SaveChanges();
                        }

                        if(transactionStatus.status_id == 0 || transactionStatus.status_id == 1)
                            response.Status = true;
                        else
                            response.Status = false;
                        response.Message = RetailerData.Message;
                        response.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(RetilerWallet.TreadWallet);
                        return Ok(response);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Something Went Wrong";
                        response.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(RetilerWallet.TreadWallet);
                        return Ok(response);
                    }
                }
                else
                {
                    res.status = false;
                    res.message = "Recharge Failed Due To InSufficient Balance";
                    res.WalletResponse = "You Don't Have Enough Balance To Complete Transaction";
                    res.ClientId = 0;
                    res.MainWallet = Convert.ToDecimal(RetilerWallet.MainWallet);
                    res.TreadWallet = Convert.ToDecimal(RetilerWallet.TreadWallet);
                    return Ok(res);
                }
            }
        }

        private Presponse Pay2AllRecharge(TB_Recharge recharge)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == recharge.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(recharge.RechargeAmount);
                if (entity.MainWallet >= TotalAmount)
                {
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    string url = "https://api.pay2all.in/v1/payment/recharge";
                    using (var client = new WebClient())
                    {
                        Recharge tkn = new Recharge()
                        {
                            number = recharge.CustomerMobile,
                            amount = Convert.ToDecimal(recharge.BillAmount),
                            provider_id = recharge.OperatorCode,
                            client_id = recharge.Client_Id
                        };

                        string jsonSerializedModel = JsonConvert.SerializeObject(tkn); // <-- Only here you need JSON.NET to serialize your model to a JSON string
                        var Tdata = entities.TB_MasterToken.Where(e => e.Tid == 1).FirstOrDefault();
                        //WebRequest requestobjpost = WebRequest.Create(url);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Accept = "application/json";
                        request.Headers.Add("Authorization", "Bearer " + Tdata.MasterToken.ToString());
                        //request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjA2Y2U5NzExZGE2ODJhZmI4Y2M1N2EyZjEwZDBjOTIyMDRkMjMxMTljMjgxNDczNWQ3NWRmM2RmZDFmNmQzYTBmMGZmYTczYjNiNzA2ZDdlIn0.eyJhdWQiOiIxIiwianRpIjoiMDZjZTk3MTFkYTY4MmFmYjhjYzU3YTJmMTBkMGM5MjIwNGQyMzExOWMyODE0NzM1ZDc1ZGYzZGZkMWY2ZDNhMGYwZmZhNzNiM2I3MDZkN2UiLCJpYXQiOjE2MzM1ODk5MjUsIm5iZiI6MTYzMzU4OTkyNSwiZXhwIjoxNjY1MTI1OTI1LCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.elZQv_wXnEaN2dYRelzRpN5jwFG3NG-r0xWZnkeOKZWiDzgiNhkSceA5lqvnJTG8xP8NUjdmg-pxO4i_4YaSCdVhHPxpSaV0rDGBr4QCBvVG82OlrHvk_iw9tFqioWYhkKB7riChn21J5ykOpnxPbsdUvS0sZAPvF-0FSV6ybO1Lu4dHwbKKNTEGeWFj02_70b87UKyukRaF4RkV6BbBnr4z3dnNhCio19Kc70jOuP8U_dA7CC3xa8CxPyCXHMKtoPSLHxs8keKfNAaS_kPKZgju0-aVkLwlv9VnHWOG-tbq6dqj3WatPFYQd1SYuBONsfGROxmqZbFJHjaEdm1ggf_OdiWi46bJoKmKdtR7VY400bRZGRa8cOLU4HOOAsptE3xLgqr-Fw-p0ydbQKmspbrfJdrh_HX5_p68UljNT5zAAtdVJr-8POeYG6Sqt912bR5sdNpbN5FYztqxmMNqi4g9bjH5_8c8B5xDe1BgFgAUp9ovygp8vvP-Dlfb1Rpxb6odkpCqjNjD_G8x8zBBAPvuMyeVTtxqHfJErQxXhCOEavK26Pakpu29qi83JKD5taN9oJReWQAokSK1fX23V2pdYjVxPTmFY8Unbypz0hbTcJLkD3NFBM6q3hbe6MRWJgZStZX6rowJeEMA3CRWpjpN6j7oGqVYvIGQbXj4alA");
                        //request.Headers["Authorization"] = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQ4YmQwMzNhNjA2N2E5ZDQyMmQ5M2VlMmRjNmQ4YmVmNWU5ZDMyNTQzMjE4NjQ4NDI1ZTg0NGZkYzFlY2EyMDNiYmIwNzAzYmFkYTAwYWJkIn0.eyJhdWQiOiIxIiwianRpIjoiNDhiZDAzM2E2MDY3YTlkNDIyZDkzZWUyZGM2ZDhiZWY1ZTlkMzI1NDMyMTg2NDg0MjVlODQ0ZmRjMWVjYTIwM2JiYjA3MDNiYWRhMDBhYmQiLCJpYXQiOjE2MjYyNTcyOTEsIm5iZiI6MTYyNjI1NzI5MSwiZXhwIjoxNjU3NzkzMjkxLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.TnusM2ttm6_95My2K2p7HpKSDwvc96QQH2jleIwjqUiVxbxtnaYyhe4Z32os3u-vbwvpN62vL567tnbnbeFxFHpHz_dMHITgigV_LQ3FFo1t00KSbcZgout08qYcYXn5J5HAl32IqwQ9OZoq-D66KAuivm_u7EtxpR9K6wgnYU5M8NVFhPaOrqgiaLzhFz17HB47bZPn0ZFoJJGv-nnHpgi26GkJ4zplulfwl97n_IOzGYhPiZBtpB6evysA2Kn38nwU4G21ITwHiuvYioET-1Uemeh_HFHfBm0LP9Pi_qu5LzoSsAvRvK2wIMSFEXzKVo0b5MxOZdepW43EBDocMxFPA2hwXN0lkvlz6mcAW12Aiiv1SBfJERsK7-lpKbjuDirQaIkD8qUhCGWDkow4PR7mng9344jZ3syuZKqPAhEtH26Q6qUFQnQIBbEiE7Snuo7SEq8gUa4Wve_anT518gP94fimejxYC2ovoaL1J2UL4qKFBiq08DwW6PGvS8B8BSj1nVAw3TlL7gOOuvFCTXoZiAomZYVpxXmwugPu-j4_zWELLISN78fQ3-sTplqbnLsROJ4OQti1zHv1JrdqZqgaYVWQ_J5pSGxUZ757JdFcftJytDBeXTUI7DLs4cTDfcI73ilaSqZY3C_m_t3Yb3xsksT1bWWdo6t3u3y7s2c";
                        //request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQ4YmQwMzNhNjA2N2E5ZDQyMmQ5M2VlMmRjNmQ4YmVmNWU5ZDMyNTQzMjE4NjQ4NDI1ZTg0NGZkYzFlY2EyMDNiYmIwNzAzYmFkYTAwYWJkIn0.eyJhdWQiOiIxIiwianRpIjoiNDhiZDAzM2E2MDY3YTlkNDIyZDkzZWUyZGM2ZDhiZWY1ZTlkMzI1NDMyMTg2NDg0MjVlODQ0ZmRjMWVjYTIwM2JiYjA3MDNiYWRhMDBhYmQiLCJpYXQiOjE2MjYyNTcyOTEsIm5iZiI6MTYyNjI1NzI5MSwiZXhwIjoxNjU3NzkzMjkxLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.TnusM2ttm6_95My2K2p7HpKSDwvc96QQH2jleIwjqUiVxbxtnaYyhe4Z32os3u-vbwvpN62vL567tnbnbeFxFHpHz_dMHITgigV_LQ3FFo1t00KSbcZgout08qYcYXn5J5HAl32IqwQ9OZoq-D66KAuivm_u7EtxpR9K6wgnYU5M8NVFhPaOrqgiaLzhFz17HB47bZPn0ZFoJJGv-nnHpgi26GkJ4zplulfwl97n_IOzGYhPiZBtpB6evysA2Kn38nwU4G21ITwHiuvYioET-1Uemeh_HFHfBm0LP9Pi_qu5LzoSsAvRvK2wIMSFEXzKVo0b5MxOZdepW43EBDocMxFPA2hwXN0lkvlz6mcAW12Aiiv1SBfJERsK7-lpKbjuDirQaIkD8qUhCGWDkow4PR7mng9344jZ3syuZKqPAhEtH26Q6qUFQnQIBbEiE7Snuo7SEq8gUa4Wve_anT518gP94fimejxYC2ovoaL1J2UL4qKFBiq08DwW6PGvS8B8BSj1nVAw3TlL7gOOuvFCTXoZiAomZYVpxXmwugPu-j4_zWELLISN78fQ3-sTplqbnLsROJ4OQti1zHv1JrdqZqgaYVWQ_J5pSGxUZ757JdFcftJytDBeXTUI7DLs4cTDfcI73ilaSqZY3C_m_t3Yb3xsksT1bWWdo6t3u3y7s2c");
                        request.Method = "POST";
                        request.ContentType = "application/json";

                        using (var streamwriter = new StreamWriter(request.GetRequestStream()))
                        {
                            streamwriter.Write(jsonSerializedModel);
                            streamwriter.Flush();
                            streamwriter.Close();

                            var results2 = (HttpWebResponse)request.GetResponse();
                            using (var StreamReader = new StreamReader(results2.GetResponseStream()))
                            {
                                var res2 = StreamReader.ReadToEnd();
                                var myUser = JsonConvert.DeserializeObject<Presponse>(res2);
                                return myUser;
                            }
                        }
                    }
                }
                else
                {
                    Presponse rest = new Presponse();
                    rest = null;
                    return rest;
                }
            }
        }

        [Route("api/Rechrage/callbackwebhook")]
        [HttpPost]
        public IHttpActionResult callbackwebhook(Webhooks _Webhook)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                MoneyTransferResponse response = new MoneyTransferResponse();
                Response res = new Response();
                try
                {
                    var RetailerData = entities.TB_Recharge.Where(e => e.Client_Id == _Webhook.client_id).FirstOrDefault();
                    var RetailerWalletData = entities.TB_CustomerWallet.Where(e => e.RegistrationId == RetailerData.RegistrationId).FirstOrDefault();
                    if(_Webhook.status_id == 2)
                    {
                        if(RetailerData.RechargeStatus == "Pending")
                        {
                            RetailerData.RechargeStatus = "Failed";
                            RetailerData.Message = "Recharge Failed";
                            RetailerData.Utr = Convert.ToString(_Webhook.utr);
                            RetailerData.ReportId = Convert.ToString(_Webhook.report_id);
                            RetailerData.TransactionId = Convert.ToString(_Webhook.report_id);
                            RetailerData.OpenBalance = RetailerData.CloseBalance;
                            RetailerData.CloseBalance = RetailerData.OpenBalance;
                            entities.Entry(RetailerData).State = EntityState.Modified;
                            entities.SaveChanges();

                            //Update Wallet Data
                            RetailerWalletData.MainWallet = Convert.ToDecimal(RetailerWalletData.MainWallet) + Convert.ToDecimal(RetailerData.RechargeAmount);
                            RetailerWalletData.MainWallet = Convert.ToDecimal(RetailerWalletData.MainWallet) - Convert.ToDecimal(RetailerData.RetailerCommision);
                            entities.Entry(RetailerWalletData).State = EntityState.Modified;
                            entities.SaveChanges();
                        }
                        else
                        {
                            RetailerData.RechargeStatus = "Failed";
                            RetailerData.Message = "Recharge Failed";
                            RetailerData.Utr = Convert.ToString(_Webhook.utr);
                            RetailerData.ReportId = Convert.ToString(_Webhook.report_id);
                            RetailerData.TransactionId = Convert.ToString(_Webhook.report_id);
                            RetailerData.OpenBalance = RetailerData.CloseBalance;
                            RetailerData.CloseBalance = RetailerData.OpenBalance;
                            entities.Entry(RetailerData).State = EntityState.Modified;
                            entities.SaveChanges();
                        }
                    }
                    if (_Webhook.status_id == 1 || _Webhook.status_id == 0)
                    {
                        RetailerData.RechargeStatus = "Success";
                        RetailerData.Message = "Recharge Successfully Done";
                    }
                    else
                    {
                        RetailerData.RechargeStatus = "Pending";
                        RetailerData.Message = "Recharge Pending";
                    }
                    RetailerData.Utr = Convert.ToString(_Webhook.utr);
                    RetailerData.ReportId = Convert.ToString(_Webhook.report_id);
                    RetailerData.TransactionId = Convert.ToString(_Webhook.report_id);
                    RetailerData.OpenBalance = RetailerData.OpenBalance;
                    RetailerData.CloseBalance = RetailerData.CloseBalance;
                    entities.Entry(RetailerData).State = EntityState.Modified;
                    entities.SaveChanges();
                    return Ok("Update Status Successfully");
                }
                catch
                {
                    response.Status = false;
                    response.Message = "Something Went Wrong";
                }
                return Ok(response);
            }
        }

    }
}
