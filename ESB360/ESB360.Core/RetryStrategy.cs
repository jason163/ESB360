using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    public class RetryStrategyConfig
    {
        public int RetryInterval { get; set; }

        public int RetryCount { get; set; }
    }
}
