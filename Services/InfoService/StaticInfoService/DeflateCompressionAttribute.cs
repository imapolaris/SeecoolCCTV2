using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Filters;

namespace StaticInfoService
{
    public class DeflateCompressionAttribute : ActionFilterAttribute
    {
        static byte[] _emptyByteArray = new byte[0];
        const string _encodingType = "deflate";
        static StringWithQualityHeaderValue _encodingHeader = StringWithQualityHeaderValue.Parse(_encodingType);
        const int _minSizeToCompress = 8192;

        public override void OnActionExecuted(HttpActionExecutedContext actContext)
        {
            var content = actContext.Response.Content;
            var bytes = content == null ? _emptyByteArray : content.ReadAsByteArrayAsync().Result;
            if (bytes.Length >= _minSizeToCompress && actContext.Request.Headers.AcceptEncoding.Contains(_encodingHeader))
            {
                var oldHeaders = content.Headers;
                var compressedContent = deflateByte(bytes);
                actContext.Response.Content = new ByteArrayContent(compressedContent);
                actContext.Response.Content.Headers.ContentEncoding.Add(_encodingType);
                actContext.Response.Content.Headers.ContentType = oldHeaders.ContentType;
            }
            base.OnActionExecuted(actContext);
        }

        private static byte[] deflateByte(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    deflate.Write(bytes, 0, bytes.Length);
                    deflate.Flush();
                }
                return ms.ToArray();
            }
        }
    }
}
