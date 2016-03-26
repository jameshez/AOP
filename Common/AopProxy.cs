using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Common
{
    public class AOPProxy : RealProxy
    {
        /// <summary>
        /// AOP协议用于记录运行日志
        /// </summary>
        /// <param name="serverType"></param>
        public AOPProxy(Type serverType)
            : base(serverType)
        { }

        public override IMessage Invoke(IMessage msg)
        {
            //方法启动时间
            DateTime startTime = DateTime.Now;
            //记录方法返回结果
            string returnvalue = "null";
            Log log = new Log();
            //log.Uri = msg.Properties["__Uri"].ToString();
            log.IP = "";
            log.Mac = "";
            log.MethodName = msg.Properties["__MethodName"].ToString();
            log.MethodSignature = msg.Properties["__MethodSignature"].ToString();
            log.TypeName = msg.Properties["__TypeName"].ToString();
            log.CallContext = msg.Properties["__CallContext"].ToString();
            log.Project = "AOPTestClient";
            object returnIMessage = null;
            if (msg is IConstructionCallMessage)
            {
                //构造函数调用
                IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
                IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)msg);
                RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
                //记录方法输入参数
                object[] args = constructCallMsg.Args;
                for (int i = 0; i < args.Length; i++)
                {
                    log.Args += args[i].ToString() + ",";
                }
                returnIMessage = constructionReturnMessage;
            }
            else
            {
                //非构造函数调用

                IMethodCallMessage callMsg = msg as IMethodCallMessage;
                IMessage message;
                try
                {
                    object[] args = callMsg.Args;
                    object o = callMsg.MethodBase.Invoke(GetUnwrappedServer(), args);
                    message = new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);
                    //记录方法输入参数
                    for (int i = 0; i < args.Length; i++)
                    {
                        log.Args += args[i].ToString() + ",";
                    }
                }
                catch (Exception e)
                {
                    message = new ReturnMessage(e, callMsg);
                    returnvalue = e.InnerException.ToString();
                }
                if (message.Properties["__Return"] != null)
                {
                    returnvalue = message.Properties["__Return"].ToString();
                }
                log.ReturnValue = returnvalue;
                Console.WriteLine(returnvalue);
                returnIMessage = message;

            }
            //方法结束时间
            DateTime endTime = DateTime.Now;
            log.UseTime = (endTime - startTime).ToString();
            //写入日志到数据库
            SysLogService.GetInstance().InsertAccessLog(log);
            return returnIMessage as IMessage;
        }

    }
}