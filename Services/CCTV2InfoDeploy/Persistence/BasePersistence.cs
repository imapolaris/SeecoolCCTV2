using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCTVModels;
using Persistence.Remote;

namespace Persistence
{
    public abstract class BasePersistence<T>
    {
        private InfoSynchronizer<T> _synchronier;
        protected BasePersistence(string section)
        {
            string baseUrl = ConfigurationManager.AppSettings["InfoServiceAddress"];
            _synchronier = new InfoSynchronizer<T>(baseUrl, section);
        }

        private void getUpdate()
        {
            _synchronier.GetUpdate();
        }

        private void update(string key, T info, bool isDeleted)
        {
            _synchronier.PutUpdate(new List<ObjectItem<T>>()
            {
                new ObjectItem<T>() {
                    Key=key,
                    IsDeleted=isDeleted,
                    Item=info
                }
            });
        }

        private void update(Dictionary<string,T> infos,bool isDeleted)
        {
            List<ObjectItem<T>> objs = new List<ObjectItem<T>>();
            foreach(string key in infos.Keys)
            {
                objs.Add(new ObjectItem<T>()
                {
                    Key = key,
                    IsDeleted = isDeleted,
                    Item = infos[key]
                });
            }
            _synchronier.PutUpdate(objs);
        }

        public void Put(string key, T info)
        {
            update(key, info, false);
        }

        public void Put(Dictionary<string,T> infos)
        {
            update(infos, false);
        }

        public T GetInfo(string key)
        {
            getUpdate();
            T info;
            _synchronier.TryGetValue(key, out info);
            return info;
        }

        public IEnumerable<T> GetAllInfos()
        {
            getUpdate();
            return _synchronier.Values;
        }

        public void Update(string key, T info)
        {
            update(key, info, false);
        }

        public void Update(Dictionary<string,T> infos)
        {
            update(infos, false);
        }

        public void Delete(string key)
        {
            T info = GetInfo(key);
            if (info != null)
                update(key, info, true);
        }

        public void Delete(IEnumerable<string> keys)
        {
            foreach(string key in keys)
            {
                Delete(key);
            }
        }
    }
}
