using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public class ObjectPool<T>
    {
        /// <summary>  
        /// ConcurrentBag<T> 的所有公共且受保护的成员都是线程安全的  
        /// 可从多个线程同时使用。  
        /// </summary>  
        ConcurrentBag<T> buffer;
        Func<T> creatFunc;
        Action<T> resetFunc;
        public int capacity { get; private set; }
        public int count { get { return buffer.Count(); } }

        public ObjectPool(Func<T> creatFunc, Action<T> resetFunc, int capacity)
        {
            this.buffer = new ConcurrentBag<T>();
            this.creatFunc = creatFunc;
            this.resetFunc = resetFunc;
            this.capacity = capacity;
        }

        /// <summary>  
        /// 申请对象，若有从池中移除并返回取出的对象  
        /// 若没有则创建新的对象，并返回该对象  
        /// </summary>  
        /// <returns></returns>  
        public T GetObject()
        {
            T obj;
            if (buffer.TryTake(out obj))
            {
                return obj;
            }
            else
                return creatFunc();
        }
        public bool PutObject(T obj)
        {
            if (count >= capacity)
            {
                return false;
            }
            resetFunc.Invoke(obj);
            buffer.Add(obj);
            return true;
        }
    }
}
