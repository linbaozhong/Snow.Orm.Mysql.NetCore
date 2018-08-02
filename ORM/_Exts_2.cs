using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    public static partial class _Exts
    {
        #region
        public static string Get(this IFormCollection form, string key)
        {
            StringValues val;
            if (form.TryGetValue(key, out val)) return val[0];
            return "";
        }
        public static string Get(this IQueryCollection query, string key)
        {
            StringValues val;
            if (query.TryGetValue(key, out val)) return val[0];
            return "";
        }

        #endregion
    }
}
