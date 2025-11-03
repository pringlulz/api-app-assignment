using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API_Connected_Database_App.Services
{ 
   enum Compare { 
        Less_Than,
        Greater_Than,
        Greater_Than_Or_Equal,
        Less_Than_Or_Equal
    }

    internal class TagStringBuilder
    {
        private String tagString;

        public String Build()
        {
            return tagString;
        }
        
        public  TagStringBuilder addScoreFilter(Compare compare, int amount)
        {
            String compareString;
            switch (compare)
            {
                case Compare.Greater_Than:
                    compareString = ">";
                    break;
                case Compare.Greater_Than_Or_Equal:
                    compareString = ">=";
                    break;
                case Compare.Less_Than:
                    compareString = "<";
                    break;
                case Compare.Less_Than_Or_Equal:
                    compareString = "<=";
                    break;
                default:
                    compareString = "=";
                    break;
            }

            tagString = String.Join("+", tagString, HttpUtility.UrlEncode(compareString) + amount);
            return this;
        }

        public TagStringBuilder addTagList(List<String> includes, List<String>? excludes = null) //optional exclude list
        {
            excludes = excludes ?? new List<String>();
            includes.ForEach(i => i = HttpUtility.UrlEncode(i));
            excludes.ForEach(i => i = HttpUtility.UrlEncode(i));
            if (String.IsNullOrEmpty(tagString))
            {
                tagString = String.Join("+", includes) + String.Join("+-", excludes);
            } else
            {
                tagString = String.Join("+", String.Join("+", includes) + String.Join("+-", excludes), tagString);
            }

            return this;
        }


    }
}
