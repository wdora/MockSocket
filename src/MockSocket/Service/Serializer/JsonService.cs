using System.Text.Json;

namespace MockSocket.Abstractions.Serializer
{
    public class JsonService
    {
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json)!;
        }

        public static T Deserialize<T>(string json, T model)
        {
            return JsonSerializer.Deserialize<T>(json)!;
        }

        public static object Deserialize(string json, Type returnType)
        {
            return JsonSerializer.Deserialize(json, returnType)!;
        }

        public static string Serialize<T>(T model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}
