using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCTVReplay.Util
{
    public class CommandDelegate : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action<object> _del;

        public CommandDelegate(Action<object> del)
        {
            _del = del;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _del(parameter);
        }
    }
}
