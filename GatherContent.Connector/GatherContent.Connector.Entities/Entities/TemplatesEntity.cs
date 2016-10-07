namespace GatherContent.Connector.Entities.Entities
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TemplatesEntity
    {
        [JsonProperty(PropertyName = "data")]
        public List<GCTemplate> Data { get; set; }
    }
}