namespace GatherContent.Connector.Entities.Entities
{
    using Newtonsoft.Json;

    public class StatusEntity
    {
        [JsonProperty(PropertyName = "data")]
        public GCStatus Data { get; set; }
    }
}