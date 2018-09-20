using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Snow.Orm
{
    /// <summary>
    /// 缓存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
	public class Cache<TKey, TValue> : ICache
    {
        private ConcurrentDictionary<TKey, Entry> Dict;
        private static TValue DefaultValue = default(TValue);
        /// <summary>
        /// 过期分钟
        /// 范围[10-10000]
        /// 0表示60
        /// </summary>
        private uint Minute;

        /// <summary>
        /// 缓存
        /// </summary>
        /// <param name="minute">缓存时间(分钟)</param>
        /// <param name="capacity">容量</param>
        /// <param name="comparer">比较器</param>
        /// <param name="concurrentNum">缓存数量</param>
		public Cache(uint minute = 0, int capacity = 0, IEqualityComparer<TKey> comparer = null, int concurrentNum = 0)
        {
            if (minute <= 0) this.Minute = 60;
            else if (minute > 10000) this.Minute = 10000;
            else this.Minute = minute;

            if (capacity < 30) capacity = 30; else if (capacity > 300000) capacity = 300000;
            if (concurrentNum < 4) concurrentNum = 4; else if (concurrentNum > 16) concurrentNum = 16;
            if (comparer == null) comparer = EqualityComparer<TKey>.Default;
            this.Dict = new ConcurrentDictionary<TKey, Entry>(concurrentNum, capacity, comparer);
        }

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
		public int Count()
        {
            return this.Dict.Count;
        }

        /// <summary>
        /// 添加/替换
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="minute"></param>
		public void Add(TKey key, TValue value, uint minute = 0)
        {
            if (key == null) return;
            Entry en = new Entry(value, minute == 0 ? this.Minute : minute);
            this.Dict.AddOrUpdate(key, en, (x, y) => { return en; });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
		public void Remove(TKey key)
        {
            if (key == null) return;
            Entry en;
            this.Dict.TryRemove(key, out en);
        }
        public void Remove(TKey[] keys)
        {
            if (keys == null) return;
            Entry en;
            foreach (var key in keys)
            {
                this.Dict.TryRemove(key, out en);
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public TValue Get(TKey key)
        {
            if (key == null) return DefaultValue;
            Entry en;
            if (!this.Dict.TryGetValue(key, out en)) return DefaultValue;
            if (en.ticks >= DateTime.Now.Ticks) return en.value;
            this.Dict.TryRemove(key, out en);
            return DefaultValue;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Get(TKey key, ref TValue value)
        {
            if (key == null) return false;
            Entry en;
            if (!this.Dict.TryGetValue(key, out en)) return false;
            if (en.ticks >= DateTime.Now.Ticks) { value = en.value; return true; }
            this.Dict.TryRemove(key, out en);
            return false;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public bool Contains(TKey key)
        {
            if (key == null) return false;
            return this.Dict.ContainsKey(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
		public void Clear()
        {
            this.Dict.Clear();
        }

        private struct Entry
        {
            public TValue value;
            public long ticks;
            public Entry(TValue val, uint minute)
            {
                this.value = val;
                this.ticks = DateTime.Now.AddMinutes(minute).Ticks;
            }
        }
    }


    /// <summary>
    /// 缓存接口
    /// </summary>
	public interface ICache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 
        /// </summary>
        void Clear();
        //void Clean();
    }

    ///// <summary>
    ///// 缓存类型
    ///// </summary>
	//public enum CacheTypes
 //   {
 //       /// <summary>
 //       /// 直接读库，不缓存
 //       /// </summary>
 //       None,
 //       /// <summary>
 //       /// 来自缓存；缓存中没有就直接读库，并缓存
 //       /// </summary>
 //       From,
 //       /// <summary>
 //       /// 直接读库，并缓存
 //       /// </summary>
 //       To
 //       ///// <summary>
 //       ///// 仅删除缓存，不获取数据
 //       ///// </summary>
 //       //Remove
 //   }
}
