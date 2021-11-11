using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiDataAccess;
using BalajiInstitute.Models;
using System.Transactions;
namespace BalajiInstitute.Controllers
{
    public class BankDetailsController : ApiController
    {
        [HttpPost]
        public AppCommonResponce Post([FromBody] TB_ServiceOrderBank custData)
        {
            AppCommonResponce res = new AppCommonResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_ServiceOrderBank.Add(custData);
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

        [HttpGet]
        public List<TB_ServiceOrderBank> Get(long orderId)
        {
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        return entities.TB_ServiceOrderBank.Where(e => e.OrderId == orderId).ToList(); ;


                    }
                }

            }
            catch (Exception)
            {
                return new List<TB_ServiceOrderBank>();
            }
        }

        [HttpDelete]
        public AppResponce Delete(long BankId)
        {
            AppResponce res = new AppResponce();
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_ServiceOrderBank.Where(e => e.BankId == BankId).FirstOrDefault();
                        entities.TB_ServiceOrderBank.Remove(entity);
                        entities.SaveChanges();
                        s.Complete();
                        res.status = true;

                    }
                }

            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
    }
}
