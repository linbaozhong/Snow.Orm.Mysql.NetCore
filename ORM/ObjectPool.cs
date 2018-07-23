﻿using System;
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
        ConcurrentQueue<T> buffer;
        Func<T> creatFunc;
        Action<T> resetFunc;
        public int capacity { get; private set; }
        public int count { get { return buffer.Count(); } }

        public ObjectPool(Func<T> creatFunc, Action<T> resetFunc, int capacity)
        {
            this.buffer = new ConcurrentQueue<T>();
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
            if (buffer.TryDequeue(out obj))
            {
                resetFunc.Invoke(obj);
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
            buffer.Enqueue(obj);
            return true;
            //return false;
        }
    }

    #region IPoolable
    public interface IPoolable : IDisposable
    {
        /// <summary>
        /// 在该Create方法中，编写创建对象的时间代价高的逻辑，从数据库中获取各种东西等。这个方法在ObjectPool类的默认构造函数之后被调用一次
        /// </summary>
        void Create();
        /// <summary>
        /// 用于初始化您的对象。无论何时从ObjectPool类中提供对象，都会调用它。使用此方法将您的对象重置为其初始状态。
        /// 例如，添加事件，初始化变量。将代码保持在最小值New。它应该尽快返回，以保持它的效率。
        /// 有时候，你会保持这个方法是空的，并把所有的非初始化逻辑放入Deposit，当你把对象返回到对象池的时候调用它
        /// </summary>
        void Reset();
    }
    #endregion

    #region ObjectPool
    public class ObjectPool
    {
        private static ConcurrentDictionary<System.Type, PoolableObject> pools = new ConcurrentDictionary<Type, PoolableObject>();

<<<<<<< HEAD
        #region Get
=======
>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public static T Get<T>() where T : IPoolable, new()
        {
            T x = default(T);
            var t = typeof(T);
            PoolableObject po;

            if (pools.TryGetValue(t, out po))
            {
                x = (T)po.Pop();
            }
            else
            {
                pools[t] = new PoolableObject(10);
            }

            if (x == null)
            {
                x = new T();
                x.Create();
            }

            x.Reset();

            return x;
        }

<<<<<<< HEAD
        #endregion

        #region PutObject
=======
>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public static void Put<T>(T obj) where T : IPoolable
        {
            var t = typeof(T);
            PoolableObject po;
            if (pools.TryGetValue(t, out po))
            {
                if (po.Push(obj))
<<<<<<< HEAD
                {
                    GC.SuppressFinalize(obj);
                }
=======
                    GC.SuppressFinalize(obj);
>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
            }
            else
            {
                throw new Exception("ObjectPool.Put can not be called for object which is not created using ObjectPool.Get");
            }
        }

<<<<<<< HEAD
        #endregion

        #region Clear

=======
>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public static void Clear()
        {
            lock (pools)
            {
                foreach (PoolableObject po in pools.Values)
                {
                    po.Clear();
                }

                pools.Clear();
            }
        }
<<<<<<< HEAD
        #endregion

    }
    #endregion

    #region PoolableObject
    public class PoolableObject
    {
        #region Data Members
        private ConcurrentStack<IPoolable> pool;
        private int capacity;
        #endregion

        #region Ctor
=======
    }
    #endregion
    #region PoolableObject
    public class PoolableObject
    {
        private ConcurrentStack<IPoolable> pool;
        private int capacity;

>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public PoolableObject(int capacity)
        {
            pool = new ConcurrentStack<IPoolable>();
            this.capacity = capacity;
        }
<<<<<<< HEAD
        #endregion

        #region Properties
=======

>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public Int32 Count
        {
            get { return pool.Count; }
        }
<<<<<<< HEAD
        #endregion

        #region Pop
=======

>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public IPoolable Pop()
        {
            IPoolable obj;
            if (pool.TryPop(out obj))
            {
                return obj;
            }

            return null;
        }
<<<<<<< HEAD
        #endregion

        #region Push
=======

>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public bool Push(IPoolable obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Items added to a Pool cannot be null");
            }
            if (Count < capacity)
            {
                pool.Push(obj);
                return true;
            }
            return false;
        }
<<<<<<< HEAD
        #endregion

        #region Clear
=======

>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
        public void Clear()
        {
            pool.Clear();
        }
<<<<<<< HEAD
        #endregion
    }
=======
    }

    #region 对象缓存处理(暂时不可用)
    //public static partial class _Exts
    //{
    //    public static void Dispose<T>(this T bean) where T : BaseEntity, IPoolable
    //    {
    //        bean.Disposed = true;
    //        ObjectPool.Put(bean);
    //    }
    //}
    #endregion
>>>>>>> 2dd96190f51adce19dcc96baddbb00e12584040c
    #endregion
}
