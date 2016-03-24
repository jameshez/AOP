using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common
{
    [Table("Log")]
    internal class Log
    {
        [Key]
        public int ID { get; set; }
        public string Args { get; internal set; }
        public string CallContext { get; internal set; }
        public string IP { get; internal set; }
        public string Mac { get; internal set; }
        public string MethodName { get; internal set; }
        public string MethodSignature { get; internal set; }
        public string Project { get; internal set; }
        public string ReturnValue { get; internal set; }
        public string TypeName { get; internal set; }
        public string UseTime { get; internal set; }
    }
}