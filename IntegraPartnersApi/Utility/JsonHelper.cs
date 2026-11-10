using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace IntegraPartnersApi.Utility
{
    public static class JsonHelper
    {
        private static readonly string JsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "employees.json");

        public static List<T> ReadFromJsonFile<T>()
        {
            using StreamReader file = File.OpenText(JsonFilePath);
            JsonSerializer serializer = new JsonSerializer();
            return (List<T>)serializer.Deserialize(file, typeof(List<T>));
        }

        public static void WriteToJsonFile<T>(List<T> data)
        {
            using StreamWriter file = File.CreateText(JsonFilePath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }
    }
}
