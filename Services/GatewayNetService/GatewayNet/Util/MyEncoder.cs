using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayNet.Util
{
    public class MyEncoder
    {
        static MyEncoder()
        {
            init();
        }

        public static Encoding Encoder { get; private set; } = Encoding.UTF8;


        private static void init()
        {
            try
            {
                string enc = Configurations.Instance.Encoding;
                switch (enc.ToUpper())
                {
                    case "UTF8":
                        Encoder = Encoding.UTF8;
                        break;
                    case "UTF7":
                        Encoder = Encoding.UTF7;
                        break;
                    case "UTF32":
                        Encoder = Encoding.UTF32;
                        break;
                    case "ASCII":
                        Encoder = Encoding.ASCII;
                        break;
                    case "UNICODE":
                        Encoder = Encoding.Unicode;
                        break;
                }
            }
            catch (Exception e)
            {
                Common.Log.Logger.Default.Error("在获取配置编码格式时出错", e);
            }
        }
    }
}
