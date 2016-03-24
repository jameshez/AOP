using System;

namespace Common
{
    internal class SysLogService
    {
        private static SysLogService _SysLogService;
        private static object obj = new object();
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
                using (LogContext db = new LogContext())
                {
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
            }
        }
    }
}