using System.Net.WebSockets;
using System.Text.Json;
using System.Xml.Serialization;
using ConsoleApp1.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace ConsoleApp1.Services;

public class OrderRepository
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    public async Task SaveToJsonAsync(IEnumerable<Order> orders, string path)
    {
        EnsureDirectoryExists(path);
        await using FileStream createStream = File.Create(path);
        await JsonSerializer.SerializeAsync(createStream, orders, _jsonOptions);
    }

    public async Task<List<Order>> LoadFromJsonAsync(string path)
    {
        if (!File.Exists(path)) return new List<Order>();

        await using FileStream openStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<List<Order>>(openStream, _jsonOptions) ?? new List<Order>();
    }

    public async Task SaveToXmlAsync(IEnumerable<Order> orders, string path)
    {
        EnsureDirectoryExists(path);
        XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));

        await using FileStream stream = new FileStream(path, FileMode.Create);
        await Task.Run(() => serializer.Serialize(stream, orders.ToList()));
    }

    public async Task<List<Order>> LoadFromXmlAsync(string path)
    {
        if (!File.Exists(path)) return new List<Order>();

        XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
        await using FileStream stream = new FileStream(path, FileMode.Open);
        
        return await Task.Run(() => (List<Order>)serializer.Deserialize(stream)!);
    }
}