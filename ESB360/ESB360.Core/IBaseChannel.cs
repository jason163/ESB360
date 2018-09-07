using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    /// <summary>
    /// 通道接口
    /// </summary>
    public interface IBaseChannel
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Startup();

        /// <summary>
        /// 停止
        /// </summary>
        void Shutdown();
    }
}
