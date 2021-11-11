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
    public class IncomeFromBusinessController : ApiController
    {
        [HttpPost]
        public AppCommonResponce Post([FromBody] TB_BusinessIncome custData)
        {
            AppCommonResponce res = new AppCommonResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_BusinessIncome.Add(custData);
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
        public List<TB_BusinessIncome> Get(long orderId)
        {
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        return entities.TB_BusinessIncome.Where(e => e.OrderId == orderId).ToList(); ;


                    }
                }

            }
            catch (Exception)
            {
                return new List<TB_BusinessIncome>();
            }
        }

        [HttpDelete]
        public AppResponce Delete(long BusinessIncomeId)
        {
            AppResponce res = new AppResponce();
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_BusinessIncome.Where(e => e.BusinessIncomeId == BusinessIncomeId).FirstOrDefault();
                        entities.TB_BusinessIncome.Remove(entity);
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
