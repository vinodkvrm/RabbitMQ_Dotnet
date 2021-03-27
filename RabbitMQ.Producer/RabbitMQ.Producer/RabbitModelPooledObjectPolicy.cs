using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RabbitMQ.Producer
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitOptions _options;

        private readonly IConnection _connection;
        private ConnectionFactory connectionFactory;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> optionsAccs)
        {
            _options = optionsAccs.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password
            };

            /*connectionFactory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                VirtualHost = _options.VHost,
                AutomaticRecoveryEnabled = true,

            };
            SslOption opt = connectionFactory.Ssl;
            opt.Enabled = true;
            if (opt != null && opt.Enabled)
            {
                opt.Version = SslProtocols.Tls12;
                opt.ServerName = _options.HostName;
            }
            connectionFactory.Ssl = opt;*/
            return connectionFactory.CreateConnection();
        }
       
        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}
