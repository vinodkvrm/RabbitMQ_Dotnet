using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace RabbitMQ.Consumer
{
    class Program
    {
        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;
        private static EventingBasicConsumer consumer;

        static void Main(string[] args)
        {
            factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };
            /*SslOption opt = factory.Ssl;
            opt.Enabled = true;
            if (opt != null && opt.Enabled)
            {
                opt.Version = SslProtocols.Tls12;
                opt.ServerName = "*********";
            }
            factory.Ssl = opt;*/
            Reconnect();
            Console.ReadLine();
            connection.ConnectionShutdown -= Connection_ConnectionShutdown;
            Cleanup();
        }

        static void Connect()
        {
            connection = factory.CreateConnection();
            connection.ConnectionShutdown += Connection_ConnectionShutdown;

            channel = connection.CreateModel();
            channel.ExchangeDeclare("demo.exchange.dotnetcore", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("product-queue", true, false, false);
            channel.QueueBind("product-queue", "demo.exchange.dotnetcore", "queue.durable.dotnetcore");
            channel.QueueDeclare("product-queue", true, false, false, null);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume("product-queue", true, consumer);
        }

        static void Cleanup()
        {
            try
            {
                if (channel != null && channel.IsOpen)
                {
                    channel.Close();
                    channel = null;
                }

                if (connection != null && connection.IsOpen)
                {
                    connection.Close();
                    connection = null;
                }
            }
            catch (IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }

        private static void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection broke!");

            Reconnect();
        }

        private static void Reconnect()
        {
            Cleanup();

            var mres = new ManualResetEventSlim(false); // state is initially false

            while (!mres.Wait(3000)) // loop until state is true, checking every 3s
            {
                try
                {
                    Connect();

                    Console.WriteLine("Connected!");
                    mres.Set(); // state set to true - breaks out of loop
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect failed!");
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var content = Encoding.UTF8.GetString(body.Span);
            Console.WriteLine(" Consumed message:");
            Console.WriteLine(content);
        }
    }
}
