using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ所需字符常量
    /// </summary>
    public class RabbitMQKeys
    {
        /// <summary>
        /// 交换器
        /// </summary>
        public const string Exchange = "Exchange";

        public const string Direct = "direct";
        public const string Topic = "topic";
        public const string Fanout = "fanout";
        public const string Header = "header";

        /// <summary>
        /// 
        /// </summary>
        public const string TDX = "tdx";

        public const string DeadLetterExchange = "x-dead-letter-exchange";

        public const string RetryInterval = "mt-retryinterval";

        public const string RetryCount = "mt-retrycount";

    }

    
}
