using RabbitMQ.Client;

namespace Communications;

public class CommunicationHandlerFactory
{
    private string _hostName;
    private IConnection _connection;
    private ConnectionFactory _factory;
    
    
    private CommunicationHandlerFactory(string hostName)
    {
        this._hostName = hostName;
    }
    
    public static async Task<CommunicationHandlerFactory> Initialize(string hostName)
    {
        var ret = new CommunicationHandlerFactory(hostName);
        
        await WaitForRabbitMQ(hostName);
        
        //Create factory
        ret._factory = new ConnectionFactory() { HostName = hostName };
        ret._connection = await ret._factory.CreateConnectionAsync();

        return ret;
    }
    
    public async Task<ConsumerHandler> CreateConsumerHandler(string queueName)
    {
        await WaitForRabbitMQ(_hostName);
        
        var handler = new ConsumerHandler();
        
        IChannel channel = await _connection.CreateChannelAsync();
        await handler.ConnectToQueueAsync(queueName, channel);
        
        return handler;
    }
    
    public async Task<ProducerHandler> CreateProducerHandler(string exchange)
    {
        await WaitForRabbitMQ(_hostName);
        
        IChannel channel = await _connection.CreateChannelAsync();
        
        //await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Topic, durable: true);
        
        return new ProducerHandler(exchange, channel);
    }
    
    public static async Task WaitForRabbitMQ(string hostName)
    {
        int tries = 0;
        var httpClient = new HttpClient();
        while (true) {
            try {
                var response = await httpClient.GetAsync($"http://{hostName}:15672/api/health/checks/alarms");
                Console.WriteLine("RabbitMQ is online!");
                break;
            }
            catch {
                Console.WriteLine($"RabbitMQ not ready yet. Retrying in 5 seconds... ({tries})");
                tries++;
                await Task.Delay(5000);
            }
        }
    }
}