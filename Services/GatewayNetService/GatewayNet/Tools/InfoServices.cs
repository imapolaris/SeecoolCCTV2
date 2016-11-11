using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels;
using CCTVModels.User;
using GatewayModels;
using GatewayModels.Util;
using GatewayNet.Util;

namespace GatewayNet.Tools
{
    public class InfoService
    {
        public static InfoService Instance { get; private set; }
        static InfoService()
        {
            Instance = new InfoService();
        }

        public CCTVDefaultInfoSync ClientHub { get; private set; }
        public InfoService()
        {
            ClientHub = new CCTVDefaultInfoSync(Configurations.Instance.InfoServiceAddress);
            SyncParams<Gateway> gwSP = new SyncParams<Gateway>("GBT28181/Gateway", Timeout.InfiniteTimeSpan);
            SyncParams<Platform> platSP = new SyncParams<Platform>("GBT28181/Platform", Timeout.InfiniteTimeSpan);
            SyncParams<SipIdMap> simSP = new SyncParams<SipIdMap>("GBT28181/DeviceIdMap", Timeout.InfiniteTimeSpan);
            ClientHub.RegisterSynchronizer<Gateway>(gwSP);
            ClientHub.RegisterSynchronizer<Platform>(platSP);
            ClientHub.RegisterSynchronizer<SipIdMap>(simSP);

            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.StaticInfo);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.ControlConfig);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.UserPrivilege);
        }

        public Gateway CurrentGateway
        {
            get
            {
                //Gateway gw = ClientHub.GetRegisteredInfo<Gateway>(Gateway.Key);
                //if (gw == null)
                //{
                //    gw = new Gateway()
                //    {
                //        SipNumber = SipIdGenner.GenDeviceID()
                //    };
                //    ClientHub.PutRegisteredInfo<Gateway>(Gateway.Key, gw, false);
                //}
                //return gw;
                return new Gateway()
                {
                    SipNumber = "162904511001001590",
                    Port = 6100
                };
            }
        }

        private Platform createTestPlatform()
        {
            return new Platform()
            {
                Id = Guid.NewGuid().ToString(),
                SipNumber = "160901171235946719",
                Name = "测试",
                Ip = "192.168.14.143",
                Port = 6090,
                UserName = "admin",
                Password = "12345",
                Realm = "seecool",
                Type = PlatformType.Superior
            };
        }

        public Platform GetPlatform(string platId)
        {
            return createTestPlatform();
            //ClientHub.UpdateRegistered<Platform>();
            //return ClientHub.GetRegisteredInfo<Platform>(platId);
        }

        public Platform[] GetAllPlatformSuper()
        {
            return new Platform[] { createTestPlatform() };
            //return getAllPlatform(PlatformType.Superior);
        }

        public Platform[] GetAllPlatformLower()
        {
            return getAllPlatform(PlatformType.Lower);
        }

        private Platform[] getAllPlatform(PlatformType pt)
        {
            ClientHub.UpdateRegistered<Platform>();
            Platform[] plats = ClientHub.GetAllRegisteredInfos<Platform>();
            return plats?.Where(p => p.Type == pt).ToArray();
        }

        public SipIdMap GetSipIdMap(string videoId)
        {
            ClientHub.UpdateRegistered<SipIdMap>();
            return ClientHub.GetRegisteredInfo<SipIdMap>(videoId);
        }

        public SipIdMap[] GetAllSipIdMap()
        {
            ClientHub.UpdateRegistered<SipIdMap>();
            return ClientHub.GetAllRegisteredInfos<SipIdMap>();
        }

        public void PutSipIdMap(string videoId, SipIdMap map, bool isDeleted)
        {
            ClientHub.PutRegisteredInfo(videoId, map, isDeleted);
        }

        public CCTVStaticInfo[] GetAllStaticInfo()
        {
            ClientHub.UpdateDefault(CCTVInfoType.StaticInfo);
            return ClientHub.GetAllStaticInfo();
        }

        public CCTVControlConfig GetControlConfig(string videoId)
        {
            ClientHub.UpdateDefault(CCTVInfoType.ControlConfig);
            return ClientHub.GetControlConfig(videoId);
        }

        public CCTVHierarchyInfo[] GetAllHierarchy()
        {
            ClientHub.UpdateDefault(CCTVInfoType.HierarchyInfo);
            return ClientHub.GetAllHierarchyInfo();
        }

        public CCTVHierarchyInfo GetHierarchy(string id)
        {
            ClientHub.UpdateDefault(CCTVInfoType.HierarchyInfo);
            return ClientHub.GetHierarchyInfo(id);
        }

        public CCTVUserPrivilege GetUserPrivilege(string userName)
        {
            ClientHub.UpdateDefault(CCTVInfoType.UserPrivilege);
            return ClientHub.GetUserPrivilege(userName);
        }

        public void PutPlatformDevices(PlatformDeviceSet platDev, bool isDel)
        {
            ClientHub.PutRegisteredInfo(platDev.PlatformId, platDev, isDel);
        }
    }
}
