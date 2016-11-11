using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenterStorageCmd;
using CenterStorageDeploy.Models;
using CenterStorageDeploy.Proxy;

namespace CenterStorageDeploy.Managers
{
    public class StorageManager
    {
        static StorageManager()
        {
            Instance = new StorageManager();
        }

        public static StorageManager Instance { get; private set; }

        private StorageInfoProxy _proxy;
        private StorageSource _oldSource;
        private StorageManager()
        {

        }

        private bool checkEqual(StorageSource one, StorageSource two)
        {
            return one.Ip.Equals(two.Ip) && one.Port == two.Port;
        }

        private void closeOldProxy()
        {
            if (_proxy != null)
                _proxy.Close();
        }

        private void createNewProxy(StorageSource ss)
        {
            closeOldProxy();
            _oldSource = ss;
            _proxy = new StorageInfoProxy(ss.Ip, ss.Port);
        }

        private StorageInfoProxy Proxy
        {
            get
            {
                StorageSource ss = NodesManager.Instance.GetStorageSource();
                if (ss != null)
                {
                    if (_oldSource == null || !checkEqual(_oldSource, ss))
                    {
                        createNewProxy(ss);
                    }
                }
                return _proxy;
            }
        }

        public IVideoInfo[] GetAllStorageVideos()
        {
            return Proxy?.GetAllStorageVideos();
        }

        public void SetStorageFlag(string videoId, int streamId, bool storageOn)
        {
            Proxy?.SetStorageFlag(videoId, streamId, storageOn);
        }
    }
}
