using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace UserPlugin.Dialogs
{
    /// <summary>
    /// LoginWnd.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWnd : RadWindow
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public LoginWnd()
        {
            InitializeComponent();
            this.Loaded += LoginWnd_Loaded;
        }

        private void LoginWnd_Loaded(object sender, RoutedEventArgs e)
        {
            LoginUserInfo lui = UserRecorder.Instance.LastLogin;
            if (lui != null)
            {
                txtUser.Text = lui.UserName;
                ckbStorePwd.IsChecked = lui.StorePwd;
                if (lui.StorePwd)
                    txtPwd.Password = lui.Password;
            }
        }

        private void onOK(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUser.Text))
            {
                UserName = txtUser.Text;
                Password = txtPwd.Password;
                string errMsg = "";
                if (!TempInfoManager.Instance.Login(UserName, Password, ref errMsg))
                {
                    MessageBox.Show(errMsg, "登录失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                UserRecorder.Instance.StoreUser(new LoginUserInfo(UserName, Password, ckbStorePwd.IsChecked.Value));
                this.DialogResult = true;
                this.Close();
            }
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
