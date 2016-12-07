namespace GatherContent.Connector.Entities.Entities
{
  using Newtonsoft.Json;

  public class Project
  {
    [JsonProperty(PropertyName = "account_id")]
    public int AccountId { get; set; }

    [JsonProperty(PropertyName = "active")]
    public bool Active { get; set; }

    [JsonProperty(PropertyName = "allowed_tags")]
    public string AllowedTags { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "overdue")]
    public bool Overdue { get; set; }

    [JsonProperty(PropertyName = "statuses")]
    public StatusesEntity Statuses { get; set; }

    [JsonProperty(PropertyName = "text_direction")]
    public string TextDirection { get; set; }
  }
}