using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        public void OnChange()
        {
            Task.Run(() =>
            {
                ListCache.Clear();
            });
        }
        public void OnChange(long id)
        {
            Task.Run(() =>
            {
                RowCache.Remove(id);
                ListCache.Clear();
            });
        }
        public void OnChange(long[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            Task.Run(() =>
            {
                RowCache.Remove(ids);
                ListCache.Clear();
            });
        }

        #region Cache
        public void RemoveCache(long id)
        {
            Task.Run(() =>
            {
                RowCache.Remove(id);
            });
        }
        public void RemoveCache(long[] ids)
        {
            Task.Run(() =>
            {
                RowCache.Remove(ids);
            });
        }
        public void RemoveListCache(T bean, string orderby = null, uint count = 1000)
        {
            Task.Run(() =>
            {
                ListCache.Remove(CombineCacheKey(bean, orderby, count));
            });
        }
        public void RemoveListCache<V>(string col, V val)
        {
            Task.Run(() =>
            {
                ListCache.Remove(CombineCacheKey(col, val));
            });
        }
        public void RemoveListCache(Sql cond)
        {
            Task.Run(() =>
            {
                ListCache.Remove(CombineCacheKey(cond));
                if (!cond.Disposed) cond.Dispose();
            });
        }
        #endregion

    }
}
