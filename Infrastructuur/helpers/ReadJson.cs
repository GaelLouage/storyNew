using Infrastructuur.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Infrastructuur.helpers
{
    public static class ReadJson
    {
        public static JsonEmailEntity GetEmailCredentials(string file)
        {
            // Read the JSON file
            if(File.Exists(file))
            {
                string jsonString = System.IO.File.ReadAllText(@"C:\Users\louagga\Desktop\mongo\cred.json");
                // Deserialize the JSON into an object
                var data = JsonConvert.DeserializeObject<JsonEmailEntity>(jsonString);
                return data;
            }
            return null;

        }

        public static ConnectionStringEntity ConnectionString()
        {
            string file = @"C:\Users\louagga\Desktop\mongo\mongo.json";
            // Read the JSON file
            if (File.Exists(file))
            {
                string jsonString = System.IO.File.ReadAllText(file);
                // Deserialize the JSON into an object
                var data = JsonConvert.DeserializeObject<ConnectionStringEntity>(jsonString);
                return data;
            }
            return null;

        }
    }

}
