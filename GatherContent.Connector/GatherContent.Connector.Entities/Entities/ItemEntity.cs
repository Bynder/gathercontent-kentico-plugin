namespace GatherContent.Connector.Entities.Entities
{
    using Newtonsoft.Json;

    public class ItemEntity
    {
        [JsonProperty(PropertyName = "data")]
        public GCItem Data { get; set; }
    }
}