using System;

namespace Common
{
    internal class SysLogService
    {
        private static SysLogService _SysLogService;
        private static object obj = new object();
        private static LogContext _DB = new LogContext();
        internal static SysLogService GetInstance()
        {
            if (_SysLogService == null)
            {
                lock (obj)
                {
                    if (_SysLogService == null)
                    {
                        _SysLogService = new SysLogService();
                    }
                }
            }
            return _SysLogService;
        }

        private SysLogService()
        { }

        public void InsertAccessLog(Log log)
        {
            lock (obj)
            {
                _DB.Logs.Add(log);
                _DB.SaveChanges();
            }
        }
    }
}