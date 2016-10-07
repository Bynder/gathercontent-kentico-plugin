namespace GatherContent.Connector.Entities.Entities
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AccountEntity
    {
        [JsonProperty(PropertyName = "data")]
        public List<Account> Data { get; set; }
    }
}