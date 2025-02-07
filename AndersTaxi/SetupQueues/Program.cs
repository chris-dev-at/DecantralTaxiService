using RabbitMQ.Client;

namespace AndersTaxi;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" }; //since run outside Docker: "localhost" else "rabbitmq"

        //Connect
        using (var connection = await factory.CreateConnectionAsync())
        using (var channel = await connection.CreateChannelAsync())
        {
            //Create Exchange
            await channel.ExchangeDeclareAsync(exchange: "taxi.topic", type: ExchangeType.Topic, durable: true);

            //Create Bindings and Queues
            CreateAndBindQueue(channel, "taxi.topic", "location_queue", "location");
            CreateAndBindQueue(channel, "taxi.topic", "payment_queue", "payment");
            CreateAndBindQueue(channel, "taxi.topic", "driver_queue", "driver");
            CreateAndBindQueue(channel, "taxi.topic", "passenger_queue", "passenger");
            //Log queue (with # for all messages)
            CreateAndBindQueue(channel, "taxi.topic", "log_queue", "#");

            Console.WriteLine("RabbitMQ infrastructure created!");
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
            
        }
    }

    private static void CreateAndBindQueue(IChannel channel, string exchange, string queueName, string routingKey)
    {
        channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBindAsync(
            queue: queueName,
            exchange: exchange,
            routingKey: routingKey);

        Console.WriteLine($"Created queue '{queueName}' bound with routing key '{routingKey}'");
    }
}
