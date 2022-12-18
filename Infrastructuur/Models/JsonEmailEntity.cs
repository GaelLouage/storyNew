using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Models
{
    public class JsonEmailEntity
    {
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("passWord")]
        public string PassWord { get; set; }
    }
}
