using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Models;
using CCTV2InfoDeploy.Persistence;
using CCTVModels.User;

namespace CCTV2InfoDeploy.Controllers
{
    public class PrivilegeController : ApiController
    {
        // GET: api/Privilege
        public IEnumerable<CCTVPrivilege> Get()
        {
            return PrivilegePersistence.Instance.GetAllInfos();
        }
    }
}
