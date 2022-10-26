using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using ShardShare;

namespace RabbitMqShardingDemo
{
    // https://github.com/rabbitmq/rabbitmq-server/tree/main/deps/rabbitmq_sharding
    // .\rabbitmqctl set_policy images-shard "^shard.images$" '{\"shards-per-node\": 4, \"routing-key\": \"r1234\"}'

    internal class Program
    {
        private static IConnection? connection;
        private static IModel? channel;

        static void Main(string[] args)
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

                // create exchange
                channel.ExchangeDeclare(exchange: AppConst.ExchangeName, "x-modulus-hash", true);

                for (int i = 0; i < 1000; i++)
                {                    
                    var msg = new MyMessage
                    {
                        CompanyId = new Random().Next(1, 10), // 1=>4
                        OrderNumber = $"{i}"
                    };

                    var json = JsonSerializer.Serialize(msg);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(AppConst.ExchangeName, msg.CompanyId.ToString(), false, null, body);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            Console.WriteLine("Hello, World!");
        }        
    }
}