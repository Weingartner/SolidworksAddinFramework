using Newtonsoft.Json;

namespace SolidworksAddinFramework
{
    public static class Json
    {
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
            JsonConvert.PopulateObject(serializeObject, target);
        }

        public static string ToJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source);
        }
        
    }
}