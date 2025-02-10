using System.Runtime.CompilerServices;
using Communications;
using Communications.ExchangeMessages;
using Communications.Models;

class Program
{
    private static List<RequestDriverMessage> openRequests = new List<RequestDriverMessage>();
    private static ProducerHandler producerHandler;
    
    private static object LockObject = new object(); //prevent multiple drivers for one passenger
    
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        //inject
        var factory = await CommunicationHandlerFactory.Initialize(GlobalConfig.HostName);
        var consumerHandler = await factory.CreateConsumerHandler("driver_queue");
        consumerHandler.OnMessage += Consume;
        producerHandler = await factory.CreateProducerHandler("taxi.topic");
        

        builder.Services.AddSingleton<CommunicationHandlerFactory>(cf => factory);
        builder.Services.AddSingleton<ConsumerHandler>(ch => consumerHandler);
        builder.Services.AddSingleton<ProducerHandler>(ph => producerHandler);

        CompletionListener(); //ensure no request is open for more then 5 seconds
        
        var app = builder.Build();

        app.MapGet("/", () => "Driver Says Hello!");

        //create endpoint for driver to update his location, all parameters over query string
        app.MapGet("/updateLocation", async (string id, int x, int y, string state) =>
        {
            var location = new Location()
            {
                X = x,
                Y = y
            };
            var driver = new Driver()
            {
                Id = id,
                Location = location,
                State = Enum.Parse<DriverState>(state)
            };
            var message = new LocationUpdateMessage()
            {
                Driver = driver
            };
            await producerHandler.SendMessageAsync("location", message);
            return "Published Location message";
        });

        app.MapGet("/getOpenRequests", async (string id) =>
        {
            lock (LockObject)
            {
                var openRequestsForDriver = openRequests.Where(x => x.Drivers.Contains(id)).ToList();
                return openRequestsForDriver;
            }
        });

        //ensure no passenger has more then one driver
        app.MapGet("/acceptRequest", async (string driverId, string passengerId) =>
        {
            lock (LockObject)
            {
                var request = openRequests.FirstOrDefault(x => x.Ride.Passenger.Id == passengerId);
                if(request == null)
                {
                    return Results.NotFound("No open request for this passenger");
                }
                openRequests.Remove(request);


                Console.WriteLine("Accepted request for " + passengerId + " by " + driverId);
                return Results.Ok("Published Accept Request message");
            }
        });

        //create endpoint for driver to complete a ride, all parameters over query string
        app.MapGet("/completeRide", async (string passengerId, string driverId, int distance, double pricePerKm) =>
        {
            //remove ride from openRequests
            lock (LockObject)
            {
                var request = openRequests.FirstOrDefault(x => x.Ride.Passenger.Id == passengerId);
                if(request != null)
                {
                    openRequests.Remove(request);
                    Console.WriteLine("");
                }
            }
            
            var ride = new Ride()
            {
                Passenger = new Passenger()
                {
                    Id = passengerId
                },
                Driver = new Driver()
                {
                    Id = driverId,
                    PricePerKm = pricePerKm
                },
                Distance = distance,
            };
            var message = new RideCompleteMessage()
            {
                Ride = ride
            };
            await producerHandler.SendMessageAsync("payment", message);
            return "Published Ride Complete message";
        });

        app.Run();
    }
    
    private static void Consume(object? sender, MessageArgs e)
    {
        switch (e.Message.Type)
        {
            case MessageType.RequestDriver:
                var requestDriverMessage = e.Message as RequestDriverMessage;
                requestDriverMessage.CreationTime = DateTime.Now;
                RequestDriverCall(requestDriverMessage!);
                break;
            default:
                Console.WriteLine("Service does not handle this message type: " + e.Message.Type);
                break;
        }
    }

    private static void RequestDriverCall(RequestDriverMessage rdm)
    {
        if(openRequests.Any((x => x.Ride.Passenger.Id == rdm.Ride.Passenger.Id)))
        {
            RequeueMessage(rdm);
            return;
        }
    
        if(rdm.Drivers.Count == 0)
        {
            //Console.WriteLine("No drivers available for this request, Requeuing...");
        
            RequeueMessage(rdm);
        }

        lock (LockObject)
        {
            openRequests.Add(rdm);
        }
    }
    private static void RequeueMessage(RequestDriverMessage rdm)
    {
        RemoveDupe(rdm);
        
        //requeue message
        var msg = new RequestRideMessage()
        {
            Ride = rdm.Ride,
        };
        Task.Run(async() =>
        {
            await Task.Delay(2000);
            await producerHandler.SendMessageAsync("location", msg);
        });
    }

    //ensures request is requeued if no driver accepts within 5 seconds (quick fix, should not be done in production)
    private static void CompletionListener()
    {
        Task.Run(async() =>
        {
            while (true)
            {
                await Task.Delay(5000);
                lock (LockObject)
                {
                    foreach (var request in openRequests)
                    {
                        if (request.CreationTime.AddSeconds(5) < DateTime.Now)
                        {
                            Console.WriteLine("Requeuing message: Expired");
                            RequeueMessage(request);
                        }
                    }
                }
            }
        });
    }

    private static void RemoveDupe(string pId, int startx, int starty, int endx, int endy)
    {
        lock (LockObject)
        {
            var duplicates = openRequests.Where(r => r.Ride.Passenger.Id == pId &&
                                                     r.Ride.StartLocation.X == startx &&
                                                     r.Ride.StartLocation.Y == starty &&
                                                     r.Ride.EndLocation.X == endx &&
                                                     r.Ride.EndLocation.Y == endy).ToList();
            foreach (var rq in duplicates)
            {
                openRequests.Remove(rq);
            }
        }
    }
    private static void RemoveDupe(RequestDriverMessage rdm) => RemoveDupe(rdm.Ride.Passenger.Id, rdm.Ride.StartLocation.X, rdm.Ride.StartLocation.Y, rdm.Ride.EndLocation.X, rdm.Ride.EndLocation.Y);
}



