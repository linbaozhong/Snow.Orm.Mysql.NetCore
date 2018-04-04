namespace Snow.Orm
{
    /// <summary>
    /// 库集合
    /// </summary>
    public static class dbs
    {
        public static DB Db = new DB("Data Source=localhost;database=rwt;user id=root;password=adeway;Pooling=true;Max Pool Size=100;Min Pool Size=1;default command timeout=20;characterset=utf8"
            , 0, true);
    }
}
