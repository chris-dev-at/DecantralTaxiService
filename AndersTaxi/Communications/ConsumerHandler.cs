using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Communications;

public class ConsumerHandler
{
    public event EventHandler<MessageArgs> OnMessage;
    
    public ConsumerHandler()
    {
    }
    
    public async Task ConnectToQueueAsync(string queueName, IChannel channel)
    {
        /*//Ensure the queue exists (optional, if the queue is already declared)
        await _channel.QueueDeclareAsync(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);/**/
        
        //Consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            await ProcessMessageAsync(ea);
        
            //Acknowledge the message
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };
    
        //Start consuming messages from the queue
        await channel.BasicConsumeAsync(queue: queueName,
            autoAck: false,
            consumer: consumer);
    
        Console.WriteLine($"Started listening for messages on queue '{queueName}'.");
        
    }
    
    //pass into own MessageArgs
    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        Console.WriteLine($"Received message: {message}");
        
        //Execute event
        OnMessage?.Invoke(this, new MessageArgs(message, ea));

        await Task.CompletedTask;
    }
}