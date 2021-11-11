using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiDataAccess;
using BalajiInstitute.Models;
using System.Transactions;
using System.Web;
using System.IO;
using System.Data.Entity;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace BalajiInstitute.Controllers
{
    public class RegistrationController : ApiController
    {
        [HttpPost]
        public AppResponce Post([FromBody] TB_Registration custData)
        {
            AppResponce res = new AppResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        //var OutlatedId = ModelsClass.GenearetOutlateId();
                        //custData.OutlateId = OutlatedId;
                        //var entity = entities.TB_Registration.Add(custData);
                        //entities.SaveChanges();
                        TB_CustomerWallet tbcw = new TB_CustomerWallet();
                        tbcw.RegistrationId = custData.RegistrationId;
                        tbcw.MainWallet = 0;
                        tbcw.TreadWallet = 0;
                        entities.TB_CustomerWallet.Add(tbcw);
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

        [Route("api/Registration/UpdateProfile")]
        [HttpPost]
        public IHttpActionResult UpdateProfile(RegistrationUpdateModel custData)
        {
            ResultResponse res = new ResultResponse();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        var entity = entities.TB_Registration.Where(e => e.RegistrationId == custData.RegistrationId).FirstOrDefault();
                        entity.CityId = custData.CityId;
                        entity.DOB = custData.DOB;
                        entity.Gender = custData.Gender;
                        entity.Pincode = custData.Pincode;
                        entity.ProfessionId = custData.ProfessionId;
                        entity.TahsilsId = custData.TahsilsId;
                        entity.StateId = custData.StateId;
                        entity.FullName = custData.FullName;
                        entity.EmailId = custData.EmailId;
                        entity.MobileNo = custData.MobileNo;

                        if (custData.ProfilePhoto != "" && custData.ProfilePhoto != null && custData.profileImageName != "" && custData.profileImageName != null)
                        {
                            String path = HttpContext.Current.Server.MapPath("~/Content/Upload/Profile/");
                            if (!System.IO.Directory.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                            }

                            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string imageName = custData.profileImageName;
                            string[] str = imageName.Split('.');
                            imageName = baseUrl + "/Content/Upload/Profile/" + imageName;
                            string imgPath = Path.Combine(path, custData.profileImageName);
                            byte[] imageBytes = Convert.FromBase64String(custData.ProfilePhoto);
                            File.WriteAllBytes(imgPath, imageBytes);
                            entity.ProfilePhoto = imageName;
                            entity.ProfileImageName = custData.profileImageName;
                        }
                        
                        entities.Entry(entity).State = EntityState.Modified;
                        entities.SaveChanges();
                        res.status = true;
                        res.message = "Update Profile Successfully Done";
                        res.FileName = entity.ProfileImageName;
                        s.Complete();
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
                return Ok(res);
            }
        }

        [Route("api/Registration/NewRegistration")]
        [HttpPost]
        public IHttpActionResult NewRegistration(NewRegistration registration)
        {
            using(DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RResponse res = new RResponse();
                var _Registration =  entities.TB_Registration.Where(e => e.MobileNo == registration.MobileNumber).FirstOrDefault();
                if(_Registration != null)
                {
                    _Registration.ShopName = registration.ShopName;
                    _Registration.ShopCategory = registration.Category;
                    _Registration.ShopAddress1 = registration.Address;
                    _Registration.ShopAddress2 = registration.Street;
                    _Registration.ShopAreaLocality = registration.Area;
                    _Registration.ShopNo = registration.ShopNo;
                    _Registration.ShopState = registration.State;
                    _Registration.ShopCity = registration.City;
                    _Registration.ShopPostOffice = registration.PostOffice;
                    _Registration.ShopPincode = registration.Pincode;
                    _Registration.FullName = registration.ContactPersoneName;
                    _Registration.MobileNo = registration.MobileNumber;
                    _Registration.LoginPassword = registration.Password;
                    _Registration.EmailId = registration.EmailId;
                    _Registration.CityId = Convert.ToInt64(registration.City);
                    _Registration.StateId = Convert.ToInt64(registration.State) ;
                    _Registration.TahsilsId = Convert.ToInt64(registration.PostOffice);
                    _Registration.Pincode = registration.Pincode;
                    _Registration.MemberType = "Branch";
                    _Registration.RegStatusId = 1;
                    entities.Entry(_Registration).State = EntityState.Modified;
                    entities.SaveChanges();
                    long Tid = Convert.ToInt64(registration.PostOffice);
                    var distCode = entities.TB_Registration.Where(e => e.TahsilsId == Tid && e.StateId == registration.State && e.CityId == registration.City && e.MemberType == "Distributor").FirstOrDefault()?.BranchCode;
                    if(distCode != null)
                    {
                        _Registration.RefCode = distCode;
                    }
                    else
                    {
                        _Registration.RefCode = "DMER547";
                    }
                    TB_CustomerWallet tbcw = new TB_CustomerWallet();
                    tbcw.RegistrationId = _Registration.RegistrationId;
                    tbcw.MainWallet = 0;
                    tbcw.TreadWallet = 0;
                    entities.TB_CustomerWallet.Add(tbcw);
                    entities.SaveChanges();

                    res.message = "Registration Successfully Done";
                    res.username = registration.ContactPersoneName;
                    res.password = registration.Password;
                    res.Status = true;
                }
                else
                {
                    res.message = "Something Went Wrong";
                    res.username = null;
                    res.password = null;
                    res.Status = false;
                }
                return Ok(res);
            }
        }

        [Route("api/Registration/VerifyOtp")]
        [HttpPost]
        public IHttpActionResult VerifyOtp(string MobileNumber, Int16 Otp)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                var Bdata = entities.TB_Registration.Where(e => (e.MobileNo == MobileNumber || e.LoginId == MobileNumber) & e.OTP == Otp).FirstOrDefault();
                if (Bdata != null)
                {
                    Bdata.RegStatusId = 1;
                    entities.Entry(Bdata).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 1;
                    res.Message = "Otp Verification Successfully Done";
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 2;
                    res.Message = "Otp Verification Failed";
                }
                return Ok(res);
            }
        }

        [Route("api/Registration/ResendOtp")]
        [HttpPost]
        public IHttpActionResult ResendOtp(string MobileNumber)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Response res = new Response();
                var _Registration = entities.TB_Registration.Where(e => e.MobileNo == MobileNumber).FirstOrDefault();
                if (_Registration != null)
                {
                    var otp = ModelsClass.GenerateRandomNo();
                    string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170&sender=MYERTN&to=" + MobileNumber + "&message=Your%20One-Time%20Password%20" + otp + "%20(OTP)%20To%20Verify%20Your%20Number%20At%20MYERTN%20-%20Do%20Not%20Share%20It%20With%20Anyone.&reqid=1&format={json|text}&pe_id=1201160310083810977&template_id=1207163107809918886&route_id=route+id&callback=Any+Callback+URL&unique=0&sendondate=25-06-2021T05:10:53";
                    //string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170&sender=MYERTN&to=" + MobileNumber + "&message=Dear%20User,%20Your%20One%20Time%20Password(OTP)%20To%20Reset%20Your%20MYERTN%20User%20Password%20Is%20" + otp + "& reqid=1&format={json|text}&pe_id=1201159170486727399&template_id=1207161779502962386&route_id=route+id&callback=Any+Callback+URL&unique=0&sendondate=20-05-2021T06:29:073";
                    // Create a request object  
                    WebRequest request = HttpWebRequest.Create(strUrl);
                    // Get the response back  
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)response.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    response.Close();
                    _Registration.OTP = otp;
                    _Registration.OTPEntryTime = DateTime.Now;
                    entities.Entry(_Registration).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.StatusCode = 200;
                    res.Otp = otp;
                    res.Message = "Otp Resend Successfully";
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = 400;
                    res.Message = "Otp Resend Process Failed";
                }
                return Ok(res);
            }
        }

        [Route("api/Registration/ForgotPassword")]
        [HttpPost]
        public IHttpActionResult ForgotPassword(string MobileNumber)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Responses res = new Responses();
                var _Registration = entities.TB_Registration.Where(e => e.MobileNo == MobileNumber || e.LoginId == MobileNumber).FirstOrDefault();
                if(_Registration != null)
                {
                    var otp = ModelsClass.GenerateRandomNo();
                    string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170%20&sender=MYRTEN&to="+ _Registration.MobileNo+ "&message=%20Dear%20User,%20Your%20One%20Time%20Password(OTP)%20to%20reset%20your%20MYERTN%20user%20password%20is%20"+otp+".&reqid=1&format={json|text}&pe_id=1401446560000025911&template_id=%201207163161380684728";
                    // Create a request object  
                    WebRequest request = HttpWebRequest.Create(strUrl);
                    // Get the response back  
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)response.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    response.Close();
                    _Registration.OTP = otp;
                    entities.Entry(_Registration).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.Message = "Verify Otp For Change Password";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Mobile Number Not Registered";
                }
                return Ok(res);
            }
        }

        [Route("api/Registration/ChangePasswoed")]
        [HttpPost]
        public IHttpActionResult ChangePasswoed(string MobileNumber,string Password)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Responses res = new Responses();
                var _Registration = entities.TB_Registration.Where(e => e.MobileNo == MobileNumber || e.LoginId == MobileNumber).FirstOrDefault();
                if (_Registration != null)
                {
                    _Registration.LoginPassword = Password;
                    entities.Entry(_Registration).State = EntityState.Modified;
                    entities.SaveChanges();
                    res.Status = true;
                    res.Message = "Password Has Been Updated Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Mobile Number Incorrect";
                }
                return Ok(res);
            }
        }

        [Route("api/Registration/SendOtp")]
        [HttpPost]
        public IHttpActionResult SendOtp(string MobileNumber,string distributor)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                Responses res = new Responses();
                var mn = entities.TB_Registration.Where(e => e.MobileNo == MobileNumber).FirstOrDefault();
                if(mn == null)
                {
                    TB_Registration _Registration = new TB_Registration();
                    var otp = ModelsClass.GenerateRandomNo();
                    string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170%20&sender=MYRTEN&to="+MobileNumber+"&message=Your%20one-time%20password%20"+otp+"%20(OTP)%20to%20verify%20your%20number%20at%20MYERTN%20-%20Do%20not%20share%20it%20with%20anyone.&reqid=1&format={json|text}&pe_id=1401446560000025911&template_id=1207163161374204502";
                    WebRequest request = HttpWebRequest.Create(strUrl);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)response.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    response.Close();
                    var Lists = entities.TB_Registration.ToList();
                    if (Lists.Count > 0)
                    {
                        var code = Lists.Last()?.BranchCode;
                        if(code == null)
                        {
                            var ids = entities.TB_Registration.Where(x => x.BranchCode.StartsWith("MER0")).Select(x => new { ID = x.BranchCode.Substring(4,20) }).ToList();
                            var maxIdValue = ids.Select(x => Int64.Parse(x.ID)).ToList().Max();
                            var Ncode2 = Convert.ToInt32(maxIdValue) + 1;
                            _Registration.LoginId = "MER0" + Ncode2;
                            _Registration.BranchCode = "MER0" + Ncode2;
                        }
                        else
                        {
                            string mystr = Regex.Replace(code, @"[a-zA-Z]", "");
                            var Ncode = Convert.ToInt32(mystr) + 1;
                            _Registration.LoginId = "MER0" + Ncode;
                            _Registration.BranchCode = "MER0" + Ncode;
                        }
                    }
                    _Registration.OTP = otp;
                    _Registration.OTPEntryTime = DateTime.Now;
                    _Registration.MobileNo = MobileNumber;
                    _Registration.RegStatusId = 3;
                    _Registration.DistributorId = distributor;
                    entities.TB_Registration.Add(_Registration);
                    entities.SaveChanges();
                }
                else
                {
                    if(mn.RegStatusId == 1)
                    {
                        res.Status = false;
                        res.Message = "Mobile Number Already Exists.";
                        return Ok(res);
                    }
                    else
                    {
                        var otp = ModelsClass.GenerateRandomNo();
                        string strUrl = "http://smsw.co.in/API/WebSMS/Http/v1.0a/index.php?username=skatmozzo&password=261dfc-28170&sender=MYERTN&to=" + MobileNumber + "&message=Your%20one-time%20password%20" + otp + "%20(OTP)%20to%20verify%20your%20number%20at%20MYERTN%20-%20Do%20not%20share%20it%20with%20anyone.%20&pe_id=1201159170486727399&template_id=1207161779515156101";
                        WebRequest request = HttpWebRequest.Create(strUrl);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)response.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        response.Close();
                        mn.OTP = otp;
                        entities.Entry(mn).State = EntityState.Modified;
                        entities.SaveChanges();
                        res.Status = true;
                        res.Message = "Otp Send Successfully On Your Mobile Number.";
                        return Ok(res);
                    }
                }
                res.Status = true;
                res.Message = "Otp Send Successfully On Your Mobile Number.";
                return Ok(res);
            }
        }
    }
}

