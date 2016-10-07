namespace GatherContent.Connector.Entities.Entities
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Config
    {
        [JsonProperty(PropertyName = "elements")]
        public List<Element> Elements { get; set; }

        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}