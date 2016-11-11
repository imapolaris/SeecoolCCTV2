using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Log;

namespace VideoStreamServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());
            //IPAddress[] host = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            //foreach (IPAddress ip in host)
            //{
            //    string str = ip.ToString();
            //    Console.WriteLine(str + "___" + LocalIPHost.Instance.IsLocalIp(str));
            //}
            //Console.WriteLine("sfsfsf"+"__"+LocalIPHost.Instance.IsLocalIp("127.0.0.1"));

            //Console.WriteLine(LocalIPHost.Instance.IsLocalIp("192.168.1.1"));

            //A a = new A();
            //B b = new B();
            //a.TestEvent += b.EventListen;
            //b = null;

            //int count = 0;
            //while (true)
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    a.OnTestEvent();
            //    if (count++ % 10 == 0)
            //    {
            //        GC.Collect();
            //    }
            //    if (count == 50)
            //        break;
            //}
            //Console.WriteLine("Press any key to continue!");
            //Console.ReadKey();

            //string ss = "hikv://user:pass@192.168.9.156:8000/stream?channel=1&profile=sub";
            //Uri uri = new Uri(ss);
            //Console.WriteLine(uri);
            Logger.Default.Trace("服务启动");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            SocketManager.Instance.Init();
            while (true)
            {
                string str = Console.ReadLine();
                if (str.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;
            }
            Logger.Default.Trace("服务关闭");
            
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.ExceptionObject.ToString());
        }
    }

    class A
    {
        public void OnTestEvent()
        {
            TestEvent(this, EventArgs.Empty);
        }
        public event EventHandler TestEvent;
    }

    class B
    {
        public void EventListen(object sender, EventArgs e)
        {
            Console.WriteLine($"Receive Event:{DateTime.Now}");
        }
    }
}
