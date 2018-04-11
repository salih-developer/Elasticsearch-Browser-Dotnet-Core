using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ElasticsearchBrowser.Controllers
{
    using ElasticsearchBrowser.Common;
    using ElasticsearchBrowser.Models;

    using Microsoft.AspNetCore.Http;

    using Nest;

    using Newtonsoft.Json.Linq;

    public class SearchController : Controller
    {
        private readonly IElasticContex client;

        public SearchController(IElasticContex client)
        {
            this.client = client;
        }
        [HttpGet]
        public IActionResult Index()
        {
            //var rs = this.client.Instance.GetAlias(x => x.Name("product_pool")).Indices.First();
            ConnectionSettings connectionSettings = new ConnectionSettings(new Uri("http://10.1.9.180:9200"));
            connectionSettings.DefaultIndex("product_pool");
            var mapping = this.client.Instance.GetMapping(new GetMappingRequest(Nest.Indices.Index("product_pool")));

            List<KeyValuePair<string, string>> FieldList = mapping.Indices.First().Value.Mappings.Values.First().Properties.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value.Type))
                .ToList();

            var documents = this.client.Instance.Search<Dictionary<string, object>>(x => x.Index("product_pool").AllTypes().MatchAll());
            var model = new SearchViewModel
            {
                Documents = documents.Hits.Select(x => x.Source).ToList(),
                FieldList = FieldList
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {

            ConnectionSettings connectionSettings = new ConnectionSettings(new Uri("http://10.1.9.180:9200"));
            connectionSettings.DefaultIndex("product_pool");
            var mapping = this.client.Instance.GetMapping(new GetMappingRequest(Nest.Indices.Index("product_pool")));

            List<KeyValuePair<string, string>> FieldList = mapping.Indices.First().Value.Mappings.Values.First().Properties.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value.Type))
                .ToList();
            SearchDescriptor<Dictionary<string, object>> search = new SearchDescriptor<Dictionary<string, object>>();
            search = search.Index("product_pool");
            search = search.AllTypes();
            var qr = new QueryContainerDescriptor<Dictionary<string, object>>();

            QueryContainer queryContainer = new QueryContainer();
            foreach (var itePair in form.Where(x => !string.IsNullOrEmpty(x.Value)).ToList())
            {
                if (FieldList.Any(x => x.Key == itePair.Key.Replace("searchKey_", "")))
                {
                    queryContainer &= qr.Term(itePair.Key.Replace("searchKey_", ""), itePair.Value[0]);
                }
            }

            search.Query(descriptor => queryContainer);
            var documents = this.client.Instance.Search<Dictionary<string, object>>(search);

            //var documents = this.client.Instance.Search<Dictionary<string, object>>(new SearchRequest() { Query = queryContainer });

            //var documents = this.client.Instance.Search<Dictionary<string, object>>(x => x.Index("product_pool").AllTypes().MatchAll());
            var model = new SearchViewModel
            {
                Documents = documents.Hits.Select(x => x.Source).ToList(),
                FieldList = FieldList
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult GetJsonObject(long id)
        {
            var model = this.client.Instance.Search<dynamic>(x => x.AllTypes().Query(b => b.Ids(descriptor => descriptor.Values(id))));
            if (model.Documents.Any())
            {
                return Json(new { message = model.Documents.First().ToString() });
            }
            return Json(new { message = "Not Find" });
        }
        [HttpGet]
        public IActionResult Indices()
        {
            var indices = this.client.Instance.CatIndices().Records.ToList().OrderBy(x => x.Index).ToList();
            var alias = this.client.Instance.CatAliases().Records.ToList();
            var model = new IndicesViewModel
            {
                Indices = indices,
                Alias = alias
            };

            return View(model);
        }
    }
}