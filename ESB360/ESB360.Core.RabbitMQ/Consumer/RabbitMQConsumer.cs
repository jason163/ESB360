using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// 消费者
    /// </summary>
    public class RabbitMQConsumer : IConsumer
    {
        private IConnection connection;
        private IModel channel;

        private Dictionary<string, string> consumerProperties;
        private IProcessor consumerProcessor;

        public RabbitMQConsumer(IConnection connection,Dictionary<string,string> properties)
        {
            this.connection = connection;
            this.consumerProperties = properties;
        }

        public void AddProcessor(IProcessor processor)
        {
            this.consumerProcessor = processor;
        }

        public Dictionary<string, string> Properties()
        {
            return this.consumerProperties;
        }

        public void Resume()
        {
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
            channel.ExchangeDeclare(consumerProperties[RabbitMQKeys.Exchange], RabbitMQKeys.Fanout, durable: true, autoDelete: false, arguments: null);

            if (consumerProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                // 申明延时队列 
                Dictionary<string, object> argsDelay = new Dictionary<string, object>();
                argsDelay.Add(RabbitMQKeys.DeadLetterExchange, tdx_value);
                channel.QueueDeclare(consumerProperties[RabbitMQKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: argsDelay);
            }
            else
            {
                channel.QueueDeclare(consumerProperties[RabbitMQKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: null);
            }

            // 交换机与队列绑定
            channel.QueueBind(consumerProperties[RabbitMQKeys.Topic], consumerProperties[RabbitMQKeys.Exchange], routingKey: string.Empty);
            channel.BasicQos(0, 1, false);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            // 监听
            channel.BasicConsume(consumerProperties[RabbitMQKeys.Topic], false, consumer: consumer);


        }


        /// <summary>
        /// 消息处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            byte[] body = e.Body;
            var msg = Encoding.UTF8.GetString(body);
            
            if(consumerProcessor == null)
            {
                throw new ESBCoreException("Processor can not be null!");
            }

            try
            {
                IMessage message = JsonConvert.DeserializeObject<TextMessage>(msg);

                bool rst = consumerProcessor.Process(message).Result;

                if (rst)
                {
                    channel.BasicAck(e.DeliveryTag, false);
                    // add monitor
                }
                else
                {
                    channel.BasicReject(e.DeliveryTag, false);
                }
            }catch(Exception ex)
            {
                channel.BasicReject(e.DeliveryTag, false);
            }

        }

        public void Shutdown()
        {
            channel.Close();
            channel.Dispose();
        }

        public void Startup()
        {
            return;
        }
    }
}
