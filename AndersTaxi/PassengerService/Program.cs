using Communications;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/publish/", async() =>
{
    var f = await CommunicationHandlerFactory.Initialize("rabbitmq");
    var producerHandler = await f.CreateProducerHandler("taxi.topic");
    await producerHandler.SendMessageAsync("somewhere", "Hello, World!");
    return "Published message";
});

app.Run();