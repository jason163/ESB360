using ESB360.Core.RabbitMQ;
using System;

namespace ESB.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init Containers
            ESB360.Startup.Init(new RabbitMQProducerContainer(), new RabbitMQConsumerContainer());
            // Init Consumer
            ESB360.Startup.ConsumerContainer.Add(string.Format("test_test"));

            Console.ReadKey();
        }
    }
}
