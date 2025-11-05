using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Connected_Database_App.Models
{
    public class Post
    {
        [PrimaryKey]
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int fav_count { get; set; }
        public bool is_favorited { get; set; }
        public bool has_notes { get; set; }

        //PROBLEM: SQLite doesn't like storing complex objects like a Dictionary.

        [Ignore]
        public Dictionary<String, Object> file { get; set; }

        [Ignore]
        public Score score { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
