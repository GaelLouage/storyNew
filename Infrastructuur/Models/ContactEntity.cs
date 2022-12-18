using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Models
{
    public class ContactEntity : JsonEmailEntity
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Message { get; set; }

    }
}
