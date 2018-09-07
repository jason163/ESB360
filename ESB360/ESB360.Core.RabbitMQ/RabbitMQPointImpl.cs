using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQPointImpl : MessageChannelPoint
    {
        private IConnection connection;
        private Dictionary<string, string> pointProperties;

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
        private MessageChannelConfig Populate(Dictionary<string,string> properties)
        {
            // 通道驱动类型
            if (!properties.TryGetValue("driverType", out string driverType))
            {
                throw new ArgumentException("there is no property [driverType]!");
            }
            // 通道地址
            if (!properties.TryGetValue("hostAddr", out string address))
            {
                throw new ArgumentException("there is no property [address]!");
            }
            // 通道端口
            if (!properties.TryGetValue("hostAddr", out string port))
            {
                throw new ArgumentException("there is no property [port]!");
            }

            if (properties.TryGetValue("networkrecoveryinterval", out string networkrecoveryinterval))
            {
                if (!int.TryParse(networkrecoveryinterval, out int interval))
                {
                    throw new ArgumentException("property [networkrecoveryinterval] must be integer!");
                }
            }

            return new MessageChannelConfig(driverType, address,port, properties);
        }

        public IList<IConsumer> Consumers()
        {
            throw new NotImplementedException();
        }

        public IConsumer CreateConsumer(Dictionary<string, string> properties)
        {
            if (pointProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                properties.Add(RabbitMQKeys.TDX, tdx_value);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryInterval, out string retry_interval))
            {
                properties.Add(RabbitMQKeys.RetryInterval, retry_interval);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryCount, out string retry_count))
            {
                properties.Add(RabbitMQKeys.RetryCount, retry_count);
            }
            return new RabbitMQConsumer(connection, properties);
        }

        public IProducer CreateProducer(Dictionary<string, string> properties)
        {
            if (pointProperties.TryGetValue(RabbitMQKeys.TDX, out string tdx_value))
            {
                properties.Add(RabbitMQKeys.TDX, tdx_value);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryInterval, out string retry_interval))
            {
                properties.Add(RabbitMQKeys.RetryInterval, retry_interval);
            }
            if (pointProperties.TryGetValue(RabbitMQKeys.RetryCount, out string retry_count))
            {
                properties.Add(RabbitMQKeys.RetryCount, retry_count);
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
            throw new NotImplementedException();
        }
    }
}
