using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Models
{
    public class StoryzonEntity
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("bodyFr")]
        public string? BodyFr { get; set; }

        [JsonProperty("bodyNl")]
        public string? BodyNl { get; set; }

        [JsonProperty("bodyEn")]
        public string? BodyEn { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("price")]
        public int? Rating { get; set; }
        [JsonProperty("addedDate")]

        public string AddedDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public List<ReviewEntity>? Reviews { get; set; } = new List<ReviewEntity>();
    }
}
