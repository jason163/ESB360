using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
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

        /// <summary>
        /// 配置信息都会加载到Key Value里面
        /// </summary>
        /// <param name="config"></param>
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
            // 通过驱动类型、服务IP、服务端口作为通道标识
            string key = string.Format("{0}_{1}_{2}", channelConfig.DriverType, channelConfig.HostAddress, channelConfig.Port);
            if (!channelPointContainer.TryGetValue(key, out MessageChannelPoint channelPoint))
            {
                // 通道实现实例
                channelPoint = GenerateChannelPoint();

                RetryStrategyConfig retryConfig = new RetryStrategyConfig();
                if (channelConfig.Properties.TryGetValue("msgexpaire", out string RetryInterval) && int.TryParse(RetryInterval, out int interval))
                {
                    retryConfig.RetryInterval = interval;
                }
                if (channelConfig.Properties.TryGetValue("retrycount",out string retryCount) && int.TryParse(retryCount,out int count))
                {
                    retryConfig.RetryCount = count;
                }                
                // 根据配置设置信息重试方式延时+重试
                channelPoint.SetRetryStrategy(retryConfig);

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
            // 配置信息都会加载到Key Value里面
            // Assembly asmb = Assembly.LoadFrom("EnterpriseServerBase.dll") ;
            string assemblyName = $"ESB360.Core.{channelConfig.DriverType}.dll";
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            Type ImplType = assembly.GetType(pointImplType);
            return Activator.CreateInstance(ImplType, args: channelConfig.Properties) as MessageChannelPoint;
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
