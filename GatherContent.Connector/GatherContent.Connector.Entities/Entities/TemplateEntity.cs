namespace GatherContent.Connector.Entities.Entities
{
    using Newtonsoft.Json;

    public class TemplateEntity
    {
        [JsonProperty(PropertyName = "data")]
        public GCTemplate Data { get; set; }
    }
}