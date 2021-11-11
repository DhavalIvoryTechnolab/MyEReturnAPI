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
    public class IncomeFromHousePropertyController : ApiController
    {
        [HttpPost]
        public AppCommonResponce Post([FromBody] TB_HouseIncome custData)
        {
            AppCommonResponce res = new AppCommonResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_HouseIncome.Add(custData);
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
        public List<TB_HouseIncome> Get(long orderId)
        {
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        return entities.TB_HouseIncome.Where(e => e.OrderId == orderId).ToList(); ;


                    }
                }

            }
            catch (Exception)
            {
                return new List<TB_HouseIncome>();
            }
        }

        [HttpDelete]
        public AppResponce Delete(long HouseIncomeId)
        {
            AppResponce res = new AppResponce();
            try
            {

                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    using (TransactionScope s = new TransactionScope())
                    {

                        var entity = entities.TB_HouseIncome.Where(e => e.HouseIncomeId == HouseIncomeId).FirstOrDefault();
                        entities.TB_HouseIncome.Remove(entity);
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
