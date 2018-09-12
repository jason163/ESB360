using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.BizConifg
{
    /// <summary>
    /// 业务配置接口，真实业务必须实现此接口
    /// </summary>
    public interface IBizConfig
    {
        /// <summary>
        /// 处理器地址，WEBAPI为URL，Assembly为实现类,SQLScript为链接字符串
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        string GetProcessorAddress(string topic);
    }
}
