using Newtonsoft.Json;

namespace Zametek.Avalonia.PropertyPersistence
{
    public class PersistenceElement
        : IPersistenceElement<PersistenceProperty>
    {
        public PersistenceElement()
        {
            Properties = [];
        }

        [JsonProperty("properties")]
        public List<PersistenceProperty> Properties
        {
            get;
            private set;
        }

        [JsonProperty("uid")]
        public string? Uid
        {
            get;
            set;
        }
    }
}
