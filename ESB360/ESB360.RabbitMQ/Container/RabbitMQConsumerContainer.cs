using ESB360.Processor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// 消费者容器实现类
    /// </summary>
    public class RabbitMQConsumerContainer : IConsumerContainer
    {
        private static ConcurrentDictionary<string, IConsumer> consumerContainer = new ConcurrentDictionary<string, IConsumer>();

        public void Add(Dictionary<string,string> properties)
        {
            if(!properties.TryGetValue(RabbitMQKeys.Topic, out string key))
            {
                throw new ESBCoreException("Properties no set topic");
            }

            if(!consumerContainer.ContainsKey(key))
            {
                // 从默认路径加载配置文件
                var config = new MessageChannelConfig();
                MessageChannelAdapter adapter = new MessageChannelAdapter(config);
                MessageChannelPoint channelPoint = adapter.GetMessageChannelPoint();

                IConsumer consumer = channelPoint.CreateConsumer(properties);

                consumer.AddProcessor(new WebAPIProcessor());

                consumer.Startup();
                consumer.Resume();
                consumerContainer.TryAdd(key, consumer);
            }
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
