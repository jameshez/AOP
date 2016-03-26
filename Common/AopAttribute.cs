using System;
using System.Runtime.Remoting.Proxies;

namespace Common
{
    public class AOPAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            //获取当前要截获的运行对象
            AOPProxy realProxy = new AOPProxy(serverType);
            //返回当前截获对象的代理的分送对象
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
