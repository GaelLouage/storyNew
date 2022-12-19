using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Models
{
    public class ReviewEntity
    {

        [JsonProperty(propertyName: "_id")]
        public string ReviewId { get; set; }
        [JsonProperty("reviewTitle")]
        public string ReviewTitle { get; set; }

        [JsonProperty("reviewBody")]
        public string ReviewBody { get; set; }
     

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("storyId")]
        public string StoryId { get; set; }
        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("addedDate")]
        public string AddedDate { get; set; } = DateTime.Now.ToString("MM/dd//yy");
    }
}
