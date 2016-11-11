using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayModels;
using GatewayNet.Lower;
using GBTModels.Global;
using GBTModels.Query;
using GBTModels.Util;

namespace GatewayNet.Tools
{
    public class CommandProcessor
    {

        public CommandProcessor()
        {

        }

        /// <summary>
        /// 解析命令内容。
        /// </summary>
        /// <param name="aor">
        /// Address of record.
        /// eg:162211411701073990@192.168.14.110
        /// </param>
        /// <param name="xml">协议命令内容。</param>
        public void Process(string aor, string xml)
        {
            CommandInfo ci = CmdIdentifier.GetCommandInfo(xml);
            switch (ci.CommandType)
            {
                case CommandType.Control:
                    break;
                case CommandType.Query:
                    {
                        switch (ci.CommandName)
                        {
                            case CommandName.KeepAlive:
                                break;
                            case CommandName.DeviceControl:
                                break;
                            case CommandName.Catalog:
                                {
                                    DeviceCatalog dc = SerializeHelper.Instance.Deserialize<DeviceCatalog>(xml);
                                    if (dc.DeviceID == InfoService.Instance.CurrentGateway.SipNumber)
                                    {
                                        Platform plat = RegisterManager.Instance.GetPlatformByAOR(aor);
                                        if (plat != null)
                                            ResourceSharer.Instance.ResponseToPlatform(plat);
                                    }
                                }
                                break;
                            case CommandName.DeviceStatus:
                                break;
                            case CommandName.DeviceInfo:
                                break;
                            case CommandName.RecordInfo:
                                break;
                            case CommandName.Alarm:
                                break;
                            case CommandName.Unknown:
                            default:
                                break;
                        }
                    }
                    break;
                case CommandType.Notify:
                    break;
                case CommandType.Response:
                    break;
                case CommandType.Unknown:
                default:
                    break;
            }
        }
    }
}
