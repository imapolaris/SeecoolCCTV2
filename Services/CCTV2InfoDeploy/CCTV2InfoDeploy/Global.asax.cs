using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CCTV2InfoDeploy.Models;
using CCTV2InfoDeploy.Persistence;

namespace CCTV2InfoDeploy
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();

            new Thread(() =>
            {
                Thread.Sleep(500);
                try
                {
                    PrivilegePersistence.Instance.Update(PrivilegeCategory.PreDefinedPrivileges.Privileges.ToDictionary(p => p.Name));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }
}
