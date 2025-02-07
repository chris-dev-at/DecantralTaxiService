using Communications;
using RabbitMQ.Client;

namespace AndersTaxi;

class Program
{
    static async Task Main(string[] args)
    {
        await CommunicationHandlerFactory.WaitForRabbitMQ(GlobalConfig.HostName);

        var factory = new ConnectionFactory() { HostName = IsRunningInDocker() ? GlobalConfig.HostName : "localhost" }; //run inside Docker
        Console.WriteLine(IsRunningInDocker() ? "Running in Docker" : "Running locally");

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
    
    private static bool IsRunningInDocker()
    {
        // Check for the .dockerenv file which is created in the root directory of a Docker container.
        if (File.Exists("/.dockerenv"))
        {
            return true;
        }

        // Alternatively, you can check /proc/self/cgroup for the presence of 'docker'.
        try
        {
            string cgroupContent = File.ReadAllText("/proc/self/cgroup");
            if (cgroupContent.Contains("docker"))
            {
                return true;
            }
        }
        catch (Exception)
        {
            // Ignore any exceptions that occur while reading /proc/self/cgroup.
            // This can happen on non-Linux systems or if the file is not accessible.
        }

        return false;
    }
}
