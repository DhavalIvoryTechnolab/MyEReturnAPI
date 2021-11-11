using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiDataAccess;
using BalajiInstitute.Models;
using System.IO;
namespace BalajiInstitute.Controllers
{
    public class GetOtpController : ApiController
    {
        [HttpPost]
        public OTPResponce Post([FromBody] MobileOTP otpData)
        {
            OTPResponce res = new OTPResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    var entity = entities.TB_Registration.FirstOrDefault(e => e.MobileNo == otpData.mobileNo);
                    if (entity != null)
                    {
                        var otp = ModelsClass.GenerateRandomNo();
                        string strUrl = "https://api.textlocal.in/send/?apikey=SP11duO1MmE-tlz20pcUVJserYspys44zyVB9MKNVB&sender=MYERTN&numbers=" + otpData.mobileNo + "&message=" + otp + " is your verification code Code valid for 15 minutes only. One time use. Please do not share this OTP with anyone to ensure account's security.";
                        // Create a request object  
                        WebRequest request = HttpWebRequest.Create(strUrl);
                        // Get the response back  
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)response.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        response.Close();
                        s.Close();
                        readStream.Close();
                        res.status = true;
                        res.otpNumber = otp;
                        return res;
                    }
                    else
                    {
                        res.status = false;
                        res.message = "Mobile number not exist";
                        return res;
                    }
                    // return  res;
                }
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
