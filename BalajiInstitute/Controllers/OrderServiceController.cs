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
    public class OrderServiceController : ApiController
    {
        [HttpPost]
        public AppCommonResponce Post([FromBody] TB_ServiceOrder custData)
        {
            AppCommonResponce res = new AppCommonResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        var entity = entities.TB_ServiceOrder.Add(custData);
                        entities.SaveChanges();
                        res.status = true;
                        res.message = entity.OrderId.ToString();
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
        public List<TB_ServiceOrder> Get(long regId)
        {
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        return entities.TB_ServiceOrder.Where(e => e.RegistrationId == regId).ToList(); ;
                    }
                }
            }
            catch (Exception)
            {
                return new List<TB_ServiceOrder>();
            }
        }
    }
}
