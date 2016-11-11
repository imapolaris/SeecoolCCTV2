using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CenterStorageCmd;
using SocketHelper.Events;

namespace CenterStorageDeploy.Proxy
{
    public class StorageInfoProxy : ProxyBase
    {
        public AutoResetEvent _waitHandler;
        private IVideoInfo[] _videos;
        public StorageInfoProxy(string ip, int port) : base(ip, port)
        {
            Client.ReceiveCompleted += Client_ReceiveCompleted;
        }

        public IVideoInfo[] GetAllStorageVideos()
        {
            EnsureStart();
            _videos = null;
            _waitHandler = new AutoResetEvent(false);
            Client.Send((int)ParamCode.StorageFlagAll, null);
            _waitHandler.WaitOne(5000);
            return _videos;
        }

        public void SetStorageFlag(string videoId, int streamId, bool storageOn)
        {
            EnsureStart();
            VideoInfo vi = new VideoInfo(videoId, streamId);
            StorageFlagParam sfp = new StorageFlagParam(vi, storageOn);
            Client.Send((int)ParamCode.StorageFlag, StorageFlagParam.Encode(sfp));
        }

        private void Client_ReceiveCompleted(object sender, ReceiveEventArgs args)
        {
            try
            {
                if (args.ByteLength > 0)
                {
                    using (MemoryStream ms = new MemoryStream(args.ReceivedBytes))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            int code = br.ReadInt32();
                            switch ((ParamCode)code)
                            {
                                case ParamCode.StorageFlagAll:
                                    {
                                        IVideoInfo[] videos = VideoInfo.DecodeArray(ms);
                                        _videos = videos;
                                        if (_waitHandler != null)
                                            _waitHandler.Set();
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _waitHandler.Set();
            }
        }

        //#region 【事件】
        //public event Action<IVideoInfo[]> StorageInfoReceived;

        //private void onStorageInfoReceived(IVideoInfo[] vis)
        //{
        //    Action<IVideoInfo[]> handler = StorageInfoReceived;
        //    if (handler != null)
        //        handler(vis);
        //}
        //#endregion 【事件】
    }
}
