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
                properties.Add(RabbitMQKeys.Exchange, key.Split('_')[0]);
                properties.Add(RabbitMQKeys.Topic, key.Split('_')[1]);

                // 从默认路径加载配置文件
                var config = new MessageChannelConfig();
                MessageChannelAdapter adapter = new MessageChannelAdapter(config);
                MessageChannelPoint channelPoint = adapter.GetMessageChannelPoint();

                IConsumer consumer = channelPoint.CreateConsumer(properties);

                //consumer.AddProcessor()

            }
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
