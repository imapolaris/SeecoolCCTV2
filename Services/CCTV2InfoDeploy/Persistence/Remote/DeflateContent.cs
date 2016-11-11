using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Remote
{
    public class DeflateContent : HttpContent
    {
        private readonly HttpContent _originalContent;
        const string _encodingType = "deflate";

        public DeflateContent(HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _originalContent = content;

            foreach (KeyValuePair<string, IEnumerable<string>> header in _originalContent.Headers)
                Headers.TryAddWithoutValidation(header.Key, header.Value);

            Headers.ContentEncoding.Add(_encodingType);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress, true))
                await _originalContent.CopyToAsync(deflateStream);
        }
    }
}
