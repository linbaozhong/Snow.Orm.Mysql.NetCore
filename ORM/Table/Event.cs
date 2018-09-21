using System;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        Action<long> _OnInsert = id => { };
        Action<long> _OnUpdate = id => { RowCache.Remove(id); };
        Action<long> _OnDelete = id => { RowCache.Remove(id); };
        /// <summary>
        /// insert 以后
        /// </summary>
        public void OnInsert()
        {
        }
        /// <summary>
        /// update 以后
        /// </summary>
        /// <param name="id"></param>
        public void OnUpdate(long id)
        {
            RowCache.Remove(id);
        }
        public void OnUpdate(long[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            RowCache.Remove(ids);
        }

        public void OnDelete(long id)
        {
            RowCache.Remove(id);
        }
        public void OnDelete(long[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            RowCache.Remove(ids);
        }
        public void OnUpdate(T bean, string orderby = null, uint count = 1000)
        {
            ListCache.Remove(CombineCacheKey(bean, orderby, count));
        }
        public void OnUpdate<V>(string col, V val, string orderby = null, uint count = 1000)
        {
            ListCache.Remove(CombineCacheKey(col, val, orderby, count));
        }
        public void OnUpdate(Sql cond)
        {
            ListCache.Remove(CombineCacheKey(cond));
            if (!cond.Disposed) cond.Dispose();
        }
    }
}
