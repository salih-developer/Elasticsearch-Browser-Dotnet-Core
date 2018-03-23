using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchBrowser.Models
{
    using Nest;

    public class IndicesViewModel
    {
        public List<CatIndicesRecord> Indices { get; set; }

        public List<CatAliasesRecord> Alias { get; set; }
    }
}
