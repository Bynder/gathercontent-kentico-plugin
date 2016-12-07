namespace GatherContent.Connector.Entities.Entities
{
  using System.Collections.Generic;

  using Newtonsoft.Json;

  public class Element
  {
    [JsonProperty(PropertyName = "label")]
    public string Label { get; set; }

    [JsonProperty(PropertyName = "limit")]
    public int Limit { get; set; }

    [JsonProperty(PropertyName = "limit_type")]
    public string LimitType { get; set; }

    [JsonProperty(PropertyName = "microcopy")]
    public string Microcopy { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "options")]
    public List<Option> Options { get; set; }

    [JsonProperty(PropertyName = "other_option")]
    public bool OtherOption { get; set; }

    [JsonProperty(PropertyName = "plain_text")]
    public bool PlainText { get; set; }

    [JsonProperty(PropertyName = "required")]
    public bool Required { get; set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }
  }
}