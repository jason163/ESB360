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

        public void Add(string key)
        {
            if(!consumerContainer.ContainsKey(key))
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                string exchange = key.Split('_')[0];
                string topic = key.Split('_')[1];
                properties.Add(RabbitMQKeys.Exchange, exchange);
                properties.Add(RabbitMQKeys.Topic, topic);

                // 从默认路径加载配置文件
                var config = new MessageChannelConfig();
                MessageChannelAdapter adapter = new MessageChannelAdapter(config);
                MessageChannelPoint channelPoint = adapter.GetMessageChannelPoint();

                IConsumer consumer = channelPoint.CreateConsumer(properties);

                consumer.AddProcessor(new WebAPIProcessor());

                consumer.Startup();
                consumer.Resume();
                consumerContainer.TryAdd(topic, consumer);
            }
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
