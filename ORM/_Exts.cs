using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class _Exts
    {

        #region Convert
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
        public static string ToStr(this object obj, string dft = null) { if (obj == null) return dft; return obj.ToString(); }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft">当是null/empty/空白时的默认值</param>
        /// <returns></returns>
        public static string ToStr(this string str, string dft = null) { if (string.IsNullOrWhiteSpace(str)) return dft; return str; }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft">无法转化时的默认值</param>
        /// <param name="start">开始位置，默认0；可为负数，-n表示倒数第几个</param>
        /// <returns></returns>
        public static string ToStr(this byte[] bytes, string dft = null, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return System.Text.Encoding.UTF8.GetString(bytes, start, bytes.Length - start); }
            catch { return dft; }
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static int ToInt(this object obj, int dft = 0)
        {
            if (obj == null) return dft;
            try { return System.Convert.ToInt32(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static int ToInt(this string str, int dft = 0)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToInt32(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static int ToInt(this byte[] bytes, int dft = 0, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return BitConverter.ToInt32(bytes, start); }
            catch { return dft; }
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static uint ToUInt(this object obj, uint dft = 0u)
        {
            if (obj == null) return dft;
            try { return System.Convert.ToUInt32(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static uint ToUInt(this string str, uint dft = 0u)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToUInt32(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static uint ToUInt(this byte[] bytes, uint dft = 0u, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return BitConverter.ToUInt32(bytes, start); }
            catch { return dft; }
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static long ToLong(this object obj, long dft = 0L)
        {
            if (obj == null) return dft;
            try { return System.Convert.ToInt64(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static long ToLong(this string str, long dft = 0L)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToInt64(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static long ToLong(this byte[] bytes, long dft = 0L, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return BitConverter.ToInt64(bytes, start); }
            catch { return dft; }
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static float ToFloat(this object obj, float dft = 0f)
        {
            if (obj == null) return dft;
            try { return System.Convert.ToSingle(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static float ToFloat(this string str, float dft = 0f)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToSingle(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static float ToFloat(this byte[] bytes, float dft = 0f, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return BitConverter.ToSingle(bytes, start); }
            catch { return dft; }
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static double ToDouble(this object obj, double dft = 0d)
        {
            if (obj == null) return dft;
            try { return System.Convert.ToDouble(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static double ToDouble(this string str, double dft = 0d)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToDouble(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static double ToDouble(this byte[] bytes, double dft = 0d, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return BitConverter.ToDouble(bytes, start); }
            catch { return dft; }
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static bool ToBool(this object obj, bool dft = false)
        {
            if (obj == null) return dft;
            if (obj is bool) return (bool)obj;
            if (obj is string) return ToBool(obj.ToString(), dft);
            if (obj is IConvertible)
            {
                int n = ((IConvertible)obj).ToInt32(null);
                if (n == 0) return false;
                if (n == 1) return true;
            }
            return dft;
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static bool ToBool(this string str, bool dft = false)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            str = str.Trim();
            if (str == "false" || str == "0" || str == "False") return false;
            if (str == "true" || str == "1" || str == "True") return true;
            return dft;
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static bool ToBool(this byte[] bytes, bool dft = false, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            if (bytes[start] == 0) return false;
            if (bytes[start] == 1) return true;
            return dft;
        }


        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static DateTime ToDateTime(this object obj, DateTime dft)
        {
            if (obj == null) return dft;
            if (obj is DateTime) return (DateTime)obj;
            try { return System.Convert.ToDateTime(obj); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dft"></param>
        /// <returns></returns>
		public static DateTime ToDateTime(this string str, DateTime dft)
        {
            if (string.IsNullOrWhiteSpace(str)) return dft;
            try { return System.Convert.ToDateTime(str); }
            catch { return dft; }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="dft"></param>
        /// <param name="start"></param>
        /// <returns></returns>
		public static DateTime ToDateTime(this byte[] bytes, DateTime dft, int start = 0)
        {
            if (bytes == null || bytes.Length < 1 || start >= bytes.Length) return dft;
            if (start < 0 && (start += bytes.Length) < 0) start = 0;
            try { return DateTime.FromBinary(BitConverter.ToInt64(bytes, start)); }
            catch { return dft; }
        }
        /// <summary>
        /// 获取指定格式的时间字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTime(this object obj, string format)
        {
            if (obj == null) return null;
            try { return System.Convert.ToDateTime(obj).ToString(format); }
            catch { return null; }
        }
        /// <summary>
        /// 获取指定格式的时间字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTime(this string str, string format)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            try { return System.Convert.ToDateTime(str).ToString(format); }
            catch { return null; }
        }
        /// <summary>
        /// 转为Unix时间戳
        /// </summary>
        /// <param name="dt">DateTime时间格式</param>
        /// <returns></returns>
        public static int ToUnixTimestamp(this DateTime dt)
        {
            var start = TimeZoneInfo.Local.ToDateTime(new DateTime(1970, 1, 1));
            return (dt - start).TotalSeconds.ToInt();
        }
        /// <summary>
        /// Unix时间戳转DateTime
        /// </summary>
        /// <param name="ts">Unix时间戳</param>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(this int ts)
        {
            var start = TimeZoneInfo.Local.ToDateTime(new DateTime(1970, 1, 1));
            return start.Add(new TimeSpan(ts * 10000000));
        }

        /// <summary>
        /// 将指定对象转换为货币元，精确到分
        /// </summary>
        /// <param name="obj">货币</param>
        /// <param name="def">缺省值</param>
        /// <returns></returns>
        public static decimal ToMoney(this object obj, decimal def = 0)
        {
            if (obj == null) return def;
            try
            {
                return Math.Round(Convert.ToDecimal(obj), 2);
            }
            catch (Exception)
            {
                return def;
            }
        }
        #endregion



        #region string
        /// <summary>
        /// 截除前后空白
        /// </summary>
        /// <param name="str"></param>
        /// <param name="position">截取的位置：0前后，1后，-1前</param>
        /// <returns></returns>
        public static string Trim(this string str, int position)
        {
            if (str == null) return str;
            if (position == 0) return str.Trim();
            if (position > 0) return str.TrimEnd();
            return str.TrimStart();
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start">开始位置；可为负数，-n表示倒数第几个</param>
        /// <param name="count">截取数量，默认0；小于等于0或超出范围时，从start开始到结束</param>
        /// <param name="whitespace">截取前先去处头尾空白</param>
        /// <returns></returns>
        public static string Substr(this string str, int start, int count = 0, bool whitespace = true)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (whitespace) str = str.Trim();
            int len = str.Length;
            if (start < 0 && (start += len) < 0) start = 0;
            else if (start >= len) return null;
            if (count < 1) count = len - start;
            else if (count + start > len) count = len - start;
            return str.Substring(start, count);
        }
        #endregion


        #region Dictionary
        /// <summary>
        /// 将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryIns<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, value);
            return dict;
        }
        /// <summary>
        /// 将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常；线程安全
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public static Dictionary<TKey, TValue> TryInsSync<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            lock (((System.Collections.ICollection)dict).SyncRoot)
            {
                if (!dict.ContainsKey(key)) dict.Add(key, value);
            }
            return dict;
        }

        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换；线程安全
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public static Dictionary<TKey, TValue> TryAddSync<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            lock (((System.Collections.ICollection)dict).SyncRoot) { dict[key] = value; }
            return dict;
        }

        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// </summary>
        public static TValue TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue val;
            return dict.TryGetValue(key, out val) ? val : defaultValue;
        }
        #endregion


        #region Array
        public readonly static char[] ItemsColPart = new char[] { '\n' };
        public static string[] ToArray(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            return source.Split(ItemsColPart, StringSplitOptions.RemoveEmptyEntries);
        }
        public static long[] ToIds(this string source)
        {
            var _items = source.ToArray();
            if (_items == null || _items.Length < 1) return null;
            var _len = _items.Length;
            List<long> ids = new List<long>(_len);
            for (int i = 0; i < _len; i++)
            {
                long id = _items[i].ToLong(-1);
                if (id > -1) ids.Add(id);
            }
            return ids.ToArray();
        }
        /// <summary>
        /// 联接成字符串
        /// </summary>
        /// <param name="source">数组</param>
        /// <param name="start">开始位置；可为负值，-1表示最后一个</param>
        /// <param name="count">截取数量，小于等于0表示从开始位置到最后</param>
        /// <param name="part">分隔符</param>
        /// <returns></returns>
        public static string Join(this Array source, int start = 0, int count = 0, string part = ",")
        {
            if (source == null) return null;
            int len = source.Length; if (len < 1) return null;
            if (start < 0) { start += len; if (start < 0) start = 0; }
            else if (start >= len) return null;
            if (count > 0 && count + start < len) len = count + start;
            StringBuilder sb = new StringBuilder(6 * (len - start));
            for (int i = start; i < len; i++) { if (sb.Length > 0) sb.Append(part); sb.Append(source.GetValue(i)); }
            return sb.ToString();
        }
        /// <summary>
        /// 联接成字符串
        /// </summary>
        /// <param name="source">数组</param>
        /// <param name="start">开始位置；可为负值，-1表示最后一个</param>
        /// <param name="count">截取数量，小于等于0表示从开始位置到最后</param>
        /// <param name="part">分隔符</param>
        /// <returns></returns>
        public static string Join<T>(this T[] source, int start = 0, int count = 0, string part = ",")
        {
            if (source == null) return null;
            int len = source.Length; if (len < 1) return null;
            if (start < 0) { start += len; if (start < 0) start = 0; }
            else if (start >= len) return null;
            if (count > 0 && count + start < len) len = count + start;
            StringBuilder sb = new StringBuilder(6 * (len - start));
            for (int i = start; i < len; i++) { if (sb.Length > 0) sb.Append(part); sb.Append(source[i]); }
            return sb.ToString();
        }
        /// <summary>
        /// 联接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start">开始位置，负数当作0</param>
        /// <param name="count">截取数量，小于等于0表示从开始位置到最后</param>
        /// <param name="part">分隔符</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, int start = 0, uint count = 0, string part = ",")
        {
            if (source == null) return null;
            StringBuilder sb = new StringBuilder(300);
            uint num = 0;
            foreach (T t in source)
            {
                if (start > 0) { start--; continue; }
                if (count > 0)
                {
                    if (++num > count) break;
                }
                if (sb.Length > 0) sb.Append(part);
                sb.Append(t);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 数组的长度
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Count(this Array array)
        {
            if (array == null) return 0;
            return array.Length;
        }
        #endregion

        public static void Dispose<T>(this T bean,bool disposing)where T:BaseEntity
        {
            if (disposing)
            {
                bean.Disposed = true;
                ObjectPool.Put(bean);
            }
        }
        public static void Dispose<T>(this T bean) where T : BaseEntity
        {
            bean.Dispose(true);
        }

    }
}
