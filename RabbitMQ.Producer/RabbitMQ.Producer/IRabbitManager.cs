using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string exchangeName, string queueName, string routeKey)
        where T : class;
    }
}
