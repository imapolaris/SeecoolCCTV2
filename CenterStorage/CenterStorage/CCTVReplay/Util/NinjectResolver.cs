using CCTVReplay.Interface;
using CCTVReplay.Temp;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVReplay.AutoSave;

namespace CCTVReplay.Util
{
    public class NinjectResolver
    {
        private static NinjectResolver _instance;
        public static NinjectResolver Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NinjectResolver();
                return _instance;
            }
        }

        private IKernel _kernel;
        public NinjectResolver()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        private void AddBindings()
        {
            //添加绑定。
            //_kernel.Bind<ISourcePersistence>().To<MemorySourceStorage>().InSingletonScope();
            _kernel.Bind<ISourcePersistence>().To<FileSourcePersistence>().InSingletonScope();
        }

        public T Get<T>()
        {
            return _kernel.TryGet<T>();
        }
    }
}
