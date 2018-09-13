using ESB360.Core;
using ESB360.Core.RabbitMQ;
using System;
using System.Collections.Generic;

namespace ESB.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init Containers
            ESB360.Startup.Init(new RabbitMQProducerContainer(), new RabbitMQConsumerContainer());
            // Init Consumer
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(RabbitMQKeys.Exchange, "test");
            dic.Add(RabbitMQKeys.Topic, "test");
            // 延时交换机
            dic.Add(RabbitMQKeys.TDX, "exchange.dlx.test");
            ESB360.Startup.ConsumerContainer.Add(dic);

            // Publish Msg
            var msg = new TextMessage()
            {
                MessageText = "hello RabbitMQ"
            };
            msg.PutHeader(RabbitMQKeys.Exchange, "test");
            msg.PutHeader(RabbitMQKeys.Topic, "test");
            msg.PutHeader(RabbitMQKeys.TraceId, Guid.NewGuid().ToString());
            ESB360.Startup.ProducerContainer.Send(msg);

            Console.ReadKey();
        }
    }
}
