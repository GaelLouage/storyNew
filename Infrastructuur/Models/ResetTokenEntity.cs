using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Models
{
    public class ResetTokenEntity
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }  = Guid.NewGuid().ToString();
        [JsonProperty(propertyName: "email")]
        public string  Email { get; set; }
        [JsonProperty(propertyName: "expirationDate")]
        public string ExpirationDate { get; set; } = DateTime.Now.AddMinutes(15).ToString();

    }
}
