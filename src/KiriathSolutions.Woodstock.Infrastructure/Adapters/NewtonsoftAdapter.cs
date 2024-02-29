using Newtonsoft.Json;

namespace KiriathSolutions.Woodstock.Infrastructure.Adapters;

public interface ISerializer
{
    T? Deserialize<T>(string json);
    string Serialize<T>(T obj);
}

public class NewtonsoftAdapter : ISerializer
{
    public T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}