using BalajiDataAccess;
using BalajiInstitute.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BalajiInstitute.Controllers
{
    public class BeneficiaryController : ApiController
    {
        [Route("api/Beneficiary/AddSender")]
        [HttpPost]
        public IHttpActionResult Addsender(TB_MoneySenderDetails senderDetails)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                var bdata = entities.TB_MoneySenderDetails.Where(e => e.MobileNumber == senderDetails.MobileNumber && e.Status == 2).FirstOrDefault();
                var otp = ModelsClass.GenerateRandomNo();
                // Create a request object  
                string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170&sender=MYERTN&to="+senderDetails.MobileNumber+"&message=Your money transfer registration otp is "+otp+ ". You will be charged 1% of  transaction amount as fee to MYERETURN.&reqid=1&format={json|text}&pe_id=1201160310083810977&template_id=1207163107800396280";
                WebRequest request = HttpWebRequest.Create(strUrl);
                // Get the response back  
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream s = (Stream)response.GetResponseStream();
                StreamReader readStream = new StreamReader(s);
                string dataString = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                if (bdata != null)
                {
                    bdata.Status = 2;
                    bdata.Otp = Convert.ToString(otp);
                    entities.Entry(bdata).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Otp = 0;
                    res.Message = "Please Verify Otp Send To Your Mobile Number";
                }
                else
                {
                    senderDetails.Status = 2;
                    senderDetails.Otp = Convert.ToString(otp);
                    entities.TB_MoneySenderDetails.Add(senderDetails);
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Otp = 0;
                    res.Message = "Please Verify Otp Send To Your Mobile Number";
                }
                return Ok(res);
            }
        }

        [Route("api/Beneficiary/VerifyOtp")]
        [HttpPost]
        public IHttpActionResult VerifyOtp(string MobileNumber,string Otp)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                var Bdata = entities.TB_MoneySenderDetails.Where(e => e.MobileNumber == MobileNumber & e.Otp == Otp).FirstOrDefault();
                if(Bdata != null)
                {
                    Bdata.Status = 1;
                    entities.Entry(Bdata).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Message = "Otp Verification Successfully Done";
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 400;
                    res.Message = "Otp Verification Failed";
                }
                return Ok(res);
            }
        }

        [Route("api/Beneficiary/Search")]
        [HttpGet]
        public IHttpActionResult SearchNumber(string MobileNumber,long? RegistrationId)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                SearchResponse sres = new SearchResponse();
                if(RegistrationId != null)
                {
                    var BTdate = entities.TB_MoneySenderDetails.Where(e => e.MobileNumber == MobileNumber && e.Status == 1 && e.RetailerId == RegistrationId).FirstOrDefault();
                    if (BTdate != null)
                    {
                        sres.Status = true;
                        sres.Message = "Found";
                        sres.SenderId = BTdate.SenderId;
                        sres.Name = BTdate.FirstName + " " + BTdate.LastName;
                        sres.MobileNumber = BTdate.MobileNumber;
                    }
                    else
                    {
                        sres.Status = false;
                        sres.Message = "Number Not Found";
                    }
                }
                else
                {
                    sres.Status = false;
                    sres.Message = "RetailerId Not Found";
                }
                return Ok(sres);
            }
        }

        [Route("api/Beneficiary/AddBeneficaryAccount")]
        [HttpPost]
        public IHttpActionResult AddBeneficaryAccount(TB_AddBeneficiary addBeneficiary)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                entities.TB_AddBeneficiary.Add(addBeneficiary);
                entities.SaveChanges();
                res.Status = true;
                res.StatusCode = 200;
                res.Message = "Beneficary Account Added Successfully";
                return Ok(res);
            }
        }

        [Route("api/Beneficiary/BeneficaryList")]
        [HttpGet]
        public IHttpActionResult BeneficaryList(long SenderId)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var blist = entities.TB_AddBeneficiary.Where(e => e.SenderId == SenderId).ToList();
                return Ok(blist);
            }
        }

        [Route("api/Beneficiary/IntialTransaction")]
        [HttpPost]
        public IHttpActionResult OrderMoneyTransfer(TB_OrderMoneyTransfer OMTmodel)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                TResponse response = new TResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == OMTmodel.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(OMTmodel.Amount);
                decimal OPBalance = 0;
                decimal CLBalance = 0;
                if (entity.TreadWallet >= TotalAmount)
                {
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    OMTmodel.TransferDateTime = DateTime.Now.Date;
                    OMTmodel.Client_Id = myRandomNo;
                    OMTmodel.Status = "Process";
                    OMTmodel.Servicecharge = OMTmodel.retailerCharges;
                    OPBalance = Convert.ToDecimal(entity.TreadWallet);
                    decimal Amount = Convert.ToDecimal(OMTmodel.Amount);
                    decimal TotalAmountT = Convert.ToDecimal(entity.TreadWallet - Amount);
                    entity.TreadWallet = Convert.ToDecimal(TotalAmountT);
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();
                    CLBalance = Convert.ToDecimal(entity.TreadWallet);
                    OMTmodel.OpenBalance = OPBalance;
                    OMTmodel.CloseBalance = CLBalance;
                    entities.TB_OrderMoneyTransfer.Add(OMTmodel);
                    entities.SaveChanges();
                    response.Status = true;
                    response.ClientId = myRandomNo;
                    response.Message = "Transaction Intialization Successfully";
                    response.WalletResponse = "You Have sufficient Balance To Complete Transaction";
                    response.MainWallet = Convert.ToDecimal(entity.MainWallet);
                    response.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.ClientId = 0;
                    response.Message = "Transaction Intialization Failed";
                    response.WalletResponse = "You Don't Have sufficient Balance To Complete Transaction";
                    response.MainWallet = Convert.ToDecimal(entity.MainWallet);
                    response.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                    return Ok(response);
                }
            }
        }

        [Route("api/Beneficiary/UpdateStatus")]
        [HttpPost]
        public IHttpActionResult UpdateStatus(UpdateTransactionStatus transactionStatus)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                MoneyTransferResponse response = new MoneyTransferResponse();
                decimal OPBalance = 0;
                decimal CLBalance = 0;
                var BTdate = entities.TB_OrderMoneyTransfer.Where(e => e.Client_Id == transactionStatus.Client_Id).FirstOrDefault();
                if(BTdate != null)
                {
                    var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == BTdate.RegistrationId).FirstOrDefault();
                    if (transactionStatus.status_id == 1)
                    {
                        BTdate.Status = "Sucess";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if (transactionStatus.status_id == 2)
                    {
                        BTdate.Status = "Failure";
                        OPBalance = Convert.ToDecimal(ent.TreadWallet);
                        decimal Amount = Convert.ToDecimal(BTdate.Amount);
                        decimal TotalAmount = Convert.ToDecimal(ent.TreadWallet + Amount);
                        ent.TreadWallet = Convert.ToDecimal(TotalAmount);
                        entities.Entry(ent).State = EntityState.Modified;
                        entities.SaveChanges();
                        CLBalance = Convert.ToDecimal(ent.TreadWallet);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if (transactionStatus.status_id == 0)
                    {
                        BTdate.Status = "Sucess";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if (transactionStatus.status_id == 3)
                    {
                        BTdate.Status = "Pending";
                        OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                        CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                        response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                    }
                    else if(transactionStatus.status_id == 4)
                    {
                        BTdate.Status = "Refund";
                        OPBalance = Convert.ToDecimal(ent.TreadWallet);
                        decimal Amount = Convert.ToDecimal(BTdate.Amount);
                        decimal TotalAmount = Convert.ToDecimal(ent.TreadWallet + Amount);
                        ent.TreadWallet = Convert.ToDecimal(TotalAmount);
                        entities.Entry(ent).State = EntityState.Modified;
                        entities.SaveChanges();
                        CLBalance = Convert.ToDecimal(ent.TreadWallet);
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
                    BTdate.OrderId = Convert.ToInt64(transactionStatus.report_id);
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

        [Route("api/Beneficiary/callbackwebhook")]
        [HttpPost]
        public IHttpActionResult callbackwebhook(Webhooks _Webhook)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                MoneyTransferResponse response = new MoneyTransferResponse();
                Response res = new Response();
                try
                {
                    decimal OPBalance = 0;
                    decimal CLBalance = 0;
                    var BTdate = entities.TB_OrderMoneyTransfer.Where(e => e.Client_Id == _Webhook.client_id).FirstOrDefault();
                    if (BTdate != null)
                    {
                        var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == BTdate.RegistrationId).FirstOrDefault();
                        if (_Webhook.status_id == 0)
                        {
                            BTdate.Status = "Sucess";
                            OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                            CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        }
                        else if (_Webhook.status_id == 1)
                        {
                            BTdate.Status = "Sucess";
                            OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                            CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        }
                        else if (_Webhook.status_id == 2)
                        {
                            BTdate.Status = "Failure";
                            OPBalance = Convert.ToDecimal(ent.TreadWallet);
                            decimal Amount = Convert.ToDecimal(BTdate.Amount);
                            var Amnt = Convert.ToDecimal(Amount);
                            decimal TotalAmount = Convert.ToDecimal(ent.TreadWallet + Amount);
                            ent.TreadWallet = Convert.ToDecimal(TotalAmount);
                            entities.Entry(ent).State = EntityState.Modified;
                            entities.SaveChanges();
                            CLBalance = Convert.ToDecimal(ent.TreadWallet);
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        }
                        else if(_Webhook.status_id == 3)
                        {
                            BTdate.Status = "Pending";
                            OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                            CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        }
                        else if(_Webhook.status_id == 4)
                        {
                            BTdate.Status = "Refund";
                            OPBalance = Convert.ToDecimal(ent.TreadWallet);
                            decimal Amount = Convert.ToDecimal(BTdate.Amount);
                            var Amnt = Convert.ToDecimal(Amount);
                            decimal TotalAmount = Convert.ToDecimal(ent.TreadWallet + Amount);
                            ent.TreadWallet = Convert.ToDecimal(TotalAmount);
                            entities.Entry(ent).State = EntityState.Modified;
                            entities.SaveChanges();
                            CLBalance = Convert.ToDecimal(ent.TreadWallet);
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                        }
                        else
                        {
                            response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                            response.Status = false;
                            response.Message = "Status Id Not Matched";
                            OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                            CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                            return Ok(response);
                        }
                        BTdate.ReportId = Convert.ToString(_Webhook.report_id);
                        BTdate.Utr = Convert.ToString(_Webhook.utr);
                        BTdate.OrderId = Convert.ToInt64(_Webhook.report_id);
                        //BTdate.Message = _Webhook.message;
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
                        response.Message = "Client_Id Not Found";
                    }
                }
                catch
                {
                    response.Status = false;
                    response.Message = "Something Went Wrong";
                }
                return Ok(response);
            }
        }

        [Route("api/Beneficiary/MoneyTransferIntial")]
        [HttpPost]
        public IHttpActionResult MoneyTransferIntial(TB_OrderMoneyTransfer OMTmodel)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                TResponse response = new TResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == OMTmodel.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(OMTmodel.Amount);
                decimal OPBalance = 0;
                decimal CLBalance = 0;
                if (entity.TreadWallet >= TotalAmount)
                {
                    Random rnd = new Random();
                    int myRandomNo = rnd.Next(10000000, 99999999);
                    OMTmodel.TransferDateTime = DateTime.Now;
                    OMTmodel.Client_Id = myRandomNo;
                    OMTmodel.Status = "Process";
                    OMTmodel.Servicecharge = OMTmodel.retailerCharges;
                    OPBalance = Convert.ToDecimal(entity.TreadWallet);
                    decimal Amount = Convert.ToDecimal(OMTmodel.Amount);
                    decimal DeductableAmount = Amount + Convert.ToDecimal(OMTmodel.retailerCharges);
                    decimal TotalAmountT = Convert.ToDecimal(entity.TreadWallet - DeductableAmount);
                    entity.TreadWallet = Convert.ToDecimal(TotalAmountT);
                    entities.Entry(entity).State = EntityState.Modified;
                    entities.SaveChanges();
                    CLBalance = Convert.ToDecimal(entity.TreadWallet);
                    OMTmodel.OpenBalance = OPBalance;
                    OMTmodel.CloseBalance = CLBalance;
                    entities.TB_OrderMoneyTransfer.Add(OMTmodel);
                    entities.SaveChanges();
                    Presponse transactionStatus = Pay2AllTransfer(OMTmodel);
                    if (transactionStatus != null)
                    {
                        var BTdate = entities.TB_OrderMoneyTransfer.Where(e => e.Client_Id == myRandomNo).FirstOrDefault();
                        if (BTdate != null)
                        {
                            var ent = entities.TB_CustomerWallet.Where(e => e.RegistrationId == BTdate.RegistrationId).FirstOrDefault();
                            if (transactionStatus.status_id == 1)
                            {
                                BTdate.Status = "Sucess";
                                OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                                CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                                response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                                response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                                response.Message = "Transfer Money Successfully Done";
                                response.Status = true;
                            }
                            else if (transactionStatus.status_id == 2)
                            {
                                BTdate.Status = "Failure";
                                OPBalance = Convert.ToDecimal(ent.TreadWallet);
                                decimal Amount2 = Convert.ToDecimal(BTdate.Amount);
                                decimal FailAmount = Amount2 + Convert.ToDecimal(OMTmodel.retailerCharges);
                                decimal TotalAmount2 = Convert.ToDecimal(ent.TreadWallet + FailAmount);
                                ent.TreadWallet = Convert.ToDecimal(TotalAmount2);
                                entities.Entry(ent).State = EntityState.Modified;
                                entities.SaveChanges();
                                CLBalance = Convert.ToDecimal(ent.TreadWallet);
                                response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                                response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                                response.Message = "Transfer Money Failed";
                                response.Status = false;

                            }

                            else if (transactionStatus.status_id == 0)
                            {
                                BTdate.Status = "Success";
                                OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                                CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                                response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                                response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                                response.Message = "Transfer Money Successfully Done";
                                response.Status = true;

                            }
                            else if (transactionStatus.status_id == 3)
                            {
                                BTdate.Status = "Pending";
                                OPBalance = Convert.ToDecimal(BTdate.OpenBalance);
                                CLBalance = Convert.ToDecimal(BTdate.CloseBalance);
                                response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                                response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                                response.Message = "Transfer Money Pending";
                                response.Status = true;

                            }
                            else if (transactionStatus.status_id == 4)
                            {
                                BTdate.Status = "Refund";
                                OPBalance = Convert.ToDecimal(ent.TreadWallet);
                                decimal Amount2 = Convert.ToDecimal(BTdate.Amount);
                                decimal RefundAmount = Amount2 + Convert.ToDecimal(OMTmodel.retailerCharges);
                                decimal TotalAmount2 = Convert.ToDecimal(ent.TreadWallet + RefundAmount);
                                ent.TreadWallet = Convert.ToDecimal(TotalAmount2);
                                entities.Entry(ent).State = EntityState.Modified;
                                entities.SaveChanges();
                                CLBalance = Convert.ToDecimal(ent.TreadWallet);
                                response.MainWallet = Convert.ToDecimal(ent.MainWallet);
                                response.TreadWallet = Convert.ToDecimal(ent.TreadWallet);
                                response.Message = "Transfer Money Process Failed Deducted Amount Will Be Refund Soon";
                                response.Status = false;

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
                            BTdate.OrderId = Convert.ToInt64(transactionStatus.report_id);
                            BTdate.Message = transactionStatus.Message;
                            BTdate.OpenBalance = OPBalance;
                            BTdate.CloseBalance = CLBalance;
                            entities.Entry(BTdate).State = EntityState.Modified;
                            entities.SaveChanges();
                            
                            response.ClientId = myRandomNo;
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Something Went Wrong";
                            response.ClientId = 0;
                            response.MainWallet = Convert.ToDecimal(entity.MainWallet);
                            response.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                        }
                        return Ok(response);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Something Went Wrong";
                        response.ClientId = 0;
                        response.MainWallet = Convert.ToDecimal(entity.MainWallet);
                        response.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                        return Ok(response);
                    }
                }
                else
                {
                    response.Status = false;
                    response.ClientId = 0;
                    response.Message = "Money Transfer Failed Due To Insufficient Balance";
                    response.WalletResponse = "You Don't Have sufficient Balance To Complete Transaction";
                    response.MainWallet = Convert.ToDecimal(entity.MainWallet);
                    response.TreadWallet = Convert.ToDecimal(entity.TreadWallet);
                    return Ok(response);
                }
            }
        }

        private Presponse Pay2AllTransfer(TB_OrderMoneyTransfer transfer)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == transfer.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(transfer.Amount);
                //if (entity.MainWallet >= TotalAmount)
                //{
                //    var client = new RestClient("https://api.pay2all.in/v1/payout/transfer");
                //    client.Timeout = -1;
                //    var request = new RestRequest(Method.POST);
                //    request.AddHeader("Accept", "application/json");
                //    string token = entities.TB_MasterToken.FirstOrDefault()?.MasterToken;
                //    request.AddHeader("Authorization", "Bearer " + token);
                //    request.AlwaysMultipartFormData = true;
                //    request.AddParameter("mobile_number", transfer.MobileNumber.ToString());
                //    request.AddParameter("amount", transfer.Amount.ToString());
                //    request.AddParameter("beneficiary_name", transfer.Beneficiry.ToString());
                //    request.AddParameter("account_number", transfer.Accountno.ToString());
                //    request.AddParameter("ifsc", transfer.IFSC.ToString());
                //    request.AddParameter("channel_id", "2");
                //    request.AddParameter("client_id", transfer.Client_Id.ToString());
                //    request.AddParameter("provider_id", "189");
                //    IRestResponse response = client.Execute(request);
                //    var myUser = JsonConvert.DeserializeObject<Presponse>(response.Content);
                //    return myUser;
                //}
                //else
                //{
                //    Presponse rest = new Presponse();
                //    rest = null;
                //    return rest;
                //}

               
                try
                {
                    var client = new RestClient("https://api.pay2all.in/v1/payout/transfer");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Accept", "application/json");
                    string token = entities.TB_MasterToken.FirstOrDefault()?.MasterToken;
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AlwaysMultipartFormData = true;
                    request.AddParameter("mobile_number", transfer.MobileNumber.ToString());
                    request.AddParameter("amount", transfer.Amount.ToString());
                    request.AddParameter("beneficiary_name", transfer.Beneficiry.ToString());
                    request.AddParameter("account_number", transfer.Accountno.ToString());
                    request.AddParameter("ifsc", transfer.IFSC.ToString());
                    request.AddParameter("channel_id", "2");
                    request.AddParameter("client_id", transfer.Client_Id.ToString());
                    request.AddParameter("provider_id", "189");
                    IRestResponse response = client.Execute(request);
                    var myUser = JsonConvert.DeserializeObject<Presponse>(response.Content);
                    return myUser;

                }
                catch (Exception e)
                {
                    Presponse presponse = new Presponse();
                    presponse.Message = e.Message;
                    presponse.status_id = 2;
                    return presponse;
                }
            }
        }
    }
}
