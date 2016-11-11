using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVInfoHub.Util
{
    public class VideoIdFormatter
    {
        const string _cctv1Prefix = "CCTV1_";
        public ulong ToCCTV1Id(string id)
        {
            ulong cctv1Id = 0;
            if (!string.IsNullOrEmpty(id) && id.Length > _cctv1Prefix.Length && id.Substring(0, _cctv1Prefix.Length) == _cctv1Prefix)
            {
                string idString = id.Substring(_cctv1Prefix.Length);
                ulong.TryParse(idString, NumberStyles.HexNumber, null, out cctv1Id);
            }
            return cctv1Id;
        }

        public string FromCCTV1Id(ulong cctv1Id)
        {
            return _cctv1Prefix + cctv1Id.ToString("X");
        }
    }
}
