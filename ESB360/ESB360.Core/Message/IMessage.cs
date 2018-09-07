using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 消息抽象
    /// </summary>
    public interface IMessage
    {
        Dictionary<string,string> Headers { get; }

        IMessage PutHeader(string key, string value);
    }
}
