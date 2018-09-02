using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    public class Ex : Exception
    {
        public Ex(string message, int n = Unknown) : base(string.Format("(HRESULT:0x{1:X8}) {0}", message, n))
        {
            HResult = n;
        }
        /// <summary>
        /// 未知错误
        /// </summary>
        public const int Unknown = 10000;
        /// <summary>
        /// 语法错误
        /// </summary>
        public const int Syntax = 10010;
        /// <summary>
        /// 空参数
        /// </summary>
        public const int Null = 10020;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int BadParameter = 10030;
        /// <summary>
        /// 没有找到目标数据
        /// </summary>
        public const int NotFound = 11000;
    }
}
