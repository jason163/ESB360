using ESB360.Core;
using ESB360.Core.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360
{
    /// <summary>
    /// ESB360启动类
    /// </summary>
    public class Startup
    {
        private static IProducerContainer producerContainer;
        private static IConsumerContainer consumerContainer;

        public static IProducerContainer ProducerContainer
        {
            get
            {
                if(null == producerContainer)
                {
                    throw new ESBCoreException("Start no init");
                }
                return producerContainer;
            }
        }

        public static IConsumerContainer ConsumerContainer
        {
            get
            {
                if (null == consumerContainer)
                {
                    throw new ESBCoreException("Start no init");
                }
                return consumerContainer;
            }
        }

        /// <summary>
        /// 初始化ESB360容器
        /// </summary>
        /// <param name="_producerContainer"></param>
        /// <param name="_consumerContainer"></param>
        public static void Init(IProducerContainer _producerContainer,IConsumerContainer _consumerContainer)
        {
            producerContainer = _producerContainer;
            consumerContainer = _consumerContainer;
        }
    }
}
