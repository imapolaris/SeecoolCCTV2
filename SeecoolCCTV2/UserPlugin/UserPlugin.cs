using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common.Command;
using Common.Message;
using Common.Util;
using PluginBaseLib.Interface;
using PluginBaseLib.ViewModels;
using UserModule;
using UserPlugin.Dialogs;
using WindowDispatcher.DispatcherLib;

namespace UserPlugin
{

    class UserPlugin : ViewModelPlugin, IUserManager
    {
        UserActionTracer _tracer;
        EventPublisher _publisher = new EventPublisher();
        User _currentUser;

        public DelegateCommand LogoutCommand { get; private set; }

        public IEnumerable<object> CurrentRoles { get; private set; }

        public IUserData CurrentUser { get { return _currentUser; } }

        public bool IsCurrentUserAdmin { get { return _currentUser?.UserName == "admin"; } }

        public UserPlugin()
        {
            LogoutCommand = createCommand();
        }

        DelegateCommand createCommand()
        {
            return new DelegateCommand(executeLogout, canExecuteLogout);
        }

        bool canExecuteLogout(object arg)
        {
            return _currentUser != null;
        }

        private void executeLogout(object obj)
        {
            _currentUser = null;
            _publisher.NotifyUserLogout();
            //todo
        }

        public override void Exit()
        {
            _publisher.NotifyUserLogout();
        }

        public bool HasPermission(string category, PermissionType pt)
        {
            //mock
            if (category == "TestPlugin1.TestCommand1"
                && pt == PermissionType.Operate
                && _currentUser != null
                && _currentUser.UserName == "wyj")
                return false;

            return true;
        }

        public override void Init()
        {
            login();
        }

        async void login()
        {
            await DialogDispatcher.UIInitCompletion.Task;
            WindowUtil.BeginInvoke(doLogin);
        }

        async void doLogin()
        {//TODO:屏蔽登录界面
         //_currentUser = await handleLogin("admin", "");
         //         if (_currentUser != null)
         //             _tracer = new UserActionTracer();
         //         _publisher.NotifyUserLogin();
         //         updatePermission();
            string aa = "admin";
            string str1 = LocalEncryptor.Instance.Encrypt(aa);
            string str2 = LocalEncryptor.Instance.Decrypt(str1);
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            var a = UserManager.Instance.CurrentUser;
            var dlg = new LoginWnd();
            var dr = DialogDispatcher.ShowDialog(dlg);
            if (dr == true)
            {
                _currentUser = await handleLogin(dlg.UserName, dlg.Password);
                if (_currentUser != null)
                    _tracer = new UserActionTracer();
                else
                    return;

                _publisher.NotifyUserLogin();
                updatePermission();
            }
            else
                DialogDispatcher.CloseMainWindow();

        }

        async void updatePermission()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));// TODO:获取用户权限

            WindowUtil.BeginInvoke(_publisher.NotifyPermissionUpdated);
            CommandManager.InvalidateRequerySuggested();
        }

        //mock
        async Task<User> handleLogin(string name, string pwd)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            return new User() { UserName = name, Id = 1 };
            //if (name.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            //	return new User() { UserName = name, Id = 1 };
            //else if (name.Equals("wyj", StringComparison.CurrentCultureIgnoreCase))
            //	return new User() { UserName = name, Id = 2 };
            //else
            //	return null;
        }

        public void SubscribeLogout(EventHandler<EventArgs> handler)
        {
            registNotify("OnUserLogout", handler);

        }

        public void SubscribeLongin(EventHandler<EventArgs> handler)
        {
            registNotify("OnUserLogin", handler);
        }

        public void SubscribePermissionUpdated(EventHandler<EventArgs> handler)
        {
            registNotify("OnPermissionUpdated", handler);
        }

        void registNotify(string eventName, EventHandler<EventArgs> handler)
        {
            WeakEventManager<EventPublisher, EventArgs>.AddHandler(_publisher, eventName, handler);
        }

        public bool Authenticate(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        class EventPublisher
        {
            public event EventHandler OnUserLogin;
            public event EventHandler OnUserLogout;
            public event EventHandler OnPermissionUpdated;


            public void NotifyUserLogin()
            {
                notifyEvent(OnUserLogin);
            }

            public void NotifyUserLogout()
            {
                notifyEvent(OnUserLogout);
            }

            public void NotifyPermissionUpdated()
            {
                notifyEvent(OnPermissionUpdated);
            }

            void notifyEvent(EventHandler handler)
            {
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
        }
    }
}
