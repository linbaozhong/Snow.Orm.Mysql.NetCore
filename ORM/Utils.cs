namespace Snow.Orm
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
	{
        /// <summary>
        /// 日志
        /// </summary>
		public readonly static log4net.ILog Log;

		static Utils()
		{
            log4net.Config.XmlConfigurator.Configure();
            Log = log4net.LogManager.GetLogger("adeway");
        }
    }
}
