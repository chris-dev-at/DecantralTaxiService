using System.Runtime.CompilerServices;
using Communications;
using Communications.ExchangeMessages;
using Communications.Models;


Dictionary<string, List<double>> invoiceEnquiries = new Dictionary<string, List<double>>();


var builder = WebApplication.CreateBuilder(args);

//inject
var factory = await CommunicationHandlerFactory.Initialize(GlobalConfig.HostName);
var consumerHandler = await factory.CreateConsumerHandler("passenger_queue");
consumerHandler.OnMessage += Consume;
var producerHandler = await factory.CreateProducerHandler("taxi.topic");

builder.Services.AddSingleton<CommunicationHandlerFactory>(cf => factory);
builder.Services.AddSingleton<ConsumerHandler>(ch => consumerHandler);
builder.Services.AddSingleton<ProducerHandler>(ph => producerHandler);

var app = builder.Build();

app.MapGet("/", () => "Passenger Says Hello!");


//create endpoint for passenger to request a ride, all parameters over query string
app.MapGet("/requestRide", async (string id, int x, int y, int destinationX, int destinationY, double maxPrice) =>
{
    var location = new Location()
    {
        X = x,
        Y = y
    };
    var destination = new Location()
    {
        X = destinationX,
        Y = destinationY
    };
    var passenger = new Passenger()
    {
        Id = id,
        MaxPricePerKm = maxPrice,
    };
    var message = new RequestRideMessage()
    {
        Ride = new Ride()
        {
            Passenger = passenger,
            StartLocation = location,
            EndLocation = destination
        }
    };
    await producerHandler.SendMessageAsync("location", message);
    return "Published Ride Request message";
});

//create endpoint for passenger to get all invoices
app.MapGet("/getInvoices", async (string id) =>
{
    if (invoiceEnquiries.ContainsKey(id))
    {
        string ret = "Your invoices:\n";
        foreach (var invoice in invoiceEnquiries[id])
        {
            ret += invoice.ToString("0.00") + "€\n";
        }
        
        ret += "---\n";
        ret += "Total: " + invoiceEnquiries[id].Sum().ToString("0.00") + "€";
        return ret;
    }
    return "No invoices found";
});


app.Run();

async void Consume(object? sender, MessageArgs e)
{
    switch (e.Message.Type)
    {
        case MessageType.InvoiceEnquiry:
            var invoiceEnquiryMessage = e.Message as InvoiceEnquiryMessage;
            InvoiceEnquiryCall(invoiceEnquiryMessage!);
            break;
        default:
            Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
            break;
    }
}

void InvoiceEnquiryCall(InvoiceEnquiryMessage iem)
{
    if (invoiceEnquiries.ContainsKey(iem.Passenger.Id))
    {
        invoiceEnquiries[iem.Passenger.Id].Add(iem.Amount);
    }
    else
    {
        invoiceEnquiries.Add(iem.Passenger.Id, new List<double>() { iem.Amount });
    }
}