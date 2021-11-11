
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiDataAccess;
using BalajiInstitute.Models;
using System.Transactions;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity;

namespace BalajiInstitute.Controllers
{
    public class UserLoginController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody] LoginRequest otpData)
        {
            LoginData login = new LoginData();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    login.Data = entities.TB_Registration.Where(e => (e.MobileNo == otpData.mobileNo || e.LoginId == otpData.mobileNo) && e.LoginPassword == otpData.password && e.MemberType == "Branch" && e.RegStatusId != 2).FirstOrDefault();
                    if (login.Data != null)
                    {
                        var walletdata = entities.TB_CustomerWallet.Where(e => e.RegistrationId == login.Data.RegistrationId).FirstOrDefault();
                        login.RegistrationId = walletdata.RegistrationId;
                        login.MainWallet = walletdata.MainWallet;
                        login.TreadWallet = walletdata.TreadWallet;
                        login.status = true;
                        login.message = "Found 1 Record";
                        login.StateName = entities.TB_State.Where(e => e.StateId == login.Data.ShopState).FirstOrDefault()?.StateName;
                        login.CityName = entities.TB_City.Where(e => e.CityId == login.Data.ShopCity).FirstOrDefault()?.CityName;
                        login.CountryName = entities.TB_Country.Where(e => e.CountryId == 1).FirstOrDefault()?.CountryName;
                        DateTime Todaydate = DateTime.Now.Date;
                        var Tdata = entities.TB_MasterToken.Where(e => e.Tid == 1).FirstOrDefault();
                        //waiting for data  get and set for this API 
                        DateTime TokenDate = Convert.ToDateTime(Tdata.LastUpdate);
                        if(TokenDate.Date != Todaydate)
                        {
                            login.AccessToken = CreateToken();
                            Tdata.LastUpdate = Todaydate;
                            Tdata.MasterToken = login.AccessToken;
                            entities.Entry(Tdata).State = EntityState.Modified;
                            entities.SaveChanges();
                        }
                        else if(Tdata == null)
                        {
                            login.AccessToken = CreateToken();
                            Tdata.LastUpdate = Todaydate;
                            Tdata.MasterToken = login.AccessToken;
                            entities.Entry(Tdata).State = EntityState.Modified;
                            entities.SaveChanges();
                        }
                        else
                        {
                            login.AccessToken = Tdata.MasterToken;
                        }
                        var entity = entities.TB_Registration.Where(e => e.MobileNo == otpData.mobileNo).FirstOrDefault();
                        Generaldetails pcm = new Generaldetails();
                        pcm.ShopName = entity.ShopName;
                        pcm.ShopAddress1 = entity.ShopAddress1;
                        pcm.ShopAddress2 = entity.ShopAddress2;
                        pcm.ShopCategory = entity.ShopCategory;
                        pcm.ShopPincode = entity.ShopPincode;
                        pcm.Adhar_Number = entity.Adhar_Number;
                        pcm.Pan_Number = entity.Pan_Number;
                        pcm.Pan_Number = entity.Pan_Number;
                        pcm.Adhar_Front_Image = entity.Adhar_Front_Image;
                        pcm.Adhar_Back_Image = entity.Adhar_Back_Image;
                        pcm.Pan_Card_Image = entity.Pan_Card_Image;
                        pcm.Bank_Name = entity.Bank_Name;
                        pcm.Bank_Account_Number = entity.Bank_Account_Number;
                        pcm.Account_Holder_Name = entity.Account_Holder_Name;
                        pcm.IFSC_Code = entity.IFSC_Code;
                        bool isNull = pcm.GetType().GetProperties()
                                            .All(p => p.GetValue(pcm) != null);
                        if (isNull != true)
                        {
                            login.Data.RegStatusId = 1;
                        }
                        return Ok(login);
                    }
                    else
                    {
                        login.status = false;
                        login.message = "Login Failed";
                        return Ok(login);
                    }
                }
            }
            catch (Exception ex)
            {
                login.status = false;
                login.message = ex.Message;
                return Ok(login);
            }
        }

        // we have created this function for careate token from tij pay2All https://api.pay2all.in/token
        private string CreateToken()
        {
            string url = "https://api.pay2all.in/token";
            using (var client = new WebClient())
            {
                Credentials tkn = new Credentials()
                {
                    email = "kpslice3@gmail.com",
                    password = "crpf@123"
                };
                string jsonSerializedModel = JsonConvert.SerializeObject(tkn); // <-- Only here you need JSON.NET to serialize your model to a JSON string
                WebRequest requestobjpost = WebRequest.Create(url);
                requestobjpost.Method = "POST";
                requestobjpost.ContentType = "application/json";
                using (var StreamWriter = new StreamWriter(requestobjpost.GetRequestStream()))
                {
                    StreamWriter.Write(jsonSerializedModel);
                    StreamWriter.Flush();
                    StreamWriter.Close();
                    var results2 = (HttpWebResponse)requestobjpost.GetResponse();
                    using (var StreamReader = new StreamReader(results2.GetResponseStream()))
                    {
                        var res = StreamReader.ReadToEnd();
                        var myUser = JsonConvert.DeserializeObject<TokenResponse>(res);
                        return myUser.access_token;
                    }
                }
            }
        }

        [HttpPut]
        public LoginResponce Put([FromBody] LoginRequest otpData)
        {
            LoginResponce res = new LoginResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    var entity = entities.TB_Registration.FirstOrDefault(e => e.MobileNo == otpData.mobileNo);
                    if (entity == null)
                    {
                        res.status = false;
                        return res;
                    }
                    else
                    {
                        entity.LoginPassword = otpData.password;
                        entities.SaveChanges();
                        res.status = true;
                        
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
    }
}
