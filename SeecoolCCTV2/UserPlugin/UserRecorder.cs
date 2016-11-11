using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Configuration.CustomSetting;

namespace UserPlugin
{
    public class UserRecorder
    {
        public static UserRecorder Instance { get; private set; }
        static UserRecorder()
        {
            Instance = new UserRecorder();
        }

        private const string UserLoginFile = "userlogin.usr";
        private const string KeyString = "userlogin";

        private UserRecorder()
        {
            readUser();
        }

        public LoginUserInfo LastLogin { get; private set; }

        public void StoreUser(LoginUserInfo lui)
        {
            if (lui == null)
                return;
            lui = new LoginUserInfo(lui);
            try
            {
                if (!lui.StorePwd)
                {
                    lui.Password = null;
                }
                else
                    lui.Password = LocalEncryptor.Instance.Encrypt(lui.Password);
                var file = new AggregateCustomSettingFile<string, LoginUserInfo>(UserLoginFile);
                file[KeyString] = lui;
                LastLogin = lui;
            }
            catch (Exception e)
            {
                Console.WriteLine("保存登录用户信息失败:" + e.Message);
            }
        }

        private void readUser()
        {
            LoginUserInfo info = null;
            var file = new AggregateCustomSettingFile<string, LoginUserInfo>(UserLoginFile);
            info = file[KeyString];
            if (info != null)
            {
                info.Password = LocalEncryptor.Instance.Decrypt(info.Password);
            }
            LastLogin = info;
        }
    }

    public class LoginUserInfo
    {
        public LoginUserInfo()
        {

        }

        public LoginUserInfo(string user,string pwd,bool storePwd)
        {
            UserName = user;
            Password = pwd;
            StorePwd = storePwd;
        }

        public LoginUserInfo(LoginUserInfo lui)
        {
            UserName = lui.UserName;
            Password = lui.Password;
            StorePwd = lui.StorePwd;
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool StorePwd { get; set; } = false;
    }
}
