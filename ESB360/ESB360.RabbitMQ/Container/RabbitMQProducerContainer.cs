using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ生产者容器实现类
    /// </summary>
    public class RabbitMQProducerContainer : IProducerContainer
    {
        private static ConcurrentDictionary<string, IProducer> producerContainer = new ConcurrentDictionary<string, IProducer>();
        public bool Send(IMessage message)
        {
            IProducer producer;
            if (!producerContainer.ContainsKey(message.Headers[RabbitMQKeys.Topic]))
            {
                // 从默认路径加载配置文件
                var config = new MessageChannelConfig();
                MessageChannelAdapter adapter = new MessageChannelAdapter(config);
                MessageChannelPoint channelPoint = adapter.GetMessageChannelPoint();
                // 出消息头中获取Exchange Topic关键信息
                Dictionary<string, string> properties = config.Properties;
                if(message.Headers.TryGetValue(RabbitMQKeys.Exchange,out string exchange))
                {
                    properties.Add(RabbitMQKeys.Exchange, exchange);
                }
                if(message.Headers.TryGetValue(RabbitMQKeys.Topic,out string topic))
                {
                    properties.Add(RabbitMQKeys.Topic, topic);
                }
                // 生成发布者
                producer = channelPoint.CreateProducer(properties);
                producer.Startup();
                producerContainer.TryAdd(message.Headers[RabbitMQKeys.Topic], producer);
            }
            else
            {
                producer = producerContainer[message.Headers[RabbitMQKeys.Topic]];
            }

            return producer.Send(message);
        }
    }
}
