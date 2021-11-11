using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalajiInstitute.Models;
using BalajiDataAccess;
namespace BalajiInstitute.Controllers
{
    public class StateController : ApiController
    {
        [HttpGet]
        public AppResponce Get()
        {
            AppResponce res = new AppResponce();
            try
            {
                using (DB_A62CA8_myreturnEntities entities = new DB_A62CA8_myreturnEntities())
                {
                    res.status = true;
                    res.stateData = (from ec in entities.TB_State
                                     select new State
                                     {
                                         id = ec.StateId,
                                         sName = ec.StateName,
                                         Citys = (from ct in entities.TB_City.Where(e => e.StateId == ec.StateId)
                                                  select new City
                                                  {
                                                      id = ct.CityId,
                                                      name = ct.CityName,
                                                      tahsil = (from ta in entities.TB_Tahsils.Where(e => e.StateId == ec.StateId && e.CityId == ct.CityId)
                                                                select new Tahsil
                                                                {
                                                                    id = ta.TahsilsId,
                                                                    tName = ta.TahsilsName
                                                                }).ToList()
                                                  }).ToList()
                                     }).ToList();
                    return  res;
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message + ex.StackTrace;
                return res;
            }
        }
    }
}

