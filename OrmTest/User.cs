using Snow.Orm;

namespace OrmTest.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [OrmTable("user")]
    public class User : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public static Table<User> Table = new Table<User>(TableTypes.Large);
        public static class Field
        {
            public readonly static string Id = nameof(User.Id);
            public readonly static string Nick = nameof(User.Nick);
            public readonly static string BirthDay = nameof(User.BirthDay);
        }

        /// <summary>
        /// ID
        /// </summary>
        [OrmColumn("id")]
        public int Id { set { Set(value); } get { return Get<int>(); } }
        /// <summary>
        /// 昵称
        /// </summary>
        [OrmColumn("nick")]
        public string Nick { set { Set(value); } get { return Get<string>(); } }
        public string Photo { set { Set(value); } get { return Get<string>(); } }
        /// <summary>
        /// 公司
        /// </summary>
        [OrmColumn("birthday")]
        public int BirthDay { set { Set(value); } get { return Get<int>(); } }
        public int WorkDay { set { Set(value); } get { return Get<int>(); } }
        public int Sex { set { Set(value); } get { return Get<int>(); } }
        public int StarSign { set { Set(value); } get { return Get<int>(); } }
        public int Profession { set { Set(value); } get { return Get<int>(); } }
        public string Position { set { Set(value); } get { return Get<string>(); } }
        public string Company { set { Set(value); } get { return Get<string>(); } }
        public string School { set { Set(value); } get { return Get<string>(); } }
        public string Hometown { set { Set(value); } get { return Get<string>(); } }
        public float Longitude { set { Set(value); } get { return Get<float>(); } }
        public float Latitude { set { Set(value); } get { return Get<float>(); } }
        public string Descr { set { Set(value); } get { return Get<string>(); } }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Goods : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public static Table<Goods> Table = new Table<Goods>(TableTypes.Large);

        /// <summary>
        /// 
        /// </summary>
        public string Name { set; get; }
    }

}