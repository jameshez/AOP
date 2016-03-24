using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class LogContext:DbContext
    {
        public DbSet<Log> Logs { get; set; }
    }
}
