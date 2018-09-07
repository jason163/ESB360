using ESB360.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESB360.Core
{
    /// <summary>
    /// 消息处理器接口
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> Process(IMessage message);
    }
}
