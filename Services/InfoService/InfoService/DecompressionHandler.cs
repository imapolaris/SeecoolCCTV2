using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfoService
{
    public class DecompressionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isGzip = request.Content.Headers.ContentEncoding.Contains("gzip");
            bool isDeflate = !isGzip && request.Content.Headers.ContentEncoding.Contains("deflate");

            if (isGzip || isDeflate)
            {
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream decompressStream = null;
                    if (isGzip)
                        decompressStream = new GZipStream(await request.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                    else if (isDeflate)
                        decompressStream = new DeflateStream(await request.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                    if (decompressStream != null)
                    {
                        using (decompressStream)
                            await decompressStream.CopyToAsync(ms);
                    }
                    bytes = ms.ToArray();
                }

                var originContent = request.Content;
                request.Content = new ByteArrayContent(bytes);

                foreach (var header in originContent.Headers)
                    request.Content.Headers.Add(header.Key, header.Value);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
