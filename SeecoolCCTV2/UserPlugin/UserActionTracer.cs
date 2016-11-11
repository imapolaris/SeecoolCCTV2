using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common.Log;
using Telerik.Windows.Controls;
using UserModule;
using WindowDispatcherLib;

namespace UserPlugin
{
	class UserActionTracer
	{
		Subject<UserActionData> _subject;

		public UserActionTracer()
		{
			_subject = new Subject<UserActionData>();
			_subject.Buffer(TimeSpan.FromSeconds(30), 30).Subscribe(postActionData);

			EventManager.RegisterClassHandler(typeof(Button), System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(onButtonClick));
			EventManager.RegisterClassHandler(typeof(RadMenuItem), RadMenuItem.ClickEvent, new RoutedEventHandler(onMenuClick));
		}

		void onButtonClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			logAction(button, button.Content, UserActionType.ButtonClick);
		}

		void onMenuClick(object sender, RoutedEventArgs e)
		{
			var menu = sender as RadMenuItem;
			logAction(menu, menu.Header, UserActionType.MenuClick);
		}

		void logAction(FrameworkElement cmdItem, object content, UserActionType actionType)
		{
			var dataContext = getDataContext(cmdItem);
			if (dataContext == null)
				return;
			var actionData = new UserActionData()
			{
				UserName = UserManager.Instance.CurrentUser.UserName,
				Time = DateTime.Now,
				ActionType = actionType,
				Name = getData(content, i => i.ToString()),
				Content = getData(dataContext, i => i.GetType().FullName),
				ToolTip = getData(cmdItem.ToolTip, i => i.ToString()),
			};

			_subject.OnNext(actionData);
		}

		void postActionData(IList<UserActionData> actions)
		{
			if (actions.Count == 0)
				return;
			actions.Select(n => $"{n.UserName} {n.Time} {n.Name} {n.Content}").ForEach(n => Logger.Default.Trace(n));
		}

		//TODO 等用C# 6.0后就用 ?. 把这个函数干掉
		TValue getData<T, TValue>(T obj, Func<T, TValue> msgFactory)
		{
			if (obj == null)
				return default(TValue);
			else
				return msgFactory(obj);
		}

		object getDataContext(FrameworkElement cmdItem)
		{
			FrameworkElement content = cmdItem;

			if ((cmdItem.DataContext == null) || (cmdItem.DataContext is ICommand))
			{
				content = cmdItem.FindVisualParent<Panel>();
			}

			return getData(content, i => i.DataContext);
		}
	}
}
