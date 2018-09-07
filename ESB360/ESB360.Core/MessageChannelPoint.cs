﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消息通道点
    /// </summary>
    public interface MessageChannelPoint : IBaseChannel
    {
        Dictionary<String, String> Properties();

        /// <summary>
        /// 创建发布者
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IProducer createProducer(Dictionary<string, string> properties);

        /// <summary>
        /// 创建消费者
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IConsumer createConsumer(Dictionary<string, string> properties);

        /// <summary>
        /// 发布者列表
        /// </summary>
        /// <returns></returns>
        IList<IProducer> Producers();

        /// <summary>
        /// 消费者列表
        /// </summary>
        /// <returns></returns>
        IList<IConsumer> Consumers();
    }
}