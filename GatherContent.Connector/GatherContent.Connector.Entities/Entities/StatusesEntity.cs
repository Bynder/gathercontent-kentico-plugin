namespace GatherContent.Connector.Entities.Entities
{
  using System.Collections.Generic;

  using Newtonsoft.Json;

  public class StatusesEntity
  {
    [JsonProperty(PropertyName = "data")]
    public List<GCStatus> Data { get; set; }
  }
}