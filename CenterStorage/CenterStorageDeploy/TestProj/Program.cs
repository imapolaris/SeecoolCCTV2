using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CenterStorageDeploy.Controllers;
using CenterStorageDeploy.Models;

namespace TestProj
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            string baseAddress = "http://127.0.0.1:12321/api/";
            var response = client.GetAsync(baseAddress + "nodes").Result;
            response.EnsureSuccessStatusCode();
            VideoStorageInfo vsi = response.Content.ReadAsAsync<VideoStorageInfo>().Result;
            ShowResult(vsi);
            while (true)
            {
                string str = Console.ReadLine();
                if (str.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                string request = baseAddress + $"?videoid={str};flag=true";
                client.PostAsJsonAsync(request, "");
            }
        }

        private static void ShowResult(VideoStorageInfo vsi)
        {
            Console.WriteLine(vsi.VideoId + "_" + vsi.StorageOn + "_" + vsi.Type + "_" + vsi.VideoName);
            if (vsi.Children != null)
                foreach (VideoStorageInfo child in vsi.Children)
                    ShowResult(child);
        }
    }
}
