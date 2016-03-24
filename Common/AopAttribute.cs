using System;
using System.Runtime.Remoting.Proxies;

namespace Common
{
    public class AopAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            AopProxy realProxy = new AopProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
