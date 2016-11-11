using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Seecool.Compression.ZLib;
using System.Collections.Concurrent;

namespace CCTVClient
{
    public class CCTVInfo
    {
        public string ServerHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public CCTVInfo(string serverHost)
        {
            Ready = false;
            ServerHost = serverHost;
            UserName = "seecool";
            Password = "seecool";

            _checkLoginTimer = new Timer(onCheckLoginTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        public delegate void OnNodeTree(VideoParser.Node tree, string xml);
        public event OnNodeTree NodeTreeEvent;
        private void fireOnNodeTree(VideoParser.Node tree, string xml)
        {
            OnNodeTree callback = NodeTreeEvent;
            if (callback != null)
                callback(tree, xml);
        }

        public delegate void OnAuthentication(bool success);
        public event OnAuthentication AuthenticationEvent;
        private void fireOnAuthentication(bool success)
        {
            OnAuthentication callback = AuthenticationEvent;
            if (callback != null)
                callback(success);
        }

        public event Action ConnectedEvent;
        private void fireOnConnected()
        {
            Action callback = ConnectedEvent;
            if (callback != null)
                callback();
        }

        public event Action DisconnectedEvent;
        private void fireOnDisconnected()
        {
            Action callback = DisconnectedEvent;
            if (callback != null)
                callback();
        }

        public delegate void OnVideoMiss(ulong videoID);
        public event OnVideoMiss VideoMissEvent;
        private void fireOnVideoMiss(ulong videoID)
        {
            OnVideoMiss callback = VideoMissEvent;
            if (callback != null)
                callback(videoID);
        }

        public delegate void OnVideoPort(ulong videoID, int port, int bandwidth);
        public event OnVideoPort VideoPortEvent;
        private void fireOnVideoPort(ulong videoID, int port, int bandwidth)
        {
            OnVideoPort callback = VideoPortEvent;
            if (callback != null)
                callback(videoID, port, bandwidth);
        }

        public delegate void OnSwitchStatus(ulong videoID, int index, int status);
        public event OnSwitchStatus SwitchStatusEvent;
        private void fireSwitchStatusEvent(ulong videoID, int index, int status)
        {
            var callback = SwitchStatusEvent;
            if (callback != null)
                callback(videoID, index, status);
        }

        public delegate void OnRealtimeInfo(VideoParser.Camera camera, VideoParser.GPS gps);
        private object _realtimeInfoEventLockObj = new object();
        public event OnRealtimeInfo RealtimeInfoEvent
        {
            add
            {
                lock (_realtimeInfoEventLockObj)
                {
                    if (_realtimeInfoEvent == null)
                        needRealTimeInfo(true);
                    _realtimeInfoEvent += value;
                }
            }
            remove
            {
                lock (_realtimeInfoEventLockObj)
                {
                    _realtimeInfoEvent -= value;
                    if (_realtimeInfoEvent == null)
                        needRealTimeInfo(false);
                }
            }
        }
        private event OnRealtimeInfo _realtimeInfoEvent;
        private void fireOnRealtimeInfo(VideoParser.Camera camera, VideoParser.GPS gps)
        {
            OnRealtimeInfo callback = _realtimeInfoEvent;
            if (callback != null)
                callback(camera, gps);
        }

        private bool _getRealtimeInfo = false;
        private void needRealTimeInfo(bool need)
        {
            _getRealtimeInfo = need;
            sendGetRealtimeMessage();
        }

        private void sendGetRealtimeMessage()
        {
            if (_connection != null && _connection.Connected)
            {
                MessageBuilder mb = new MessageBuilder(0x100B0); // Msg_Need_Realtime_Info
                mb.Writer.Write(_getRealtimeInfo ? (int)1 : 0);
                _connection.Send(mb.ToMessage());
            }
        }

        CCTVConnection _connection;

        public void Start()
        {
            _connection = new CCTVConnection(ServerHost, 60010);
            _connection.ConnectEvent += onConnected;
            _connection.DisconnectEvent += onDisconnect;
            _connection.MessageEvent += onMessage;
            _connection.Start();
        }

        public bool Ready { get; private set; }

        private void onDisconnect()
        {
            Ready = false;
            _checkCtrlTimer.Dispose();
            fireOnDisconnected();
        }

        private DateTime _refTime = new DateTime(1970, 1, 1);
        private string _serverName = "";
        private int _userStage;
        private int _userRights;
        private ulong _serverID;
        private ulong[] _userAuth = new ulong[0];
        private int _netctrlID;
        private string _lastBaseXml = "";

        private Timer _checkCtrlTimer;
        private Timer _checkLoginTimer;

        private void onConnected()
        {
            _lastBaseXml = "";

            fireOnConnected();

            sendLoginMessage();

            _checkCtrlTimer = new Timer(onCheckCtrlTimer, null, 1000, 1000);
            _checkLoginTimer.Change(3000, 3000);
        }

        private void onCheckLoginTimer(object state)
        {
            if (Ready)
                cancelLoginChecker();
            else
                sendLoginMessage();
        }

        private void cancelLoginChecker()
        {
            _checkLoginTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void sendLoginMessage()
        {
            MessageBuilder mb = new MessageBuilder(0x11010); // Msg_NetCtrl_Login
            mb.Writer.Write(0x01091027); // version
            mb.Writer.Write(UserName);
            mb.Writer.Write(Password);
            mb.Writer.Write((int)4); // CLIENT_TYPE_VIDEOCTRL
            mb.Writer.Write((new DateTime(2013, 3, 8, 4, 0, 0) - _refTime).TotalSeconds);
            _connection.Send(mb.ToMessage());
        }

        private void onMessage(byte[] message)
        {
            MessageReader reader = new MessageReader(message);
            MessageReader.StreamReader stream = reader.Reader;
            switch (reader.MessageID)
            {
                case 0x11017: // Msg_NetCtrl_Nodebase_Name
                    _serverName = stream.ReadString();
                    break;
                case 0x11013: // Msg_NetCtrl_User_Params
                    {
                        string user = stream.ReadString();
                        bool valid = stream.ReadInt32() != 0;
                        if (valid)
                        {
                            if (!Ready)
                            {
                                _userStage = stream.ReadInt32();
                                _userRights = stream.ReadInt32();
                                _serverID = stream.ReadUInt64();
                                int len = stream.ReadInt32();
                                _userAuth = new ulong[len];
                                for (int i = 0; i < len; i++)
                                    _userAuth[i] = stream.ReadUInt64();
                                Ready = true;
                                fireOnAuthentication(true);

                                sendGetRealtimeMessage();
                            }
                        }
                        else
                        {
                            Ready = false;
                            cancelLoginChecker();
                            fireOnAuthentication(false);
                            _connection.Reconnect();
                        }
                    }
                    break;
                case 0x11015: // Msg_NetCtrl_ID
                    _netctrlID = stream.ReadInt32();
                    break;
                case 0x10030: // Msg_Tree_Info
                    break;
                case 0x10032: // Msg_Xml_Tree
                    {
                        bool up = stream.ReadInt32() != 0;
                        int size = stream.ReadInt32();
                        int uncompSize = stream.ReadInt32();
                        byte[] compressed = stream.ReadBytes(size - sizeof(Int32));
                        byte[] uncompressed = ZLibUtil.Uncompress(compressed, uncompSize);
                        string xml = Encoding.Default.GetString(uncompressed);
                        if (xml.Last() == 0)
                            xml = xml.Substring(0, xml.Length - 1);

                        VideoParser.Node tree = null;
                        if (xml != _lastBaseXml && VideoParser.TryParseNode(xml, out tree))
                        {
                            fireOnNodeTree(tree, xml);
                            _lastBaseXml = xml;
                            updateTree(tree);
                        }
                    }
                    break;
                case 0x11021: // Msg_Video_Port
                    {
                        ulong videoID = stream.ReadUInt64();
                        int oldPort = stream.ReadInt32();
                        int bandwidth = stream.ReadInt32();
                        int port = stream.ReadInt32();
                        fireOnVideoPort(videoID, port, bandwidth);
                    }
                    break;
                case 0x11022: // Msg_Video_Miss
                    {
                        ulong videoID = stream.ReadUInt64();
                        fireOnVideoMiss(videoID);
                    }
                    break;
                case 0x100B1: //Msg_Realtime_Info
                    {
                        string xml = stream.ReadString();
                        VideoParser.Camera camera = null;
                        VideoParser.GPS gps = null;
                        if (VideoParser.TryParseRealtime(xml, out camera) || VideoParser.TryParseRealtime(xml, out gps))
                            fireOnRealtimeInfo(camera, gps);
                    }
                    break;
                case 0x11028: // Msg_Video_Ctrl
                    {
                        ulong videoId = stream.ReadUInt64();
                        int msgId = stream.ReadInt32();
                        onFeedbackMessage(videoId, msgId, reader);
                    }
                    break;
            }
        }

        private void onFeedbackMessage(ulong videoId, int msgId, MessageReader reader)
        {
            MessageReader.StreamReader stream = reader.Reader;
            switch (msgId)
            {
                case 0x12211: // Msg_CtrlCam_Result
                    {
                        int result = stream.ReadInt32();
                        string reason = stream.ReadString();
                        onCtrlCamResult(videoId, result, reason);
                    }
                    break;
                case 0x12221: // Msg_Onoff_Name
                    {
                        int onoffCount = stream.ReadInt32();
                        for (int i = 0; i < onoffCount; i++)
                        {
                            stream.ReadString();
                            stream.ReadInt32();
                        }
                        for (int i = 1; i <= 4; i++)
                            onSwitchState(videoId, i, stream.ReadInt32());
                        if (reader.BytesLeft > 0)
                        {
                            for (int i = 0; i < 4; i++)
                                stream.ReadString();
                        }
                        if (reader.BytesLeft > 0)
                        {
                            for (int i = 5; i <= 9; i++)
                                onSwitchState(videoId, i, stream.ReadInt32());
                        }
                        if (reader.BytesLeft > 0)
                            onSwitchState(videoId, 10, stream.ReadInt32());
                    }
                    break;
                case 0x12251: // Msg_Onoff_State
                    {
                        int index = stream.ReadInt32();
                        int state = stream.ReadInt32();
                        onSwitchState(videoId, -index, state);
                    }
                    break;
            }
        }

        private void onSwitchState(ulong videoId, int index, int state)
        {
            fireSwitchStatusEvent(videoId, index, state);
        }

        private void onCtrlCamResult(ulong videoId, int result, string reason)
        {
        }

        private void updateTree(VideoParser.Node tree)
        {
            HashSet<ulong> servers = new HashSet<ulong>();
            Dictionary<ulong, VideoInfo> infos = new Dictionary<ulong, VideoInfo>();
            getInfos(servers, infos, tree);

            lock (_videoInfos)
            {
                _videoInfos.Clear();
                foreach (var pair in infos)
                    _videoInfos.Add(pair.Key, pair.Value);
            }
        }

        private void getInfos(HashSet<ulong> servers, Dictionary<ulong, VideoInfo> infos, VideoParser.Node node)
        {
            VideoParser.Server server = node as VideoParser.Server;
            if (server != null && !servers.Contains(server.Id))
            {
                servers.Add(server.Id);
                foreach (VideoParser.Node child in server.Childs)
                    getInfos(servers, infos, child);
            }

            VideoParser.Front front = node as VideoParser.Front;
            if (front != null)
            {
                DVRCustom custom = new DVRCustom()
                {
                    User = front.User,
                    Pass = front.Pass,
                    Port = front.Port,
                };
                _dvrCustoms[front.Id] = custom;

                int index = 1;
                foreach (VideoParser.Video video in front.Childs)
                {
                    getInfo(infos, video, front.Id, front.Host, front.Type, index);
                    index++;
                }
            }
        }

        private void getInfo(Dictionary<ulong, VideoInfo> infos, VideoParser.Video video, ulong dvrId, string host, int type, int channel)
        {
            VideoInfo info = new VideoInfo();
            info.DvrId = dvrId;
            info.DvrHost = host;
            info.DvrType = type;
            info.Channel = channel;
            if (video.DvrChannel > 0)
                info.Channel = video.DvrChannel;
            if (!infos.ContainsKey(video.Id))
                infos.Add(video.Id, info);
        }

        private struct VideoInfo
        {
            public ulong DvrId;
            public int DvrType;
            public string DvrHost;
            public int Channel;
        }

        Dictionary<ulong, VideoInfo> _videoInfos = new Dictionary<ulong, VideoInfo>();

        private struct DVRCustom
        {
            public string User;
            public string Pass;
            public int Port;
        }
        static DVRCustom _defaultDvrCustom = new DVRCustom()
        {
            User = "admin",
            Pass = "12345",
            Port = 8000,
        };

        ConcurrentDictionary<ulong, DVRCustom> _dvrCustoms = new ConcurrentDictionary<ulong, DVRCustom>();

        public static DVRChannelInfo.DVRType GetDvrType(int type)
        {
            switch (type)
            {
                case 1:
                case 4:
                case 6:
                case 7:
                    return DVRChannelInfo.DVRType.HikVision;
                case 8:
                case 9:
                case 10:
                case 11:
                    return DVRChannelInfo.DVRType.USNT;
                default:
                    return DVRChannelInfo.DVRType.Unknown;
            }
        }

        public DVRChannelInfo GetDVRChannelInfo(ulong videoID)
        {
            DVRChannelInfo info = null;
            ulong dvrID = 0;
            lock (_videoInfos)
            {
                if (_videoInfos.ContainsKey(videoID))
                {
                    VideoInfo vi = _videoInfos[videoID];
                    info = new DVRChannelInfo();
                    info.Type = GetDvrType(vi.DvrType);
                    info.Host = vi.DvrHost;
                    info.Channel = vi.Channel;
                    dvrID = vi.DvrId;
                }
            }

            if (info != null)
            {
                DVRCustom dc = getDVRInfo(dvrID);
                info.Port = dc.Port;
                info.User = dc.User;
                info.Pass = dc.Pass;
            }

            return info;
        }

        DVRCustom getDVRInfo(ulong frontID)
        {
            DVRCustom result;
            if (_dvrCustoms.TryGetValue(frontID, out result))
                return result;
            else
                return _defaultDvrCustom;
        }

        public void Stop()
        {
            foreach (ulong videoID in _ctrlEndTimes.Keys)
                EndCtrl(videoID);

            cancelLoginChecker();
            _connection.Stop();
        }

        public void QueryVideo(ulong videoID)
        {
            MessageBuilder mb = new MessageBuilder(0x11020); // Msg_Connect_Video
            mb.Writer.Write(videoID);
            _connection.Send(mb.ToMessage());
        }

        public void StartCtrl(ulong videoID, TimeSpan endSpan)
        {
            MessageBuilder mb = new MessageBuilder(0x11028); // Msg_Video_Ctrl
            mb.Writer.Write(videoID);
            mb.Writer.Write(0x12210); // Msg_Start_CtrlCam
            mb.Writer.Write((int)0);
            mb.Writer.Write((int)0);
            mb.Writer.Write((int)100);
            mb.Writer.Write((int)0);
            mb.Writer.Write("CCTV2用户");
            mb.Writer.Write((int)0);
            _connection.Send(mb.ToMessage());

            DateTime endTime = DateTime.Now + endSpan;
            _ctrlEndTimes[videoID] = endTime;
        }

        public void EndCtrl(ulong videoID)
        {
            DateTime dummy;
            _ctrlEndTimes.TryRemove(videoID, out dummy);

            MessageBuilder mb = new MessageBuilder(0x11028); // Msg_Video_Ctrl
            mb.Writer.Write(videoID);
            mb.Writer.Write(0x12220); // Msg_End_CtrlCam
            _connection.Send(mb.ToMessage());
        }

        public void TrackTarget(ulong videoID, string targetType, string targetID, double lon, double lat, double alt, int targetWidth, int targetWidth2)
        {
            StartCtrl(videoID, TimeSpan.FromSeconds(30));
            MessageBuilder mb = new MessageBuilder(0x11060); // Msg_Track_Target
            mb.Writer.Write(videoID);
            mb.Writer.Write(targetType);
            mb.Writer.Write(targetID);
            mb.Writer.Write(lon);
            mb.Writer.Write(lat);
            mb.Writer.Write(alt);
            mb.Writer.Write(targetWidth);
            mb.Writer.Write(targetWidth2);
            _connection.Send(mb.ToMessage());
        }

        public void StopTrack(ulong videoID)
        {
            MessageBuilder mb = new MessageBuilder(0x11070); // Msg_Stop_Track_Target
            mb.Writer.Write(videoID);
            _connection.Send(mb.ToMessage());
            EndCtrl(videoID);
        }

        public enum CameraAction
        {
            Stop = 0,
            Up, Down, Left, Right, LeftUp, LeftDown, RightUp, RightDown, AutoScan,
            LensStop, IrisOpen, IrisClose, FocusNear, FocusFar, ZoomWide, ZoomTele,
            AuxOn, AuxOff,
            SetPreset, GoPreset,
        }

        private int[] _actionCodes = new int[] { 0x00, 0x01, 0x02, 0x04, 0x08, 0x05, 0x06, 0x09, 0x0A, 0x0100, 0x00, 0x0401, 0x0400, 0x0101, 0x0100, 0x0201, 0x0200, 0x01, 0x00, 0x00, 0x00 };
        public void CameraControl(ulong videoID, CameraAction action, int speed, int aux)
        {
            int param = speed * 11 / 256;
            int actionIndex = (int)action;
            if (action == CameraAction.AuxOn || action == CameraAction.AuxOff)
                param = aux;

            int msg = 0x12230; // Msg_Ctrl_Pan
            if (actionIndex >= 10)
                msg = 0x12240; // Msg_Ctrl_Lens
            if (actionIndex >= 17)
                msg = 0x12250; // Msg_Ctrl_Onoff
            if (actionIndex == 19)
                msg = 0x12242; // Msg_Set_Preset
            if (actionIndex == 20)
                msg = 0x12244; // Msg_Goto_Preset
            int actCode = _actionCodes[actionIndex];

            TimeSpan endSpan = TimeSpan.FromSeconds(30);
            if (action == CameraAction.Stop || action == CameraAction.LensStop || action == CameraAction.AuxOn || action == CameraAction.AuxOff)
                endSpan = TimeSpan.FromSeconds(5);
            StartCtrl(videoID, endSpan);
            MessageBuilder mb = new MessageBuilder(0x11028); // Msg_Video_Ctrl
            mb.Writer.Write(videoID);
            mb.Writer.Write(msg);
            if (msg == 0x12250) // Msg_Ctrl_Onoff
            {
                mb.Writer.Write(-param);
                mb.Writer.Write(actCode);
                mb.Writer.Write((int)0);
            }
            else if (msg == 0x12242 || msg == 0x12244) // Msg_Set_Preset || Msg_Goto_Preset
            {
                mb.Writer.Write(aux);
                mb.Writer.Write((int)0);
            }
            else
            {
                mb.Writer.Write(actCode);
                mb.Writer.Write((int)0);
                mb.Writer.Write(param);
            }
            _connection.Send(mb.ToMessage());
        }

        private ConcurrentDictionary<ulong, DateTime> _ctrlEndTimes = new ConcurrentDictionary<ulong, DateTime>();
        private void onCheckCtrlTimer(object state)
        {
            DateTime now = DateTime.Now;
            foreach (var pair in _ctrlEndTimes.ToArray())
            {
                if (pair.Value < now)
                    EndCtrl(pair.Key);
            }
        }
    }
}
