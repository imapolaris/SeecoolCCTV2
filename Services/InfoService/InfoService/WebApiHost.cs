using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace InfoService
{
    public class WebApiHost : IDisposable
    {
        ILog _log { get { return LogManager.GetLogger(GetType()); } }

        HttpSelfHostServer _server;
        string _baseAddress;

        public WebApiHost()
        {
            string webApiPort = ConfigurationManager.AppSettings["WebApiPort"];
            _baseAddress = $"http://0.0.0.0:{webApiPort}/";
            var config = new HttpSelfHostConfiguration(_baseAddress);
            config.MessageHandlers.Add(new DecompressionHandler());
            config.MaxReceivedMessageSize = 64 * 1024 * 1024;
            loadControllerDlls(config.Routes);
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}", new { action = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();

            _log.InfoFormat("WebApi 服务 {0} 开始工作。", _baseAddress);

        }

        private static Assembly loadAssemblyFile(string file)
        {
            try
            {
                return Assembly.LoadFile(file);
            }
            catch
            {
            }
            return null;
        }

        private static void loadControllerDlls(HttpRouteCollection routeCollection)
        {
            foreach (string dllFile in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                Assembly assembly = loadAssemblyFile(dllFile);
                if (assembly != null)
                    mapHttpRoute(assembly, routeCollection);
            }
        }

        private static void mapHttpRoute(Assembly assembly, HttpRouteCollection routeCollection)
        {
            Type[] typeArray = null;
            try
            {
                typeArray = assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                ILog log = Common.Logging.LogManager.GetLogger<WebApiHost>();
                log.Error($"Error occured while loading {assembly.FullName}.");
                foreach (var e in ex.LoaderExceptions)
                    log.Error(e.ToString());
            }

            if (typeArray != null)
            {
                foreach (Type type in typeArray)
                {
                    if (type.IsSubclassOf(typeof(ApiController)))
                    {
                        MethodInfo methodInfo = type.GetMethod("MapHttpRoute", BindingFlags.Public | BindingFlags.Static);
                        if (methodInfo != null)
                            methodInfo.Invoke(null, new object[] { routeCollection });
                    }
                }
            }
        }

        public void Dispose()
        {
            _server.Dispose();
            _log.InfoFormat("WebApi 服务 {0} 停止工作。", _baseAddress);
        }
    }
}
