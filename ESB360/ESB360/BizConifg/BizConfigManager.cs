using ESB360.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.BizConifg
{
    /// <summary>
    /// 业务配置管理类
    /// </summary>
    public class BizConfigManager
    {
        private static IBizConfig bizConfig;

        public static IBizConfig BizConfig
        {
            get
            {
                if(null == bizConfig)
                {
                    throw new ESBCoreException("");
                }
                return bizConfig;
            }
        }

        public static void Init(IBizConfig _bizConfig)
        {
            bizConfig = _bizConfig;
        }
    }
}
