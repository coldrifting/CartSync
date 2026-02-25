using System.Text.Json;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database;

public static class DbSetExtensions
{
    public static void Seed<T>(this DbSet<T> set) where T : class
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new Cysharp.Serialization.Json.UlidJsonConverter());
        jsonSerializerOptions.Converters.Add(new FractionJsonConverter());

        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string seedPath = Path.Combine(Path.GetDirectoryName(exePath) ?? "", "Database", "Seed");

        string typeName = typeof(T).Name;
        
        string jsonFileName = typeName.EndsWith('y') ? typeName.Replace("y", "ies") : typeName + "s";
        
        string jsonString = File.ReadAllText(Path.Combine(seedPath, $"{jsonFileName}.json"));
        List<T>? items = JsonSerializer.Deserialize<List<T>>(jsonString, jsonSerializerOptions);

        set.AddRange(items ?? []);
    }
}