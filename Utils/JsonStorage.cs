using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomLayoutMonitors.Utils
{
    public static class JsonStorage
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true, // Essential for NativeMethods structs
            PropertyNameCaseInsensitive = true
        };

        public static void Save<T>(string filePath, T data)
        {
            var json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(filePath, json);
        }

        public static T? Load<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default;
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, Options);
        }
    }
}
