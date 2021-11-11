using BalajiDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BalajiInstitute.Models
{
    public class ModelsClass
    {
        public static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public static string GenearetOutlateId()
        {
            //int _min = 100000;
            //int _max = 999999;
            Random random = new Random();
            //return _rdm.Next(_min, _max);

            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomstring = Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray();
            string outid = new string(randomstring);

            return outid;

        }

        public static string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }

        public static string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (System.Collections.DictionaryEntry key in data)
            {

                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }


            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

    }

    public class PayMoneyReq
    {
        public string payAmount { get; set; }
        public long regId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string distributorCode { get; set; }
    }

    public class PayMoneyResponce
    {
        public string key { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string productinfo { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string surl { get; set; }
        public string furl { get; set; }
        public string hash { get; set; }
        public string service_provider { get; set; }
    }

    public class LoginData
    {
        public TB_Registration Data { get; set; }
        //public string MobileNo { get; set; }
        //public string EmailId { get; set; }
        //public string LoginPassword { get; set; }
        //public string FullName { get; set; }
        //public long WalletId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public Nullable<decimal> MainWallet { get; set; }
        public Nullable<decimal> TreadWallet { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string AccessToken { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
    }

    public class AppResponce
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string FileName { get; set; }
        public List<State> stateData { get; set; }
    }

    public class ResultResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string FileName { get; set; }
    }

    public class AppCommonResponce
    {
        public bool status { get; set; }
        public string message { get; set; }
        public  decimal MainWallet { get; set; }
        public  decimal TreadWallet { get; set; }
    }

    public class City
    {
        public long id { get; set; }
        public string name { get; set; }
        public List<Tahsil> tahsil { get; set; }
    }

    public class State
    {
        public long id { get; set; }
        public string sName { get; set; }
        public List<City> Citys { get; set; }
    }

    public class Tahsil
    {
        public long id { get; set; }
        public string tName { get; set; }

    }

    public class MobileOTP
    {
        public string mobileNo { get; set; }
    }

    public class LoginRequest
    {
        public string mobileNo { get; set; }
        public string password { get; set; }
    }

    public class OTPResponce
    {
        public long otpNumber { get; set; }
        public bool status { get; set; }
        public string memberType { get; set; }
        public string message { get; set; }
    }

    public class LoginResponce
    {
        public bool status { get; set; }
        public TB_Registration data { get; set; }
        public string message { get; set; }
    }

    public class PayentModel
    {
        public long PaymentId { get; set; }
        public long RegistrationId { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentFor { get; set; }
        public decimal PaidAmount { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public System.DateTime PaidDate { get; set; }
        public string UploadFile { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
    }

    public class WalletResponse
    {
        public long WalletId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public Nullable<decimal> MainWallet { get; set; }
        public Nullable<decimal> TreadWallet { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class Credentials
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
    }

    public class OMTransfer
    {
        public long? OrderTransferId { get; set; }
        public Nullable<System.DateTime> TransferDateTime { get; set; }
        public long? OrderId { get; set; }
        public string Sender { get; set; }
        public string Beneficiry { get; set; }
        public string Accountno { get; set; }
        public string Amount { get; set; }
        public Nullable<decimal> Servicecharge { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string BankRefNo { get; set; }
        public long? RegistrationId { get; set; }
        public Nullable<decimal> OpenBalance { get; set; }
        public Nullable<decimal> CloseBalance { get; set; }
        public string ProcessBy { get; set; }
        public string ProcessByCode { get; set; }
        public string AdminStatus { get; set; }
        public long? SenderId { get; set; }
        public long? RetailerId { get; set; }
        public string ReportId { get; set; }
        public string Message { get; set; }
        public long? Client_Id { get; set; }
        public string Utr { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal retailerCharges { get; set; }
        public decimal retailerCommission { get; set; }
        public Cmodel Complaint { get; set; }
        public string TransactionId { get; set; }
        public DateTime Tdate { get; set; }
    }

    public class Cmodel
    {
        public long? ComplaintId { get; set; }
        public string ComplaintType { get; set; }
        public string Subject { get; set; }
        public long? TransactionId { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public long? RegistrationId { get; set; }
    }

    public class RechargeLists
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public string CustomerMobile { get; set; }
        public Nullable<long> OperatorCode { get; set; }
        public string OperatorName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> RechargeAmount { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<decimal> BillAmount { get; set; }
        public string PaymentType { get; set; }
        public string TransactionId { get; set; }
        public string RechargeType { get; set; }
        public string RechargeStatus { get; set; }
        public string OrderType { get; set; }
        public Nullable<decimal> OpenBalance { get; set; }
        public Nullable<decimal> CloseBalance { get; set; }
        public DateTime Rdate { get; set; }

        public Cmodel Complaint { get; set; }
    }

    public class PanComplaintModel
    {
        public long PanId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string NoOfQty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string ProcessBy { get; set; }
        public string ProcessByCode { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime Pdate { get; set; }
        public Cmodel Complaint { get; set; }
    }

    public class AddMoneyComplaintModel
    {
        public long PaymentId { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentFor { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public string UploadFile { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public string RejectReason { get; set; }
        public string PaymentBy { get; set; }
        public string PaymentByCode { get; set; }

        public Cmodel Complaint { get; set; }

    }

    public class WResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public long StatusCode { get; set; }
    }

    public class RechargeResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public long ClientId { get; set; }
        public string WalletResponse { get; set; }
        public decimal MainWallet { get; set; }
        public decimal TreadWallet { get; set; }
    }

    public class AEPSList
    {
        public long AepsId { get; set; }
        public long status_id { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string utr { get; set; }
        public string report_id { get; set; }
        public string number { get; set; }
        public Nullable<long> payment_id { get; set; }
        public string outlet_id { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<long> RegistrationId { get; set; }
        public Nullable<decimal> DistributorCommision { get; set; }
        public Nullable<decimal> RetailerCommision { get; set; }
    }
}