using Communications;

namespace LogService;

class Program
{
    static async Task Main(string[] args)
    {
        //Log for now to Console
        var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
        var consumerHandler = await f.CreateConsumerHandler("log_queue");
        
        consumerHandler.OnMessage += (sender, messageArgs) =>
        {
            Console.WriteLine($"Logged Message: {messageArgs.Message}");
        };
        
        //Keep alive
        while (true)
        {
            await Task.Delay(10000);
        }
    }
}