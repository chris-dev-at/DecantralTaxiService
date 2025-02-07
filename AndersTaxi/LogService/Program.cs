using Communications;

namespace LogService;

class Program
{
    static async Task Main(string[] args)
    {
        ManualResetEvent _quitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, eArgs) =>
        {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        //Log for now to Console
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("log_queue");

        consumerHandler.OnMessage += Consume;


        //keep alive
        _quitEvent.WaitOne();
    }
    
    public static void Consume(object sender, MessageArgs e)
    {
        Console.WriteLine($"Logged Message: {e.Message}");
    }
}