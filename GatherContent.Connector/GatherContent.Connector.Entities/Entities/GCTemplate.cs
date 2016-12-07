namespace GatherContent.Connector.Entities.Entities
{
  using System;
  using System.Collections.Generic;

  using Newtonsoft.Json;

  public class GCTemplate
  {
    [JsonProperty(PropertyName = "config")]
    public List<Config> Config { get; set; }

    [JsonProperty(PropertyName = "created_at")]
    public int Created { get; set; }

    [JsonProperty(PropertyName = "created_by")]
    public int CreatedBy { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "project_id")]
    public int ProjectId { get; set; }

    [JsonProperty(PropertyName = "updated_at")]
    public int Updated { get; set; }

    [JsonProperty(PropertyName = "updated_by")]
    public int UpdatedBy { get; set; }

    [JsonProperty(PropertyName = "usage")]
    public Usage Usage { get; set; }

    [JsonProperty(PropertyName = "used_at")]
    public DateTime? UsedAt { get; set; }
  }
}