using BalajiDataAccess;
using BalajiInstitute.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;

namespace BalajiInstitute.Controllers
{
    public class TestController : ApiController
    {
        [Route("api/Test/IntialTransaction")]
        [HttpPost]
        public IHttpActionResult IntialTransaction(TB_Recharge _Recharge)
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
                    string url = "https://api.pay2all.in/v1/payment/recharge";
                    using (var client = new WebClient())
                    {
                        Recharge tkn = new Recharge()
                        {
                            number = _Recharge.CustomerMobile,
                            amount = Convert.ToDecimal(_Recharge.BillAmount),
                            provider_id = _Recharge.OperatorCode,
                            client_id = myRandomNo
                        };

                        string jsonSerializedModel = JsonConvert.SerializeObject(tkn); // <-- Only here you need JSON.NET to serialize your model to a JSON string
                        var Tdata = entities.TB_MasterToken.Where(e => e.Tid == 1).FirstOrDefault();
                        //WebRequest requestobjpost = WebRequest.Create(url);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Accept = "application/json";
                        request.Headers.Add("Authorization", "Bearer " + "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQ4YmQwMzNhNjA2N2E5ZDQyMmQ5M2VlMmRjNmQ4YmVmNWU5ZDMyNTQzMjE4NjQ4NDI1ZTg0NGZkYzFlY2EyMDNiYmIwNzAzYmFkYTAwYWJkIn0.eyJhdWQiOiIxIiwianRpIjoiNDhiZDAzM2E2MDY3YTlkNDIyZDkzZWUyZGM2ZDhiZWY1ZTlkMzI1NDMyMTg2NDg0MjVlODQ0ZmRjMWVjYTIwM2JiYjA3MDNiYWRhMDBhYmQiLCJpYXQiOjE2MjYyNTcyOTEsIm5iZiI6MTYyNjI1NzI5MSwiZXhwIjoxNjU3NzkzMjkxLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.TnusM2ttm6_95My2K2p7HpKSDwvc96QQH2jleIwjqUiVxbxtnaYyhe4Z32os3u-vbwvpN62vL567tnbnbeFxFHpHz_dMHITgigV_LQ3FFo1t00KSbcZgout08qYcYXn5J5HAl32IqwQ9OZoq-D66KAuivm_u7EtxpR9K6wgnYU5M8NVFhPaOrqgiaLzhFz17HB47bZPn0ZFoJJGv-nnHpgi26GkJ4zplulfwl97n_IOzGYhPiZBtpB6evysA2Kn38nwU4G21ITwHiuvYioET-1Uemeh_HFHfBm0LP9Pi_qu5LzoSsAvRvK2wIMSFEXzKVo0b5MxOZdepW43EBDocMxFPA2hwXN0lkvlz6mcAW12Aiiv1SBfJERsK7-lpKbjuDirQaIkD8qUhCGWDkow4PR7mng9344jZ3syuZKqPAhEtH26Q6qUFQnQIBbEiE7Snuo7SEq8gUa4Wve_anT518gP94fimejxYC2ovoaL1J2UL4qKFBiq08DwW6PGvS8B8BSj1nVAw3TlL7gOOuvFCTXoZiAomZYVpxXmwugPu-j4_zWELLISN78fQ3-sTplqbnLsROJ4OQti1zHv1JrdqZqgaYVWQ_J5pSGxUZ757JdFcftJytDBeXTUI7DLs4cTDfcI73ilaSqZY3C_m_t3Yb3xsksT1bWWdo6t3u3y7s2c");
                        //request.Headers["Authorization"] = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQ4YmQwMzNhNjA2N2E5ZDQyMmQ5M2VlMmRjNmQ4YmVmNWU5ZDMyNTQzMjE4NjQ4NDI1ZTg0NGZkYzFlY2EyMDNiYmIwNzAzYmFkYTAwYWJkIn0.eyJhdWQiOiIxIiwianRpIjoiNDhiZDAzM2E2MDY3YTlkNDIyZDkzZWUyZGM2ZDhiZWY1ZTlkMzI1NDMyMTg2NDg0MjVlODQ0ZmRjMWVjYTIwM2JiYjA3MDNiYWRhMDBhYmQiLCJpYXQiOjE2MjYyNTcyOTEsIm5iZiI6MTYyNjI1NzI5MSwiZXhwIjoxNjU3NzkzMjkxLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.TnusM2ttm6_95My2K2p7HpKSDwvc96QQH2jleIwjqUiVxbxtnaYyhe4Z32os3u-vbwvpN62vL567tnbnbeFxFHpHz_dMHITgigV_LQ3FFo1t00KSbcZgout08qYcYXn5J5HAl32IqwQ9OZoq-D66KAuivm_u7EtxpR9K6wgnYU5M8NVFhPaOrqgiaLzhFz17HB47bZPn0ZFoJJGv-nnHpgi26GkJ4zplulfwl97n_IOzGYhPiZBtpB6evysA2Kn38nwU4G21ITwHiuvYioET-1Uemeh_HFHfBm0LP9Pi_qu5LzoSsAvRvK2wIMSFEXzKVo0b5MxOZdepW43EBDocMxFPA2hwXN0lkvlz6mcAW12Aiiv1SBfJERsK7-lpKbjuDirQaIkD8qUhCGWDkow4PR7mng9344jZ3syuZKqPAhEtH26Q6qUFQnQIBbEiE7Snuo7SEq8gUa4Wve_anT518gP94fimejxYC2ovoaL1J2UL4qKFBiq08DwW6PGvS8B8BSj1nVAw3TlL7gOOuvFCTXoZiAomZYVpxXmwugPu-j4_zWELLISN78fQ3-sTplqbnLsROJ4OQti1zHv1JrdqZqgaYVWQ_J5pSGxUZ757JdFcftJytDBeXTUI7DLs4cTDfcI73ilaSqZY3C_m_t3Yb3xsksT1bWWdo6t3u3y7s2c";
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
                                res.ClientId = Convert.ToInt64(myUser.status_id);
                                //return myUser.cftoken;
                            }
                        }
                    }
                    res.status = true;
                    res.WalletResponse = "You Have Sufficient Balance To Complete Transaction";
                    res.message = "Transaction Intialization Successfully";
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
       
        [Route("api/Money/Transfer")]
        [HttpPost]
        public IHttpActionResult Transfer(TB_OrderMoneyTransfer transfer)
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                RechargeResponse res = new RechargeResponse();
                var entity = entities.TB_CustomerWallet.Where(e => e.RegistrationId == transfer.RegistrationId).FirstOrDefault();
                decimal TotalAmount = Convert.ToDecimal(transfer.Amount);
                if (entity.MainWallet >= TotalAmount)
                {
                    string url = "https://api.pay2all.in/v1/payout/transfer";
                    using (var client = new WebClient())
                    {
                        TransferMoney tkn = new TransferMoney()
                        {
                            mobile_number = Convert.ToInt64(transfer.MobileNumber),
                            beneficiary_name = transfer.Beneficiry,
                            account_number = Convert.ToInt64(transfer.Accountno),
                            amount = Convert.ToInt16(transfer.Amount),
                            ifsc = transfer.IFSC,
                            channel_id = 2,
                            provider_id = 189,
                            client_id = Convert.ToInt64(transfer.Client_Id)
                        };

                        string jsonSerializedModel = JsonConvert.SerializeObject(tkn); // <-- Only here you need JSON.NET to serialize your model to a JSON string
                        var Tdata = entities.TB_MasterToken.Where(e => e.Tid == 1).FirstOrDefault();
                        //WebRequest requestobjpost = WebRequest.Create(url);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Accept = "application/json";
                        // request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + Tdata.MasterToken.ToString());
                        //request.Headers["Authorization"] = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQ4YmQwMzNhNjA2N2E5ZDQyMmQ5M2VlMmRjNmQ4YmVmNWU5ZDMyNTQzMjE4NjQ4NDI1ZTg0NGZkYzFlY2EyMDNiYmIwNzAzYmFkYTAwYWJkIn0.eyJhdWQiOiIxIiwianRpIjoiNDhiZDAzM2E2MDY3YTlkNDIyZDkzZWUyZGM2ZDhiZWY1ZTlkMzI1NDMyMTg2NDg0MjVlODQ0ZmRjMWVjYTIwM2JiYjA3MDNiYWRhMDBhYmQiLCJpYXQiOjE2MjYyNTcyOTEsIm5iZiI6MTYyNjI1NzI5MSwiZXhwIjoxNjU3NzkzMjkxLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.TnusM2ttm6_95My2K2p7HpKSDwvc96QQH2jleIwjqUiVxbxtnaYyhe4Z32os3u-vbwvpN62vL567tnbnbeFxFHpHz_dMHITgigV_LQ3FFo1t00KSbcZgout08qYcYXn5J5HAl32IqwQ9OZoq-D66KAuivm_u7EtxpR9K6wgnYU5M8NVFhPaOrqgiaLzhFz17HB47bZPn0ZFoJJGv-nnHpgi26GkJ4zplulfwl97n_IOzGYhPiZBtpB6evysA2Kn38nwU4G21ITwHiuvYioET-1Uemeh_HFHfBm0LP9Pi_qu5LzoSsAvRvK2wIMSFEXzKVo0b5MxOZdepW43EBDocMxFPA2hwXN0lkvlz6mcAW12Aiiv1SBfJERsK7-lpKbjuDirQaIkD8qUhCGWDkow4PR7mng9344jZ3syuZKqPAhEtH26Q6qUFQnQIBbEiE7Snuo7SEq8gUa4Wve_anT518gP94fimejxYC2ovoaL1J2UL4qKFBiq08DwW6PGvS8B8BSj1nVAw3TlL7gOOuvFCTXoZiAomZYVpxXmwugPu-j4_zWELLISN78fQ3-sTplqbnLsROJ4OQti1zHv1JrdqZqgaYVWQ_J5pSGxUZ757JdFcftJytDBeXTUI7DLs4cTDfcI73ilaSqZY3C_m_t3Yb3xsksT1bWWdo6t3u3y7s2c";
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
                                return Ok(myUser);
                            }
                        }
                    }
                }
                else
                {
                    Presponse rest = new Presponse();
                    rest = null;
                    return Ok(rest);
                }
            }
        }

        [Route("api/Money/RestTransfer")]
        [HttpPost]
        public IHttpActionResult RestTransfer()
        {
            using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
            {
                var client = new RestClient("https://api.pay2all.in/v1/payout/transfer");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6ImZhYmZmYzRjYmZkNTY0NDAyNWNhOTE4MWUzY2I4NjMyOGU4MjYyZWYyODc0MWY0ODE3MTUzYTQyOWQwZTY0MjMzODg0NmU4NTE2YTI2NmFiIn0.eyJhdWQiOiIxIiwianRpIjoiZmFiZmZjNGNiZmQ1NjQ0MDI1Y2E5MTgxZTNjYjg2MzI4ZTgyNjJlZjI4NzQxZjQ4MTcxNTNhNDI5ZDBlNjQyMzM4ODQ2ZTg1MTZhMjY2YWIiLCJpYXQiOjE2MjY4NjQxNjAsIm5iZiI6MTYyNjg2NDE2MCwiZXhwIjoxNjU4NDAwMTYwLCJzdWIiOiIxNDkiLCJzY29wZXMiOltdfQ.RZxMHS2fQ9kYRZgxQhIl-naijuKDsxb0vqnHttffTWFahrPrfOFEwPWOb1T0y54d9C9IMBU_tGaGx9_7fuib7m30KNOZwMbM6ROmtBeuAUQMskCJjfBr63jSnlZxpqGvz2l7eFfGQyjW2zHieOlc-TJGyk5AmePaPujQL4Gk7N9acEupgiyJteKJV-4bUBFvluIxAq3BF_RShWi4yqWvB_Znl8GXTPY9SQX7lOfBfQOCjyK-u9sN8qhKlwoeM8pocznZ_RKZ9boezKwd9upIdC4jZce4h7u-tFBdCjPiQwdkaStcM1vDxp0nTFc0y79Iu9W2_t8wIVlRpru6PBq2Eeiu9yw_w6lSuqXBv7KlBtBW_VIINzFMbE9uKj0Qe7oxymEJik0pN0rn5cFJLJ-upwcTQ492j7IUSr0toQe_4tEm7maHvZkk2Sq5eEIknvSk-RaOtXSFySIfTwo-_erb9Fov7GcpYH8dkVt1ZyWeK2XfsOqOedTk-_C4Txto2ghhhE9CWXRmR2pYWvLbYK6LgGNaVOTT8qtbRzfrvkwedfBFeS-xx9XORXIP-OFjTAJgDk0aGIBTEijyxR6nXjE200RUuDxbdJolJMThtGssa9jSvZviSIoMrbhoMaG3EjWLl2tzrPxDYDwi7E_8WOblawO7wZlkcF7l34n95n6NNUg");
                request.AlwaysMultipartFormData = true;
                request.AddParameter("mobile_number", "9409666888");
                request.AddParameter("amount", "100");
                request.AddParameter("beneficiary_name", "Kalpash R Patil");
                request.AddParameter("account_number", "30882825117");
                request.AddParameter("ifsc", "SBIN0000569");
                request.AddParameter("channel_id", "2");
                request.AddParameter("client_id", "786786");
                request.AddParameter("provider_id", "189");
                IRestResponse response = client.Execute(request);
                var myUser = JsonConvert.DeserializeObject<Presponse>(response.Content);
                return Ok(myUser);
            }
        }

        

    }
}
