using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class RabbitMQProducer : IProducer
    {
        private IConnection connection;
        private Dictionary<string, string> producerProperties;
        private IModel channel;

        public RabbitMQProducer(IConnection connection,Dictionary<string,string> properties)
        {
            this.connection = connection;
            this.producerProperties = properties;

            InitChannel();
        }

        /// <summary>
        /// 通道相关初始化
        /// </summary>
        public virtual void InitChannel()
        {
            // 创建RabbitMQ通道
            channel = connection.CreateModel();
            // 申明交换机 fanout exchange的路由规则很简单直接将消息路由到所有绑定的队列中，无须对消息的routingkey进行匹配操作
            channel.ExchangeDeclare(producerProperties[RabbitMQKeys.Exchange], RabbitMQKeys.Fanout, durable: true, autoDelete: false, arguments: null);

            if (producerProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                // 申明死信队列 
                Dictionary<string, object> argsDelay = new Dictionary<string, object>();
                argsDelay.Add(RabbitMQKeys.DeadLetterExchange, tdx_value);
                channel.QueueDeclare(producerProperties[RabbitMQKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: argsDelay);
            }
            else
            {
                channel.QueueDeclare(producerProperties[RabbitMQKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: null);
            }

            // 交换机与队列绑定
            channel.QueueBind(producerProperties[RabbitMQKeys.Topic], producerProperties[RabbitMQKeys.Exchange], routingKey: string.Empty);
        }

        public Dictionary<string, string> Properties()
        {
            return this.producerProperties;
        }

        public virtual bool Send(IMessage message)
        {
            byte[] msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            IBasicProperties props = channel.CreateBasicProperties();
            // 设置是否持久化
            props.Persistent = true;
            // 开启Confirm模式
            channel.ConfirmSelect();
            // 发布
            channel.BasicPublish(message.Headers[RabbitMQKeys.Exchange], routingKey: "", basicProperties: props, body: msgBody);

            return channel.WaitForConfirms();

        }

        public void Startup()
        {
            throw new NotImplementedException();
        }

        public virtual void Shutdown()
        {
            channel.Close();
            channel.Dispose();
        }
        
    }
}
