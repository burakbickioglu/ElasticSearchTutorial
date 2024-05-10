using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModel;
using System.Collections.Immutable;

namespace ElasticSearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        //Direkt olarak arama yapar, tam olarak eşleşir, eksik harf kabul etmez
        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            //1. yol
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.Field("customer_first_name.keyword").Value(customerFirstName))));

            //2. yol
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Term(t => t.CustomerFirstName.Suffix("keyword"), customerFirstName)));

            //3. yol //CaseInsensitive = true büyük küçük harf duyarlılığını yok sayar.
            var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Direkt olarak birden fazla harf için arama yapar, tam olarak eşleşir
        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x => terms.Add(x));

            //1. yol
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));


            //2. yol
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Query(q => q
                                                                    .Terms(t => t
                                                                    .Field(f => f.CustomerFirstName.Suffix("keyword"))
                                                                    .Terms(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Belirli bir alanın değerine sahip belgeleri bulmak için bir önek (başlangıç) terimi kullanarak sorgu yapma yöntemidir
        public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(100)
                                                                    .Query(q => q
                                                                    .Prefix(p => p
                                                                    .Field(f => f.CustomerFullName
                                                                    .Suffix("keyword"))
                                                                    .Value(customerFullName))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Belirli bir aralıktaki değerlere sahip belgeleri bulmak için kullanılan bir sorgu türüdür
        public async Task<ImmutableList<ECommerce>> RangeQuery(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(100)
                                                                    .Query(q => q
                                                                    .Range(r => r
                                                                    .NumberRange(nr => nr
                                                                    .Field(f => f.TaxFulTotalPrice)
                                                                    .Gte(fromPrice)
                                                                    .Lte(toPrice)))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Tüm veriyi çekmek için kullanılır
        public async Task<ImmutableList<ECommerce>> MatchAllQuery(int page, int pageSize)
        {
            var pageFrom = (page - 1) * pageSize;
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .From(pageFrom)
                                                                    .Size(pageSize)
                                                                    .Query(q => q.MatchAll()));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Belirli bir desene uyan terimlere sahip belgeleri bulmak için kullanılan bir sorgu türüdür
        public async Task<ImmutableList<ECommerce>> WildCardQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Query(q => q
                                                                    .Wildcard(w => w
                                                                    .Field(f => f.CustomerFullName
                                                                    .Suffix("keyword"))
                                                                    .Wildcard(customerFullName))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Benzer ancak tam olarak eşleşmeyen terimleri bulmak için kullanılan bir sorgu türüdür.
        public async Task<ImmutableList<ECommerce>> FuzzyQuery(string customerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Query(q => q
                                                                    .Fuzzy(fu => fu
                                                                    .Field(field: f => f.CustomerFirstName.Suffix("keyword"))
                                                                    .Value(customerName)
                                                                    .Fuzziness(new Fuzziness(1))))
                                                                    .Sort(sort => sort
                                                                    .Field(f => f.TaxFulTotalPrice, new FieldSort() { Order = SortOrder.Desc})));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
            return result.Documents.ToImmutableList();
        }

        //Belirli bir alan içinde belirli bir terimi aramak için kullanılan temel bir sorgu türüdür. Bu sorgu, belirtilen alan içindeki belgeleri belirli bir terim veya ifadeyle eşleştirmek için kullanılır
        public async Task<ImmutableList<ECommerce>> MatchQueryFullText(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .Match(m => m
                                                                    .Field(f => f.Category).Query(categoryName).Operator(Operator.And))));
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Aranılan metinde en sonda yer alan kelimeyi prefix olarak algılamaktadır. Sondaki kelime prefix ardından varsa diğer kelimeler OR koşuluyla aranmaktadır. Prefix olarak belirlenen kelime hariç, diğer kelimelerde tam eşleşme beklemektedir.
        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixQueryFullText(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .MatchBoolPrefix(m => m
                                                                    .Field(f => f.CustomerFullName).Query(customerFullName))));
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Öbek kelimeleri aramak için kullanılır. Örneğin Cihat Solak diye aradığınızda klasik match sorgusunda Cihat veya Solak geçenleri arar. Match Phrase olduğunda Cihat Solak geçenleri getirir ve sırası da önemlidir. İlk kelime cihat sonraki kelime solak olmak zorundadır.

        public async Task<ImmutableList<ECommerce>> MatchPhraseQueryFullText(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .MatchPhrase(m => m
                                                                    .Field(f => f.CustomerFullName).Query(customerFullName))));
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Birden fazla koşul bbir arada kullanılmak istenirse aşağıdaki gibi bir konfigürasyon yapılabilir
        public async Task<ImmutableList<ECommerce>> CompaundQueryExampleOne(string cityName, double taxFulTotalPrice, string categoryName, string manufacturer) 
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .Bool(b => b
                                                                    .Must(m => m
                                                                        .Term(t => t.Field("geoip.city_name").Value(cityName)))
                                                                    .MustNot(mn => mn
                                                                        .Range(r => r.NumberRange(nr => nr.Field(f => f.TaxFulTotalPrice).Lte(taxFulTotalPrice))))
                                                                    .Should(s => s.Term(t => t.Field(f => f.Category.Suffix("keyword")).Value(categoryName)))
                                                                    .Filter(f => f.Term(t => t.Field("manufacturer.keyword").Value(manufacturer))))));

                                                                    
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Hem Term Query hem de Full Text query bir arada kullanılmak istenirse aşağıdaki gibi konfigüre edilebilir
        public async Task<ImmutableList<ECommerce>> CompaundQueryExampleTwo(string customerFullName)
        {
            //1. yol
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .Bool(b => b
                                                                    .Should(m => m
                                                                        .Match(m => m
                                                                            .Field(f => f.CustomerFullName)
                                                                            .Query(customerFullName))
                                                                        .Prefix(p => p
                                                                            .Field(f => f.CustomerFullName.Suffix("keyword"))
                                                                            .Value(customerFullName))))));


            //2. yol
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            //                                                        .Size(1000)
            //                                                        .Query(q => q
            //                                                        .MatchPhrasePrefix(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //Aranmak istenen terim birden fazla alan içerisinde aranmak istenirse 
        public async Task<ImmutableList<ECommerce>> MultiMatchQueryFullText(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                                                                    .Size(1000)
                                                                    .Query(q => q
                                                                    .MultiMatch(mm => mm.Fields(new Field("customer_first_name").And("customer_last_name").And                                                                          ("customer_full_name"))
                                                                    .Query(name))));
            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }
    }
}
