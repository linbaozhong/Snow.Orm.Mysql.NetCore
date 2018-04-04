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
		public readonly static log4net.ILog Log = log4net.LogManager.GetLogger("adeway", "");

		static Utils()
		{
            log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository("adeway"));
        }
    }
}
