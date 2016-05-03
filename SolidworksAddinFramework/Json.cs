using Newtonsoft.Json;

namespace SolidworksAddinFramework
{
    public static class Json
    {
        public static T JsonClone<T>(T tool)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(tool));
        }

        public static void JsonCopyTo<T>(T source, T target)
        {
            JsonConvert.PopulateObject(JsonConvert.SerializeObject(source),target);
        }
        
    }
}