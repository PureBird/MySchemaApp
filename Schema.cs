using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchemaApp
{
    public class Schema
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public Schema() { }
        public Schema(string title, string url)
        {
            Title = title;
            Url = url;
        }

    }
}
