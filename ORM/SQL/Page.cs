namespace Snow.Orm
{
    public partial class Sql
    {
        /// <summary>
        /// 分页{ 页码,页容量 }
        /// </summary>
        uint[] _Page = new uint[2];

        /// <summary>
        /// 分页读取
        /// </summary>
        /// <param name="index">页码(缺省从0开始)</param>
        /// <param name="size">页容量(缺省每页20条记录)</param>
        /// <returns></returns>
        public Sql Page(uint index = 0, uint size = 20)
        {
            _Page[0] = index;
            _Page[1] = size;
            return this;
        }
        /// <summary>
        /// 读取前n条数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Sql Top(uint size)
        {
            return Page(0, size);
        }
    }
}
