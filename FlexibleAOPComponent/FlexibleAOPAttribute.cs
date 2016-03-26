using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleAOPComponent
{
    public class FlexibleAOPAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            FlexibleDynamicProxy proxy = new FlexibleDynamicProxy(serverType);
            proxy.Filter = m => !m.Name.StartsWith("Add");
            proxy.BeforeExecute += Proxy_BeforeExecute;
            proxy.AfterExecute += Proxy_AfterExecute;
            proxy.ErrorExecuting += Proxy_ErrorExecuting;
            return proxy.GetTransparentProxy() as MarshalByRefObject;
        }
        //记录方法调用的函数
        private void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            File.AppendAllLines("Log.txt", new string[] {
                "Execute DateTime : "+DateTime.Now,
                msg, arg.ToString() });
            Console.ResetColor();
        }
        private void Proxy_ErrorExecuting(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e)
        {
            Log(e.MethodName, e.Args.ToString());
        }

        private void Proxy_AfterExecute(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e)
        {
            Log(e.MethodName, e.Args.ToString());
        }

        private void Proxy_BeforeExecute(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e)
        {
            Log(e.MethodName, e.Args.ToString());
        }
    }
}
