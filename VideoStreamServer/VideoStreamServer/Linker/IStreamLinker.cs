using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketHelper.Events;
using VideoStreamModels.Model;

namespace VideoStreamServer.Linker
{
    public interface IStreamLinker : IDisposable
    {
        Uri StreamUri { get; }
        IStreamHeader CurrentHeader { get; }
        bool HasListener { get; }
        event EventHandler<IStreamHeader> StreamHeaderReceived;
        event EventHandler<StreamData> StreamDataReceived;
        event EventHandler<ErrorEventArgs> ErrorOccurred;
    }
}
