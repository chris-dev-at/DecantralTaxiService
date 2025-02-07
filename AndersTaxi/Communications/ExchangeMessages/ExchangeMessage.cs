using Newtonsoft.Json;

namespace Communications.ExchangeMessages;

public class ExchangeMessage  /*maybe make abstract when time*/
{
    public MessageType Type { get; set; }

    public static async Task<T?> FromMessage<T>(string message) where T : ExchangeMessage
    {
        try
        {
            // Use Newtonsoft.Json to deserialize the message string into an ExchangeMessage object
            return await Task.Run(() =>
                JsonConvert.DeserializeObject<T>(message, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }));
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            return null;
            //throw new InvalidCastException($"Failed to deserialize message: {ex.Message}", ex);
        }
    }

    public static async Task<string> ToMessage(ExchangeMessage message)
    {
        try
        {
            return await Task.Run(() =>
                JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                }));
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to serialize message: {ex.Message}", ex);
        }
    }
}