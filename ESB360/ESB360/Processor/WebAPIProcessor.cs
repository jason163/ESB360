using ESB360.BizConifg;
using ESB360.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESB360.Processor
{
    /// <summary>
    /// WebAPI处理器
    /// </summary>
    public class WebAPIProcessor : BaseProcessor,IProcessor
    {
        public async Task<bool> Process(IMessage message)
        {
            message = (TextMessage)message;
            // 从持久化层获取配置信息
            string host = BizConfigManager.BizConfig.GetProcessorAddress("topic");
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(host);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(message));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(host, content);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var rspStr = await response.Content.ReadAsStringAsync();
                    StandResponse standResponse = JsonConvert.DeserializeObject<StandResponse>(rspStr);
                    if(standResponse != null && standResponse.Code == 0)
                    {
                        return true;
                    }
                    throw new ESBCoreException(rspStr);
                }
                throw new ESBCoreException($"ResponseStatusCode:{response.StatusCode}\r\n{await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
