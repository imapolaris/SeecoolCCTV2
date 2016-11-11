using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using VideoStreamModels.Model;
using VideoStreamServer.Linker;

namespace VideoStreamServer
{
    internal class StreamLinkerManager
    {
        public static StreamLinkerManager Instance { get; private set; }
        static StreamLinkerManager()
        {
            Instance = new StreamLinkerManager();
        }

        private Dictionary<string, IStreamLinker> _dictLinkers = new Dictionary<string, IStreamLinker>();
        private string _template = "{0}:{1}";
        public IStreamLinker GetStreamLinker(StreamAddress addr)
        {
            if (!string.IsNullOrWhiteSpace(addr.Url))
            {
                string key = createKey(addr);
                //如果已创建码流数据通道，则使用现有通道，否则，新建一个通道。
                if (_dictLinkers.ContainsKey(key))
                {
                    return _dictLinkers[key];
                }
                else
                {
                    clearLinker(); //清除不再使用的linker

                    try
                    {
                        bool isRemote = !string.IsNullOrWhiteSpace(addr.PreferredServerIp) && (!LocalIPHost.Instance.IsLocalIp(addr.PreferredServerIp));
                        IStreamLinker sl;
                        if (isRemote)
                        {
                            Console.WriteLine($"路由至远程流媒体服务:{addr.PreferredServerIp}:{addr.PreferredServerPort}");
                            sl = new RemoteStreamLinker(new Uri(addr.Url), addr.PreferredServerIp, addr.PreferredServerPort);
                        }
                        else
                        {
                            Console.WriteLine("本地视频直连!");
                            sl = new DirectStreamLinker(new Uri(addr.Url));
                        }
                        _dictLinkers[key] = sl;
                        return sl;
                    }
                    catch (Exception e)
                    {
                        Logger.Default.Error(e.Message);
                    }
                }
            }
            return null;
        }

        private string createKey(StreamAddress addr)
        {
            return string.Format(_template, addr.Url, addr.PreferredServerIp);
        }

        private void clearLinker()
        {
            foreach (string key in _dictLinkers.Keys.ToArray())
            {
                if (!_dictLinkers[key].HasListener)
                    _dictLinkers.Remove(key);
            }
        }
    }
}
