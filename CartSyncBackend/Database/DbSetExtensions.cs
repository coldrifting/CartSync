using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database;

public static class DbSetExtensions
{
    public static void Seed<T>(this DbSet<T> set) where T : class
    {
        List<T> items = Deserialize<T>();
        set.AddRange(items);
    }
    
    public static List<T> Deserialize<T>()
    {

        Console.WriteLine("TESTING");
        Console.WriteLine(typeof(T).Name);
        
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new Cysharp.Serialization.Json.UlidJsonConverter());

        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string seedPath = Path.Combine(Path.GetDirectoryName(exePath) ?? "", "Database", "Seed");

        string typeName = typeof(T).Name;
        Console.WriteLine(typeName);
        
        string jsonFileName = nameof(T).EndsWith('y') ? nameof(T).Replace("y", "ies") : typeof(T).Name + "s";
        
        string jsonString = File.ReadAllText(Path.Combine(seedPath, $"{jsonFileName}.json"));
        List<T>? items = JsonSerializer.Deserialize<List<T>>(jsonString, jsonSerializerOptions);

        return items ?? [];
    }
}