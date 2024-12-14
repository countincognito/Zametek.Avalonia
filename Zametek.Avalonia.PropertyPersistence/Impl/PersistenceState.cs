using Newtonsoft.Json;

namespace Zametek.Avalonia.PropertyPersistence
{
    public class PersistenceState
        : IPersistenceState<PersistenceElement>
    {
        public PersistenceState()
        {
            Elements = [];
        }


        [JsonProperty("elements")]
        public List<PersistenceElement> Elements
        {
            get;
            private set;
        }
    }
}
