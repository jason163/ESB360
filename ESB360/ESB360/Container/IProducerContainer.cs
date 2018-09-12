using ESB360.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360
{
    /// <summary>
    /// 生产者容器接口
    /// </summary>
    public interface IProducerContainer
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool Send(IMessage message);
    }
}
