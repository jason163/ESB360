﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消费者抽象接口
    /// </summary>
    public interface IConsumer : IBaseChannel
    {
        Dictionary<string, string> Properties();

        void AddProcessor(IProcessor processor);

        void Resume();


    }
}
