using Elastic.Clients.Elasticsearch;
using Elastic.Transport;


namespace ElasticSearch.API.Extensions
{
    public static class ElasticsearchExtension
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            //NEST
            //var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"]!));
            //var settings = new ConnectionSettings(pool);
            //var client = new ElasticClient(settings);


            //ElasticSearch.Clients
            
            var userName = (configuration.GetSection("Elastic")["Username"])!;
            var password = (configuration.GetSection("Elastic")["Password"])!;

            var settings = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"]!))
                                                    .Authentication(new BasicAuthentication(userName, password));
            
            var client = new ElasticsearchClient(settings);
            services.AddSingleton(client);
        }
    }
}
