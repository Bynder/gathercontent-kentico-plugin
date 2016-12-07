namespace GatherContent.Connector.Entities.Entities
{
  using Newtonsoft.Json;

  public class Option
  {
    [JsonProperty(PropertyName = "label")]
    public string Label { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "selected")]
    public bool Selected { get; set; }
  }
}