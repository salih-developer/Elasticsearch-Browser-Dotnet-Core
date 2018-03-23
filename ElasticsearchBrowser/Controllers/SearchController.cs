using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ElasticsearchBrowser.Controllers
{
    using Elasticsearch.Net;

    using ElasticsearchBrowser.Models;

    using Nest;

    using Newtonsoft.Json.Linq;

    public class ElasticsearchResponseModel<T>: ElasticsearchResponse<T>
    { }
    public class SearchResponseModel<T>: SearchResponse<T> where T : class { }
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            //Nest.ElasticClient clientAlias = new ElasticClient(new Uri("http://10.1.9.180:9200"));
            //var rs = clientAlias.GetAlias(x => x.Name("product_pool")).Indices.First();


            //ConnectionConfiguration configuration = new ConnectionConfiguration(new Uri("http://10.1.9.180:9200"));

            //Elasticsearch.Net.ElasticLowLevelClient client = new Elasticsearch.Net.ElasticLowLevelClient(configuration);
            //var documents = client.Search<ElasticsearchResponseModel<dynamic>>(rs.Key.Name, new SearchRequestParameters());

            Nest.ElasticClient clientAlias = new ElasticClient(new Uri("http://10.1.9.180:9200"));
            var rs = clientAlias.GetAlias(x => x.Name("product_pool")).Indices.First();

            ConnectionSettings connectionSettings = new ConnectionSettings(new Uri("http://10.1.9.180:9200"));
            connectionSettings.DefaultIndex(rs.Key.Name);


            
            Nest.ElasticClient client = new ElasticClient(connectionSettings);
            var documents = client.Search<Dictionary<string, object>>(x=>x.Index(rs.Key.Name).AllTypes().MatchAll());
            var model = new SearchViewModel
            {
                Documents = documents.Hits.Select(x=>x.Source).ToList(),
            };
            
           
            return View(model);
        }

        public IActionResult Indices()
        {
            Nest.ElasticClient client = new ElasticClient(new Uri("http://10.1.9.180:9200"));
            var indices = client.CatIndices().Records.ToList().OrderBy(x=>x.Index).ToList();
            var alias = client.CatAliases().Records.ToList();
            var model = new IndicesViewModel
                            {
                                Indices = indices,
                                Alias= alias
            };

            return View(model);

            
        }
    }
}