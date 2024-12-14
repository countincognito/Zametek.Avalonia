using Newtonsoft.Json;

namespace Zametek.Avalonia.PropertyPersistence
{
    public class PersistenceProperty
        : IPersistenceProperty
    {
        [JsonProperty("name")]
        public string? Name
        {
            get;
            set;
        }

        [JsonProperty("value")]
        public string? Value
        {
            get;
            set;
        }
    }
}
