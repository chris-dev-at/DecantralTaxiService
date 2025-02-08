using System.Text;
using Communications.ExchangeMessages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Communications;

public class ConsumerHandler
{
    public event EventHandler<MessageArgs> OnMessage;
    public event EventHandler<RawMessageArgs> OnRawMessage;
    private bool _raw = false; //little hacky, but just for debugging
    
    
    public ConsumerHandler(bool raw = false)
    {
        _raw = raw;
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
            var consumed = await ProcessMessageAsync(ea);

            if (consumed)
            {
                //Acknowledge the message
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            else
            {
                //Reject the message
                Console.WriteLine("Rejecting message");
                await channel.BasicRejectAsync(ea.DeliveryTag, requeue: true);
            }
        };

        //Start consuming messages from the queue
        await channel.BasicConsumeAsync(queue: queueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine($"Started listening for messages on queue '{queueName}'.");

    }

    //pass into own MessageArgs
    private async Task<bool> ProcessMessageAsync(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var rawmessage = Encoding.UTF8.GetString(body);

        //Build Message
        if(_raw)
        {
            OnRawMessage?.Invoke(this, new RawMessageArgs(rawmessage, ea));
            return true;
        }
        
        ExchangeMessage msg = await ExchangeMessage.FromMessage<ExchangeMessage>(rawmessage);
        if(msg == null)
        {
            Console.WriteLine($"Invalid Message received: {rawmessage}");
            return false;
        }
        
        /* Could be fixed like:
         *
         *   Create Wrapper for ExchangeMessage (maybe Cloudevents wasn't such a bad idea)
         *   Then rely on TypeHandeling from Newtonsoft.Json
         *
         *   Benefit: Automatically deserialize to correct message type (no need for switch)
         */
        //Load Actual Message (IT IS UGLY; IK; BUT LEAVE ME ALONE I SEARCHED FOR AN HOUR STRAIGHT ON WHY MY DATA WAS NULL)
        switch(msg.Type)
        {
            case MessageType.LocationUpdate:
                msg = await ExchangeMessage.FromMessage<LocationUpdateMessage>(rawmessage);
                break;
            case MessageType.RequestRide:
                msg = await ExchangeMessage.FromMessage<RequestRideMessage>(rawmessage);
                break;
            case MessageType.RequestDriver:
                msg = await ExchangeMessage.FromMessage<RequestDriverMessage>(rawmessage);
                break;
            case MessageType.OfferRide:
                msg = await ExchangeMessage.FromMessage<OfferRideMessage>(rawmessage);
                break;
            case MessageType.AcceptRide:
                msg = await ExchangeMessage.FromMessage<AcceptRideMessage>(rawmessage);
                break;
            case MessageType.RideComplete:
                msg = await ExchangeMessage.FromMessage<RideCompleteMessage>(rawmessage);
                break;
            case MessageType.InvoiceEnquiry:
                msg = await ExchangeMessage.FromMessage<InvoiceEnquiryMessage>(rawmessage);
                break;
            default:
                Console.WriteLine($"Unknown message type: {msg.Type}");
                return false;
        }
        
        //Execute event
        Console.WriteLine($"Received Message: {msg.Type}");
        OnMessage?.Invoke(this, new MessageArgs(msg, ea));

        return true;
    }
}