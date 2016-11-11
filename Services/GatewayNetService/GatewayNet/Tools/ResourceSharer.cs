using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;
using CCTVModels.User;
using GatewayModels;
using GatewayModels.Util;
using GatewayNet.Server;
using GatewayNet.Util;
using GBTModels.Global;
using GBTModels.Notify;
using GBTModels.Response;
using GBTModels.Util;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace GatewayNet.Tools
{
    public class ResourceSharer
    {
        public static ResourceSharer Instance { get; private set; }
        static ResourceSharer()
        {
            Instance = new ResourceSharer();
        }

        private ResourceSharer()
        {

        }

        public void NotifyToPlatform(string platId)
        {
            Gateway gw = InfoService.Instance.CurrentGateway;
            Platform pf = InfoService.Instance.GetPlatform(platId);
            string localIp = IPAddressHelper.GetLocalIp();
            SIP_Stack stack = SipProxyWrapper.Instance.Stack;

            DeviceCatalogNotify dcd = createCatalog(gw, pf);
            string body = SerializeHelper.Instance.Serialize(dcd);

            SIP_t_NameAddress from = new SIP_t_NameAddress($"sip:{gw.SipNumber}@{localIp}:{gw.Port}");
            SIP_t_NameAddress to = new SIP_t_NameAddress($"sip:{pf.SipNumber}@{pf.Ip}:{pf.Port}");

            SIP_Request message = stack.CreateRequest(SIP_Methods.NOTIFY, to, from);
            message.ContentType = "Application/MANSCDP+xml";
            message.Data = MyEncoder.Encoder.GetBytes(body);
            SIP_RequestSender send = stack.CreateRequestSender(message);
            send.Start();
        }

        public void ResponseToPlatform(Platform plat)
        {
            Gateway gw = InfoService.Instance.CurrentGateway;
            string localIp = IPAddressHelper.GetLocalIp();
            SIP_Stack stack = SipProxyWrapper.Instance.Stack;

            DeviceCatalogResp resp = createCatalogResp(gw, plat);
            string body = SerializeHelper.Instance.Serialize(resp);

            SIP_t_NameAddress from = new SIP_t_NameAddress($"sip:{gw.SipNumber}@{localIp}:{gw.Port}");
            SIP_t_NameAddress to = new SIP_t_NameAddress($"sip:{plat.SipNumber}@{plat.Ip}:{plat.Port}");

            SIP_Request message = stack.CreateRequest(SIP_Methods.MESSAGE, to, from);
            message.ContentType = "Application/MANSCDP+xml";
            message.Data = MyEncoder.Encoder.GetBytes(body);
            SIP_RequestSender send = stack.CreateRequestSender(message);
            send.Start();
        }

        private List<string> getUserDeviceId(Platform plat)
        {
            CCTVUserPrivilege up = InfoService.Instance.GetUserPrivilege(plat.UserName);
            if (up != null && up.AccessibleNodes != null)
            {
                CCTVHierarchyInfo[] hInfos = InfoService.Instance.GetAllHierarchy().Where(hi => up.AccessibleNodes.Contains(hi.Id)).ToArray();
                if (hInfos != null && hInfos.Length > 0)
                {
                    return hInfos.Select(hi => hi.ElementId).ToList();
                }
            }
            return new List<string>();
        }

        private DeviceCatalogNotify createCatalog(Gateway gw, Platform plat)
        {
            DeviceCatalogNotify notify = new DeviceCatalogNotify();
            notify.DeviceID = gw.SipNumber;
            DeviceItemsCollection items = buildDeviceItems(gw.SipNumber, plat);
            if (items != null)
                notify.Items = items;
            return notify;
        }

        private DeviceCatalogResp createCatalogResp(Gateway gw, Platform plat)
        {
            DeviceCatalogResp resp = new DeviceCatalogResp();
            resp.DeviceID = gw.SipNumber;
            DeviceItemsCollection items = buildDeviceItems(gw.SipNumber, plat);
            if (items != null)
                resp.Items = items;
            return resp;
        }

        private DeviceItemsCollection buildDeviceItems(string parentId, Platform plat)
        {
            //过滤出仅平台鉴权用户可用的视频列表。
            List<string> acIds = getUserDeviceId(plat);
            IEnumerable<CCTVStaticInfo> infos = InfoService.Instance.GetAllStaticInfo();
            if (infos != null)
            {
                Dictionary<string, string> dictIdPairs = new Dictionary<string, string>();
                IEnumerable<SipIdMap> dids = InfoService.Instance.GetAllSipIdMap();
                if (dids != null)
                {
                    foreach (SipIdMap sp in dids)
                    {
                        dictIdPairs[sp.StaticId] = sp.SipNumber;
                    }
                }
                DeviceItemsCollection items = new DeviceItemsCollection();
                foreach (CCTVStaticInfo si in infos)
                {
                    CCTVControlConfig cc = InfoService.Instance.GetControlConfig(si.VideoId);
                    string sip = null;
                    if (dictIdPairs.ContainsKey(si.VideoId))
                        sip = dictIdPairs[si.VideoId];
                    else
                    {
                        sip = SipIdGenner.GenDeviceID();
                        InfoService.Instance.PutSipIdMap(si.VideoId, new SipIdMap(si.VideoId, sip),false);
                    }
                    ItemType it = new ItemType()
                    {
                        DeviceID = sip,
                        ParentID = parentId,
                        Event = StatusEvent.ADD,
                        Name = si.Name,
                        Manufacturer = "Seecool",
                        Model = "Seecool",
                        Owner = "Seecool",
                        CivilCode = sip,
                        Block = "",
                        Address = "1",
                        Parental = 0,
                        SafetyWay = 0,
                        RegisterWay = 1,
                        CertNum = "1",
                        Certifiable = 1,
                        ErrCode = 400,
                        EndTime = DateTime.Now,
                        Secrecy = 0,
                        IPAddress = cc?.Ip,
                        Port = cc == null ? 8000 : cc.Port,
                        Password = cc?.Password,
                        Status = StatusType.ON,
                        Longitude = si.Longitude,
                        Latitude = si.Latitude
                    };
                    items.Add(it);
                }
                return items;
            }
            else
                return null;
        }
    }
}
