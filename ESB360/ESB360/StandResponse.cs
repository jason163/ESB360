using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360
{
    /// <summary>
    /// 统一返回结果
    /// </summary>
    public class StandResponse
    {
        /// <summary>
        /// 返回代码：-1 系统错误，0 表示成功，1表示业务错误，具体信息参考Desc
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 返回数据以Json格式
        /// </summary>
        public string Data { get; set; }
    }
}
