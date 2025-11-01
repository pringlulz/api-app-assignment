using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Connected_Database_App.Models
{
    internal class Post
    {
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Dictionary<String, Object> file { get; set; }
        public Score score { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
