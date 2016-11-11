using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using CCTVModels;

namespace Persistence.Remote
{
    public class InfoServiceApiClient : IDisposable
    {
        string _baseAddress;
        string _section;
        const string _requestBase = "api/StaticInfo";
        HttpClientHandler _clientHandler;
        HttpClient _client;

        public InfoServiceApiClient(string baseAddress, string section)
        {
            _baseAddress = baseAddress;
            _section = section;

            _clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _client = new HttpClient(_clientHandler);
            _client.BaseAddress = new Uri(_baseAddress);
        }

        public void Dispose()
        {
            _client.Dispose();
            _clientHandler.Dispose();
        }

        public UpdateInfo GetUpdate(long version)
        {
            var response = _client.GetAsync($"{_requestBase}/{_section}?version={version}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<UpdateInfo>().Result;
        }

        public void PutUpdate(IEnumerable<InfoItem> items)
        {
            HttpContent content = getHttpContent(items);
            var response = _client.PutAsync($"{_requestBase}/{_section}", content).Result;
            response.EnsureSuccessStatusCode();
        }

        private static HttpContent getHttpContent(IEnumerable<InfoItem> items)
        {
            HttpContent content = new ObjectContent<IEnumerable<InfoItem>>(items, new JsonMediaTypeFormatter());

            int totalSize = 0;
            foreach (InfoItem item in items)
                totalSize += 30 + item.Info.Length;
            const int minSizeToCompress = 8192;
            if (totalSize >= minSizeToCompress)
                content = new DeflateContent(content);

            return content;
        }
    }
}
