using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Imps.Services.CommonV4
{
    sealed class RemotingCachedObjectPool
    {
        private string _uri;
        private int _size;
        private Type _type;

        private int _counter;
        private int _expiredSeconds;
        private RemotingCachedObject[] _objects;

        public int Size
        {
            get { return _size; }
        }

        public string Uri
        {
            get { return _uri; }
        }

        public RemotingCachedObjectPool(string uri, int size, Type type, int expiredSecond)
        {
            _uri = uri;
            _counter = 0;
            _size = size;
            _type = type;
            _expiredSeconds = expiredSecond;

            if (_size < 0)
                throw new InvalidOperationException("Object Count Invaild: " + _size);

            Activate();
        }

        public void Activate()
        {
            //
            // when ipc object only wanna use a clustermode, use this mode
            if (_size == 0)
                return;

            // 
            // Initialize Remoting PoolObject
            _objects = new RemotingCachedObject[_size];
            DateTime expiredTime = DateTime.Now.AddSeconds(_expiredSeconds);
            for (int i = 0; i < _size; i++)
            {
                _objects[i] = new RemotingCachedObject();
                _objects[i].Object = Activator.GetObject(_type, _uri.Trim());
                _objects[i].ExpiredTime = expiredTime;
            }
        }

        public T GetProxyObject<T>()
        {
            if (_size == 1)
            {
                //
                // single object
                return (T)_objects[0].Object;
            }
            else if (_size > 1)
            {
                //
                // auto use cache load balance 
                int i;
                unchecked
                {
                    i = Interlocked.Increment(ref _counter);
                    if (i < 0)
                        i = -i;
                }
                var pool = _objects[_counter % _size];
                DateTime now = DateTime.Now;
                if (now > pool.ExpiredTime)
                {
                    pool.Object = Activator.GetObject(_type, _uri.Trim());
                    pool.ExpiredTime = now.AddSeconds(_expiredSeconds);
                }
                return (T)pool.Object;
            }
            else
            {
                throw new InvalidOperationException("RemotingObjectPool size=0");
            }
        }

        private class RemotingCachedObject
        {
            public object Object;
            public DateTime ExpiredTime;
        }
    }
}
