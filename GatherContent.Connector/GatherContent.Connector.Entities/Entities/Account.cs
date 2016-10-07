namespace GatherContent.Connector.Entities.Entities
{
    using Newtonsoft.Json;

    public class Account
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "slug")]
        public string Slug { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }
    }
}