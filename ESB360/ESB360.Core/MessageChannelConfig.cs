﻿using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 消息通道扩展参数
        /// </summary>
        public Dictionary<string,string> Properties { get; private set; }

        public MessageChannelConfig(string driverType,string hostAddr,string port,Dictionary<string,string> dic)
            : this(driverType, hostAddr,port, string.Empty, string.Empty)
        {
            this.Properties = dic;
        }

        public MessageChannelConfig(string driverType, string hostAddr,string port, string user, string pwd)
            : this(driverType, hostAddr,port, user, pwd, string.Empty)
        {  
        }

        public MessageChannelConfig(string driverType,string hostAddr,string port,string user,string pwd,string extends)
        {
            this.DriverType = driverType;
            this.HostAddress = hostAddr;
            this.Port = port;
            this.UserName = user;
            this.Password = pwd;

            // 处理扩展参数
            this.Properties = convertExtendParams(extends);
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
