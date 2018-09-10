using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消息通道配置信息
    /// </summary>
    public class MessageChannelConfig
    {
        /// <summary>
        /// 消息通道驱动类型如:RabbitMQ、ActiveMQ
        /// </summary>
        public string DriverType{ get;private set;  }
        /// <summary>
        /// 通道地址
        /// </summary>
        public string HostAddress { get; private set; }

        /// <summary>
        /// 通道地址端口
        /// </summary>
        public string Port { get; private set; }

        /// <summary>
        /// 登录消息通道名
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// 登录消息通道密码
        /// </summary>
        public string Password { get; private set; }

        private Dictionary<string, string> m_properties;
        /// <summary>
        /// 消息通道扩展参数
        /// </summary>
        public Dictionary<string,string> Properties
        {
            get
            {
                m_properties = convertExtendParams(this.Extends);
                return m_properties;
            }
            set
            {
                m_properties = value;
            }
        }

        /// <summary>
        /// 字符串类型消息通道扩展参数
        /// </summary>
        public string Extends { get; private set; }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public virtual MessageChannelConfig LoadConfig()
        {
            // 默认从appsettings.json中加载内容
            MessageChannelConfig config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true)
                .Build().GetSection("MessageChannel").Get<MessageChannelConfig>();

            return config;
        }

        public MessageChannelConfig()
        {
            LoadConfig();
        }

        /// <summary>
        /// 字符串转换为键值对
        /// </summary>
        /// <param name="extends">UserName=test&Password=test</param>
        private Dictionary<string,string> convertExtendParams(string extends)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(extends))
            {
                string[] dics = extends.Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach(var item in dics)
                {
                    string[] kv = item.Split('=');
                    if(kv.Length == 2)
                    {
                        result.Add(kv[0].ToLower(), kv[1]);
                    }
                }
            }

            return result;
        }
        
    }
}
