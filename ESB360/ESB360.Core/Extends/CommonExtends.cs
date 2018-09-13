using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core.Extends
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class CommonExtends
    {
        /// <summary>
        /// 字典扩展若存在相同的Key,用新Value替换旧的Value
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddWithDuplicate(this Dictionary<string,string> dic, string key,string value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
    }
}
