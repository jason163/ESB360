using ESB360.Core.Extends;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ实现类
    /// </summary>
    public class RabbitMQPointImpl : MessageChannelPoint
    {
        private IConnection connection;
        private Dictionary<string, string> pointProperties;
        private RetryStrategyConfig retryStrategy;
        private IModel channel;

        // 锁对象 类级别
        private static object sync_obj = new object();

        public RabbitMQPointImpl(Dictionary<string,string> pointProperties)
        {
            this.pointProperties = pointProperties;

            Init();

        }
        
        private void Init()
        {
            if (null != connection)
                return;

            lock (sync_obj)
            {
                if (null != connection)
                    return;
                var config = Populate(this.pointProperties);
                // 初始化连接工厂
                var factory = new ConnectionFactory
                {
                    Port = int.Parse(config.Port),
                    HostName = config.HostAddress,
                    UserName = config.UserName,
                    Password = config.Password,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = new TimeSpan(int.Parse(pointProperties.GetValueOrDefault("networkrecoveryinterval", "1000")))
                };
                connection = connection ?? factory.CreateConnection();
                connection.ConnectionShutdown += Connection_ConnectionShutdown;

            }

        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            string message = $"{DateTime.Now.ToString("yyyy-mm-dd hh:MM:ss")} connection shutdown:{e.ReplyText}";
            Console.WriteLine(message);
            // add logger
            //if (loggers != null && loggers.Count > 0)
            //{
            //    loggers.ForEach(logger => logger.error(message, null));
            //}
        }

        /// <summary>
        /// 键值对转换成实体
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private RabbitFactoryInfo Populate(Dictionary<string,string> properties)
        {
            RabbitFactoryInfo info = new RabbitFactoryInfo();
            // 通道驱动类型
            if (!properties.TryGetValue("driverType", out string driverType))
            {
                throw new ArgumentException("there is no property [driverType]!");
            }
            info.DriverType = driverType;
            // 通道地址
            if (!properties.TryGetValue("hostAddr", out string address))
            {
                throw new ArgumentException("there is no property [address]!");
            }
            info.HostAddress = address;
            // 通道端口
            if (!properties.TryGetValue("port", out string port))
            {
                throw new ArgumentException("there is no property [port]!");
            }
            info.Port = port;
            // 用户名
            if (!properties.TryGetValue("username", out string username))
            {
                throw new ArgumentException("there is no property [username]!");
            }
            info.UserName = username;
            // 密码
            if (!properties.TryGetValue("password", out string password))
            {
                throw new ArgumentException("there is no property [port]!");
            }
            info.Password = password;

            if (properties.TryGetValue("networkrecoveryinterval", out string networkrecoveryinterval))
            {
                if (!int.TryParse(networkrecoveryinterval, out int interval))
                {
                    throw new ArgumentException("property [networkrecoveryinterval] must be integer!");
                }
                info.NetworkRecoveryInterval = interval;
            }
            

            return info;
        }

        public IList<IConsumer> Consumers()
        {
            throw new NotImplementedException();
        }

        public IConsumer CreateConsumer(Dictionary<string, string> properties)
        {
            if (pointProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                properties.AddWithDuplicate(RabbitMQKeys.TDX, tdx_value);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryInterval, out string retry_interval))
            {
                properties.AddWithDuplicate(RabbitMQKeys.RetryInterval, retry_interval);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryCount, out string retry_count))
            {
                properties.AddWithDuplicate(RabbitMQKeys.RetryCount, retry_count);
            }
            return new RabbitMQConsumer(connection, properties);
        }

        public IProducer CreateProducer(Dictionary<string, string> properties)
        {
            if (pointProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                properties.AddWithDuplicate(RabbitMQKeys.TDX, tdx_value);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryInterval, out string retry_interval))
            {
                properties.AddWithDuplicate(RabbitMQKeys.RetryInterval, retry_interval);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryCount, out string retry_count))
            {
                properties.AddWithDuplicate(RabbitMQKeys.RetryCount, retry_count);
            }

            return new RabbitMQProducer(connection, properties);
        }

        public IList<IProducer> Producers()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> Properties()
        {
            return pointProperties;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void Startup()
        {
            return;
        }

        /// <summary>
        /// 重试方案
        /// </summary>
        /// <param name="strategy"></param>
        public void SetRetryStrategy(RetryStrategyConfig strategy)
        {
            this.retryStrategy = strategy;
            if(null != strategy && strategy.RetryInterval > 0&& strategy.RetryCount > 0)
            {
                pointProperties.Add(RabbitMQKeys.RetryInterval, strategy.RetryInterval.ToString());
                pointProperties.Add(RabbitMQKeys.RetryCount, strategy.RetryCount.ToString());
                channel = connection.CreateModel();
                channel.ExchangeDeclare(RabbitMQKeys.TDX_Value, RabbitMQKeys.Fanout, durable: true, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(RabbitMQKeys.DLX_Value, RabbitMQKeys.Fanout, durable: true, autoDelete: false, arguments: null);

                Dictionary<string, object> argsDelay = new Dictionary<string, object>();
                argsDelay.Add(RabbitMQKeys.DeadLetterExchange, RabbitMQKeys.DLX_Value);
                argsDelay.Add(RabbitMQKeys.MessageTTL, strategy.RetryInterval);
                // 延时队列 发生死信后，再被投递到死信队列。
                channel.QueueDeclare(RabbitMQKeys.TDQ_Value, true, false, false, argsDelay);
                channel.QueueBind(RabbitMQKeys.TDQ_Value, RabbitMQKeys.TDX_Value, string.Empty);

                // 死信队列；消息重新投递到消息队列，并在消息头设置消息重试次数
                channel.QueueDeclare(RabbitMQKeys.DLQ_Value, true, false, false, null);
                channel.QueueBind(RabbitMQKeys.DLQ_Value, RabbitMQKeys.DLX_Value, string.Empty);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicQos(0, 1, false);
                // 监听死信队列
                channel.BasicConsume(RabbitMQKeys.DLQ_Value, false, consumer);

                pointProperties.AddWithDuplicate(RabbitMQKeys.TDX, RabbitMQKeys.TDX_Value);
                pointProperties.AddWithDuplicate(RabbitMQKeys.DLX_Value, RabbitMQKeys.DLX_Value);
            }
        }

        /// <summary>
        /// 死信队列监听处理;重新投递消息到消息队列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var msgStr = Encoding.UTF8.GetString(body);
            IMessage message = JsonConvert.DeserializeObject<TextMessage>(msgStr);
            if(pointProperties.TryGetValue(RabbitMQKeys.RetryCount,out string retryCount) && int.TryParse(retryCount,out int totalCount))
            {
                if(message.Headers.TryGetValue(RabbitMQKeys.CurrentRetryNum,out string retryNum) && int.TryParse(retryNum,out int curCount))
                {
                    if(totalCount > curCount)
                    {
                        curCount += 1;
                        message.Headers[RabbitMQKeys.CurrentRetryNum] = curCount.ToString();
                        var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                        var props = channel.CreateBasicProperties();
                        props.Persistent = true;
                        channel.ConfirmSelect();
                        // 重新投递到消息队列
                        channel.BasicPublish(message.Headers[RabbitMQKeys.Exchange], "", props, msgBody);
                        channel.WaitForConfirms();
                    }
                }
                else
                {
                    message.PutHeader(RabbitMQKeys.CurrentRetryNum, "1");
                    var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;
                    channel.ConfirmSelect();
                    // 重新投递到消息队列
                    channel.BasicPublish(message.Headers[RabbitMQKeys.Exchange], "", props, msgBody);
                    channel.WaitForConfirms();
                }

            }
            }

        public RetryStrategyConfig GetRetryStrategy()
        {
            return this.retryStrategy;
        }
    }

    internal class RabbitFactoryInfo
    {
        public string DriverType { get; set; }

        public string HostAddress { get; set; }

        public string Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int NetworkRecoveryInterval { get; set; }
    }
}
