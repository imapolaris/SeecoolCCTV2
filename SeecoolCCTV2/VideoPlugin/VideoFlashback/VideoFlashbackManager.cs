using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoNS.VideoFlashback
{
    internal class VideoFlashbackManager
    {
        public static readonly VideoFlashbackManager Instance = new VideoFlashbackManager();

        string _baseDir;

        private VideoFlashbackManager()
        {
            _baseDir = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), @"Flashback");
            //Directory.CreateDirectory(_baseDir);
        }

        ConcurrentDictionary<string, FlashbackChannel> _channelDict = new ConcurrentDictionary<string, FlashbackChannel>();

        public FlashbackChannel GetChannel(string videoId)
        {
            return _channelDict.GetOrAdd(videoId, x => new FlashbackChannel(x, _baseDir));
        }

        public IFlashbackPlayer GetPlayer(string videoId)
        {
            FlashbackChannel channel = GetChannel(videoId);
            return channel.GetPlayer();
        }
    }
}
