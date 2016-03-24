using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Common
{
    public class AopProxy : RealProxy
    {
        /// <summary>
        /// AOP协议用于记录运行日志
        /// </summary>
        /// <param name="serverType"></param>
        public AopProxy(Type serverType)
            : base(serverType)
        { }

        public override IMessage Invoke(IMessage msg)
        {
            DateTime startTime = DateTime.Now;//方法启动时间
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
                Console.WriteLine("Call constructor");
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
                    #region 记录方法输入参数
                    //log.IP = args[args.Length-2].ToString();
                    //log.Mac = args[args.Length - 1].ToString();
                    for (int i = 0; i < args.Length; i++)
                    {
                        log.Args += args[i].ToString() + ",";
                    }
                    #endregion
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
            DateTime endTime = DateTime.Now;//方法结束时间
            log.UseTime = (endTime - startTime).ToString();
            SysLogService.GetInstance().InsertAccessLog(log);//写入日志到数据库
            //WriteLog.GetInstance().NewLog(string.Format("{0}:{1}", log.MethodName, log.ReturnValue));//写入日志到本地文件
            return returnIMessage as IMessage;
        }

    }
}