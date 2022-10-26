using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShardShare;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ShardConsumer
{
    internal class Program
    {
        private static IConnection? connection;
        private static IModel? channel;

        static void Main(string[] args)
        {
            InitilizeConnection(args[0]);

            Console.WriteLine("Hello, World!");

            Console.ReadKey();
        }

        private static void InitilizeConnection(string key)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.BasicQos(0, 1, false);
                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                var consumer = new EventingBasicConsumer(channel);
                
                consumer.Received += async (ch, ea) =>
                {
                    // received message  
                    var notificationMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var data = JsonSerializer.Deserialize<MyMessage>(notificationMessage);

                    if (data != null)
                    {
                        Console.WriteLine($"Company {data.CompanyId}\tOrderNumber {data.OrderNumber}");
                        Thread.Sleep(1000);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);                    
                };

                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

                // there are 2 ways to comsume 'shard' queue
                //channel.BasicConsume("shard.images", false, consumer); // this way rabbitmq will auto assign consumer to queue
                //or specializing explicit queue name
                channel.BasicConsume(key, false, consumer); // .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 0"
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public static EventHandler<ShutdownEventArgs> RabbitMQ_ConnectionShutdown { get; private set; }
        public static EventHandler<ShutdownEventArgs> OnConsumerShutdown { get; private set; }
        public static EventHandler<ConsumerEventArgs> OnConsumerRegistered { get; private set; }
        public static EventHandler<ConsumerEventArgs> OnConsumerUnregistered { get; private set; }
        public static EventHandler<ConsumerEventArgs> OnConsumerConsumerCancelled { get; private set; }

    }
}