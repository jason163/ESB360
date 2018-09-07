using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    public class TextMessage : IMessage
    {
        public Dictionary<string, string> Headers { get; set; }

        public IMessage PutHeader(string key, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }
            Headers.Add(key, value);
            return this;
        }

        public string MessageText { get; set; }
    }
}
