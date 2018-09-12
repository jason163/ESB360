using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360
{
    /// <summary>
    /// 消费者容器接口
    /// </summary>
    public interface IConsumerContainer
    {
        /// <summary>
        /// 增加消费者
        /// </summary>
        /// <param name="key">RabbitMQ:exchange_topic形式</param>
        void Add(string key);

        /// <summary>
        /// 删除消费者
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
