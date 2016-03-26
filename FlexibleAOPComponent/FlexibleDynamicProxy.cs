using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace FlexibleAOPComponent
{
    #region 泛型动态代理
    //public sealed class FlexibleDynamicProxy<T> : RealProxy
    //{
    //    //设置真实对象引用
    //    private readonly T _decorated;

    //    public FlexibleDynamicProxy(T decorated) : base(typeof(T))
    //    {
    //        _decorated = decorated;
    //    }
    //    //记录方法调用的函数
    //    private void Log(string msg, object arg = null)
    //    {
    //        Console.ForegroundColor = ConsoleColor.Red;
    //        File.AppendAllLines("Logs/Log.txt", new string[] {
    //            "Execute DateTime : "+DateTime.Now,
    //            msg, arg.ToString() });
    //        Console.ResetColor();
    //    }
    //    //调用代理的类型的方法
    //    public override IMessage Invoke(IMessage msg)
    //    {
    //        object returnIMessage = null;
    //        //如果调用方法是一般方法，进入If语句
    //        if (msg is IMethodCallMessage)
    //        {
    //            var methodCall = msg as IMethodCallMessage;
    //            var methodInfo = methodCall.MethodBase as MethodInfo;
    //            Log("In Dynamic Proxy - Before executing '{0}'", methodCall.MethodName);
    //            try
    //            {
    //                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
    //                Log("In Dynamic Proxy - After executing '{0}' ", methodCall.MethodName);
    //                returnIMessage = new ReturnMessage(result, null, 0,
    //    methodCall.LogicalCallContext, methodCall);
    //            }
    //            catch (Exception ex)
    //            {
    //                Log(string.Format(
    //   "In Dynamic Proxy- Exception {0} executing '{1}'", ex),
    //   methodCall.MethodName);
    //                returnIMessage = new ReturnMessage(ex, methodCall);
    //            }
    //        }
    //        //如果是调用了构造函数就进入使用下面的代码
    //        else
    //        {
    //            //构造函数调用
    //            IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
    //            Log("In Dynamic Proxy - Before Call The Constructor Method '{0}'", constructCallMsg.MethodName);
    //            IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)msg);
    //            RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
    //            returnIMessage = constructionReturnMessage;
    //        }
    //        return returnIMessage as IMessage;
    //    }
    //}

    #endregion

    #region 一般动态代理
    public sealed class FlexibleDynamicProxy : RealProxy
    {
        private Predicate<MethodInfo> _Filter;
        public Predicate<MethodInfo> Filter
        {
            get { return _Filter; }
            set
            {
                if (value == null)
                    _Filter = m => true;
                else
                    _Filter = value;
            }
        }
        public event EventHandler<IMethodCallMessage> BeforeExecute;
        public event EventHandler<IMethodCallMessage> AfterExecute;
        public event EventHandler<IMethodCallMessage> ErrorExecuting;
        public FlexibleDynamicProxy(Type type) :
            base(type)
        {
            _Filter = m => true;
        }
        private void OnBeforeExecute(IMethodCallMessage methodCall)
        {
            if (BeforeExecute != null)
            {
                var methodInfo = methodCall.MethodBase as MethodInfo;
                if (_Filter(methodInfo))
                    BeforeExecute(this, methodCall);
            }
        }
        private void OnAfterExecute(IMethodCallMessage methodCall)
        {
            if (AfterExecute != null)
            {
                var methodInfo = methodCall.MethodBase as MethodInfo;
                if (_Filter(methodInfo))
                    AfterExecute(this, methodCall);
            }
        }
        private void OnErrorExecuting(IMethodCallMessage methodCall)
        {
            if (ErrorExecuting != null)
            {
                var methodInfo = methodCall.MethodBase as MethodInfo;
                if (_Filter(methodInfo))
                    ErrorExecuting(this, methodCall);
            }
        }
        
        //调用代理的类型的方法
        public override IMessage Invoke(IMessage msg)
        {
            object returnIMessage = null;
            //如果是调用了构造函数就进入使用下面的代码 
            if (msg is IConstructionCallMessage)
            {
                //构造函数调用
                IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
                IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)msg);
                RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
                returnIMessage = constructionReturnMessage;

            }
            //如果调用方法是一般方法，进入If语句
            else
            {
               
                var methodCall = msg as IMethodCallMessage;
                var methodInfo = methodCall.MethodBase as MethodInfo;
                //if(_Filter(methodInfo))
                //    Log("In Dynamic Proxy - Before executing '{0}'", methodCall.MethodName);
                OnBeforeExecute(methodCall);
                try
                {
                    var result = methodInfo.Invoke(GetUnwrappedServer(), methodCall.InArgs);
                    //if (_Filter(methodInfo))
                    //    Log("In Dynamic Proxy - After executing '{0}' ", methodCall.MethodName);
                    OnAfterExecute(methodCall);
                    returnIMessage = new ReturnMessage(result, null, 0,
        methodCall.LogicalCallContext, methodCall);
                }
                catch (Exception ex)
                {
                    //             if (_Filter(methodInfo))
                    //                 Log(string.Format(
                    //"In Dynamic Proxy- Exception {0} executing '{1}'", ex),
                    //methodCall.MethodName);
                    OnErrorExecuting(methodCall);
                    returnIMessage = new ReturnMessage(ex, methodCall);
                }
            }
            return returnIMessage as IMessage;
        }
    }
    #endregion
}
