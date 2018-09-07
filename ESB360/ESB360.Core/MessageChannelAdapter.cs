using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消息通道适配
    /// </summary>
    public class MessageChannelAdapter
    {
        private MessageChannelConfig channelConfig;
        /// <summary>
        /// 通道容器字典
        /// </summary>
        private static ConcurrentDictionary<string, MessageChannelPoint> channelPointContainer = new ConcurrentDictionary<string, MessageChannelPoint>();

        public MessageChannelAdapter(MessageChannelConfig config)
        {
            if (this.channelConfig != null)
                return;
            this.channelConfig = config;
        }

        /// <summary>
        /// 根据配置信息构建消息通道
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual MessageChannelPoint GetMessageChannelPoint()
        {
            string key = string.Format("{0}_{1}_{2}", channelConfig.DriverType, channelConfig.HostAddress, channelConfig.Port);
            if (!channelPointContainer.TryGetValue(key, out MessageChannelPoint channelPoint))
            {
                // 通道实现实例
                channelPoint = GenerateChannelPoint();

                channelPoint.Startup();
                channelPointContainer.TryAdd(key, channelPoint);
            }

            return channelPoint;
        }

        /// <summary>
        /// 实例化通道
        /// </summary>
        /// <returns></returns>
        public virtual MessageChannelPoint GenerateChannelPoint()
        {
            // 通道实现实例
            string pointImplType = ParseDriverImpl(channelConfig.DriverType, channelConfig.Properties);
            return Activator.CreateInstance(Type.GetType(pointImplType), args: channelConfig.Properties) as MessageChannelPoint;
        }

        private string ParseDriverImpl(string driverType,Dictionary<string,string> properties)
        {
            if (properties.ContainsKey("driverimpl"))
            {
                return properties["driverimpl"];
            }
            return $"ESB360.Core.{driverType}.{driverType}PointImpl";
        }


    }
}
