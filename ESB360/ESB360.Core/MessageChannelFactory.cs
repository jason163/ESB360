using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消息通道工厂
    /// </summary>
    public class MessageChannelFactory
    {
        /// <summary>
        /// 根据配置信息构建消息通道
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static MessageChannelPoint GetMessageChannelPoint(MessageChannelConfig settings)
        {
            MessageChannelPoint vendorImpl = Activator.CreateInstance(Type.GetType(settings.DriverType),args:settings) as MessageChannelPoint;

            return vendorImpl;
        }
    }
}
