using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Communications;

public class ProducerHandler
{
    private string _exchange;
    IChannel _channel;
    
    public ProducerHandler(string exchange, IChannel channel)
    {
        this._exchange = exchange;
        this._channel = channel;
    }
    
    public async Task SendMessageAsync(string routingKey, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        //send logic here
        await _channel.BasicPublishAsync(exchange: _exchange,
            routingKey: routingKey,
            body: body);
        
        Console.WriteLine($"Sent message: {message}");
    }
    
}