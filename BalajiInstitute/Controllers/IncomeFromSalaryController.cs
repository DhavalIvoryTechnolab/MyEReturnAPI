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
    public class IncomeFromSalaryController : ApiController
    {
        [HttpPost]
        public AppCommonResponce Post([FromBody] TB_SalaryIncome custData)
        {
            AppCommonResponce res = new AppCommonResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {
                        var entity = entities.TB_SalaryIncome.Add(custData);
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
        public List<TB_SalaryIncome> Get(long orderId)
        {
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        return entities.TB_SalaryIncome.Where(e => e.OrderId == orderId).ToList(); ;


                    }
                }

            }
            catch (Exception)
            {
                return new List<TB_SalaryIncome>();
            }
        }

        [HttpDelete]
        public AppResponce Delete(long SalaryIncomeId)
        {
            AppResponce res = new AppResponce();
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_SalaryIncome.Where(e => e.SalaryIncomeId == SalaryIncomeId).FirstOrDefault();
                        entities.TB_SalaryIncome.Remove(entity);
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
