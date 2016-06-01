using Newtonsoft.Json;

namespace SolidworksAddinFramework
{
    public static class Json
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        public static T Clone<T>(T tool)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(tool));
        }

        public static void Copy<T>(T source, T target)
        {
            var serializeObject = JsonConvert.SerializeObject(source);
            Copy(serializeObject, target);
        }

        public static void Copy<T>(string serializeObject, T target)
        {
            JsonConvert.PopulateObject(serializeObject, target, SerializerSettings);
        }

        public static T FromJson<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }

        public static string ToJson(this object source)
        {
            return JsonConvert.SerializeObject(source, SerializerSettings);
        }
        
    }
}