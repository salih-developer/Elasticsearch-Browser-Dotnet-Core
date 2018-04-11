using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchBrowser.Models
{
    using Nest;

    public class SearchViewModel
    {
        public List<Dictionary<string, object>> Documents { get; set; }

        public List<KeyValuePair<string, string>> FieldList { get; set; }

        
    }
}
