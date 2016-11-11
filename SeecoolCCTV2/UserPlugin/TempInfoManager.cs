using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVInfoHub;
using CCTVInfoHub.Entity;
using CCTVModels.User;
using Common.Configuration;

namespace UserPlugin
{
    /// <summary>
    /// 临时的信息服务类。
    /// </summary>
    public class TempInfoManager
    {
        public static TempInfoManager Instance { get; private set; }
        static TempInfoManager()
        {
            Instance = new TempInfoManager();
        }

        private TempInfoManager()
        {
            init();
        }

        public bool Login(string userName, string pwd, ref string errMsg)
        {
            ClientHub.UpdateDefault(CCTVInfoType.UserInfo);
            CCTVUserInfo ui = ClientHub.GetUserInfo(userName);
            if (ui != null)
            {
                if (pwd != null && MD5Encryptor.GetMD5Hash(pwd).Equals(ui.Password))
                    return true;
                else
                {
                    errMsg = "密码错误";
                    return false;
                }
            }
            else
            {
                errMsg = "用户不存在";
                return false;
            }
        }

        public string WebApiBaseUri { get; private set; } = ConfigHandler.GetValue<UserPlugin>("UserInfoApiUri");
        public CCTVDefaultInfoSync ClientHub { get; private set; }

        private void init()
        {
            ClientHub = new CCTVDefaultInfoSync(WebApiBaseUri);
            ClientHub.RegisterDefaultWithoutUpdate(CCTVInfoType.UserInfo);
        }
    }
}
