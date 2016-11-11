using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterStorageCmd;

namespace TestCenterStorageService.CenterStorageCmd.Packet
{
    [TestClass]
    public class MessagePacketTest
    {
        [TestMethod]
        public void TestMessagePacket()
        {
            MessagePacket packet = new MessagePacket( MessageType.Error, "errorTest", null);
            byte[] buffer = MessagePacket.Encode(packet);
            MessagePacket packet2 = MessagePacket.Decode(buffer);
            Assert.AreNotSame(packet, packet2);
            Assert.AreEqual(packet.Type, packet2.Type);
            Assert.AreEqual(packet.Message, packet2.Message);
            Assert.AreEqual(packet.Operate, packet2.Operate);
        }
    }
}
