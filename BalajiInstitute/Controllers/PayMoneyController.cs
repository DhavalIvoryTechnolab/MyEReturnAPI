using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using BalajiInstitute.Models;

namespace BalajiInstitute.Controllers
{
    public class PayMoneyController : ApiController
    {
        public string  Post([FromBody] PayMoneyReq req)
        {
          //  int i = objBLRegistration.SaveRegistrationPayment(regId, "Pay u Money", "Registration Fee", Convert.ToDecimal(payAmount), "", "", DateTime.UtcNow, "", "Pending", distributorCode);

          //  int update = objBLRegistration.UpdateRegStatus(regId, 8);

                  Guid orderNo = Guid.NewGuid();

                // decimal totAmt = 1;
              //  decimal totAmt = req.payAmount;

                Random rnd = new Random();
                string strHash = ModelsClass.Generatehash512(rnd.ToString() + DateTime.Now);
                string txnid1 = strHash.ToString().Substring(0, 20);

                string key1 = ConfigurationManager.AppSettings["MERCHANT_KEY"];
                string salt = ConfigurationManager.AppSettings["SALT"];

                string txnid = txnid1;
                string remoteUrl = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";

                string hash_string = string.Empty;

                hash_string = key1 + "|" + txnid + "|" + req.payAmount + "|" + req.regId.ToString() + "|" + req.name + "|" + req.email + "|||||||||||" + salt;
                string hash1 = ModelsClass.Generatehash512(hash_string).ToLower();

                System.Collections.Hashtable collections = new System.Collections.Hashtable();

            //NameValueCollection collections = new NameValueCollection();

            collections.Add("key", key1);
            collections.Add("txnid", txnid);
            collections.Add("amount", req.payAmount);
            collections.Add("productinfo", req.regId.ToString());
            collections.Add("firstname", req.name);
            collections.Add("email", req.email);
            collections.Add("phone", req.mobile);
            collections.Add("surl", ConfigurationManager.AppSettings["surl"]);
            collections.Add("furl", ConfigurationManager.AppSettings["furl"]);
            collections.Add("hash", hash1);
            collections.Add("service_provider", ConfigurationManager.AppSettings["provider"]);

            string strForm = ModelsClass.PreparePOSTForm(remoteUrl, collections);

            //NameValueCollection collections = new NameValueCollection();
            /*  PayMoneyResponce res = new PayMoneyResponce();

              res.key =  key1;
              res.txnid = txnid;
              res.amount = req.payAmount;
              res.productinfo = req.regId.ToString();
              res.firstname = req.name;
              res.email=req.email;
              res.phone =  req.mobile;
              res.surl = ConfigurationManager.AppSettings["surl"];
              res.furl = ConfigurationManager.AppSettings["furl"];
              res.hash = hash1;
              res.service_provider = ConfigurationManager.AppSettings["provider"];*/

            // string strForm = PreparePOSTForm(remoteUrl, collections);

            return strForm;
               // Page.Controls.Add(new LiteralControl(strForm));
            
        }
    }
}


