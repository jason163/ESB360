using ESB360.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 发布者抽象
    /// </summary>
    public interface IProducer
    {
        /// <summary>
        /// 消息发布
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool Send(IMessage message);

        /// <summary>
        /// 发布者属性
        /// </summary>
        /// <returns></returns>
        Dictionary<String, String> Properties();
    }
}
