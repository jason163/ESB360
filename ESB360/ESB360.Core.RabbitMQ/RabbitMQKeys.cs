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
        #region 关键字
        /// <summary>
        /// 交换器
        /// </summary>
        public const string Exchange = "Exchange";
        public const string Direct = "direct";
        public const string Topic = "topic";
        public const string Fanout = "fanout";
        public const string Header = "header";
        public const string TraceId = "TraceId";
        public const string SearchKey = "SearchKey";
        /// <summary>
        /// 死信交换机
        /// </summary>
        public const string DeadLetterExchange = "x-dead-letter-exchange";
        /// <summary>
        /// 消息存活时间
        /// </summary>
        public const string MessageTTL = "x-messageg-ttl";
        public const string RetryInterval = "mt-retryinterval";

        public const string RetryCount = "mt-retrycount";
        /// <summary>
        /// 延时队列交换机
        /// </summary>
        public const string TDX = "tdx";
        /// <summary>
        /// 死信队列交换机
        /// </summary>
        public const string DLX = "dlx";

        /// <summary>
        /// 消息重试次数
        /// </summary>
        public const string MessageRetryCount = "retrycount";
        /// <summary>
        /// 消息存活时间
        /// </summary>
        public const string MessageInterval = "msgexpaire";
        /// <summary>
        /// 消息已重试次数
        /// </summary>
        public const string CurrentRetryNum = "currentNums";
        #endregion

        #region 关键字对应的值
        /// <summary>
        /// 延时队列交换机
        /// </summary>
        public const string TDX_Value = "exchange.delay.test";
        /// <summary>
        /// 延时队列
        /// </summary>
        public const string TDQ_Value = "queue.delay.test";
        /// <summary>
        /// 死信交换机
        /// </summary>
        public const string DLX_Value = "exchange.dlx.test";
        /// <summary>
        /// 死信队列 
        /// </summary>
        public const string DLQ_Value = "queue.dlx.test";
        
        #endregion







    }

    
}
