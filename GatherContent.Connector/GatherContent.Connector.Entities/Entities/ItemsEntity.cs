namespace GatherContent.Connector.Entities.Entities
{
  using System.Collections.Generic;

  using Newtonsoft.Json;

  public class ItemsEntity
  {
    [JsonProperty(PropertyName = "data")]
    public List<GCItem> Data { get; set; }
  }
}