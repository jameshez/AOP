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
        private DateTime _StartTime;
        private DateTime _EndTime;
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            FlexibleDynamicProxy proxy = new FlexibleDynamicProxy(serverType);
            proxy.Filter = m => m.Name.StartsWith("Add");
            proxy.BeforeExecute += Proxy_BeforeExecute;
            proxy.AfterExecute += Proxy_AfterExecute;
            proxy.ErrorExecuting += Proxy_ErrorExecuting;
            return proxy.GetTransparentProxy() as MarshalByRefObject;
        }
        //记录方法调用的函数
        private void Log(string[] parameters)
        {
            File.AppendAllLines("Log.txt", parameters);
        }
        private void Proxy_ErrorExecuting(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e, object innerException)
        {
            _EndTime = DateTime.Now;
            //Log(e.MethodName, e.Args);
            string[] parameters = new string[] {
                //"Executed Method     -> "+e.MethodName,
                "Exception Message   -> "+innerException.ToString(),
                "Start DateTime      -> " +_StartTime,
                "End DateTime        -> " +_EndTime,
                "Used Time           -> "+(_EndTime-_StartTime).Seconds.ToString(),
                "------------------------------------------------------------------"
            };
            Log(parameters);
        }

        private void Proxy_AfterExecute(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e, object returnValue)
        {
            _EndTime = DateTime.Now;
            string[] parameters = new string[5];
            if (returnValue != null)
            {
                if (!returnValue.GetType().IsGenericType)
                {
                    parameters = new string[] {
                        "Returned Value      -> " + returnValue.ToString(),
                        "Start DateTime      -> " +_StartTime,
                        "End DateTime        -> " +_EndTime,
                        "Used Time           -> " +(_EndTime-_StartTime).TotalMilliseconds.ToString(),
                        "------------------------------------------------------------------" };
                }
                else
                {
                    //    int index = 0;
                    //    StringBuilder args = new StringBuilder();
                    //    foreach (string arg in returnValue.GetType().)
                    //    {
                    //        args.Append(e.GetInArgName(index++));
                    //        args.Append(" : ");
                    //        args.Append(arg + ",");
                    //    }
                    //    File.AppendAllLines("Log.txt", new string[] {
                    //"Execute DateTime : "+DateTime.Now,
                    //e.MethodName, args.ToString() });
                }
            }
            else
            {
                parameters = new string[] {
                    "Start DateTime      -> " +_StartTime,
                    "End DateTime        -> " +_EndTime,
                    "Used Time           -> " +(_EndTime-_StartTime).TotalMilliseconds.ToString(),
                    "------------------------------------------------------------------" };
            }

            Log(parameters);
        }

        private void Proxy_BeforeExecute(object sender, System.Runtime.Remoting.Messaging.IMethodCallMessage e)
        {
            _StartTime = DateTime.Now;
            int index = 0;
            StringBuilder args = new StringBuilder();
            foreach (object arg in e.Args)
            {
                args.Append(e.GetInArgName(index++));
                args.Append(" : ");
                args.Append(arg + ",");
            }

            string[] parameters = new string[] {
                "Execute Method Name -> "+e.MethodName,
                "Passed Parameters   -> "+args.ToString() };
            Log(parameters);
        }
    }
}
