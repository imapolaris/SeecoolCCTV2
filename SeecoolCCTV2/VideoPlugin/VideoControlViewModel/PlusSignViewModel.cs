using AopUtil.WpfBinding;
using Common.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoNS
{
    public class PlusSignViewModel : ObservableObject
    {
        [AutoNotify]
        public bool IsVisible { get; set; } = true;

        public ICommand PlusCommand { get; set; }
    }
}
