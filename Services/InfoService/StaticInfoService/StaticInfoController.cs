using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace StaticInfoService
{
    public class StaticInfoController : ApiController
    {
        public static void MapHttpRoute(HttpRouteCollection routeCollection)
        {
            const string thisController = "StaticInfo";
            routeCollection.MapHttpRoute(thisController + "1", "api/" + thisController + "/{section}", new { controller = thisController });
            routeCollection.MapHttpRoute(thisController + "2", "api/" + thisController + "/{group}/{section}", new { controller = thisController });
        }

        private UpdateInfo getUpdate(string section, long version)
        {
            return getManager(section).GetUpdate(version);
        }

        private void putUpdate(string section, IEnumerable<InfoItem> items)
        {
            getManager(section).PutUpdate(items);
        }

        [DeflateCompression]
        public UpdateInfo GetUpdate(string section, long version)
        {
            return getUpdate(section, version);
        }

        public void PutUpdate(string section, [FromBody]IEnumerable<InfoItem> items)
        {
            putUpdate(section, items);
        }

        [DeflateCompression]
        public UpdateInfo GetUpdate(string group, string section, long version)
        {
            return getUpdate(string.Join("/", group, section), version);
        }

        public void PutUpdate(string group, string section, [FromBody]IEnumerable<InfoItem> items)
        {
            putUpdate(string.Join("/", group, section), items);
        }

        private static ConcurrentDictionary<string, StaticInfoManager> _managerDict = new ConcurrentDictionary<string, StaticInfoManager>();
        
        private static StaticInfoManager getManager(string section)
        {
            return _managerDict.GetOrAdd(section, sec => new StaticInfoManager(sec));
        }
    }
}
