using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Producer
{
    public class RabbitManager : IRabbitManager
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string exchangeName, string queueName, string routeKey)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                channel.ExchangeDeclare(exchangeName,ExchangeType.Direct, true, false, null);
                channel.QueueDeclare(queueName,true,false,false);
                channel.QueueBind(queueName,exchangeName,routeKey);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
        
    }
}
