namespace ElasticsearchBrowser.Common
{
    #region

    using System;
    using System.Linq;

    using Nest;

    #endregion

    public class ElasticContex : IElasticContex
    {
        private ElasticClient _instance=null;

        public ElasticClient Instance
        {
            get
            {
                if (this._instance == null)
                {
                    //ElasticClient clientAlias = new ElasticClient(new Uri("http://163.172.166.162:9200"));
                    //var rs = clientAlias.GetAlias(x => x.Name("product_pool")).Indices.First();
                    ConnectionSettings connectionSettings = new ConnectionSettings(new Uri("http://10.1.9.180:9200"));
                    connectionSettings.DefaultIndex("listofamerica");
                    this._instance = new ElasticClient(connectionSettings);
                }

                return _instance;
            }
        }
    }

    public interface IElasticContex
    {
        ElasticClient Instance { get; }
    }
}